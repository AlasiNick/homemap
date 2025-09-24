import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as { id: string, name: string, email: string } | null,
    accessToken: null as string | null,
    refreshHandle: null as number | null,
    refreshing: false,
  }),

  actions: {
    initializeFromStorage() {
      if (!import.meta.client)
        return

      const storedUser = localStorage.getItem('user')
      if (storedUser)
        this.user = JSON.parse(storedUser)
    },
    setUser(user: { id: string, name: string, email: string }) {
      this.user = user
      if (import.meta.client)
        localStorage.setItem('user', JSON.stringify(user))
    },
    setAccessToken(token: string) {
      this.accessToken = token
      this.scheduleSessionRefresh()
    },
    clearAccessToken() {
      this.accessToken = null
    },
    clearUser() {
      this.user = null
      if (import.meta.client)
        localStorage.removeItem('user')
    },
    scheduleSessionRefresh(delayMs = 5 * 60 * 1000) {
      if (!import.meta.client)
        return

      if (this.refreshHandle)
        clearTimeout(this.refreshHandle)

      if (!this.accessToken)
        return

      this.refreshHandle = window.setTimeout(() => {
        this.refreshSession().catch((error) => {
          console.error('Failed to refresh session', error)
        })
      }, delayMs)
    },
    clearRefreshTimer() {
      if (!import.meta.client)
        return

      if (this.refreshHandle) {
        clearTimeout(this.refreshHandle)
        this.refreshHandle = null
      }
    },
    async refreshSession() {
      if (!import.meta.client || this.refreshing)
        return

      this.refreshing = true
      try {
        const config = useRuntimeConfig()
        const response = await $fetch<{ accessToken: string }>(
          `${config.public.apiBaseUrl}/Auth/refresh-token`,
          {
            method: 'POST',
            credentials: 'include',
          },
        )

        if (response?.accessToken) {
          this.setAccessToken(response.accessToken)
        }
        else {
          this.logout()
        }
      }
      catch (error) {
        this.logout()
        throw error
      }
      finally {
        this.refreshing = false
      }
    },
    logout() {
      this.clearRefreshTimer()
      this.clearUser()
      this.clearAccessToken()
    },
  },
})
