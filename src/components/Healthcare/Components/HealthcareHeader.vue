<template>
  <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 shrink-0">
    <h1 v-if="title" class="text-lg font-bold text-slate-800">{{ title }}</h1>
    <div v-else></div>

    <div class="flex items-center gap-3">
      <div class="text-right">
        <p class="text-sm font-semibold">{{ worker.fullName }}</p>
        <p class="text-xs text-slate-400">{{ worker.userType }}</p>
      </div>
      <div class="w-9 h-9 bg-emerald-700 rounded-full flex items-center justify-center text-white text-sm font-bold">
        {{ initials }}
      </div>
    </div>
  </header>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  worker: {
    // { fullName, userType }
    type: Object,
    required: true,
  },
  title: {
    type: String,
    default: '',
  },
})

const initials = computed(() =>
  (props.worker.fullName || '')
    .split(' ')
    .filter(Boolean)
    .map(n => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2)
)
</script>