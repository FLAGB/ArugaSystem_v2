<script setup>
import { ref, computed, reactive } from 'vue'
import AppSidebar from './Components/AppSidebar.vue'
import AppHeader from './Components/AppHeader.vue'

/* -------------------------------- Summary ---------------------------------- */
const summary = [
  { label: 'Vaccinations This Month', value: '1,284', icon: '💉', tint: 'bg-emerald-50' },
  { label: 'Total Registered Children', value: '3,942', icon: '🧒', tint: 'bg-teal-50' },
  { label: 'Completion Rate', value: '87%', icon: '📈', tint: 'bg-sky-50' },
  { label: 'Delayed Vaccinations', value: '56', icon: '⚠️', tint: 'bg-rose-50' },
  { label: 'Available Doses', value: '612', icon: '📦', tint: 'bg-amber-50' },
  { label: 'Reports Generated', value: '38', icon: '📄', tint: 'bg-violet-50' },
]

/* -------------------------------- Report categories -------------------------------- */
const categories = [
  {
    name: 'Patient Reports', icon: '🧒',
    reports: ['Registered Children', 'Active Patients', 'Archived Patients', 'Parent-Child Relationship Report'],
  },
  {
    name: 'Vaccination Reports', icon: '💉',
    reports: ['Vaccinations Performed', 'Children Fully Vaccinated', 'Delayed Vaccinations', 'Upcoming Vaccinations', 'Vaccinations by Month', 'Vaccinations by Vaccine'],
  },
  {
    name: 'Inventory Reports', icon: '📦',
    reports: ['Current Inventory', 'Low Stock Vaccines', 'Expired Vaccine Batches', 'Inventory Movement History', 'Received Stocks'],
  },
  {
    name: 'Queue Reports', icon: '🎫',
    reports: ['Daily Queue Summary', 'Completed Queue', 'Late Patients', 'Missed Vaccinations'],
  },
  {
    name: 'Notification Reports', icon: '🔔',
    reports: ['Notifications Sent', 'Failed Notifications', 'Announcement History', 'Reminder Logs'],
  },
  {
    name: 'User Reports', icon: '👥',
    reports: ['Registered Parents', 'Healthworkers', 'Staff Members', 'User Activity'],
  },
  {
    name: 'Audit Reports', icon: '📋',
    reports: ['Audit Logs', 'Login History', 'Account Status Changes'],
  },
]

const expandedCategory = ref('Vaccination Reports')
const toggleCategory = (name) => (expandedCategory.value = expandedCategory.value === name ? null : name)

const selectedReport = ref('Vaccinations by Month')
const selectReport = (name) => {
  selectedReport.value = name
  generatedReport.value = null
}

/* -------------------------------- Filters -------------------------------- */
const filters = reactive({
  period: 'This Month',
  customFrom: '',
  customTo: '',
  barangay: 'All Barangays',
  vaccine: 'All Vaccines',
  ageGroups: [],
})

const barangayOptions = ['All Barangays', 'San Isidro', 'Malinis', 'Poblacion', 'Bagong Silang', 'Sto. Niño', 'Malaya']
const vaccineOptions = ['All Vaccines', 'BCG', 'Hepatitis B', 'Pentavalent', 'OPV', 'IPV', 'PCV', 'MMR', 'DPT Booster']
const ageGroupOptions = ['Birth', '0–6 Months', '7–12 Months', '1–2 Years', '3–5 Years']

const toggleAgeGroup = (group) => {
  const idx = filters.ageGroups.indexOf(group)
  if (idx === -1) filters.ageGroups.push(group)
  else filters.ageGroups.splice(idx, 1)
}

const resetFilters = () => {
  filters.period = 'This Month'
  filters.customFrom = ''
  filters.customTo = ''
  filters.barangay = 'All Barangays'
  filters.vaccine = 'All Vaccines'
  filters.ageGroups = []
}

