// components/SignupForm.vue
<template>
  <form class="space-y-4" @submit="onSubmit">
    <!-- Name field -->
    <div>
      <label for="name" class="block text-sm font-medium">Name</label>
      <input
        id="name"
        v-model="state.name"
        type="text"
        class="mt-1 block w-full border-gray-300 rounded shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
      >
      <p v-if="errors.name" class="text-sm text-red-500">
        {{ errors.name }}
      </p>
    </div>

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
        {{ isLoading ? 'Signing up...' : 'Submit' }}
      </button>

      <GoogleSignInButton
        @success="handleLoginSuccess"
        @error="handleLoginError"
      />
    </div>

    <!-- Signup error message -->
    <p v-if="signupError" class="text-sm text-red-500">
      {{ signupError }}
    </p>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { z } from 'zod'
import { GoogleSignInButton, type CredentialResponse } from 'vue3-google-signin'

const schema = z.object({
  email: z.string().email('Invalid email'),
  password: z.string().min(8, 'Must be at least 8 characters'),
  name: z.string().min(1, 'Name is required'),
})

type Schema = z.infer<typeof schema>

const state = reactive<Schema>({
  email: '',
  password: '',
  name: '',
})

const errors = reactive<{ [key in keyof Schema]?: string }>({})
const isLoading = ref(false)
const signupError = ref('')
const user = ref(null)

async function onSubmit(event: Event) {
  event.preventDefault()
  signupError.value = ''

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
    const response = await fetch('http://localhost:5155/api/Register/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(state),
    })

    if (!response.ok) {
      const errorData = await response.json()
      signupError.value = errorData.message || 'Signup failed. Please try again.'
      return
    }

    const responseData = await response.json()
    localStorage.setItem('accessToken', responseData.accessToken)
    localStorage.setItem('refreshToken', responseData.refreshToken)
    user.value = responseData.user
    window.location.href = '/'
  } catch (error) {
    signupError.value = 'An error occurred. Please try again later.'
    console.error('Signup error:', error)
  } finally {
    isLoading.value = false
  }
}

const handleLoginSuccess = async (response: CredentialResponse) => {
  const { credential } = response
  if (credential) {
    try {
      const user = await useFetch('http://localhost:5155/api/Auth/google-login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ idToken: credential }),
      })
      await navigateTo({ path: '/' })
    } catch (error) {
      console.error('Login error:', error)
    }
  }
}

const handleLoginError = (error: any) => {
  console.error('Google Sign-In failed', error)
  if (error.type === 'popup_blocked') {
    alert('Popup was blocked. Please enable popups and try again.')
  } else if (error.type === 'network_error') {
    alert('Network error. Please check your internet connection.')
  } else {
    alert('Google Sign-In failed. Please try again.')
  }
}
</script>