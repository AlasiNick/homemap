<script setup lang="ts">
import LoginForm from '~/components/auth/LoginForm.vue'
import SignupForm from '~/components/auth/SignupForm.vue'

definePageMeta({
  layout: false,
})

const activeTab = ref<'login' | 'signup'>('login')
</script>

<template>
  <div class="grid h-screen content-center justify-items-center bg-gray-100 px-4 lg:px-8 sm:px-6">
    <!-- Headings -->
    <div class="max-w-2xl text-center">
      <div class="mx-auto w-fit text-3xl text-zinc-600 sm:text-4xl">
        <Icon name="i-material-symbols-lock-outline-rounded" />
      </div>
      <h1 class="mt-4 text-2xl font-semibold tracking-tight sm:text-3xl">
        Welcome!
      </h1>
      <p class="mt-2 text-zinc-700 leading-7">
        Log in to your account or sign up to get started
      </p>
    </div>

    <!-- Auth Card -->
    <div class="mt-8 max-w-md w-full overflow-hidden rounded-xl bg-white shadow-xl">
      <!-- Tab Navigation -->
      <div class="flex border-b border-gray-200">
        <button
          class="w-1/2 py-4 text-center font-semibold transition-colors duration-300"
          :class="activeTab === 'login'
            ? 'bg-blue-50 text-blue-600 border-b-2 border-blue-500'
            : 'text-gray-500 hover:bg-gray-50'"
          @click="activeTab = 'login'"
        >
          Sign In
        </button>
        <button
          class="w-1/2 py-4 text-center font-semibold transition-colors duration-300"
          :class="activeTab === 'signup'
            ? 'bg-blue-50 text-blue-600 border-b-2 border-blue-500'
            : 'text-gray-500 hover:bg-gray-50'"
          @click="activeTab = 'signup'"
        >
          Sign Up
        </button>
      </div>

      <!-- Form Container -->
      <div class="p-8">
        <LoginForm v-if="activeTab === 'login'" />
        <SignupForm v-else />
      </div>
    </div>
  </div>
</template>
