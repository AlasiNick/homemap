<script setup lang="ts">
import { computed } from 'vue'

const sizes = {
  sm: 'size-7 text-sm rounded-full',
  md: 'size-8 text-base rounded-full',
  lg: 'size-10 text-lg rounded-md',
}

interface Props {
  username?: string
  size?: keyof typeof sizes
}

const props = withDefaults(defineProps<Props>(), {
  username: 'User Name',
  size: 'md',
})

// Compute initials from name
const initials = computed(() => {
  return props.username
    .split(' ')
    .map(word => word[0])
    .join('')
    .toUpperCase()
    .slice(0, 2)
})
</script>

<template>
  <div
    class="inline-flex items-center justify-center border border-zinc-200 bg-zinc-50 text-zinc-700 font-medium uppercase"
    :class="sizes[size]"
    :title="username"
  >
    <span>{{ initials }}</span>
  </div>
</template>
