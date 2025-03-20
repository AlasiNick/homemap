// components/LoginForm.vue
<template>
  <form class="space-y-4" @submit="onSubmit">
    <!-- Email field -->
    <div>
      <label for="email" class="block text-sm font-medium">Email</label>
      <input
        id="email"
        v-model="state.email"
        type="email"
        class="mt-1 block w-full border-gray-300 rounded shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
      >
      <p v-if="errors.email" class="text-sm text-red-500">
        {{ errors.email }}
      </p>
    </div>

    <!-- Password field -->
    <div>
      <label for="password" class="block text-sm font-medium">Password</label>
      <input
        id="password"
        v-model="state.password"
        type="password"
        class="mt-1 block w-full border-gray-300 rounded shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
      >
      <p v-if="errors.password" class="text-sm text-red-500">
        {{ errors.password }}
      </p>
    </div>

    <!-- Submit button -->
    <div>
      <button
        type="submit"
        :disabled="isLoading"
        class="inline-flex items-center border border-transparent rounded bg-indigo-600 px-4 py-2 text-sm text-white font-medium shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
      >
        {{ isLoading ? 'Logging in...' : 'Submit' }}
      </button>
    </div>

    <!-- Login error message -->
    <p v-if="loginError" class="text-sm text-red-500">
      {{ loginError }}
    </p>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { z } from 'zod'

const schema = z.object({
  email: z.string().email('Invalid email'),
  password: z.string().min(0, 'Invalid password'),
})

type Schema = z.infer<typeof schema>

const state = reactive<Schema>({
  email: '',
  password: '',
})

const errors = reactive<{ [key in keyof Schema]?: string }>({})
const isLoading = ref(false)
const loginError = ref('')

async function onSubmit(event: Event) {
  event.preventDefault()
  loginError.value = ''

  Object.keys(errors).forEach(key => (errors[key as keyof Schema] = undefined))

  const result = schema.safeParse(state)
  if (!result.success) {
    result.error.errors.forEach((err) => {
      const field = err.path[0] as keyof Schema
      errors[field] = err.message
    })
    return
  }

  try {
    isLoading.value = true
    const response = await fetch('http://localhost:5155/api/Auth/login', {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(state),
    })

    if (!response.ok) {
      if (response.status === 401) {
        loginError.value = 'Invalid email or password'
      } else {
        loginError.value = 'Login failed. Please try again.'
      }
      return
    }

    const responseData = await response.json()
    localStorage.setItem('user', JSON.stringify(responseData.user))
    localStorage.setItem('accessToken', responseData.accessToken)

    useAuthStore().setUser(responseData.user)
    useAuthStore().setAccessToken(responseData.accessToken)

    
    navigateTo('/')
  } catch (error) {
    loginError.value = 'An error occurred. Please try again later.'
    console.error('Login error:', error)
  } finally {
    isLoading.value = false
  }
}
</script>