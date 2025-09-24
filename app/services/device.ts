import { z } from 'zod'
import { deviceSchema } from '~/domain/device'
import type { Receiver } from '~/domain/receiver'
import { useAuthStore } from '~/stores/auth'

export const useDeviceService = () => {
  const config = useRuntimeConfig()
  const authStore = useAuthStore()
  const repository = createDeviceRepository(
    $fetch.create({
      baseURL: `${config.public.apiBaseUrl}/devices`,
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
    async getDevices(receiverId: Receiver['id']) {
      const response = await repository.findAll(receiverId)

      return z.array(deviceSchema).parse(response)
    },
  }
}
