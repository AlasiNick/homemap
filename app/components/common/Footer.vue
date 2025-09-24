<script setup lang="ts">
const footerLinks = ['Terms', 'Privacy', 'Security', 'Contact', 'Share feedback']

const blockNavigation = (event: MouseEvent) => {
  event.preventDefault()
}
</script>

<template>
  <div class="mx-auto max-w-screen-xl px-6 py-4 lg:px-10">
    <nav
      class="flex flex-col-reverse items-center gap-4 sm:(flex-row justify-between)"
      aria-label="Footer navigation"
    >
      <p class="text-xs text-zinc-600">
        &copy; 2024 Homemap, Ltd.
      </p>
      <ul class="flex flex-wrap items-center justify-center gap-4">
        <li
          v-for="item in footerLinks"
          :key="item"
        >
          <a
            href="#"
            class="block text-xs text-zinc-600 hover:(text-blue-600 underline)"
            role="link"
            aria-disabled="true"
            @click="blockNavigation"
          >
            {{ item }}
          </a>
        </li>
      </ul>
    </nav>
  </div>
</template>
