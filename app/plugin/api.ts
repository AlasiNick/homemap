import type { NitroFetchRequest } from 'nitropack'

export default defineNuxtPlugin(async () => {
    const config = useRuntimeConfig()
    const api = $fetch.create({
      baseURL: `${config.public.apiBaseUrl}/api/`,
      onRequest({ options }) {
        if (process.client) {
          const accessToken = localStorage.getItem('accessToken')
          if (accessToken) {
            options.headers.set('Authorization', `Bearer ${accessToken}`)
          }
        }
      },
    async onResponseError({ response, options }) {
      if (response.status === 401) {
        try {
          const newAccessToken = await refreshAccessToken()

          if (newAccessToken) {
            localStorage.setItem('accessToken', newAccessToken)
          }
          else {
            throw new Error('New access token is undefined')
          }

          options.headers.set('Authorization', `Bearer ${newAccessToken}`)

          const requestOptions = {
            ...options,
          } as NitroFetchRequest

          return $fetch(requestOptions)
        }
        catch (refreshError) {
          console.error('Error refreshing token', refreshError)
          await navigateTo('/login') // Redirect to login if refresh fails
        }
      }
    },
  })

  return {
    provide: {
      api,
    },
  }
})

async function refreshAccessToken() {
  const baseURL = useRuntimeConfig().public.apiBaseUrl

  const { data, status } = await useFetch<{ accessToken: string }>(
    `${baseURL}/api/Auth/refresh-token`,
    {
      method: 'POST',
      credentials: 'include',
    },
  )

  if (status.value === 'success') {
    return data.value?.accessToken
  }
  else {
    throw new Error('Token refresh failed')
  }
}
