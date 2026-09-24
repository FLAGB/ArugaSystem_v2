<script setup>
import { ref, computed, reactive } from 'vue'
import AppSidebar from './Components/AppSidebar.vue'
import AppHeader from './Components/AppHeader.vue'

/* ----------------------------- Sidebar state ------------------------------ */
const isCollapsed = ref(false)
const toggleSidebar = () => (isCollapsed.value = !isCollapsed.value)



/* -------------------------------- Status meta -------------------------------- */
const statusMeta = {
  Success: { tint: 'bg-emerald-50', text: 'text-emerald-700', dot: 'bg-emerald-500' },
  Failed: { tint: 'bg-rose-50', text: 'text-rose-600', dot: 'bg-rose-500' },
  Warning: { tint: 'bg-amber-50', text: 'text-amber-700', dot: 'bg-amber-500' },
}

const roleOptions = ['Parent', 'Staff', 'Healthworker', 'System Admin']
const moduleOptions = ['User Management', 'Patient Management', 'Vaccine Management', 'Inventory', 'Notifications', 'Reports', 'Authentication']
const actionOptions = ['Login', 'Logout', 'Create', 'Update', 'Archive', 'Restore', 'Link Parent', 'Unlink Parent', 'Receive Stock', 'Adjust Inventory', 'Vaccinate Child', 'Generate Report']

/* -------------------------------- Log data -------------------------------- */
const logs = ref([
  {
    id: 1, timestamp: 'Jul 12, 2026, 9:15 AM', user: 'Renzo Miguel', role: 'System Admin', module: 'User Management',
    action: 'Create', affectedRecord: 'Staff Account – Carlo Reyes', ipAddress: '192.168.1.14', device: 'Chrome on Windows 11',
    status: 'Success', description: 'Created a new Staff account with clinic front-desk permissions.',
    oldValue: '—', newValue: 'Status: Active, Role: Staff',
  },
  {
    id: 2, timestamp: 'Jul 12, 2026, 9:22 AM', user: 'Carlo Reyes', role: 'Staff', module: 'Patient Management',
    action: 'Create', affectedRecord: 'Patient PT-00234 – Isabela Aquino', ipAddress: '192.168.1.28', device: 'Chrome on Windows 11',
    status: 'Success', description: 'Registered a new patient and linked mother as primary guardian.',
    oldValue: '—', newValue: 'Patient Status: Active',
  },
  {
    id: 3, timestamp: 'Jul 12, 2026, 9:40 AM', user: 'Elena Cruz', role: 'Healthworker', module: 'Patient Management',
    action: 'Vaccinate Child', affectedRecord: 'Patient PT-00231 – Miguel Bautista', ipAddress: '192.168.1.31', device: 'Safari on iPad',
    status: 'Success', description: 'Administered MMR vaccine (1st dose) during scheduled visit.',
    oldValue: 'Latest Vaccine: Pentavalent (3rd dose)', newValue: 'Latest Vaccine: MMR (1st dose)',
  },
  {
    id: 4, timestamp: 'Jul 12, 2026, 10:05 AM', user: 'Renzo Miguel', role: 'System Admin', module: 'Inventory',
    action: 'Receive Stock', affectedRecord: 'Batch MMR-2026-015', ipAddress: '192.168.1.14', device: 'Chrome on Windows 11',
    status: 'Success', description: 'Received new vaccine stock delivery and added to inventory.',
    oldValue: 'Remaining: 27', newValue: 'Remaining: 127 (+100 doses received)',
  },
  {
    id: 5, timestamp: 'Jul 12, 2026, 10:22 AM', user: 'Unknown', role: 'Parent', module: 'Authentication',
    action: 'Login', affectedRecord: 'Account – msantos', ipAddress: '203.177.42.9', device: 'Chrome on Android',
    status: 'Failed', description: 'Failed login attempt due to incorrect password (3rd attempt).',
    oldValue: '—', newValue: '—',
  },
  {
    id: 6, timestamp: 'Jul 12, 2026, 11:03 AM', user: 'Bea Fernandez', role: 'Healthworker', module: 'Patient Management',
    action: 'Update', affectedRecord: 'Patient PT-00198 – Diego Ramos', ipAddress: '192.168.1.42', device: 'Chrome on Windows 11',
    status: 'Warning', description: 'Updated patient status to Inactive without a documented reason on first attempt.',
    oldValue: 'Status: Active', newValue: 'Status: Inactive',
  },
  {
    id: 7, timestamp: 'Jul 12, 2026, 11:47 AM', user: 'Renzo Miguel', role: 'System Admin', module: 'Vaccine Management',
    action: 'Update', affectedRecord: 'Vaccine VX-006 – PCV', ipAddress: '192.168.1.14', device: 'Chrome on Windows 11',
    status: 'Success', description: 'Updated dose interval information for Pneumococcal Conjugate Vaccine.',
    oldValue: 'Interval: 6 weeks apart', newValue: 'Interval: 4 weeks apart',
  },
  {
    id: 8, timestamp: 'Jul 12, 2026, 1:12 PM', user: 'Jhun Aquino', role: 'Staff', module: 'Notifications',
    action: 'Generate Report', affectedRecord: 'Reminder Logs Report', ipAddress: '192.168.1.55', device: 'Edge on Windows 10',
    status: 'Success', description: 'Generated Reminder Logs report for the current month.',
    oldValue: '—', newValue: '—',
  },
  {
    id: 9, timestamp: 'Jul 12, 2026, 1:40 PM', user: 'Angeli Torres', role: 'Parent', module: 'Authentication',
    action: 'Login', affectedRecord: 'Account – atorres', ipAddress: '203.177.44.18', device: 'Safari on iPhone',
    status: 'Success', description: 'Successful login to parent portal.',
    oldValue: '—', newValue: '—',
  },
  {
    id: 10, timestamp: 'Jul 12, 2026, 2:05 PM', user: 'Renzo Miguel', role: 'System Admin', module: 'Inventory',
    action: 'Adjust Inventory', affectedRecord: 'Batch BT-2025-176 – IPV', ipAddress: '192.168.1.14', device: 'Chrome on Windows 11',
    status: 'Success', description: 'Marked remaining doses as expired following expiration date.',
    oldValue: 'Remaining: 8', newValue: 'Remaining: 0 (Status: Expired)',
  },
])

