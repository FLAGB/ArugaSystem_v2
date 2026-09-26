<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar />

    <!-- MAIN -->
    <div class="flex flex-col flex-1 overflow-hidden">

      <!-- Header -->
      <HealthcareHeader />

      <main class="flex-1 overflow-y-auto p-6">

        <!-- Page title -->
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Vaccination Log</h1>
          <p class="text-sm text-slate-500 mt-1">
            Showing completed vaccination sessions up to today
            <span class="ml-2 text-xs font-medium text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full">
              {{ todayLabel }}
            </span>
          </p>
        </div>

        <!-- Loading state -->
        <div v-if="loading" class="flex items-center justify-center h-64 text-slate-400">
          <div class="text-center">
            <div class="w-8 h-8 border-2 border-emerald-500 border-t-transparent rounded-full animate-spin mx-auto mb-3"></div>
            <p class="text-sm font-medium">Loading vaccination history...</p>
          </div>
        </div>

        <template v-else>
          <!-- Calendar card -->
          <div class="bg-white rounded-xl border border-slate-200 p-6">

            <!-- Month nav -->
            <div class="flex items-center justify-between mb-6">
              <div>
                <h2 class="text-lg font-bold text-slate-800">{{ monthYearLabel }}</h2>
                <p class="text-xs text-slate-400 mt-0.5">{{ monthSessionCount }} session{{ monthSessionCount !== 1 ? 's' : '' }} this month</p>
              </div>
              <div class="flex items-center gap-2">
                <button @click="prevMonth"
                  class="p-2 rounded-lg text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors">
                  <ChevronLeft class="w-4 h-4" />
                </button>
                <button @click="goToToday"
                  class="px-3 py-1.5 text-xs font-medium text-emerald-700 bg-emerald-50 rounded-lg hover:bg-emerald-100 transition-colors">
                  Today
                </button>
                <button @click="nextMonth"
                  :disabled="isAtOrBeyondCurrentMonth"
                  class="p-2 rounded-lg transition-colors"
                  :class="isAtOrBeyondCurrentMonth
                    ? 'text-slate-200 cursor-not-allowed'
                    : 'text-slate-400 hover:text-slate-700 hover:bg-slate-100'">
                  <ChevronRight class="w-4 h-4" />
                </button>
              </div>
            </div>

            <!-- Day headers -->
            <div class="grid grid-cols-7 mb-2">
              <div v-for="day in ['Sun','Mon','Tue','Wed','Thu','Fri','Sat']" :key="day"
                class="text-center text-xs font-medium text-slate-400 py-2">{{ day }}</div>
            </div>

            <!-- Calendar grid -->
            <div class="grid grid-cols-7 gap-1">
              <div v-for="(cell, idx) in calendarCells" :key="idx"
                class="min-h-[90px] rounded-lg p-1.5 transition-colors"
                :class="[
                  cell.isCurrentMonth ? 'bg-white' : 'bg-slate-50',
                  cell.isToday ? 'ring-2 ring-emerald-500 ring-offset-1' : '',
                  cell.isFuture ? 'opacity-100' : '',
                ]">
                <p class="text-xs font-medium mb-1 w-6 h-6 flex items-center justify-center rounded-full"
                  :class="cell.isToday
                    ? 'bg-emerald-600 text-white'
                    : cell.isCurrentMonth
                      ? 'text-slate-900'
                      : 'text-slate-300'">
                  {{ cell.day }}
                </p>

                <!-- Sessions for this day — only completed ones -->
                <div class="space-y-0.5">
                  <div
                    v-for="session in cell.sessions"
                    :key="session.id"
                    @click="openDetail(session)"
                    class="text-xs px-1.5 py-0.5 rounded font-medium truncate cursor-pointer hover:opacity-80 transition-opacity"
                    :class="vaccineColor(session.vaccineID)"
                    :title="`${session.childName} — ${session.vaccineName} Dose ${session.doseNumber}`">
                    {{ shortName(session.childName) }}
                  </div>

                  <!-- +N more indicator -->
                  <div v-if="cell.overflow > 0"
                    class="text-[10px] text-slate-400 font-medium px-1">
                    +{{ cell.overflow }} more
                  </div>
                </div>
              </div>
            </div>

            <!-- Legend -->
            <div class="flex flex-wrap items-center gap-4 mt-4 pt-4 border-t border-slate-100">
              <p class="text-xs text-slate-400 font-medium mr-1">Vaccine:</p>
              <div v-for="l in vaccineLegend" :key="l.label" class="flex items-center gap-1.5">
                <div class="w-2.5 h-2.5 rounded-full" :class="l.dot"></div>
                <span class="text-xs text-slate-500">{{ l.label }}</span>
              </div>
            </div>
          </div>

          <!-- Recent sessions this week -->
          <div class="mt-6 bg-white rounded-xl border border-slate-200 p-5">
            <div class="flex items-center justify-between mb-4">
              <h3 class="font-semibold text-slate-800">Sessions This Week</h3>
              <span class="text-xs text-slate-400">{{ weekRangeLabel }}</span>
            </div>

            <div v-if="sessionsThisWeek.length === 0"
              class="text-sm text-slate-400 text-center py-6">
              <Syringe class="w-8 h-8 mx-auto mb-2 opacity-30" />
              No completed sessions recorded this week.
            </div>

            <div v-else class="space-y-2">
              <div v-for="session in sessionsThisWeek" :key="session.id"
                @click="openDetail(session)"
                class="flex items-center gap-3 p-3 rounded-lg bg-slate-50 hover:bg-slate-100 transition-colors cursor-pointer">
                <div class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                  :style="{ backgroundColor: avatarColor(session.childName) }">
                  {{ initials(session.childName) }}
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-semibold text-slate-800">{{ session.childName }}</p>
                  <p class="text-xs text-slate-400">
                    {{ session.vaccineName }} · Dose {{ session.doseNumber }}
                    <span v-if="session.administeredByName" class="ml-1">· {{ session.administeredByName }}</span>
                  </p>
                </div>
                <div class="text-right shrink-0">
                  <p class="text-xs font-medium text-slate-600">{{ session.dateLabel }}</p>
                  <span class="text-[10px] px-2 py-0.5 rounded-full font-medium bg-emerald-100 text-emerald-700">
                    Completed
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Stats summary -->
          <div class="mt-6 grid grid-cols-3 gap-4">
            <div class="bg-white rounded-xl border border-slate-200 p-5 text-center">
              <p class="text-3xl font-black text-emerald-600">{{ totalCompleted }}</p>
              <p class="text-xs text-slate-400 font-medium mt-1 uppercase tracking-wide">Total Doses Recorded</p>
            </div>
            <div class="bg-white rounded-xl border border-slate-200 p-5 text-center">
              <p class="text-3xl font-black text-slate-700">{{ monthSessionCount }}</p>
              <p class="text-xs text-slate-400 font-medium mt-1 uppercase tracking-wide">This Month</p>
            </div>
            <div class="bg-white rounded-xl border border-slate-200 p-5 text-center">
              <p class="text-3xl font-black text-blue-600">{{ sessionsThisWeek.length }}</p>
              <p class="text-xs text-slate-400 font-medium mt-1 uppercase tracking-wide">This Week</p>
            </div>
          </div>
        </template>

      </main>
    </div>

    <!-- Session detail modal -->
    <Transition name="modal">
      <div v-if="selectedSession"
        class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm"
        @click.self="selectedSession = null">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-sm overflow-hidden">
          <div class="bg-emerald-600 px-6 py-5 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center text-white font-bold">
                {{ initials(selectedSession.childName) }}
              </div>
              <div>
                <p class="text-white font-bold leading-tight">{{ selectedSession.childName }}</p>
                <p class="text-emerald-200 text-xs mt-0.5">Vaccination Record</p>
              </div>
            </div>
            <button @click="selectedSession = null" class="text-white/70 hover:text-white text-xl">✕</button>
          </div>
          <div class="p-6 space-y-3">
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-slate-50 rounded-xl px-4 py-3">
                <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1">Vaccine</p>
                <p class="text-sm font-bold text-slate-800">{{ selectedSession.vaccineName }}</p>
              </div>
              <div class="bg-slate-50 rounded-xl px-4 py-3">
                <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1">Dose</p>
                <p class="text-sm font-bold text-slate-800">Dose {{ selectedSession.doseNumber }}</p>
              </div>
              <div class="bg-slate-50 rounded-xl px-4 py-3">
                <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1">Date Given</p>
                <p class="text-sm font-bold text-slate-800">{{ selectedSession.dateLabel }}</p>
              </div>
              <div class="bg-slate-50 rounded-xl px-4 py-3">
                <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1">Lot Number</p>
                <p class="text-sm font-bold text-slate-800">{{ selectedSession.lotNumber || '—' }}</p>
              </div>
              <div class="bg-slate-50 rounded-xl px-4 py-3 col-span-2">
                <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1">Administered By</p>
                <p class="text-sm font-bold text-slate-800">{{ selectedSession.administeredByName || '—' }}</p>
              </div>
              <div v-if="selectedSession.remarks" class="bg-slate-50 rounded-xl px-4 py-3 col-span-2">
                <p class="text-[9px] font-black text-slate-400 uppercase tracking-widest mb-1">Remarks</p>
                <p class="text-sm text-slate-600">{{ selectedSession.remarks }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>


  </div>
</template>

<script setup>
import { API_ORIGIN } from '@/utils/apiBase'
import { getUser } from '@/utils/auth'
import HealthcareSidebar from './Components/HealthcareSidebar.vue'
import HealthcareHeader from './Components/HealthcareHeader.vue'
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import axios from 'axios'
import {
  Home, Users, Calendar, Syringe, FileText, Settings,
  Bell, LogOut, ChevronLeft, ChevronRight, ListChecks
} from 'lucide-vue-next'

const API = API_ORIGIN

const router = useRouter()
const route  = useRoute()

// ── Auth ──────────────────────────────────────────────────────────────────
const doctor = ref({ userId: '', fullName: '', userType: '' })
onMounted(async () => {
  const u = getUser()
if (!u) { router.push('/'); return }
  doctor.value = {
    userId:   u.userID  ?? u.UserID,
    fullName: `${u.firstName ?? u.FirstName} ${u.lastName ?? u.LastName}`,
    userType: u.userType ?? u.UserType
  }
  await loadSessions()
})


const doctorInitials = computed(() =>
  doctor.value.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
)

// ── Nav ───────────────────────────────────────────────────────────────────
// ── Data ──────────────────────────────────────────────────────────────────
const loading          = ref(true)
const completedSessions = ref([]) // flat list of completed vaccination records
const selectedSession  = ref(null)

/**
 * Fetch ALL VaccinationRecords with Status = 'Completed' across all children.
 * We use the existing /api/VaccinationRecords/all-completed endpoint.
 * If that doesn't exist yet, we fall back to fetching per-child from children list.
 */
async function loadSessions() {
  loading.value = true
  try {
    // GET /api/VaccinationRecords/all — same endpoint Doctor Reports and
    // Doctor Vaccination Records use; filter to Completed client-side.
    const res = await axios.get(`${API}/api/VaccinationRecords/all`)
    completedSessions.value = res.data
      .filter(r => (r.status ?? r.Status) === 'Completed')
      .map(normalizeRecord)
  } catch {
    // Fallback: load all children, then fetch records for each
    try {
      const childrenRes = await axios.get(`${API}/api/Children/all`)
      const children = childrenRes.data

      const allRecords = []
      for (const child of children) {
        try {
          const recRes = await axios.get(`${API}/api/VaccinationRecords/child/${child.childID}`)
          const completed = recRes.data
            .filter(r => (r.status ?? r.Status) === 'Completed')
            .map(r => ({ ...normalizeRecord(r), childName: `${child.firstName} ${child.lastName}` }))
          allRecords.push(...completed)
        } catch { /* skip children with no records */ }
      }
      completedSessions.value = allRecords
    } catch (err) {
      console.error('[Calendar] Failed to load sessions:', err)
      completedSessions.value = []
    }
  }
  loading.value = false
}

function normalizeRecord(r) {
  return {
    id:                r.vaccinationRecordID ?? r.recordID   ?? r.RecordID,
    childID:           r.childID    ?? r.ChildID,
    childName:         r.childName  ?? r.ChildName ?? '—',
    vaccineID:         r.vaccineID  ?? r.VaccineID,
    vaccineName:       r.vaccineName ?? r.VaccineName ?? `Vaccine ${r.vaccineID ?? r.VaccineID}`,
    doseNumber:        r.doseNumber ?? r.DoseNumber,
    dateAdministered:  (r.vaccinationDate ?? r.dateAdministered) ? new Date(r.vaccinationDate ?? r.dateAdministered) : null,
    administeredByName: r.administeredByName ?? r.administeredBy ?? r.AdministeredBy ?? null,
    lotNumber:         r.lotNumber  ?? r.LotNumber  ?? null,
    remarks:           r.nurseObservation ?? r.remarks ?? r.Remarks ?? null,
    status:            r.status     ?? r.Status,
  }
}

// ── Calendar state ────────────────────────────────────────────────────────
const today     = new Date()
const viewYear  = ref(today.getFullYear())
const viewMonth = ref(today.getMonth())

const todayLabel = computed(() =>
  today.toLocaleDateString('en-PH', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })
)

