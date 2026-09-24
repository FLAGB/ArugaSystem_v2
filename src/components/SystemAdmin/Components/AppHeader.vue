<script setup>
import { computed } from 'vue'
import { Bell, Settings } from 'lucide-vue-next'

defineProps({
  title: { type: String, required: true },
  breadcrumb: { type: String, default: '' },
})

defineEmits(['notification-click', 'settings-click'])

const account = computed(() => {
  try {
    return JSON.parse(localStorage.getItem('account') || 'null')
  } catch {
    return null
  }
})

const loggedInUser = computed(() => account.value?.user || {})

const userInitials = computed(() => {
  const first = loggedInUser.value.firstName?.[0] || ''
  const last = loggedInUser.value.lastName?.[0] || ''

  if (first || last) {
    return `${first}${last}`.toUpperCase()
  }

  if (account.value?.role === 'SystemAdmin') {
    return 'SA'
  }

  return 'U'
})
</script>

<template>
  <header
    class="h-17.5 sticky top-0 z-20 bg-white border-b border-slate-200 flex items-center justify-between px-6 gap-4"
  >
    <!-- Page title -->
    <div class="min-w-0">
      <h1 class="text-lg font-bold text-slate-900 truncate">
        {{ title }}
      </h1>

      <p
        v-if="breadcrumb"
        class="text-xs text-slate-500 truncate"
      >
        {{ breadcrumb }}
      </p>
    </div>

    <!-- Right side -->
    <div class="flex items-center gap-3 shrink-0">

      <!-- Notifications -->
      <button
        @click="$emit('notification-click')"
        class="relative w-9 h-9 rounded-lg flex items-center justify-center text-slate-500 hover:bg-slate-50 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
        aria-label="Notifications"
      >
        <Bell
          class="w-5 h-5"
          :stroke-width="1.8"
        />

        <!-- Notification indicator -->
        <span
          class="absolute top-1.5 right-1.5 w-2 h-2 rounded-full bg-rose-500 ring-2 ring-white"
        ></span>
      </button>

      <!-- Settings -->
      <button
        @click="$emit('settings-click')"
        class="w-9 h-9 rounded-lg flex items-center justify-center text-slate-500 hover:bg-slate-50 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
        aria-label="Settings"
      >
        <Settings
          class="w-5 h-5"
          :stroke-width="1.8"
        />
      </button>

      <!-- Logged-in user avatar -->
      <div
        class="w-9 h-9 rounded-full bg-emerald-100 text-emerald-700 flex items-center justify-center text-xs font-bold shrink-0"
      >
        {{ userInitials }}
      </div>

    </div>
  </header>
</template>