/* ---------------------------- Toolbar / filters ---------------------------- */
const searchQuery = ref('')
const roleFilter = ref('All')
const moduleFilter = ref('All')
const actionFilter = ref('All')
const dateRangeFilter = ref('Today')
const customFrom = ref('')
const customTo = ref('')

const appliedFilters = reactive({
  role: 'All', module: 'All', action: 'All', dateRange: 'Today',
})

const applyFilters = () => {
  Object.assign(appliedFilters, {
    role: roleFilter.value, module: moduleFilter.value, action: actionFilter.value, dateRange: dateRangeFilter.value,
  })
}

const filteredLogs = computed(() =>
  logs.value.filter((l) => {
    const q = searchQuery.value.trim().toLowerCase()
    const matchesSearch =
      !q || l.user.toLowerCase().includes(q) || l.module.toLowerCase().includes(q) ||
      l.action.toLowerCase().includes(q) || l.affectedRecord.toLowerCase().includes(q)
    const matchesRole = appliedFilters.role === 'All' || l.role === appliedFilters.role
    const matchesModule = appliedFilters.module === 'All' || l.module === appliedFilters.module
    const matchesAction = appliedFilters.action === 'All' || l.action === appliedFilters.action
    return matchesSearch && matchesRole && matchesModule && matchesAction
  })
)

/* -------------------------------- Summary ---------------------------------- */
const summary = computed(() => ({
  totalToday: logs.value.length,
  successfulLogins: logs.value.filter((l) => l.action === 'Login' && l.status === 'Success').length,
  failedLogins: logs.value.filter((l) => l.action === 'Login' && l.status === 'Failed').length,
  userChanges: logs.value.filter((l) => l.module === 'User Management').length,
  patientUpdates: logs.value.filter((l) => l.module === 'Patient Management').length,
  inventoryTx: logs.value.filter((l) => l.module === 'Inventory').length,
}))

/* -------------------------------- Details drawer ---------------------------- */
const showDrawer = ref(false)
const selectedLog = ref(null)
const openDrawer = (log) => {
  selectedLog.value = log
  showDrawer.value = true
}
const closeDrawer = () => (showDrawer.value = false)

/* -------------------------------- Timeline ---------------------------- */
const timeline = computed(() => logs.value.slice(0, 6))
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex text-slate-900">
    <!-- ============================ SIDEBAR ============================ -->
    <AppSidebar/>

    <!-- ============================ MAIN ============================ -->
    <div class="flex-1 min-w-0 flex flex-col">
      <!-- Top navbar -->
      <AppHeader
  title="Audit Logs"
  breadcrumb="System Administration / Audit Logs"
  user-initials="RM"
