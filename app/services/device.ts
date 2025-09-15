import { z } from 'zod'
import { deviceSchema } from '~/domain/device'
import type { Receiver } from '~/domain/receiver'

export const useDeviceService = () => {
  const config = useRuntimeConfig()
  const repository = createDeviceRepository(
    $fetch.create({
      baseURL: `${config.public.apiBaseUrl}/api/devices`,
    }),
  )

  return {
    async getDevices(receiverId: Receiver['id']) {
      const response = await repository.findAll(receiverId)

      return z.array(deviceSchema).parse(response)
    },
  }
}
