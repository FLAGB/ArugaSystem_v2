<script setup>
import { ref } from 'vue'
import AppSidebar from './Components/AppSidebar.vue'
import AppHeader from './Components/AppHeader.vue'
import {
  Users, Baby, Syringe, AlertTriangle, UserPlus, Package,
  FileText, KeyRound, ClipboardList, ChevronRight, CircleCheck,
  CalendarClock,
} from 'lucide-vue-next'

const activeNav = ref('Dashboard')
function handleLogout() {
  console.log('logout clicked')
}
function goTo(route) {
  console.log('navigate to', route)
}

/* Mock data — replace with API calls once the backend endpoints are ready */
const summaryCards = [
  { label: 'Total Users', value: 128, description: 'Registered system users', icon: Users, tint: 'bg-teal-50', text: 'text-teal-700' },
  { label: 'Total Patients', value: 342, description: 'Registered children', icon: Baby, tint: 'bg-emerald-50', text: 'text-emerald-700' },
  { label: 'Vaccinations This Month', value: 186, description: 'Vaccinations recorded', icon: Syringe, tint: 'bg-sky-50', text: 'text-sky-700' },
  { label: 'Low Stock Vaccines', value: 4, description: 'Vaccines need attention', icon: AlertTriangle, tint: 'bg-amber-50', text: 'text-amber-700' },
]

const userBreakdown = [
  { role: 'Parent / Guardian', count: 82 },
  { role: 'Healthcare', count: 24 },
  { role: 'Staff', count: 21 },
  { role: 'System Admin', count: 1 },
]
const maxUserCount = Math.max(...userBreakdown.map((u) => u.count))

const vaccinationTrend = [
  { month: 'Mar', count: 120 },
  { month: 'Apr', count: 145 },
  { month: 'May', count: 132 },
  { month: 'Jun', count: 168 },
  { month: 'Jul', count: 154 },
  { month: 'Aug', count: 186 },
]
const maxVaccinationCount = Math.max(...vaccinationTrend.map((v) => v.count))

const statusMeta = {
  Good: { tint: 'bg-emerald-50', text: 'text-emerald-700', dot: 'bg-emerald-500' },
  'Low Stock': { tint: 'bg-amber-50', text: 'text-amber-700', dot: 'bg-amber-500' },
  Critical: { tint: 'bg-red-100', text: 'text-red-800', dot: 'bg-red-700' },
}
const vaccineInventory = [
  { name: 'BCG', stock: 42, status: 'Good' },
  { name: 'Hepatitis B', stock: 18, status: 'Low Stock' },
  { name: 'Pentavalent', stock: 65, status: 'Good' },
  { name: 'Polio', stock: 12, status: 'Low Stock' },
  { name: 'MMR', stock: 51, status: 'Good' },
]

const recentActivity = [
  { icon: UserPlus, description: 'Healthcare account created', detail: 'Juan Dela Cruz', time: '10 minutes ago' },
  { icon: Package, description: 'Vaccine inventory updated', detail: 'Hepatitis B', time: '32 minutes ago' },
  { icon: Baby, description: 'New patient record registered', detail: 'Maria Santos', time: '1 hour ago' },
  { icon: CalendarClock, description: 'Vaccination schedule updated', detail: 'Pentavalent series', time: '2 hours ago' },
  { icon: KeyRound, description: 'User account password reset', detail: 'Ana Reyes', time: '3 hours ago' },
  { icon: FileText, description: 'System Admin viewed vaccination report', detail: 'August summary', time: '5 hours ago' },
]

const quickActions = [
  { label: 'Add User', icon: UserPlus, route: '/system-admin/user-management' },
  { label: 'Add Patient', icon: Baby, route: '/system-admin/patients' },
  { label: 'Add Vaccine', icon: Syringe, route: '/system-admin/vaccines' },
  { label: 'Manage Inventory', icon: Package, route: '/system-admin/inventory' },
  { label: 'View Reports', icon: ClipboardList, route: '/system-admin/reports' },
]

