<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar @logout="logout" />

    <div class="flex flex-col flex-1 overflow-hidden">
      <HealthcareHeader :worker="worker" title="Calendar" />

      <main class="flex-1 flex flex-col overflow-hidden p-4 gap-2 min-h-0">

        <div class="shrink-0">
          <h1 class="text-lg font-bold text-slate-800">Calendar</h1>
          <p class="text-xs text-slate-500">View vaccination schedules and appointments</p>
        </div>

        <!-- LOAD ERROR -->
        <div v-if="loadError" class="flex items-start gap-2 px-3 py-2 bg-red-50 border border-red-100 rounded-lg text-xs text-red-700 shrink-0">
          <AlertCircle class="w-3.5 h-3.5 shrink-0 mt-0.5" />
          <span>{{ loadError }}</span>
        </div>

        <!--
          TODO — SCHEDULED / PENDING / MISSED
          Only "Completed" pills are real, sourced from
          GET /api/VaccinationRecords/all (confirmed). The other three
          statuses need the VaccinationTimeline model, still
          unconfirmed, so no entries are plotted for them — the
          legend keeps their colors visible so the design isn't
          silently missing pieces, but nothing is invented under them.
        -->
        <div class="flex items-start gap-2 px-3 py-2 bg-amber-50 border border-amber-100 rounded-lg text-xs text-amber-700 shrink-0">
          <Info class="w-3.5 h-3.5 shrink-0 mt-0.5" />
          <span>Only "Completed" appointments are live (from actual vaccination records). Scheduled, Pending, and Missed need the VaccinationTimeline data — not shown yet.</span>
        </div>

        <!-- MONTH NAV -->
        <div class="flex items-center justify-between shrink-0">
          <h2 class="text-base font-bold text-slate-800">{{ monthLabel }}</h2>
          <div class="flex items-center gap-1.5">
            <button @click="prevMonth" class="p-1.5 rounded-lg border border-slate-200 hover:bg-white transition-colors">
              <ChevronLeft class="w-3.5 h-3.5 text-slate-500" />
            </button>
            <button @click="goToday" class="px-2.5 py-1.5 rounded-lg border border-slate-200 hover:bg-white text-[11px] font-medium text-slate-500 transition-colors">
              Today
            </button>
            <button @click="nextMonth" class="p-1.5 rounded-lg border border-slate-200 hover:bg-white transition-colors">
              <ChevronRight class="w-3.5 h-3.5 text-slate-500" />
            </button>
          </div>
        </div>

        <!-- LOADING -->
        <div v-if="loading" class="flex-1 flex items-center justify-center">
          <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
          <span class="ml-2 text-sm text-slate-400">Loading calendar...</span>
        </div>

        <!-- CALENDAR GRID -->
        <div v-else class="flex-1 bg-white rounded-xl border border-slate-200 overflow-hidden flex flex-col min-h-0">
          <div class="grid grid-cols-7 border-b border-slate-100 shrink-0">
            <div v-for="d in weekdayLabels" :key="d" class="px-2 py-1.5 text-[10px] font-semibold text-slate-400 uppercase tracking-wide text-center">
              {{ d }}
            </div>
          </div>

          <div
            class="grid grid-cols-7 flex-1 min-h-0"
            :style="{ gridTemplateRows: `repeat(${rowCount}, minmax(0, 1fr))` }"
          >
            <button
              v-for="(day, idx) in calendarCells"
              :key="idx"
              @click="day.date && (selectedKey = day.key)"
              class="text-left border-b border-r border-slate-100 p-1.5 relative min-h-0 overflow-hidden transition-colors"
              :class="[
                idx % 7 === 6 ? 'border-r-0' : '',
                !day.inMonth ? 'bg-slate-50' : '',
                day.key === selectedKey ? 'ring-2 ring-inset ring-amber-400 bg-amber-50/30' : 'hover:bg-slate-50',
              ]"
            >
              <template v-if="day.date">
                <div class="flex items-center justify-between">
                  <span
                    class="text-[11px] font-medium w-5 h-5 flex items-center justify-center rounded-full shrink-0"
                    :class="[
                      day.isToday ? 'bg-emerald-600 text-white' : (day.inMonth ? 'text-slate-600' : 'text-slate-300'),
                    ]"
                  >
                    {{ day.date.getDate() }}
                  </span>
                  <span v-if="day.closedDot" class="w-1.5 h-1.5 rounded-full bg-red-400 shrink-0" title="Clinic closed"></span>
                </div>

                <!-- Selected day: full stacked list. Others: truncated preview. -->
                <div class="mt-1 space-y-0.5">
                  <template v-if="day.key === selectedKey">
                    <div v-for="(e, i) in day.entries" :key="i" class="text-[9px] leading-tight px-1 py-0.5 rounded truncate" :class="statusClasses(e.status)">
                      {{ e.name }}
                    </div>
                    <div v-if="day.entries.length === 0" class="text-[9px] text-slate-300 px-1">No appointments</div>
                  </template>
                  <template v-else>
                    <div v-for="(e, i) in day.entries.slice(0, 2)" :key="i" class="text-[9px] leading-tight px-1 py-0.5 rounded truncate" :class="statusClasses(e.status)">
                      {{ e.name }}
                    </div>
                    <div v-if="day.entries.length > 2" class="text-[9px] text-slate-400 px-1">+{{ day.entries.length - 2 }} more</div>
                  </template>
                </div>
              </template>
            </button>
          </div>
        </div>

        <!-- LEGEND -->
        <div class="flex items-center gap-4 text-[11px] text-slate-500 shrink-0">
          <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-emerald-400"></span> Completed</span>
          <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-blue-400"></span> Scheduled</span>
          <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-amber-400"></span> Pending</span>
          <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-red-400"></span> Missed</span>
          <span class="flex items-center gap-1.5 ml-auto"><span class="w-2 h-2 rounded-full bg-red-400"></span> Clinic closed</span>
        </div>

      </main>
    </div>
  </div>