const monthYearLabel = computed(() =>
  new Date(viewYear.value, viewMonth.value).toLocaleDateString('en-PH', { month: 'long', year: 'numeric' })
)

const isAtOrBeyondCurrentMonth = computed(() =>
  viewYear.value > today.getFullYear() ||
  (viewYear.value === today.getFullYear() && viewMonth.value >= today.getMonth())
)

function prevMonth() {
  if (viewMonth.value === 0) { viewMonth.value = 11; viewYear.value-- }
  else viewMonth.value--
}
function nextMonth() {
  if (isAtOrBeyondCurrentMonth.value) return
  if (viewMonth.value === 11) { viewMonth.value = 0; viewYear.value++ }
  else viewMonth.value++
}
function goToToday() {
  viewYear.value  = today.getFullYear()
  viewMonth.value = today.getMonth()
}

// ── Calendar cells ────────────────────────────────────────────────────────
const MAX_PER_CELL = 3 // show at most 3 names per day cell

const calendarCells = computed(() => {
  const firstDay    = new Date(viewYear.value, viewMonth.value, 1).getDay()
  const daysInMonth = new Date(viewYear.value, viewMonth.value + 1, 0).getDate()
  const daysInPrev  = new Date(viewYear.value, viewMonth.value, 0).getDate()

  const cells = []

  // Previous month padding
  for (let i = firstDay - 1; i >= 0; i--)
    cells.push({ day: daysInPrev - i, isCurrentMonth: false, isToday: false, isFuture: false, sessions: [], overflow: 0 })

  // Current month days
  for (let d = 1; d <= daysInMonth; d++) {
    const cellDate = new Date(viewYear.value, viewMonth.value, d)
    const isToday  = cellDate.toDateString() === today.toDateString()
    const isFuture = cellDate > today

    // Only show completed sessions on past/today dates
    const daySessions = isFuture ? [] : completedSessions.value.filter(s => {
      if (!s.dateAdministered) return false
      return s.dateAdministered.toDateString() === cellDate.toDateString()
    }).map(s => ({
      ...s,
      dateLabel: s.dateAdministered.toLocaleDateString('en-PH', { weekday: 'short', month: 'short', day: 'numeric', year: 'numeric' })
    }))

    const visible  = daySessions.slice(0, MAX_PER_CELL)
    const overflow = Math.max(0, daySessions.length - MAX_PER_CELL)

    cells.push({ day: d, isCurrentMonth: true, isToday, isFuture, sessions: visible, overflow })
  }

  // Next month padding
  const remaining = 42 - cells.length
  for (let i = 1; i <= remaining; i++)
    cells.push({ day: i, isCurrentMonth: false, isToday: false, isFuture: true, sessions: [], overflow: 0 })

  return cells
})