const notifications = [
  { icon: AlertTriangle, text: '4 vaccines are running low', tint: 'text-amber-600' },
  { icon: KeyRound, text: '2 user accounts require password changes', tint: 'text-sky-600' },
  { icon: ClipboardList, text: '1 vaccination schedule rule needs review', tint: 'text-amber-600' },
  { icon: CircleCheck, text: 'Daily system backup completed', tint: 'text-emerald-600' },
]

const todaysActivity = [
  { label: 'New Users', value: 3 },
  { label: 'New Patients', value: 7 },
  { label: 'Vaccinations Recorded', value: 18 },
  { label: 'Inventory Updates', value: 5 },
]
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex text-slate-900">
    <AppSidebar v-model:active-nav="activeNav" @logout="handleLogout" />

    <div class="flex-1 min-w-0 flex flex-col">
      <AppHeader title="Dashboard" breadcrumb="System Administration / Dashboard" />

      <main class="p-6 space-y-6">
        <!-- Welcome / system status -->
        <section class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
          <div>
            <h1 class="text-xl font-bold text-slate-900">Good morning, Admin</h1>
            <p class="text-sm text-slate-500">Here's an overview of the Aruga system today.</p>
          </div>
          <div class="flex items-center gap-2 text-xs font-medium text-emerald-700 bg-emerald-50 border border-emerald-100 rounded-full px-3 py-1.5 w-fit">
            <span class="w-1.5 h-1.5 rounded-full bg-emerald-500"></span>
            System Operational
          </div>
        </section>

        <!-- Overview cards -->
        <section class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
          <div v-for="card in summaryCards" :key="card.label" class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">{{ card.label }}</p>
              <div :class="card.tint" class="w-8 h-8 rounded-lg flex items-center justify-center shrink-0">
                <component :is="card.icon" :class="card.text" class="w-4 h-4" />
              </div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ card.value }}</p>
            <p class="mt-1 text-xs text-slate-500">{{ card.description }}</p>
          </div>
        </section>

        <section class="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
          <!-- Vaccination activity -->
          <div class="lg:col-span-2 bg-white border border-slate-200 rounded-xl shadow-sm p-5">
            <h2 class="text-sm font-bold text-slate-900 mb-1">Vaccination Activity</h2>
            <p class="text-xs text-slate-500 mb-5">Vaccinations recorded over the past 6 months</p>
            <div class="flex items-end gap-4 h-48">
              <div v-for="point in vaccinationTrend" :key="point.month" class="flex-1 flex flex-col items-center gap-2 h-full justify-end">
                <span class="text-xs font-semibold text-slate-600">{{ point.count }}</span>
                <div
                  class="w-full max-w-10 bg-emerald-500 rounded-t-md"
                  :style="{ height: (point.count / maxVaccinationCount) * 100 + '%' }"
                ></div>
                <span class="text-xs text-slate-400">{{ point.month }}</span>
              </div>
            </div>
          </div>

          <!-- User overview -->
          <div class="bg-white border border-slate-200 rounded-xl shadow-sm p-5">
            <h2 class="text-sm font-bold text-slate-900 mb-4">User Overview</h2>
            <div class="space-y-4">
              <div v-for="entry in userBreakdown" :key="entry.role">
                <div class="flex items-center justify-between text-xs mb-1.5">
                  <span class="font-medium text-slate-600">{{ entry.role }}</span>
                  <span class="font-semibold text-slate-900">{{ entry.count }}</span>
                </div>
                <div class="w-full h-2 rounded-full bg-slate-100">
                  <div class="h-2 rounded-full bg-emerald-500" :style="{ width: (entry.count / maxUserCount) * 100 + '%' }"></div>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
          <!-- Vaccine inventory -->
          <div class="lg:col-span-2 bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
            <div class="flex items-center justify-between px-5 py-4 border-b border-slate-200">
              <h2 class="text-sm font-bold text-slate-900">Vaccine Inventory</h2>
              <button @click="goTo('/system-admin/inventory')" class="text-xs font-semibold text-emerald-700 hover:text-emerald-800 flex items-center gap-1">
                View Inventory
                <ChevronRight class="w-3.5 h-3.5" />
              </button>
            </div>
            <div class="max-h-72 overflow-y-auto divide-y divide-slate-100">
              <div v-for="vaccine in vaccineInventory" :key="vaccine.name" class="flex items-center justify-between px-5 py-3">
                <span class="text-sm font-medium text-slate-900">{{ vaccine.name }}</span>
                <div class="flex items-center gap-4">
                  <span class="text-xs text-slate-500 whitespace-nowrap">{{ vaccine.stock }} doses</span>
                  <span :class="[statusMeta[vaccine.status].tint, statusMeta[vaccine.status].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full whitespace-nowrap">
                    <span :class="statusMeta[vaccine.status].dot" class="w-1.5 h-1.5 rounded-full"></span>
                    {{ vaccine.status }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Notifications -->
          <div class="bg-white border border-slate-200 rounded-xl shadow-sm p-5">
            <h2 class="text-sm font-bold text-slate-900 mb-4">Notifications</h2>
            <div class="space-y-3">
              <div v-for="(note, idx) in notifications" :key="idx" class="flex items-start gap-2.5">
                <component :is="note.icon" :class="note.tint" class="w-4 h-4 mt-0.5 shrink-0" />
                <p class="text-sm text-slate-700">{{ note.text }}</p>
              </div>
            </div>
            <button @click="goTo('/system-admin/notifications')" class="mt-4 w-full text-xs font-semibold text-center px-3 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">
              View All Notifications
            </button>
          </div>
        </section>

        <section class="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
          <!-- Recent system activity -->
          <div class="lg:col-span-2 bg-white border border-slate-200 rounded-xl shadow-sm p-5">
            <h2 class="text-sm font-bold text-slate-900 mb-4">Recent System Activity</h2>
            <div class="space-y-4">
              <div v-for="(activity, idx) in recentActivity" :key="idx" class="flex items-start gap-3">
                <div class="w-8 h-8 rounded-lg bg-slate-50 flex items-center justify-center shrink-0">
                  <component :is="activity.icon" class="w-4 h-4 text-slate-500" />
                </div>
                <div class="min-w-0">
                  <p class="text-sm font-medium text-slate-900">{{ activity.description }}</p>
                  <p class="text-xs text-slate-500">{{ activity.detail }} · {{ activity.time }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick actions -->
          <div class="bg-white border border-slate-200 rounded-xl shadow-sm p-5">
            <h2 class="text-sm font-bold text-slate-900 mb-4">Quick Actions</h2>
            <div class="grid grid-cols-1 gap-2">
              <button
                v-for="action in quickActions"
                :key="action.label"
                @click="goTo(action.route)"
                class="flex items-center gap-2.5 rounded-lg border border-slate-200 px-3.5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50 transition-colors"
              >
                <component :is="action.icon" class="w-4 h-4 text-emerald-600 shrink-0" />
                {{ action.label }}
              </button>
            </div>
          </div>
        </section>

        <!-- Today's activity summary -->
        <section class="bg-white border border-slate-200 rounded-xl shadow-sm px-5 py-4">
          <h2 class="text-xs font-semibold uppercase tracking-wide text-slate-500 mb-3">Today's Activity</h2>
          <div class="flex flex-wrap gap-x-8 gap-y-2">
            <div v-for="item in todaysActivity" :key="item.label" class="flex items-center gap-2">
              <span class="text-xs text-slate-500">{{ item.label }}</span>
              <span class="text-sm font-bold text-slate-900">{{ item.value }}</span>
            </div>
          </div>
        </section>
      </main>
    </div>
  </div>
</template>