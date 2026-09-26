<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar />

    <!-- MAIN CONTENT -->
    <div class="flex flex-col flex-1 overflow-hidden">

      <!-- TOP BAR -->
      <HealthcareHeader />

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <!-- WELCOME -->
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Welcome back, {{ worker.fullName }}</h1>
          <p class="text-sm text-slate-500 mt-1">{{ todayFormatted }}</p>
        </div>

        <!-- MY STATION + TODAY'S QUEUE -->
        <div class="bg-white rounded-xl border border-slate-200 mb-6">
          <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
            <div>
              <h2 class="font-semibold text-slate-800">Today's Queue</h2>
              <p class="text-xs text-slate-400 mt-0.5">The Admission Staff sends each patient to a station — updates automatically</p>
            </div>
            <button @click="router.push('/healthcare/queue')"
              class="text-xs bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-1.5 rounded-lg font-medium transition-colors flex items-center gap-1.5">
              <ListChecks class="w-3.5 h-3.5" />
              Go to Queue
            </button>
          </div>

          <div class="flex divide-x divide-slate-100">
            <!-- My station -->
            <div class="w-64 shrink-0 p-5 flex flex-col items-center justify-center text-center">
              <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest mb-1">
                {{ myStation ? myStation.roomName : 'My Station' }}
              </p>
              <template v-if="myVisit">
                <p class="text-4xl font-black text-slate-800">#{{ myVisit.queueNumber }}</p>
                <p class="text-xs text-slate-500 mt-1 truncate max-w-full">{{ myVisit.children.map(c => c.name).join(', ') }}</p>
                <button @click="openVisit(myVisit)"
                  class="mt-3 text-xs bg-slate-800 hover:bg-slate-900 text-white px-3 py-1.5 rounded-lg font-medium transition-colors flex items-center gap-1.5">
                  <Syringe class="w-3.5 h-3.5" /> Open Visit
                </button>
              </template>
              <template v-else>
                <p class="text-4xl font-black text-slate-300">—</p>
                <p class="text-xs text-slate-400 mt-1">
                  {{ myStation ? 'No patient sent to you yet' : 'You are not assigned to a station' }}
                </p>
              </template>
            </div>

            <!-- Waiting list -->
            <div class="flex-1 p-5">
              <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest mb-3">Waiting for a station ({{ waitingQueue.length }})</p>
              <div v-if="loadingQueue" class="text-sm text-slate-400 py-4 text-center">Loading...</div>
              <div v-else-if="waitingQueue.length === 0" class="text-sm text-slate-400 py-4 text-center">No one else waiting</div>
              <div v-else class="flex flex-wrap gap-2">
                <div v-for="q in waitingQueue" :key="q.queueID"
                  class="px-3 py-2 bg-slate-50 border border-slate-100 rounded-lg text-xs">
                  <span class="font-bold text-slate-700">#{{ q.queueNumber }}</span>
                  <span class="text-slate-400 ml-1.5">{{ q.children.map(c => c.name).join(', ') || q.requestBy }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- STAT CARDS -->
        <div class="grid grid-cols-4 gap-4 mb-6">
          <div v-for="stat in statCards" :key="stat.label" class="bg-white rounded-xl border border-slate-200 p-5">
            <div class="flex items-center justify-between mb-3">
              <component :is="stat.icon" class="w-5 h-5" :class="stat.iconColor" />
            </div>
            <p class="text-3xl font-bold text-slate-800">{{ stat.value }}</p>
            <p class="text-xs text-slate-400 mt-1">{{ stat.label }}</p>
          </div>
        </div>

        <div class="grid grid-cols-3 gap-6">

          <!-- PENDING VACCINATIONS TODAY -->
          <div class="col-span-2 bg-white rounded-xl border border-slate-200">
            <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
              <div>
                <h2 class="font-semibold text-slate-800">Pending Vaccinations Today</h2>
                <p class="text-xs text-slate-400 mt-0.5">Children scheduled for vaccination today and where they are</p>
              </div>
              <button class="text-xs text-emerald-600 hover:underline font-medium" @click="router.push('/healthcare/vaccination-records')">
                View All Records →
              </button>
            </div>

            <div v-if="loadingPending" class="flex items-center justify-center py-12">
              <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
              <span class="ml-2 text-sm text-slate-400">Loading...</span>
            </div>

            <div v-else-if="pendingToday.length === 0" class="flex flex-col items-center justify-center py-12 text-slate-400">
              <CheckCircle class="w-8 h-8 mb-2 opacity-40" />
              <p class="text-sm">All vaccinations for today are done</p>
            </div>

            <div v-else class="overflow-x-auto">
              <table class="w-full text-sm">
                <thead>
                  <tr class="text-xs text-slate-400 uppercase tracking-wide">
                    <th class="px-5 py-3 text-left font-medium">Child</th>
                    <th class="px-5 py-3 text-left font-medium">Parent</th>
                    <th class="px-5 py-3 text-left font-medium">Vaccine</th>
                    <th class="px-5 py-3 text-left font-medium">Dose</th>
                    <th class="px-5 py-3 text-left font-medium">Status</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr v-for="item in pendingToday" :key="item.timelineId" class="hover:bg-slate-50 transition-colors">
                    <td class="px-5 py-3">
                      <div class="flex items-center gap-2.5">
                        <div class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                          :style="{ backgroundColor: avatarColor(item.childName) }">
                          {{ initials(item.childName) }}
                        </div>
                        <span class="font-medium text-slate-800">{{ item.childName }}</span>
                      </div>
                    </td>
                    <td class="px-5 py-3 text-slate-500">{{ item.parentName || '—' }}</td>
                    <td class="px-5 py-3 text-slate-700">{{ item.vaccineName }}</td>
                    <td class="px-5 py-3 text-slate-500">Dose {{ item.doseNumber }}</td>
                    <td class="px-5 py-3">
                      <button v-if="whereIs(item.childId).mine" @click="openVisit(whereIs(item.childId).visit, item.childId)"
                        class="text-xs bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-1.5 rounded-lg font-medium transition-colors">
                        Vaccinate
                      </button>
                      <span v-else class="text-xs font-medium px-2.5 py-1 rounded-full" :class="whereIs(item.childId).tone">
                        {{ whereIs(item.childId).label }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- RIGHT COLUMN -->
          <div class="flex flex-col gap-4">

            <!-- QUICK ACTIONS -->
            <div class="bg-white rounded-xl border border-slate-200 p-5">
              <h2 class="font-semibold text-slate-800 mb-3">Quick Actions</h2>
              <div class="space-y-2">
                <button v-for="action in quickActions" :key="action.label"
                  @click="router.push(action.path)"
                  class="w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition-colors text-left">
                  <component :is="action.icon" class="w-4 h-4 text-emerald-600 shrink-0" />
                  {{ action.label }}
                </button>
              </div>
            </div>

            <!-- VACCINATED TODAY -->
            <div class="bg-white rounded-xl border border-slate-200 p-5 flex-1">
              <h2 class="font-semibold text-slate-800 mb-3">Vaccinated Today</h2>
              <div v-if="recentlyVaccinated.length === 0" class="text-sm text-slate-400 text-center py-4">
                No vaccinations recorded yet today
              </div>
              <div v-else class="space-y-3">
                <div v-for="rec in recentlyVaccinated" :key="rec.recordId" class="flex items-start gap-3">
                  <div class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0 mt-0.5"
                    :style="{ backgroundColor: avatarColor(rec.childName) }">
                    {{ initials(rec.childName) }}
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium text-slate-800 truncate">{{ rec.childName }}</p>
                    <p class="text-xs text-slate-400">{{ rec.vaccineName }} · Dose {{ rec.doseNumber }}</p>
                  </div>
                  <span class="text-xs text-emerald-600 font-medium shrink-0">Done</span>
                </div>
              </div>
            </div>

          </div>
        </div>

      </main>
    </div>
  </div>
</template>

<script setup>
import { getUser } from '@/utils/auth'
import HealthcareSidebar from './Components/HealthcareSidebar.vue'
import HealthcareHeader from './Components/HealthcareHeader.vue'
import { fetchMyStation, isAtMyStation } from './Components/station.js'
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import {
  Users, Calendar, Syringe, FileText, Loader2, CheckCircle,
  Clock, AlertCircle, TrendingUp, ListChecks,
} from 'lucide-vue-next'

const router = useRouter()
const API = import.meta.env.VITE_API_URL || 'http://localhost:57147'

const worker = ref({ userId: '', fullName: '' })

let pollHandle = null

onMounted(() => {
  const u = getUser()
  if (!u) { router.push('/'); return }
  worker.value = { userId: u.UserID, fullName: `${u.FirstName} ${u.LastName}` }

  fetchStats()
  fetchPendingToday()
  fetchRecentlyVaccinated()
  fetchQueue()
  // The queue moves quickly; the rest refreshes with it every few polls.
  let tick = 0
  pollHandle = setInterval(() => {
    fetchQueue()
    if (++tick % 6 === 0) { fetchStats(); fetchPendingToday(); fetchRecentlyVaccinated() }
  }, 5000)
})

onUnmounted(() => {
  if (pollHandle) clearInterval(pollHandle)
})

// ─────────────────────────────────────────────────────────────
// QUEUE + MY STATION
// GET /api/Queue/today — every visit today with the station/health worker
// the Admission Staff assigned. A health worker only vaccinates the
// patient sent to their own station.
// ─────────────────────────────────────────────────────────────
const todaysQueue  = ref([])
const myStation    = ref(null)
const loadingQueue = ref(false)

async function fetchQueue() {
  loadingQueue.value = todaysQueue.value.length === 0
  try {
    const [q, station] = await Promise.all([axios.get(`${API}/api/Queue/today`), fetchMyStation()])
    todaysQueue.value = q.data
    myStation.value = station
  } catch (e) {
    console.error('fetchQueue:', e)
  } finally {
    loadingQueue.value = false
  }
}

const myVisit = computed(() => todaysQueue.value.find(isAtMyStation) || null)
const waitingQueue = computed(() => todaysQueue.value.filter(q => q.status === 'Waiting'))

function openVisit(visit, childId = null) {
  const child = childId || visit.children[0]?.childID
  router.push(`/healthcare/vaccination/${visit.queueID}?child=${child}`)
}

// Where a child due today is right now — drives the Pending table's
// status column and the "Vaccinate" shortcut for the child at my station.
function whereIs(childId) {
  const visit = todaysQueue.value.find(q =>
    q.children.some(c => String(c.childID).toLowerCase() === String(childId).toLowerCase()))
  if (!visit) return { label: 'Not checked in', tone: 'bg-slate-100 text-slate-500' }
  if (isAtMyStation(visit)) return { mine: true, visit }
  const s = (visit.status || '').toLowerCase().replace(/\s+/g, '')
  if (s === 'completed') return { label: 'Visit completed', tone: 'bg-emerald-50 text-emerald-700' }
  if (s === 'inprogress') return { label: `At ${visit.stationName || 'a station'}`, tone: 'bg-blue-50 text-blue-700' }
  return { label: `Waiting · #${visit.queueNumber}`, tone: 'bg-amber-50 text-amber-700' }
}

// ─────────────────────────────────────────────────────────────
// NAVIGATION
// ─────────────────────────────────────────────────────────────
const quickActions = [
  { label: 'Patient List',        path: '/healthcare/patients',            icon: Users    },
  { label: 'View Calendar',       path: '/healthcare/calendar',            icon: Calendar },
  { label: 'Vaccination Records', path: '/healthcare/vaccination-records', icon: Syringe  },
  { label: 'Daily Report',        path: '/healthcare/reports',             icon: FileText },
]

// ─────────────────────────────────────────────────────────────
// STATS — GET /api/VaccinationRecords/stats
// ─────────────────────────────────────────────────────────────
const stats = ref({ vaccinatedToday: 0, pendingToday: 0, missedTotal: 0, weeklyTotal: 0 })

const statCards = computed(() => [
  { label: 'Vaccinated Today',  value: stats.value.vaccinatedToday, icon: Syringe,     iconColor: 'text-emerald-500' },
  { label: 'Pending Today',     value: stats.value.pendingToday,    icon: Clock,       iconColor: 'text-amber-500'   },
  { label: 'Missed Follow-ups', value: stats.value.missedTotal,     icon: AlertCircle, iconColor: 'text-red-400'     },
  { label: 'Weekly Total',      value: stats.value.weeklyTotal,     icon: TrendingUp,  iconColor: 'text-blue-400'    },
])

async function fetchStats() {
  try {
    const res = await axios.get(`${API}/api/VaccinationRecords/stats`)
    stats.value = res.data
  } catch (e) { console.error('fetchStats:', e) }
}

// ─────────────────────────────────────────────────────────────
// PENDING VACCINATIONS TODAY — GET /api/VaccinationTimeline/due-today
// ─────────────────────────────────────────────────────────────
const pendingToday   = ref([])
const loadingPending = ref(false)

async function fetchPendingToday() {
  loadingPending.value = pendingToday.value.length === 0
  try {
    const res = await axios.get(`${API}/api/VaccinationTimeline/due-today`)
    pendingToday.value = res.data
  } catch (e) { console.error('fetchPendingToday:', e) }
  finally { loadingPending.value = false }
}

// ─────────────────────────────────────────────────────────────
// VACCINATED TODAY — GET /api/VaccinationRecords/completed-today
// ─────────────────────────────────────────────────────────────
const recentlyVaccinated = ref([])

async function fetchRecentlyVaccinated() {
  try {
    const res = await axios.get(`${API}/api/VaccinationRecords/completed-today`)
    recentlyVaccinated.value = res.data
  } catch (e) { console.error('fetchRecentlyVaccinated:', e) }
}

// ─────────────────────────────────────────────────────────────
// HELPERS
// ─────────────────────────────────────────────────────────────
const AVATAR_COLORS = ['#4a7c59','#6b7c45','#8b5e3c','#4a6fa5','#7b4f8e','#5a7a6b','#8b6914']
function avatarColor(name) {
  if (!name) return AVATAR_COLORS[0]
  return AVATAR_COLORS[name.charCodeAt(0) % AVATAR_COLORS.length]
}
function initials(name) {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

const todayFormatted = computed(() =>
  new Date().toLocaleDateString('en-PH', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })
)
</script>
