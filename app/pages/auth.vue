// pages/auth.vue
<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div class="text-center">
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900">
          {{ activeTab === 'login' ? 'Sign in to your account' : 'Create new account' }}
        </h2>
      </div>

      <!-- Tab navigation -->
      <div class="flex justify-center space-x-4 border-b">
        <button
          v-for="tab in ['login', 'signup']"
          :key="tab"
          @click="activeTab = tab"
          :class="[
            'py-2 px-4 border-b-2',
            activeTab === tab
              ? 'border-indigo-500 text-indigo-600'
              : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
          ]"
        >
          {{ tab === 'login' ? 'Sign In' : 'Sign Up' }}
        </button>
      </div>

      <!-- Form container -->
      <div class="mt-8">
        <LoginForm v-if="activeTab === 'login'" />
        <SignupForm v-else />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LoginForm from '../components/LoginForm.vue'
import SignupForm from '../components/SignupForm.vue'

definePageMeta({
  middleware: ['check-session']
})

const activeTab = ref('login')
</script>