// ── Summary stats ─────────────────────────────────────────────────────────
const totalCompleted = computed(() => completedSessions.value.length)

const monthSessionCount = computed(() =>
  completedSessions.value.filter(s => {
    if (!s.dateAdministered) return false
    return s.dateAdministered.getFullYear() === viewYear.value &&
           s.dateAdministered.getMonth()    === viewMonth.value
  }).length
)

// Sessions within the past 7 days (up to and including today)
const weekRangeLabel = computed(() => {
  const start = new Date(today)
  start.setDate(start.getDate() - 6)
  start.setHours(0,0,0,0)
  const fmt = d => d.toLocaleDateString('en-PH', { month: 'short', day: 'numeric' })
  return `${fmt(start)} – ${fmt(today)}`
})

const sessionsThisWeek = computed(() => {
  const start = new Date(today)
  start.setDate(start.getDate() - 6)
  start.setHours(0,0,0,0)
  const end = new Date(today)
  end.setHours(23,59,59,999)

  return completedSessions.value
    .filter(s => s.dateAdministered && s.dateAdministered >= start && s.dateAdministered <= end)
    .sort((a, b) => b.dateAdministered - a.dateAdministered) // newest first
    .map(s => ({
      ...s,
      dateLabel: s.dateAdministered.toLocaleDateString('en-PH', { weekday: 'short', month: 'short', day: 'numeric' })
    }))
})