</template>

<script setup>
import { getUser, getToken, logout as clearSession } from '@/utils/auth'
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { AlertCircle, Info, ChevronLeft, ChevronRight, Loader2 } from 'lucide-vue-next'
import HealthcareSidebar from '@/components/Healthcare/Components/HealtcareSidebar.vue'
import HealthcareHeader from '@/components/Healthcare/Components/HealthcareHeader.vue'

const router = useRouter()

// VITE_API_URL is the bare host (no /api) per the project's existing
// convention — /api is appended here, not stored in the env var.
const API_BASE = `${(import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '')}/api`

function authHeaders() {
  return { Authorization: `Bearer ${getToken()}` }
}

// ─────────────────────────────────────────────────────────────
// AUTH
// ─────────────────────────────────────────────────────────────
const SKIP_AUTH = false
const worker = ref({ userId: '', fullName: '', userType: '' })

onMounted(() => {
  const account = getUser()

  if (!account && !SKIP_AUTH) {
    router.push('/')
    return
  }

  const u = account || { UserID: 'dev-test-user', FirstName: 'Test', LastName: 'Worker', UserType: 'Nurse' }

  worker.value = {
    userId:   u.UserID || u.userID,
    fullName: `${u.FirstName || u.firstName} ${u.LastName || u.lastName}`.trim(),
    userType: u.UserType || u.userType || u.role,
  }

  fetchCompletedVaccinations()
  fetchExceptions()
  fetchOperatingSchedule()
})

function logout() {
  clearSession()
  router.push('/')
}

// ─────────────────────────────────────────────────────────────
// COMPLETED VACCINATIONS (real data)
// GET /api/VaccinationRecords/all — grouped by VaccinationDate.
// These are actual administered records, so they map to the
// "Completed" (green) status in the legend. Confirmed real.
// ─────────────────────────────────────────────────────────────
const completedByDate = ref({}) // { 'YYYY-MM-DD': [{name, status:'completed'}] }
const loading   = ref(false)
const loadError = ref('')

function dateKey(d) {
  const dt = new Date(d)
  return `${dt.getFullYear()}-${String(dt.getMonth() + 1).padStart(2, '0')}-${String(dt.getDate()).padStart(2, '0')}`
}

async function fetchCompletedVaccinations() {
  loading.value = true
  loadError.value = ''
  try {
    const res = await fetch(`${API_BASE}/VaccinationRecords/all`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`VaccinationRecords request failed (${res.status})`)
    const data = await res.json()

    const map = {}
    for (const r of data) {
      const vaccinationDate = r.vaccinationDate ?? r.VaccinationDate
      if (!vaccinationDate) continue
      const childName = r.childName ?? r.ChildName ?? 'Unknown'

      const key = dateKey(vaccinationDate)
      if (!map[key]) map[key] = []
      map[key].push({ name: childName, status: 'completed' })
    }
    completedByDate.value = map
  } catch (e) {
    console.error('fetchCompletedVaccinations:', e)
    loadError.value = 'Could not load vaccination records for the calendar.'
  } finally {
    loading.value = false
  }
}

