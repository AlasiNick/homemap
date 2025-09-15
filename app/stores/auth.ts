import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as { id: string, name: string, email: string } | null,
    accessToken: null as string | null,
  }),

  actions: {
    initializeFromStorage() {
      if (import.meta.client) {
        const storedUser = localStorage.getItem('user')
        const storedToken = localStorage.getItem('accessToken')

        if (storedUser) this.user = JSON.parse(storedUser)
        if (storedToken) this.accessToken = storedToken
      }
    },
    setUser(user: { id: string, name: string, email: string }) {
      this.user = user
    },
    setAccessToken(token: string) {
      this.accessToken = token
    },
    logout() {
      this.user = null
      this.accessToken = null
    },
  },
})
