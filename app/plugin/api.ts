import { useAuthStore } from '~/stores/auth'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  const authStore = useAuthStore()

  const api = $fetch.create({
    baseURL: `${config.public.apiBaseUrl}/api/`,
    credentials: 'include',
    onRequest({ options }) {
      const token = authStore.accessToken
      const headers = new Headers(options.headers as HeadersInit | undefined)

      if (token)
        headers.set('Authorization', `Bearer ${token}`)
      else
        headers.delete('Authorization')

      options.headers = headers
    },
    async onResponseError({ request, options, response }) {
      if (response.status !== 401)
        throw response._error ?? response

      try {
        await authStore.refreshSession()

        if (!authStore.accessToken)
          throw new Error('Unable to refresh session')

        const headers = new Headers(options.headers as HeadersInit | undefined)
        headers.set('Authorization', `Bearer ${authStore.accessToken}`)
        options.headers = headers

        return api(request, options)
      }
      catch (error) {
        console.error('Error refreshing token', error)
        authStore.logout()
        if (import.meta.client)
          await navigateTo('/auth')

        throw error
      }
    },
  })

  return {
    provide: {
      api,
    },
  }
})
