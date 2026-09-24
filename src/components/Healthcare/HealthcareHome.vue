<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar @logout="logout" />

    <div class="flex flex-col flex-1 overflow-hidden">
      <HealthcareHeader :worker="worker" />

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <!-- WELCOME -->
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Welcome back, {{ worker.fullName }}</h1>
          <p class="text-sm text-slate-500 mt-1">{{ todayFormatted }}</p>
        </div>

        <!-- LOAD ERROR -->
        <div v-if="loadError" class="mb-6 flex items-start gap-2 p-4 bg-red-50 border border-red-100 rounded-xl text-sm text-red-700">
          <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
          <span>{{ loadError }}</span>
        </div>

        <!-- STAT CARDS -->
        <!--
          NOTE: The Queue.Status field is a free-text string set by whatever
          called PUT /api/Queue/{id}/status. The backend code we've seen only
          ever sets "Waiting" (on create) and lets callers pass "In Progress" /
          "Completed". There is no confirmed "Ready" or "Late" status coming
          from the backend, so those buckets are NOT invented here — anything
          that isn't Waiting / In Progress / Completed is grouped as "Other"
          until the backend defines a real status enum.
        -->
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

          <!-- PENDING PATIENTS -->
          <div class="col-span-2 bg-white rounded-xl border border-slate-200">
            <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
              <div>
                <h2 class="font-semibold text-slate-800">Pending Patients Today</h2>
                <p class="text-xs text-slate-400 mt-0.5">Children still waiting or in progress today</p>
              </div>
              <button class="text-xs text-emerald-600 hover:underline font-medium" @click="$router.push('/healthcare/queue')">
                Go to Queue →
              </button>
            </div>

            <div v-if="loadingQueue" class="flex items-center justify-center py-12">
              <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
              <span class="ml-2 text-sm text-slate-400">Loading...</span>
            </div>

            <div v-else-if="pendingPatients.length === 0" class="flex flex-col items-center justify-center py-12 text-slate-400">
              <CheckCircle class="w-8 h-8 mb-2 opacity-40" />
              <p class="text-sm">No pending patients right now</p>
            </div>

            <div v-else class="overflow-x-auto">
              <table class="w-full text-sm">
                <thead>
                  <tr class="text-xs text-slate-400 uppercase tracking-wide">
                    <th class="px-5 py-3 text-left font-medium">Queue #</th>
                    <th class="px-5 py-3 text-left font-medium">Child</th>
                    <th class="px-5 py-3 text-left font-medium">Requested By</th>
                    <th class="px-5 py-3 text-left font-medium">Status</th>
                    <th class="px-5 py-3 text-left font-medium">Action</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr v-for="q in pendingPatients" :key="q.queueID" class="hover:bg-slate-50 transition-colors">
                    <td class="px-5 py-3 font-medium text-slate-700">#{{ q.queueNumber }}</td>
                    <td class="px-5 py-3">
                      <div class="flex flex-col gap-1">
                        <div v-for="c in q.children" :key="c.childID" class="flex items-center gap-2">
                          <div class="w-6 h-6 rounded-full flex items-center justify-center text-[10px] font-bold text-white shrink-0"
                            :style="{ backgroundColor: avatarColor(c.name) }">
                            {{ initials(c.name) }}
                          </div>
                          <span class="font-medium text-slate-800">{{ c.name }}</span>
                        </div>
                      </div>
                    </td>
                    <td class="px-5 py-3 text-slate-500">{{ q.requestBy }}</td>
                    <td class="px-5 py-3">
                      <span class="text-xs px-2 py-1 rounded-full font-medium" :class="statusBadgeClass(q.status)">
                        {{ q.status }}
                      </span>
                    </td>
                    <td class="px-5 py-3">
                      <button
                        @click="$router.push('/healthcare/queue')"
                        class="text-xs bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-1.5 rounded-lg font-medium transition-colors"
                      >
                        Open in Queue
                      </button>
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
                  @click="$router.push(action.path)"
                  class="w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition-colors text-left">
                  <component :is="action.icon" class="w-4 h-4 text-emerald-600 shrink-0" />
                  {{ action.label }}
                </button>
              </div>
            </div>

            <!-- VACCINATED TODAY -->
            <!--
              Sourced from GET /api/VaccinationRecords/all, filtered client-side to
              VaccinationDate == today. NOTE: RecordVaccination() in
              VaccinationRecordsController does not currently set a Status
              field on the created record, so we cannot reliably filter by
              Status == "Completed". We filter by date only.
            -->
            <div class="bg-white rounded-xl border border-slate-200 p-5 flex-1">
              <h2 class="font-semibold text-slate-800 mb-3">Vaccinated Today</h2>
              <div v-if="loadingRecords" class="flex items-center justify-center py-6">
                <Loader2 class="w-4 h-4 animate-spin text-slate-400" />
              </div>
              <div v-else-if="recentlyVaccinated.length === 0" class="text-sm text-slate-400 text-center py-4">
                No vaccinations recorded yet today
              </div>
              <div v-else class="space-y-3">
                <div v-for="rec in recentlyVaccinated" :key="rec.vaccinationRecordID" class="flex items-start gap-3">
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
import { getUser, getToken, logout as clearSession } from '@/utils/auth'
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  Users, Syringe, Loader2, CheckCircle,
  Clock, AlertCircle, ListChecks
} from 'lucide-vue-next'
import HealthcareSidebar from '../Healthcare/Components/HealtcareSidebar.vue'
import HealthcareHeader from '../Healthcare/Components/HealthcareHeader.vue'


const router = useRouter()