/>

      <!-- Content -->
      <main class="p-6 space-y-6">
        <!-- Summary cards -->
        <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Total Logs Today</p>
              <div class="bg-teal-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">📋</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.totalToday }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Successful Logins</p>
              <div class="bg-emerald-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">✅</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.successfulLogins }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Failed Logins</p>
              <div class="bg-rose-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">⛔</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.failedLogins }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">User Account Changes</p>
              <div class="bg-sky-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">👥</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.userChanges }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Patient Record Updates</p>
              <div class="bg-amber-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">🧒</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.patientUpdates }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Inventory Transactions</p>
              <div class="bg-violet-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">📦</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.inventoryTx }}</p>
          </div>
        </section>

        <!-- Toolbar -->
        <section class="bg-white border border-slate-200 rounded-xl p-4 shadow-sm space-y-3">
          <div class="relative">
            <span class="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-sm" aria-hidden="true">🔍</span>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by user name, module, action, or record ID..."
              class="w-full pl-9 pr-3 py-2 text-sm rounded-lg border border-slate-200 bg-slate-50 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
            />
          </div>

          <div class="flex flex-col lg:flex-row lg:items-center gap-3">
            <select
              v-model="roleFilter"
              class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
            >
              <option value="All">All Roles</option>
              <option v-for="r in roleOptions" :key="r">{{ r }}</option>
            </select>

            <select
              v-model="moduleFilter"
              class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
            >
              <option value="All">All Modules</option>
              <option v-for="m in moduleOptions" :key="m">{{ m }}</option>
            </select>

            <select
              v-model="actionFilter"
              class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
            >
              <option value="All">All Actions</option>
              <option v-for="a in actionOptions" :key="a">{{ a }}</option>
            </select>

            <select
              v-model="dateRangeFilter"
              class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
            >
              <option>Today</option>
              <option>This Week</option>
              <option>This Month</option>
              <option>Custom Date</option>
            </select>

            <div v-if="dateRangeFilter === 'Custom Date'" class="flex items-center gap-2">
              <input v-model="customFrom" type="date" class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-2.5 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
              <span class="text-xs text-slate-400">to</span>
              <input v-model="customTo" type="date" class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-2.5 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
            </div>

            <div class="flex items-center gap-2 shrink-0 lg:ml-auto">
              <button @click="applyFilters" class="text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500">
                Apply Filters
              </button>
              <button class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500">
                Export Logs
              </button>
            </div>
          </div>
        </section>

        <!-- Audit log table -->
        <section class="bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-sm">
              <thead>
                <tr class="border-b border-slate-200 bg-slate-50/60">
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Date &amp; Time</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">User</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Role</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Module</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Action</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Affected Record</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">IP Address</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Status</th>
                  <th class="text-right font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="log in filteredLogs"
                  :key="log.id"
                  class="border-b border-slate-100 last:border-0 hover:bg-slate-50 transition-colors"
                >
                  <td class="px-5 py-3 text-slate-500 whitespace-nowrap">{{ log.timestamp }}</td>
                  <td class="px-3 py-3 font-semibold text-slate-900 whitespace-nowrap">{{ log.user }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ log.role }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ log.module }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ log.action }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap max-w-[220px] truncate" :title="log.affectedRecord">{{ log.affectedRecord }}</td>
                  <td class="px-3 py-3 text-slate-400 font-mono text-xs whitespace-nowrap">{{ log.ipAddress }}</td>
                  <td class="px-3 py-3">
                    <span :class="[statusMeta[log.status].tint, statusMeta[log.status].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full whitespace-nowrap">
                      <span :class="statusMeta[log.status].dot" class="w-1.5 h-1.5 rounded-full"></span>
                      {{ log.status }}
                    </span>
                  </td>
                  <td class="px-5 py-3 text-right">
                    <button
                      @click="openDrawer(log)"
                      class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors"
                    >
                      View
                    </button>
                  </td>
                </tr>

                <tr v-if="filteredLogs.length === 0">
                  <td colspan="9" class="px-5 py-12 text-center text-sm text-slate-400">No logs match your search or filters.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <!-- Log Timeline -->
        <section class="bg-white border border-slate-200 rounded-xl shadow-sm p-5">
          <h2 class="text-sm font-bold text-slate-900 mb-4">Recent Activity Timeline</h2>
          <div class="space-y-0">
            <div v-for="(log, idx) in timeline" :key="'t-' + log.id" class="flex gap-3">
              <div class="flex flex-col items-center">
                <span :class="statusMeta[log.status].dot" class="w-2.5 h-2.5 rounded-full shrink-0 mt-1"></span>
                <span v-if="idx !== timeline.length - 1" class="w-px flex-1 bg-slate-200"></span>
              </div>
              <div class="flex-1 min-w-0 pb-5">
                <div class="flex items-center justify-between gap-2">
                  <p class="text-xs font-semibold text-slate-500">{{ log.timestamp.split(', ').slice(-1)[0] }}</p>
                  <span :class="[statusMeta[log.status].tint, statusMeta[log.status].text]" class="text-xs font-semibold px-2 py-0.5 rounded-full">{{ log.status }}</span>
                </div>
                <p class="text-sm text-slate-800 mt-1">
                  <span class="font-semibold">{{ log.user }}</span> ({{ log.role }}) — {{ log.description }}
                </p>
                <p class="text-xs text-slate-400 mt-0.5">{{ log.module }} · {{ log.affectedRecord }}</p>
              </div>
            </div>
          </div>
        </section>
      </main>
    </div>

    <!-- ============================ LOG DETAILS DRAWER ============================ -->
    <transition name="fade">
      <div v-if="showDrawer" class="fixed inset-0 bg-slate-900/30 z-40" @click="closeDrawer"></div>
    </transition>
    <transition name="slide">
      <aside v-if="showDrawer" class="fixed top-0 right-0 h-screen w-full max-w-sm bg-white border-l border-slate-200 shadow-lg z-50 flex flex-col">
        <div class="h-[70px] flex items-center justify-between px-5 border-b border-slate-200 shrink-0">
          <h2 class="text-sm font-bold text-slate-900">Audit Log Details</h2>
          <button @click="closeDrawer" class="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-50 transition-colors">✕</button>
        </div>

        <div v-if="selectedLog" class="flex-1 overflow-y-auto p-6 space-y-6">
          <div>
            <span :class="[statusMeta[selectedLog.status].tint, statusMeta[selectedLog.status].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full mb-3">
              <span :class="statusMeta[selectedLog.status].dot" class="w-1.5 h-1.5 rounded-full"></span>
              {{ selectedLog.status }}
            </span>
            <p class="text-base font-bold text-slate-900">{{ selectedLog.action }} — {{ selectedLog.module }}</p>
            <p class="text-xs text-slate-500 mt-1">{{ selectedLog.timestamp }}</p>
          </div>

          <div>
            <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Description</h3>
            <p class="text-sm text-slate-700 leading-relaxed bg-slate-50 rounded-lg p-4">{{ selectedLog.description }}</p>
          </div>

          <div class="bg-slate-50 rounded-lg divide-y divide-slate-200">
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Performed By</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedLog.user }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">User Role</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedLog.role }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Module</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedLog.module }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Action</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedLog.action }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Affected Record</span>
              <span class="text-sm font-medium text-slate-900 text-right ml-4">{{ selectedLog.affectedRecord }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">IP Address</span>
              <span class="text-sm font-medium text-slate-900 font-mono">{{ selectedLog.ipAddress }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Device / Browser</span>
              <span class="text-sm font-medium text-slate-900 text-right ml-4">{{ selectedLog.device }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Result</span>
              <span :class="[statusMeta[selectedLog.status].text]" class="text-sm font-semibold">{{ selectedLog.status }}</span>
            </div>
          </div>

          <div>
            <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Value Change</h3>
            <div class="grid grid-cols-1 gap-3">
              <div class="rounded-lg border border-rose-100 bg-rose-50/60 px-4 py-3">
                <p class="text-xs font-semibold text-rose-500 mb-1">Old Value</p>
                <p class="text-sm text-slate-700">{{ selectedLog.oldValue }}</p>
              </div>
              <div class="rounded-lg border border-emerald-100 bg-emerald-50/60 px-4 py-3">
                <p class="text-xs font-semibold text-emerald-600 mb-1">New Value</p>
                <p class="text-sm text-slate-700">{{ selectedLog.newValue }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="border-t border-slate-200 p-4 flex items-center gap-2 shrink-0">
          <button @click="closeDrawer" class="flex-1 text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Close</button>
        </div>
      </aside>
    </transition>
  </div>
</template>

<style scoped>
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
.slide-enter-active, .slide-leave-active { transition: transform 0.25s ease; }
.slide-enter-from, .slide-leave-to { transform: translateX(100%); }

@media (prefers-reduced-motion: reduce) {
  * { transition-duration: 0.01ms !important; }
}
</style>