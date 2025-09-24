import { z } from 'zod'
import { deviceLogSchema } from '~/domain/device-log'
import { useAuthStore } from '~/stores/auth'
import { updateProjectSchema, projectSchema, type Project, createProjectSchema } from '~/domain/project'

export const useProjectService = () => {
  const config = useRuntimeConfig()
  const baseUrl = `${config.public.apiBaseUrl}/projects`
  const authStore = useAuthStore()

  const repository = createProjectRepository(
    $fetch.create({
      baseURL: baseUrl,
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
    async getProjects() {
      const response = await repository.findAll()
      return z.array(projectSchema).parse(response)
    },

    async getProjectById(id: Project['id']) {
      const response = await repository.findOne(id)
      return projectSchema.parse(response)
    },

    *streamDeviceLogsById(id: Project['id'], signal: AbortSignal) {
      const eventStream = createServerSentEventStream(`${baseUrl}/${id}/logs/stream`, {
        signal,
      })

      for (const event of eventStream) {
        yield {
          read: async () => {
            const message = await event.message()
            return deviceLogSchema.parse(message)
          },
        }
      }
    },

    async removeProjectById(id: Project['id']) {
      await repository.remove(id)
    },

    async updateProject(id: Project['id'], updatedProject: Record<string, unknown>) {
      const project = updateProjectSchema.parse({ id, ...updatedProject })

      const response = await repository.update(id, project)
      return projectSchema.parse(response)
    },

    async createProject(newProject: Record<string, unknown>) {
      const project = createProjectSchema.parse(newProject)

      const response = await repository.add(project)
      return projectSchema.parse(response)
    },
  }
}
