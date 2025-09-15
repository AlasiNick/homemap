import { z } from 'zod'
import type { Project } from '~/domain/project'
import { receiverSchema } from '~/domain/receiver'

export const useReceiverService = () => {
  const config = useRuntimeConfig()
  const repository = createReceiverRepository(
    $fetch.create({
      baseURL: `${config.public.apiBaseUrl}/api/receivers`,
    }),
  )

  return {
    async getReceivers(projectId: Project['id']) {
      const response = await repository.findAll(projectId)

      return z.array(receiverSchema).parse(response)
    },
  }
}