function statusClasses(status) {
  if (status === 'completed') return 'bg-emerald-50 text-emerald-700'
  if (status === 'scheduled') return 'bg-blue-50 text-blue-700'
  if (status === 'pending') return 'bg-amber-50 text-amber-700'
  if (status === 'missed') return 'bg-red-50 text-red-700'
  return 'bg-slate-100 text-slate-500'
}

// ─────────────────────────────────────────────────────────────
// CLINIC CLOSURES (real data, kept as a small dot indicator so it
// doesn't compete visually with the per-child pills, which are the
// focus of this design).
// ─────────────────────────────────────────────────────────────
const exceptions = ref([])
const operatingSchedule = ref([])

async function fetchExceptions() {
  try {
    const res = await fetch(`${API_BASE}/ClinicScheduleExceptions`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`ClinicScheduleExceptions request failed (${res.status})`)
    const data = await res.json()
    exceptions.value = data.map(e => ({
      exceptionDate: e.exceptionDate ?? e.ExceptionDate,
      isOpen: e.isOpen ?? e.IsOpen,
    }))
  } catch (e) {
    console.error('fetchExceptions:', e)
  }
}

async function fetchOperatingSchedule() {
  try {
    const res = await fetch(`${API_BASE}/ClinicOperatingSchedule`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`ClinicOperatingSchedule request failed (${res.status})`)
    const data = await res.json()
    operatingSchedule.value = data.map(s => ({
      dayOfWeek: s.dayOfWeek ?? s.DayOfWeek, // confirmed int matching JS Date.getDay()
      isOpen: s.isOpen ?? s.IsOpen,
    }))
  } catch (e) {
    console.error('fetchOperatingSchedule:', e)
  }
}

const exceptionsByDate = computed(() => {
  const map = {}
  for (const e of exceptions.value) map[dateKey(e.exceptionDate)] = e
  return map
})

function isClosed(date) {
  const key = dateKey(date)
  const exception = exceptionsByDate.value[key]
  if (exception) return exception.isOpen === false
  const weekly = operatingSchedule.value.find(s => s.dayOfWeek === date.getDay())
  return weekly ? weekly.isOpen === false : false
}

// ─────────────────────────────────────────────────────────────
// CALENDAR GRID
// ─────────────────────────────────────────────────────────────
const today = new Date()
const viewYear  = ref(today.getFullYear())
const viewMonth = ref(today.getMonth())
const selectedKey = ref(dateKey(today))

const weekdayLabels = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

const monthLabel = computed(() =>
  new Date(viewYear.value, viewMonth.value, 1).toLocaleDateString('en-PH', { month: 'long', year: 'numeric' })
)

function prevMonth() {
  if (viewMonth.value === 0) { viewMonth.value = 11; viewYear.value--; }
  else viewMonth.value--
}
function nextMonth() {
  if (viewMonth.value === 11) { viewMonth.value = 0; viewYear.value++; }
  else viewMonth.value++
}
function goToday() {
  viewYear.value = today.getFullYear()
  viewMonth.value = today.getMonth()
  selectedKey.value = dateKey(today)
}

const calendarCells = computed(() => {
  const firstOfMonth = new Date(viewYear.value, viewMonth.value, 1)
  const startOffset = firstOfMonth.getDay()
  const daysInMonth = new Date(viewYear.value, viewMonth.value + 1, 0).getDate()
  const daysInPrevMonth = new Date(viewYear.value, viewMonth.value, 0).getDate()

  const cells = []

  for (let i = startOffset - 1; i >= 0; i--) {
    cells.push(buildCell(new Date(viewYear.value, viewMonth.value - 1, daysInPrevMonth - i), false))
  }
  for (let day = 1; day <= daysInMonth; day++) {
    cells.push(buildCell(new Date(viewYear.value, viewMonth.value, day), true))
  }
  const remainder = cells.length % 7
  if (remainder !== 0) {
    for (let i = 1; i <= 7 - remainder; i++) {
      cells.push(buildCell(new Date(viewYear.value, viewMonth.value + 1, i), false))
    }
  }

  return cells
})

const rowCount = computed(() => calendarCells.value.length / 7)

function buildCell(date, inMonth) {
  const key = dateKey(date)
  return {
    date,
    key,
    inMonth,
    isToday: key === dateKey(today),
    closedDot: isClosed(date),
    entries: completedByDate.value[key] || [],
  }
}
</script>