<template>
  <form
    class="space-y-6"
    @submit.prevent="onSubmit"
  >
    <BaseInputField
      v-model="state.name"
      name="name"
      label="Name"
      type="text"
      placeholder="Enter your full name"
      :error="errors.name"
      :required="true"
    />

    <BaseInputField
      v-model="state.email"
      name="email"
      label="Email"
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
      placeholder="Create a password"
      :error="errors.password"
      :required="true"
    />

    <BaseButton
      :label="isLoading ? 'Signing up...' : 'Sign Up'"
      variant="primary"
      :disabled="isLoading"
    >
      <template
        v-if="isLoading"
        #leadingIcon
      >
        <svg
          class="h-5 w-5 animate-spin text-white"
          fill="none"
          viewBox="0 0 24 24"
        >
          <circle
            class="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            stroke-width="4"
          />
          <path
            class="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
          />
        </svg>
      </template>
    </BaseButton>

    <p
      v-if="signupError"
      class="text-center text-sm text-red-600"
    >
      {{ signupError }}
    </p>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import BaseInputField from '~/components/common/Input.vue'
import BaseButton from '~/components/common/Button.vue'
import { signupSchema, handleApiRequest, saveUserData } from '~/utils/authValidation'

const state = reactive({
  name: '',
  email: '',
  password: '',
})

const errors = reactive<Record<string, string>>({})
const isLoading = ref(false)
const signupError = ref('')

async function onSubmit() {
  Object.keys(errors).forEach(key => delete errors[key])
  signupError.value = ''

  const result = signupSchema.safeParse(state)
  if (!result.success) {
    result.error.errors.forEach((err) => {
      errors[err.path[0]] = err.message
    })
    return
  }

  try {
    isLoading.value = true
    const responseData = await handleApiRequest(
      'http://localhost:5155/api/Register/register',
      'POST',
      state,
      'Signup failed',
    )

    saveUserData(responseData)
    navigateTo('/')
  }
  catch (error: any) {
    signupError.value = error.message || 'Signup failed. Please try again.'
  }
  finally {
    isLoading.value = false
  }
}
</script>
