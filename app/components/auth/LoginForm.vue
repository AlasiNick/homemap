<template>
  <form @submit.prevent="onSubmit" class="space-y-6">
    <BaseInputField
      v-model="state.email"
      name="email"
      label="Email address"
      type="email"
      placeholder="Enter your email"
      :error="errors.email"
      :required="true"
    />

    <BaseInputField
      v-model="state.password"
      name="password"
      label="Password"
      type="password"
      placeholder="Enter your password"
      :error="errors.password"
      :required="true"
    />

    <BaseButton
      :label="isLoading ? 'Logging in...' : 'Sign In'"
      variant="primary"
      :disabled="isLoading"
    >
      <template v-if="isLoading" #leadingIcon>
        <svg class="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      </template>
    </BaseButton>

    <div class="relative my-4">
      <div class="absolute inset-0 flex items-center">
        <div class="w-full border-t border-gray-300"></div>
      </div>
      <div class="relative flex justify-center text-sm">
        <span class="px-2 bg-white text-gray-500">Or continue with</span>
      </div>
    </div>

    <div>
      <GoogleSignInButton
        @success="handleGoogleSignIn"
        @error="handleGoogleSignInError"
        class="w-full"
      />
    </div>

    <p v-if="loginError" class="text-center text-sm text-red-600">
      {{ loginError }}
    </p>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { GoogleSignInButton, type CredentialResponse } from 'vue3-google-signin'

import BaseInputField from '../common/Input.vue'
import BaseButton from '../common/Button.vue'

import { loginSchema } from '~/utils/authValidation'
import { handleApiRequest, saveUserData } from '~/utils/authValidation'

const state = reactive({
  email: '',
  password: '',
})

const errors = reactive<Record<string, string>>({})
const isLoading = ref(false)
const loginError = ref('')

async function onSubmit() {
  errors.email = ''
  errors.password = ''
  loginError.value = ''

  const result = loginSchema.safeParse(state)
  if (!result.success) {
    result.error.errors.forEach((err) => {
      errors[err.path[0]] = err.message
    })
    return
  }

  try {
    isLoading.value = true
    const responseData = await handleApiRequest(
      'http://localhost:5155/api/Auth/login',
      'POST',
      state,
      'Invalid email or password'
    )
    saveUserData(responseData)
    navigateTo('/')
  } catch (error: any) {
    loginError.value = error.message || 'Login failed. Please try again.'
  } finally {
    isLoading.value = false
  }
}

const handleGoogleSignIn = async (response: CredentialResponse) => {
  const { credential } = response
  if (credential) {
    try {
      isLoading.value = true
      const responseData = await handleApiRequest(
        'http://localhost:5155/api/Auth/google-login',
        'POST',
        { idToken: credential },
        'Google Sign-In failed'
      )
      await addUser(responseData.user)
      saveUserData(responseData)
      navigateTo('/')
    } catch (error: any) {
      loginError.value = error.message || 'Google Sign-In failed'
    } finally {
      isLoading.value = false
    }
  }
}

async function addUser(userData: any) {
  try {
    const localUsers = JSON.parse(localStorage.getItem('registeredUsers') || '[]')
    localUsers.push({
      id: userData.id,
      name: userData.name,
      email: userData.email,
      registeredAt: new Date().toISOString(),
    })
    localStorage.setItem('registeredUsers', JSON.stringify(localUsers))
  } catch (error) {
    console.error('Error adding user locally:', error)
  }
}

const handleGoogleSignInError = (error: any) => {
  console.error('Google Sign-In failed', error)
  if (error.type === 'popup_blocked') {
    loginError.value = 'Popup was blocked. Please enable popups and try again.'
  } else if (error.type === 'network_error') {
    loginError.value = 'Network error. Please check your internet connection.'
  } else {
    loginError.value = 'Google Sign-In failed. Please try again.'
  }
}
</script>