/* -------------------------------- Report generation (mock) -------------------------------- */
const chartData = [
  { label: 'Feb', value: 210 }, { label: 'Mar', value: 264 }, { label: 'Apr', value: 198 },
  { label: 'May', value: 302 }, { label: 'Jun', value: 276 }, { label: 'Jul', value: 224 },
]
const maxChartValue = Math.max(...chartData.map((d) => d.value))

const tableRows = [
  { date: 'Jul 01, 2026', vaccine: 'Pentavalent', barangay: 'San Isidro', count: 18, status: 'Completed' },
  { date: 'Jul 02, 2026', vaccine: 'OPV', barangay: 'Malinis', count: 14, status: 'Completed' },
  { date: 'Jul 03, 2026', vaccine: 'BCG', barangay: 'Poblacion', count: 9, status: 'Completed' },
  { date: 'Jul 04, 2026', vaccine: 'MMR', barangay: 'Bagong Silang', count: 22, status: 'Completed' },
  { date: 'Jul 05, 2026', vaccine: 'PCV', barangay: 'Sto. Niño', count: 16, status: 'Completed' },
]

const generatedReport = ref(null)
const isGenerating = ref(false)

const generateReport = () => {
  isGenerating.value = true
  setTimeout(() => {
    generatedReport.value = {
      name: selectedReport.value,
      dateGenerated: 'Jul 12, 2026, 3:42 PM',
      preparedBy: 'Renzo Miguel',
      dateRange: filters.period === 'Custom Date Range'
        ? `${filters.customFrom || '—'} to ${filters.customTo || '—'}`
        : filters.period,
      totalRecords: tableRows.reduce((sum, r) => sum + r.count, 0),
    }
    isGenerating.value = false
  }, 500)
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex text-slate-900">
    <!-- ============================ SIDEBAR ============================ -->
    <AppSidebar />

    <!-- ============================ MAIN ============================ -->
    <div class="flex-1 min-w-0 flex flex-col">
      <!-- Top navbar -->
     <AppHeader
  title="Reports"
  breadcrumb="System Administration / Reports"
  user-initials="RM"
/>

      <!-- Content -->
      <main class="p-6 space-y-6">
        <!-- Summary cards -->
        <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
          <div v-for="card in summary" :key="card.label" class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">{{ card.label }}</p>
              <div :class="card.tint" class="w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">{{ card.icon }}</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ card.value }}</p>
          </div>
        </section>

        <!-- Main layout: left panel + report preview -->
        <section class="grid grid-cols-1 xl:grid-cols-4 gap-6 items-start">
          <!-- LEFT PANEL -->
          <div class="xl:col-span-1 space-y-6">
            <!-- Report Categories -->
            <div class="bg-white border border-slate-200 rounded-xl shadow-sm p-4">
              <h2 class="text-sm font-bold text-slate-900 mb-3">Report Categories</h2>
              <div class="space-y-1">
                <div v-for="cat in categories" :key="cat.name">
                  <button
                    @click="toggleCategory(cat.name)"
                    class="w-full flex items-center justify-between gap-2 rounded-lg px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 transition-colors"
                  >
                    <span class="flex items-center gap-2 min-w-0">
                      <span class="text-base shrink-0" aria-hidden="true">{{ cat.icon }}</span>
                      <span class="truncate">{{ cat.name }}</span>
                    </span>
                    <span :class="expandedCategory === cat.name ? 'rotate-90' : ''" class="text-slate-400 text-xs transition-transform shrink-0">▶</span>
                  </button>
                  <div v-if="expandedCategory === cat.name" class="pl-9 pr-2 py-1 space-y-0.5">
                    <button
                      v-for="r in cat.reports"
                      :key="r"
                      @click="selectReport(r)"
                      :class="selectedReport === r ? 'bg-emerald-50 text-emerald-700 font-semibold' : 'text-slate-500 hover:bg-slate-50 hover:text-slate-800'"
                      class="w-full text-left rounded-lg px-2.5 py-1.5 text-xs transition-colors"
                    >
                      {{ r }}
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Filter Panel -->
            <div class="bg-white border border-slate-200 rounded-xl shadow-sm p-4 space-y-4">
              <h2 class="text-sm font-bold text-slate-900">Filters</h2>

              <div>
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Report Period</label>
                <select v-model="filters.period" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                  <option>Today</option>
                  <option>This Week</option>
                  <option>This Month</option>
                  <option>Custom Date Range</option>
                </select>
              </div>

              <div v-if="filters.period === 'Custom Date Range'" class="grid grid-cols-2 gap-2">
                <input v-model="filters.customFrom" type="date" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-2.5 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                <input v-model="filters.customTo" type="date" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-2.5 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
              </div>

              <div>
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Barangay <span class="text-slate-400 font-normal normal-case">(optional)</span></label>
                <select v-model="filters.barangay" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                  <option v-for="b in barangayOptions" :key="b">{{ b }}</option>
                </select>
              </div>

              <div>
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Vaccine <span class="text-slate-400 font-normal normal-case">(optional)</span></label>
                <select v-model="filters.vaccine" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                  <option v-for="v in vaccineOptions" :key="v">{{ v }}</option>
                </select>
              </div>

              <div>
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Age Group</label>
                <div class="flex flex-wrap gap-1.5">
                  <button
                    v-for="g in ageGroupOptions"
                    :key="g"
                    @click="toggleAgeGroup(g)"
                    :class="filters.ageGroups.includes(g) ? 'bg-emerald-600 text-white border-emerald-600' : 'border-slate-200 text-slate-600 hover:bg-slate-50'"
                    class="text-xs font-semibold px-2.5 py-1.5 rounded-lg border transition-colors"
                  >
                    {{ g }}
                  </button>
                </div>
              </div>

              <div class="flex items-center gap-2 pt-1">
                <button @click="generateReport" class="flex-1 text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors">Generate Report</button>
                <button @click="resetFilters" class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Reset</button>
              </div>
            </div>
          </div>

          <!-- REPORT PREVIEW -->
          <div class="xl:col-span-3 bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
            <div class="px-6 py-4 border-b border-slate-200 flex flex-wrap items-center justify-between gap-3">
              <div>
                <h2 class="text-base font-bold text-slate-900">{{ selectedReport }}</h2>
                <p class="text-xs text-slate-500 mt-0.5">Report Preview</p>
              </div>
              <div class="flex items-center gap-2">
                <button :disabled="!generatedReport" :class="generatedReport ? 'text-slate-600 hover:bg-slate-50' : 'text-slate-300 cursor-not-allowed'" class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 transition-colors">Print</button>
                <button :disabled="!generatedReport" :class="generatedReport ? 'text-slate-600 hover:bg-slate-50' : 'text-slate-300 cursor-not-allowed'" class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 transition-colors">Export PDF</button>
                <button :disabled="!generatedReport" :class="generatedReport ? 'text-slate-600 hover:bg-slate-50' : 'text-slate-300 cursor-not-allowed'" class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 transition-colors">Export Excel</button>
              </div>
            </div>

            <!-- Empty state -->
            <div v-if="!generatedReport && !isGenerating" class="flex flex-col items-center justify-center text-center py-24 px-6">
              <div class="w-14 h-14 rounded-xl bg-slate-50 flex items-center justify-center text-2xl mb-4">📄</div>
              <p class="text-sm font-semibold text-slate-700">No report generated yet</p>
              <p class="text-xs text-slate-400 mt-1 max-w-xs">Select a report from the categories panel, set your filters, then click Generate Report.</p>
            </div>

            <!-- Loading state -->
            <div v-else-if="isGenerating" class="flex flex-col items-center justify-center text-center py-24 px-6">
              <div class="w-8 h-8 border-2 border-emerald-500 border-t-transparent rounded-full animate-spin mb-4"></div>
              <p class="text-sm text-slate-500">Generating report...</p>
            </div>

            <!-- Report content -->
            <div v-else class="p-6 space-y-6">
              <!-- Report Details -->
              <div class="bg-slate-50 rounded-lg p-4 grid grid-cols-2 sm:grid-cols-5 gap-4">
                <div>
                  <p class="text-xs text-slate-500">Report Name</p>
                  <p class="text-sm font-semibold text-slate-900 mt-0.5">{{ generatedReport.name }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500">Date Generated</p>
                  <p class="text-sm font-semibold text-slate-900 mt-0.5">{{ generatedReport.dateGenerated }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500">Prepared By</p>
                  <p class="text-sm font-semibold text-slate-900 mt-0.5">{{ generatedReport.preparedBy }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500">Date Range</p>
                  <p class="text-sm font-semibold text-slate-900 mt-0.5">{{ generatedReport.dateRange }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500">Total Records</p>
                  <p class="text-sm font-semibold text-slate-900 mt-0.5">{{ generatedReport.totalRecords }}</p>
                </div>
              </div>

              <!-- Chart -->
              <div>
                <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Monthly Trend</h3>
                <div class="bg-slate-50 rounded-lg p-5">
                  <div class="flex items-end justify-between gap-3 h-40">
                    <div v-for="d in chartData" :key="d.label" class="flex-1 flex flex-col items-center justify-end h-full gap-2">
                      <span class="text-xs font-semibold text-slate-600">{{ d.value }}</span>
                      <div
                        class="w-full bg-emerald-500 rounded-t-md"
                        :style="{ height: (d.value / maxChartValue) * 100 + '%' }"
                      ></div>
                      <span class="text-xs text-slate-400">{{ d.label }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Table -->
              <div>
                <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Record Detail</h3>
                <div class="border border-slate-200 rounded-lg overflow-hidden">
                  <table class="w-full text-sm">
                    <thead>
                      <tr class="border-b border-slate-200 bg-slate-50/60">
                        <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-4 py-2.5">Date</th>
                        <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-4 py-2.5">Vaccine</th>
                        <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-4 py-2.5">Barangay</th>
                        <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-4 py-2.5">Count</th>
                        <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-4 py-2.5">Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(row, idx) in tableRows" :key="idx" class="border-b border-slate-100 last:border-0 hover:bg-slate-50 transition-colors">
                        <td class="px-4 py-2.5 text-slate-500 whitespace-nowrap">{{ row.date }}</td>
                        <td class="px-4 py-2.5 font-medium text-slate-900 whitespace-nowrap">{{ row.vaccine }}</td>
                        <td class="px-4 py-2.5 text-slate-500 whitespace-nowrap">{{ row.barangay }}</td>
                        <td class="px-4 py-2.5 text-slate-500 whitespace-nowrap">{{ row.count }}</td>
                        <td class="px-4 py-2.5">
                          <span class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full bg-emerald-50 text-emerald-700 whitespace-nowrap">
                            <span class="w-1.5 h-1.5 rounded-full bg-emerald-500"></span>
                            {{ row.status }}
                          </span>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                <div class="flex items-center justify-between mt-3">
                  <p class="text-xs text-slate-400">Showing 1–5 of {{ generatedReport.totalRecords }} records</p>
                  <div class="flex items-center gap-1">
                    <button class="w-7 h-7 rounded-lg border border-slate-200 text-slate-400 flex items-center justify-center text-xs hover:bg-slate-50 transition-colors">‹</button>
                    <button class="w-7 h-7 rounded-lg bg-emerald-600 text-white flex items-center justify-center text-xs">1</button>
                    <button class="w-7 h-7 rounded-lg border border-slate-200 text-slate-500 flex items-center justify-center text-xs hover:bg-slate-50 transition-colors">2</button>
                    <button class="w-7 h-7 rounded-lg border border-slate-200 text-slate-500 flex items-center justify-center text-xs hover:bg-slate-50 transition-colors">3</button>
                    <button class="w-7 h-7 rounded-lg border border-slate-200 text-slate-400 flex items-center justify-center text-xs hover:bg-slate-50 transition-colors">›</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
      </main>
    </div>
  </div>
</template>

<style scoped>
@media (prefers-reduced-motion: reduce) {
  * { transition-duration: 0.01ms !important; animation-duration: 0.01ms !important; }
}
</style>