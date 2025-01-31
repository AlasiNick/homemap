// TODO: update the code to not fucking load the page before the validation of the token
// TODO: check against accessToken, not refreshToken!

export default defineNuxtRouteMiddleware(async (to) => {
    if (!import.meta.client) {
      const sessionValid = await validateSessionOnServer()
      if (!sessionValid && to.path !== '/login') {
        return navigateTo('/login')
      }
    }
  
    if (import.meta.client) {
      const accessToken = localStorage.getItem('accessToken')
      if (!accessToken && to.path !== '/login') {
        return navigateTo('/login')
      }
      if (accessToken && to.path === '/login') {
        return navigateTo('/')
      }
    }
  })
  
  async function validateSessionOnServer() {
    try {
      const response = await fetch('http://localhost:5155/api/Auth/check-session', {
        method: 'GET',
        credentials: 'include',
      })
      return response.ok
    }
    catch {
      return false
    }
  }
  