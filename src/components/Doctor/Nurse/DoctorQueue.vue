<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <DoctorSidebar />

    <!-- MAIN CONTENT -->
    <div class="flex flex-col flex-1 overflow-hidden">

      <!-- TOP BAR -->
      <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 shrink-0">
        <h1 class="font-semibold text-slate-800">Today's Queue</h1>
        <div class="flex items-center gap-3">
          <div class="text-right">
            <p class="text-sm font-semibold">{{ doctor.fullName }}</p>
            <p class="text-xs text-slate-400">{{ doctor.userType }}</p>
          </div>
          <div class="w-9 h-9 bg-emerald-700 rounded-full flex items-center justify-center text-white text-sm font-bold">
            {{ doctorInitials }}
          </div>
        </div>
      </header>

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <!-- STATUS TABS -->
        <div class="flex items-center gap-2 mb-5">
          <button
            v-for="tab in tabs"
            :key="tab.key"
            @click="activeTab = tab.key"
            class="px-4 py-2 rounded-lg text-sm font-medium transition-colors"
            :class="activeTab === tab.key ? 'bg-emerald-600 text-white' : 'bg-white text-slate-500 border border-slate-200 hover:bg-slate-50'"
          >
            {{ tab.label }}
            <span class="ml-1 text-xs opacity-75">({{ tab.count }})</span>
          </button>
        </div>

        <!-- LOADING -->
        <div v-if="loadingQueue" class="flex items-center justify-center py-20">
          <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
          <span class="ml-2 text-sm text-slate-400">Loading queue...</span>
        </div>

        <!-- EMPTY -->
        <div v-else-if="filteredQueue.length === 0" class="flex flex-col items-center justify-center py-20 text-slate-400">
          <ListChecks class="w-8 h-8 mb-2 opacity-40" />
          <p class="text-sm">No patients in this view</p>
        </div>

        <!-- QUEUE LIST -->
        <div v-else class="space-y-3">
          <div v-for="q in filteredQueue" :key="q.queueID"
            class="bg-white rounded-xl border p-4 flex items-center justify-between"
            :class="q.queueStatus === 'InProgress' ? 'border-emerald-300 ring-1 ring-emerald-100' : 'border-slate-200'">

            <div class="flex items-center gap-3">
              <div class="w-9 h-9 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                :style="{ backgroundColor: avatarColor(q.childName || q.parentName) }">
                {{ initials(q.childName || q.parentName) }}
              </div>
              <div>
                <div class="flex items-center gap-2">
                  <span class="text-sm font-bold text-slate-700">#{{ q.queueNumber }}</span>
                  <span class="text-sm font-medium text-slate-800">{{ q.childName || q.parentName || 'Walk-in' }}</span>
                </div>
                <p v-if="q.vaccineName" class="text-xs text-slate-400 mt-0.5">
                  {{ q.vaccineName }}<span v-if="q.doseNumber"> · Dose {{ q.doseNumber }}</span>
                </p>
              </div>
            </div>

            <span class="text-xs px-2.5 py-1 rounded-full font-medium shrink-0" :class="statusBadgeClass(q.queueStatus)">
              {{ statusLabel(q.queueStatus) }}
            </span>
          </div>
        </div>

      </main>
    </div>
  </div>
</template>

<script setup>
import { getUser } from '@/utils/auth'
import DoctorSidebar from './DoctorSidebar.vue'
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import {
  Home, Users, Calendar, Syringe, FileText, Settings,
  LogOut, Loader2, ListChecks, Info
} from 'lucide-vue-next'

const router = useRouter()
const API = import.meta.env.VITE_API_URL || 'http://localhost:57147'

// ─────────────────────────────────────────────────────────────
// AUTH — same pattern as DoctorHomepage.vue
// ─────────────────────────────────────────────────────────────
const doctor = ref({ userId: '', fullName: '', userType: '' })

onMounted(() => {
  const u = getUser()
  if (!u) { router.push('/'); return }
  doctor.value = {
    userId:   u.UserID,
    fullName: `${u.FirstName} ${u.LastName}`,
    userType: u.UserType,
  }
  fetchQueue()
  queuePollHandle = setInterval(fetchQueue, 5000)
})

onUnmounted(() => {
  if (queuePollHandle) clearInterval(queuePollHandle)
})

const doctorInitials = computed(() =>
  doctor.value.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
)


// ─────────────────────────────────────────────────────────────
// NAVIGATION — mirrors DoctorHomepage.vue's navItems
// ─────────────────────────────────────────────────────────────
// ─────────────────────────────────────────────────────────────
// LIVE QUEUE — same GET /api/Queue/board endpoint the Homepage
// widget already polls, so both views always agree.
// Read-only here: no call-next / complete actions. Those live on
// the Staff/Admission Queue page.
// ─────────────────────────────────────────────────────────────
const queueBoard   = ref([])
const loadingQueue = ref(false)
let queuePollHandle = null

async function fetchQueue() {
  loadingQueue.value = queueBoard.value.length === 0
  try {
    const res = await axios.get(`${API}/api/Queue/board`)
    queueBoard.value = res.data
  } catch (e) {
    console.error('fetchQueue:', e)
  } finally {
    loadingQueue.value = false
  }
}

// ─────────────────────────────────────────────────────────────
// TABS — buckets on queueStatus, same canonical values the board
// endpoint returns ("Waiting" / "InProgress" / "Completed").
// Anything else is grouped under "Other" rather than invented.
// ─────────────────────────────────────────────────────────────
const activeTab = ref('all')

function bucketOf(status) {
  const s = (status || '').toLowerCase().replace(/\s+/g, '')
  if (s === 'waiting') return 'waiting'
  if (s === 'inprogress') return 'inProgress'
  if (s === 'completed') return 'completed'
  return 'other'
}

function statusLabel(status) {
  const b = bucketOf(status)
  if (b === 'waiting') return 'Waiting'
  if (b === 'inProgress') return 'In Progress'
  if (b === 'completed') return 'Completed'
  return status || 'Other'
}

function statusBadgeClass(status) {
  const b = bucketOf(status)
  if (b === 'waiting')    return 'bg-amber-50 text-amber-700'
  if (b === 'inProgress') return 'bg-blue-50 text-blue-700'
  if (b === 'completed')  return 'bg-emerald-50 text-emerald-700'
  return 'bg-slate-100 text-slate-500'
}

const tabs = computed(() => {
  const counts = { all: queueBoard.value.length, waiting: 0, inProgress: 0, completed: 0, other: 0 }
  for (const q of queueBoard.value) counts[bucketOf(q.queueStatus)]++
  return [
    { key: 'all',        label: 'All',         count: counts.all },
    { key: 'waiting',    label: 'Waiting',     count: counts.waiting },
    { key: 'inProgress', label: 'In Progress', count: counts.inProgress },
    { key: 'completed',  label: 'Completed',   count: counts.completed },
    { key: 'other',      label: 'Other',       count: counts.other },
  ]
})

const filteredQueue = computed(() => {
  if (activeTab.value === 'all') return queueBoard.value
  return queueBoard.value.filter(q => bucketOf(q.queueStatus) === activeTab.value)
})

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
</script>