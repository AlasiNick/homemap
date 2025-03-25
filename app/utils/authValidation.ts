// utils/authValidation.ts
import { z } from 'zod'

export const loginSchema = z.object({
  email: z.string().email('Invalid email'),
  password: z.string().min(1, 'Password is required')
})

export const signupSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters'),
  email: z.string().email('Invalid email'),
  password: z.string().min(8, 'Password must be at least 8 characters')
})

export async function handleApiRequest(
  url: string, 
  method: 'POST' | 'GET', 
  data: any, 
  errorMessage: string = 'An error occurred'
) {
  try {
    const response = await fetch(url, {
      method,
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })

    if (!response.ok) {
      const errorData = await response.json()
      throw new Error(errorData.message || errorMessage)
    }

    return await response.json()
  } catch (error) {
    console.error(`API Request Error: ${error}`)
    throw error
  }
}

export function saveUserData(userData: any) {
  localStorage.setItem('user', JSON.stringify(userData.user))
  localStorage.setItem('accessToken', userData.accessToken)
  
  const authStore = useAuthStore()
  authStore.setUser(userData.user)
  authStore.setAccessToken(userData.accessToken)
}