<template>
  <div class="flex h-screen bg-stone-50 antialiased text-stone-900" style="font-family: 'Inter','Segoe UI',sans-serif;">

    <StaffSidebar />

    <!-- MAIN SECTION -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <StaffTopbar title="Calendar" breadcrumb="Aruga / Calendar">
        <button @click="goPrevMonth" class="p-2 hover:bg-slate-100 rounded-lg text-slate-500"><ChevronLeft class="w-5 h-5" /></button>
        <span class="px-4 py-2 font-bold text-sm w-32 text-center">{{ monthLabel }}</span>
        <button @click="goNextMonth" class="p-2 hover:bg-slate-100 rounded-lg text-slate-500"><ChevronRight class="w-5 h-5" /></button>
        <button @click="goToday" class="ml-2 px-3 py-1.5 text-xs font-bold text-emerald-700 border border-emerald-200 rounded-lg hover:bg-emerald-50">Today</button>
        <button @click="fetchRecords" class="ml-2 p-2 hover:bg-slate-100 rounded-lg text-slate-400 hover:text-emerald-600">
          <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': isLoading }" />
        </button>
      </StaffTopbar>

      <main class="flex-1 p-8 overflow-y-auto custom-scrollbar">
        <div class="max-w-7xl mx-auto">

          <!-- CALENDAR GRID -->
          <div class="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
            <!-- Days of Week Header -->
            <div class="grid grid-cols-7 bg-slate-50 border-b border-slate-200">
              <div v-for="day in ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']" :key="day"
                class="py-3 text-center text-[10px] font-bold text-slate-400 uppercase tracking-widest">
                {{ day }}
              </div>
            </div>

            <!-- Calendar Days -->
            <div class="grid grid-cols-7 auto-rows-[120px]">
              <div v-for="(cell, idx) in calendarCells" :key="idx"
                class="border-r border-b border-slate-100 p-2 transition-colors relative overflow-hidden"
                :class="cell.inMonth ? 'hover:bg-slate-50' : 'bg-slate-50/40'">
                <span v-if="cell.day" class="text-xs font-bold" :class="cell.isToday ? 'text-emerald-600' : (cell.inMonth ? 'text-slate-400' : 'text-slate-300')">
                  {{ cell.day }}
                </span>

                <!-- Appointment Pills, from real VaccinationRecords for this date -->
                <div class="mt-1 space-y-1 overflow-y-auto max-h-[85px] pr-0.5">
                  <div v-for="(ev, i) in cell.events" :key="i"
                    class="p-1 rounded text-[10px] font-bold border truncate"
                    :class="pillClass(i)"
                    :title="`${ev.time} - ${ev.childName} (${ev.vaccineName})`">
                    {{ ev.time }} - {{ ev.childName }} ({{ ev.vaccineName }})
                  </div>
                </div>
              </div>
            </div>
          </div>

          <p v-if="!isLoading && records.length === 0" class="text-xs text-slate-400 mt-4 text-center">
            No vaccination records found for {{ monthLabel }}.
          </p>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import axios from 'axios'
import { ChevronLeft, ChevronRight, RefreshCw } from 'lucide-vue-next'
import StaffSidebar from './StaffSidebar.vue'
import StaffTopbar from './StaffTopbar.vue'

const API_BASE = (import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '') + '/api'

const isLoading = ref(false)
const records = ref([]) // raw rows from VaccinationRecords/all

// Real month/year state — starts on the actual current month instead of a
// hardcoded "May 2026", and the prev/next buttons now actually change it.
const today = new Date()
const viewYear = ref(today.getFullYear())
const viewMonth = ref(today.getMonth()) // 0-indexed

const monthLabel = computed(() =>
  new Date(viewYear.value, viewMonth.value, 1).toLocaleDateString('en-US', { month: 'long', year: 'numeric' })
)

const goPrevMonth = () => {
  if (viewMonth.value === 0) { viewMonth.value = 11; viewYear.value-- } else { viewMonth.value-- }
}
const goNextMonth = () => {
  if (viewMonth.value === 11) { viewMonth.value = 0; viewYear.value++ } else { viewMonth.value++ }
}
const goToday = () => { viewYear.value = today.getFullYear(); viewMonth.value = today.getMonth() }

// NOTE: this pulls from VaccinationRecords/all (administered vaccinations, keyed by
// vaccinationDate) since that's the confirmed real, working endpoint that returns
// per-record dates — same shaped DTO (childName/vaccineName/vaccinationDate) that
// DoctorVaccinationRecords.vue and the Healthcare pages already read correctly.
// If what you actually want here is UPCOMING/scheduled appointments rather than
// already-administered doses, that likely lives behind whatever endpoint backs the
// separate "Vaccine Schedule" page instead — that file wasn't part of this batch,
// so this calendar can't be pointed at it yet.
const fetchRecords = async () => {
  isLoading.value = true
  try {
    const res = await axios.get(`${API_BASE}/VaccinationRecords/all`)
    records.value = res.data
  } catch (err) {
    console.error('Fetch vaccination records error:', err)
  } finally {
    isLoading.value = false
  }
}

// Group this month's records by day-of-month for fast lookup while rendering.
const eventsByDay = computed(() => {
  const map = {}
  for (const r of records.value) {
    const raw = r.dateAdministered ?? r.vaccinationDate
    if (!raw) continue
    const d = new Date(raw)
    if (d.getFullYear() !== viewYear.value || d.getMonth() !== viewMonth.value) continue
    const day = d.getDate()
    if (!map[day]) map[day] = []
    map[day].push({
      time: d.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false }),
      childName: r.childName || 'Unknown',
      vaccineName: r.vaccineName || 'Unknown',
    })
  }
  return map
})

const calendarCells = computed(() => {
  const firstOfMonth = new Date(viewYear.value, viewMonth.value, 1)
  const startWeekday = firstOfMonth.getDay() // 0 = Sun
  const daysInMonth = new Date(viewYear.value, viewMonth.value + 1, 0).getDate()

  const cells = []
  for (let i = 0; i < startWeekday; i++) cells.push({ day: null, inMonth: false, events: [] })
  for (let d = 1; d <= daysInMonth; d++) {
    cells.push({
      day: d,
      inMonth: true,
      isToday: d === today.getDate() && viewMonth.value === today.getMonth() && viewYear.value === today.getFullYear(),
      events: eventsByDay.value[d] || [],
    })
  }
  while (cells.length % 7 !== 0) cells.push({ day: null, inMonth: false, events: [] })
  return cells
})

const PILL_STYLES = [
  'bg-emerald-100 text-emerald-700 border-emerald-200',
  'bg-blue-100 text-blue-700 border-blue-200',
  'bg-amber-100 text-amber-700 border-amber-200',
  'bg-purple-100 text-purple-700 border-purple-200',
]
const pillClass = (i) => PILL_STYLES[i % PILL_STYLES.length]

watch([viewYear, viewMonth], fetchRecords)
onMounted(fetchRecords)
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #E2E8F0; border-radius: 10px; }
</style>