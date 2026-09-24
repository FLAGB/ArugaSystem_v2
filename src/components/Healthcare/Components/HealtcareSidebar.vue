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
      <button
  @click="handleLogout"
  class="w-full flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium text-slate-500 hover:bg-slate-50 hover:text-slate-900 transition-colors"
>
  <LogOut class="w-4 h-4" />
  Logout
</button>
    </div>
  </aside>
</template><script setup>
import { useRouter } from 'vue-router'
import { LogOut, Home, Users, ListChecks, Calendar, Settings, FileText, Syringe } from 'lucide-vue-next'
import { logout } from '@/utils/auth'
import logoIcon from '@/assets/logo-icon.svg'

const router = useRouter()

const navItems = [
  {
    id: 'home',
    label: 'Home',
    path: '/healthcare/home',
    icon: Home
  },
  {
    id: 'queue',
    label: 'Queue',
    path: '/healthcare/queue',
    icon: ListChecks
  },
  {
    id: 'calendar',
    label: 'Calendar',
    path: '/healthcare/calendar',
    icon: Calendar
  },
  {
    id: 'patients',
    label: 'Patients',
    path: '/healthcare/patients',
    icon: Users
  },
  {
    id: 'vaccination-records',
    label: 'Vaccination Records',
    path: '/healthcare/vaccination-records',
    icon: Syringe
  },
  {
    id: 'reports',
    label: 'Reports',
    path: '/healthcare/reports',
    icon: FileText
  },
  {
    id: 'account',
    label: 'My Account',
    path: '/healthcare/accounts',
    icon: Settings
  }
]

function handleLogout() {
  logout()
  router.push('/')
}
</script>