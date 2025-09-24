import { z } from 'zod'
import type { Project } from '~/domain/project'
import { receiverSchema } from '~/domain/receiver'
import { useAuthStore } from '~/stores/auth'

export const useReceiverService = () => {
  const config = useRuntimeConfig()
  const authStore = useAuthStore()
  const repository = createReceiverRepository(
    $fetch.create({
      baseURL: `${config.public.apiBaseUrl}/receivers`,
      credentials: 'include',
      onRequest({ options }) {
        const headers = new Headers(options.headers as HeadersInit | undefined)
        const token = authStore.accessToken

        if (token)
          headers.set('Authorization', `Bearer ${token}`)
        else
          headers.delete('Authorization')

        options.headers = headers
      },
    }),
  )

  return {
    async getReceivers(projectId: Project['id']) {
      const response = await repository.findAll(projectId)

      return z.array(receiverSchema).parse(response)
    },
  }
}
