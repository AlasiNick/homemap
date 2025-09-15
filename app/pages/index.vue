<script setup lang="ts">
import { NuxtLink } from '#components'

definePageMeta({
  middleware: ['check-session'],
})
</script>

<template>
  <div class="grid grow-1 content-center justify-items-center">
    <div class="max-w-2xl">
      <div class="mx-auto w-fit text-3xl text-zinc-600 sm:text-4xl">
        <Icon name="i-material-symbols-draft-outline-rounded" />
      </div>
      <h1 class="mt-4 text-center text-2xl font-semibold tracking-tight sm:text-3xl">
        Open existing project
      </h1>
      <p class="mt-2 text-center text-zinc-700 leading-7">
        Or create a new one and start connecting your devices
      </p>
    </div>
    <div class="mt-8 max-w-3xl w-full">
      <ProjectList />
    </div>
    <div class="mt-10 max-w-2xl w-full flex flex-col gap-2 sm:(flex-row items-center justify-center)">
      <Button
        :as="NuxtLink"
        to="/projects/new"
        leading-icon="i-material-symbols-add-2-rounded"
        label="Add project"
        variant="primary"
      />
      <Tooltip label="Feature is not available">
        <Button
          as="button"
          type="button"
          trailing-icon="i-material-symbols-folder-open-outline-rounded"
          label="Open project"
          disabled
        />
      </Tooltip>
    </div>
  </div>
</template>
