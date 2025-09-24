import { useAuthStore } from '~/stores/auth'

export default defineNuxtPlugin(() => {
  if (!import.meta.client)
    return

  const authStore = useAuthStore()

  authStore.initializeFromStorage()

  authStore.refreshSession().catch((error) => {
    // A 401 here simply means there is no active session yet.
    console.debug('Session refresh skipped', error)
  })
})