// ── Detail modal ──────────────────────────────────────────────────────────
function openDetail(session) {
  selectedSession.value = session
}

// ── Helpers ───────────────────────────────────────────────────────────────

// Color-code calendar pills by vaccine type
const VACCINE_COLORS = [
  'bg-emerald-100 text-emerald-700', // BCG
  'bg-blue-100 text-blue-700',       // Hep B
  'bg-amber-100 text-amber-700',     // Pentavalent
  'bg-purple-100 text-purple-700',   // OPV
  'bg-red-100 text-red-700',         // IPV
  'bg-teal-100 text-teal-700',       // PCV
  'bg-pink-100 text-pink-700',       // MMR
]
function vaccineColor(vaccineID) {
  const idx = (vaccineID - 1) % VACCINE_COLORS.length
  return VACCINE_COLORS[Math.max(0, idx)]
}

const vaccineLegend = [
  { label: 'BCG',        dot: 'bg-emerald-500' },
  { label: 'Hep B',      dot: 'bg-blue-500'    },
  { label: 'Pentavalent',dot: 'bg-amber-500'   },
  { label: 'OPV',        dot: 'bg-purple-500'  },
  { label: 'IPV',        dot: 'bg-red-400'     },
  { label: 'PCV',        dot: 'bg-teal-500'    },
  { label: 'MMR',        dot: 'bg-pink-500'    },
]

// Shorten "Anna Cruz" → "Anna C."
function shortName(name) {
  if (!name) return '—'
  const parts = name.trim().split(' ')
  if (parts.length === 1) return parts[0]
  return `${parts[0]} ${parts[parts.length - 1].charAt(0)}.`
}

const AVATAR_COLORS = ['#4a7c59','#6b7c45','#8b5e3c','#4a6fa5','#7b4f8e','#5a7a6b','#8b6914']
function avatarColor(name) {
  if (!name) return AVATAR_COLORS[0]
  return AVATAR_COLORS[name.charCodeAt(0) % AVATAR_COLORS.length]
}
function initials(name) {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

</script>

<style scoped>
.modal-enter-active, .modal-leave-active { transition: opacity 0.2s ease; }
.modal-enter-from, .modal-leave-to { opacity: 0; }
.modal-enter-active > div, .modal-leave-active > div { transition: transform 0.25s cubic-bezier(0.4,0,0.2,1); }
.modal-enter-from > div, .modal-leave-to > div { transform: scale(0.95); }

</style>