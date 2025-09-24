<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useAuthStore } from '~/stores/auth'
import { NuxtLink } from '#components'

const authStore = useAuthStore()
const { user } = storeToRefs(authStore)

const initials = computed(() => {
  return user.value?.name
    ?.split(' ')
    .map(n => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2) || '??'
})

const handleSignOut = async () => {
  authStore.logout()

  await navigateTo('/auth')
}
</script>

<template>
  <Dropdown>
    <template #trigger>
      <DropdownTrigger>
        <slot>
          <button
            type="button"
            class="rounded-full p-1 active:(bg-zinc-950/10 shadow-inner) hover:bg-zinc-950/5"
          >
            <div class="size-8 inline-flex items-center justify-center border border-zinc-950/10 rounded-full bg-zinc-50 text-zinc-600 font-medium uppercase sm:(size-7 text-sm)">
              <span>{{ initials }}</span>
            </div>
          </button>
        </slot>
      </DropdownTrigger>
    </template>

    <DropdownGroup>
      <DropdownItem
        :as="NuxtLink"
        to="/"
        label="My account"
        icon="i-material-symbols-account-circle"
      />
    </DropdownGroup>
    <DropdownSeparator />
    <DropdownGroup>
      <DropdownItem
        :as="NuxtLink"
        to="/"
        label="Privacy policy"
        icon="i-material-symbols-verified-user-outline-rounded"
      />
      <DropdownItem
        :as="NuxtLink"
        to="/"
        label="Share feedback"
        icon="i-material-symbols-lightbulb-2-outline-rounded"
      />
    </DropdownGroup>
    <DropdownSeparator />
    <DropdownGroup>
      <DropdownItem
        as="button"
        label="Sign out"
        icon="i-material-symbols-logout-rounded"
        variant="destructive"
        type="button"
        @click="handleSignOut"
      />
    </DropdownGroup>
  </Dropdown>
</template>