// VITE_API_URL is the bare host (no /api) per the project's existing
// convention — /api is appended here, not stored in the env var.
const API_BASE = `${(import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '')}/api`

function authHeaders() {
  return { Authorization: `Bearer ${getToken()}` }
}

// ─────────────────────────────────────────────────────────────
// AUTH
// SKIP_AUTH lets you test the page without a real login session.
// Set to false (or delete the block) before shipping.
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
    userType: u.UserType || u.userType || u.role, // Doctor / Nurse / Midwife
  }

  fetchQueue()
  fetchRecentlyVaccinated()
})

function logout() {
  clearSession()
  router.push('/')
}

const quickActions = [
  { label: 'Go to Queue',     path: '/healthcare/queue',    icon: ListChecks },
  { label: 'Search Patients', path: '/healthcare/patients', icon: Users      },
]

// ─────────────────────────────────────────────────────────────
// QUEUE (for stats + pending patients)
// GET /api/Queue — no confirmed date filter param on the backend,
// so "today" is filtered client-side against QueueDate.
// ─────────────────────────────────────────────────────────────
const allQueues    = ref([])
const loadingQueue = ref(false)
const loadError    = ref('')

function isToday(dateStr) {
  const d = new Date(dateStr)
  const now = new Date()
  return d.getFullYear() === now.getFullYear() &&
         d.getMonth() === now.getMonth() &&
         d.getDate() === now.getDate()
}

async function fetchQueue() {
  loadingQueue.value = true
  loadError.value = ''
  try {
    const res = await fetch(`${API_BASE}/Queue`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`Queue request failed (${res.status})`)
    const data = await res.json()
    allQueues.value = data.filter(q => isToday(q.queueDate ?? q.QueueDate))
  } catch (e) {
    console.error('fetchQueue:', e)
    loadError.value = 'Could not load today\'s queue. Please refresh.'
  } finally {
    loadingQueue.value = false
  }
}

const todaysQueue = computed(() =>
  allQueues.value.map(q => ({
    queueID:     q.queueID ?? q.QueueID,
    queueNumber: q.queueNumber ?? q.QueueNumber,
    requestBy:   q.requestBy ?? q.RequestBy,
    status:      q.status ?? q.Status,
    queueDate:   q.queueDate ?? q.QueueDate,
    children:    (q.children ?? q.Children ?? []).map(c => ({
      childID: c.childID ?? c.ChildID,
      name:    c.name ?? c.Name,
    })),
  }))
)

const statusCounts = computed(() => {
  const counts = { waiting: 0, inProgress: 0, completed: 0, other: 0 }
  for (const q of todaysQueue.value) {
    const s = (q.status || '').toLowerCase()
    if (s === 'waiting') counts.waiting++
    else if (s === 'in progress') counts.inProgress++
    else if (s === 'completed') counts.completed++
    else counts.other++
  }
  return counts
})

const statCards = computed(() => [
  { label: 'Waiting',     value: statusCounts.value.waiting,    icon: Clock,       iconColor: 'text-amber-500'   },
  { label: 'In Progress', value: statusCounts.value.inProgress, icon: Loader2,     iconColor: 'text-blue-400'    },
  { label: 'Completed',   value: statusCounts.value.completed,  icon: CheckCircle, iconColor: 'text-emerald-500' },
  { label: 'Other',       value: statusCounts.value.other,      icon: AlertCircle, iconColor: 'text-slate-400'   },
])

const pendingPatients = computed(() =>
  todaysQueue.value.filter(q => (q.status || '').toLowerCase() !== 'completed')
)

function statusBadgeClass(status) {
  const s = (status || '').toLowerCase()
  if (s === 'waiting') return 'bg-amber-50 text-amber-700'
  if (s === 'in progress') return 'bg-blue-50 text-blue-700'
  if (s === 'completed') return 'bg-emerald-50 text-emerald-700'
  return 'bg-slate-100 text-slate-500'
}

// ─────────────────────────────────────────────────────────────
// RECENTLY VACCINATED (today)
// GET /api/VaccinationRecords/all — filtered client-side by
// VaccinationDate == today (Status not set on create — see note).
// ─────────────────────────────────────────────────────────────
const recentlyVaccinated = ref([])
const loadingRecords     = ref(false)

async function fetchRecentlyVaccinated() {
  loadingRecords.value = true
  try {
    const res = await fetch(`${API_BASE}/VaccinationRecords/all`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`VaccinationRecords request failed (${res.status})`)
    const data = await res.json()

    recentlyVaccinated.value = data
      .filter(r => isToday(r.vaccinationDate ?? r.VaccinationDate))
      .map(r => ({
        vaccinationRecordID: r.vaccinationRecordID ?? r.VaccinationRecordID,
        childName: r.childName ?? r.ChildName ?? 'Unknown',
        vaccineName: r.vaccineName ?? r.VaccineName ?? 'Unknown',
        doseNumber: r.doseNumber ?? r.DoseNumber,
        vaccinationDate: r.vaccinationDate ?? r.VaccinationDate,
      }))
      .sort((a, b) => new Date(b.vaccinationDate) - new Date(a.vaccinationDate))
  } catch (e) {
    console.error('fetchRecentlyVaccinated:', e)
  } finally {
    loadingRecords.value = false
  }
}

// ─────────────────────────────────────────────────────────────
// HELPERS
// ─────────────────────────────────────────────────────────────
const AVATAR_COLORS = ['#4a7c59', '#6b7c45', '#8b5e3c', '#4a6fa5', '#7b4f8e', '#5a7a6b', '#8b6914']
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