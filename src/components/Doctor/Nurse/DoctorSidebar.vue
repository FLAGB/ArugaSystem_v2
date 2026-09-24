<!--
  DoctorSidebar.vue
  ==================
  Single source of truth for the doctor-side left nav. Every Doctor*.vue
  page should import and render this instead of hand-rolling its own
  <aside> + navItems array — before this, all 7 Doctor pages carried an
  identical, independently-maintained copy of both, so any nav change
  (like the Leveriza logo swap) had to be hand-applied 7 times.

  To add/remove/reorder a nav item in the future, edit NAV_ITEMS below —
  it updates on every Doctor page at once.
-->
<template>
  <aside class="w-64 bg-white border-r border-slate-200 flex flex-col shrink-0">
    <div class="p-6">
      <div class="flex items-center gap-3 px-2">
        <div class="w-8 h-8 rounded overflow-hidden shrink-0">
          <img :src="logoIcon" alt="Leveriza Health Center" class="w-full h-full object-cover" />
        </div>
        <div>
          <span class="text-xl font-bold tracking-tight">Aruga</span>
          <p class="text-xs text-slate-400">Pediatric Health System</p>
        </div>
      </div>
    </div>

    <nav class="flex-1 px-3 space-y-1 overflow-y-auto">
      <button
        v-for="item in navItems"
        :key="item.id"
        @click="$router.push(item.path)"
        class="w-full flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-colors cursor-pointer"
        :class="$route.path === item.path ? 'bg-emerald-50 text-emerald-700' : 'text-slate-500 hover:bg-slate-50 hover:text-slate-900'"
      >
        <component :is="item.icon" class="w-4 h-4" />
        {{ item.label }}
      </button>
    </nav>

    <div class="p-3 border-t border-slate-200">
      <button @click="logout" class="w-full flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium text-slate-500 hover:bg-slate-50 hover:text-slate-900 transition-colors">
        <LogOut class="w-4 h-4" />
        Logout
      </button>
    </div>
  </aside>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { logout as clearSession } from '@/utils/auth'
import logoIcon from '@/assets/logo-icon.svg'
import {
  Home, ListChecks, Users, Calendar, Syringe, FileText, Settings, LogOut,
} from 'lucide-vue-next'

const router = useRouter()

const navItems = [
  { id: 'home',     label: 'Home',                path: '/doctor/home',     icon: Home       },
  { id: 'queue',    label: 'Queue',               path: '/doctor/queue',    icon: ListChecks },
  { id: 'patients', label: 'Patients (Children)', path: '/doctor/patients', icon: Users      },
  { id: 'calendar', label: 'Calendar',            path: '/doctor/calendar', icon: Calendar   },
  { id: 'records',  label: 'Vaccination Records', path: '/doctor/records',  icon: Syringe    },
  { id: 'reports',  label: 'Reports',             path: '/doctor/reports', icon: FileText    },
  { id: 'account',  label: 'My Account',          path: '/doctor/account', icon: Settings    },
]

function logout() {
  clearSession()
  router.push('/')
}
</script>