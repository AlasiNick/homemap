import type { $Fetch } from 'nitropack'
import type { Entity } from '~/domain/entity'

// TODO: remove it later
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const sleep = () => new Promise((resolve) => {
  setTimeout(() => resolve(true), 3000)
})

type Id = Entity['id']

export const createRepository = (fetch: $Fetch) => ({
  async findAll() {
    return await fetch<unknown[]>('/')
  },

  async findOne(id: Id) {
    // await sleep()
    return await fetch<unknown>(`/${id}`)
  },

  async add<T extends Record<string, unknown>>(entity: T) {
    return await fetch<unknown>('/', {
      method: 'POST',
      body: entity,
    })
  },

  async update<T extends Record<string, unknown>>(id: Id, entity: T) {
    return await fetch<unknown>(`/${id}`, {
      method: 'PUT',
      body: entity,
    })
  },

  async remove(id: Id) {
    return await fetch<unknown>(`/${id}`, {
      method: 'DELETE',
    })
  },
})
