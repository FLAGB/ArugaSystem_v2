<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar />

    <!-- MAIN CONTENT -->
    <div class="flex flex-col flex-1 overflow-hidden">

      <HealthcareHeader title="Today's Queue" />

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <!-- MY STATION -->
        <div class="mb-5 rounded-xl border p-4 flex items-center gap-3"
          :class="myStation ? 'bg-emerald-50 border-emerald-100' : 'bg-amber-50 border-amber-100'">
          <DoorOpen class="w-5 h-5 shrink-0" :class="myStation ? 'text-emerald-600' : 'text-amber-600'" />
          <p v-if="myStation" class="text-sm text-emerald-800">
            You are at <strong>{{ myStation.roomName }}</strong>.
            <span v-if="myStation.currentPatient">Now with you: <strong>#{{ myStation.currentQueueNumber }} {{ myStation.currentPatient }}</strong>.</span>
            <span v-else>Waiting for the Admission Staff to send the next patient.</span>
          </p>
          <p v-else class="text-sm text-amber-800">
            You haven't been assigned to a station yet. Ask the Admission Staff to put you at a station — you can record
            vaccinations once they send a patient to you.
          </p>
        </div>

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
          <span class="ml-auto text-xs text-slate-400">Updates automatically</span>
        </div>

        <!-- LOADING -->
        <div v-if="loadingQueue" class="flex items-center justify-center py-20">
          <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
          <span class="ml-2 text-sm text-slate-400">Loading queue...</span>
        </div>

        <!-- EMPTY -->
        <div v-else-if="filteredQueue.length === 0" class="flex flex-col items-center justify-center py-20 text-slate-400">
          <ListChecks class="w-8 h-8 mb-2 opacity-40" />
          <p class="text-sm">{{ activeTab === 'mine' ? 'No patient at your station right now' : 'No patients in this view' }}</p>
        </div>

        <!-- QUEUE LIST — one card per visit (a parent can bring several children) -->
        <div v-else class="space-y-3">
          <div v-for="q in filteredQueue" :key="q.queueID"
            class="bg-white rounded-xl border p-5"
            :class="isAtMyStation(q) ? 'border-emerald-300 ring-1 ring-emerald-100' : 'border-slate-200'">

            <div class="flex items-start justify-between mb-3">
              <div>
                <div class="flex items-center gap-2">
                  <span class="text-sm font-bold text-slate-700">Queue #{{ q.queueNumber }}</span>
                  <span class="text-xs px-2.5 py-0.5 rounded-full font-medium" :class="statusBadgeClass(q.status)">
                    {{ statusLabel(q.status) }}
                  </span>
                </div>
                <p class="text-xs text-slate-400 mt-1">
                  Parent / Guardian: {{ withRelationship(q.requestBy, q.requestByRelationship) }}<span v-if="q.checkedInAt"> · Checked in {{ formatTime(q.checkedInAt) }}</span>
                </p>
              </div>
              <span v-if="q.stationName" class="text-xs font-medium px-2.5 py-1 rounded-lg"
                :class="isAtMyStation(q) ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-500'">
                {{ q.stationName }} · {{ isAtMyStation(q) ? 'You' : (q.assignedWorkerName || '—') }}
              </span>
            </div>

            <div class="divide-y divide-slate-50">
              <div v-for="c in q.children" :key="c.childID"
                class="flex items-center justify-between py-3 first:pt-0 last:pb-0">
                <div class="flex items-center gap-2.5">
                  <div class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                    :style="{ backgroundColor: avatarColor(c.name) }">
                    {{ initials(c.name) }}
                  </div>
                  <span class="text-sm font-medium text-slate-800">{{ c.name }}</span>
                </div>

                <button v-if="isAtMyStation(q)"
                  @click="startVaccinating(q, c)"
                  class="text-xs bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-1.5 rounded-lg font-medium transition-colors flex items-center gap-1.5"
                >
                  <Syringe class="w-3.5 h-3.5" /> Start Vaccinating
                </button>
                <span v-else class="text-xs text-slate-400">{{ actionHint(q) }}</span>
              </div>
            </div>
          </div>
        </div>

      </main>
    </div>
  </div>
</template>

<script setup>
import HealthcareSidebar from './Components/HealthcareSidebar.vue'
import HealthcareHeader from './Components/HealthcareHeader.vue'
import { fetchMyStation, isAtMyStation } from './Components/station.js'
import { withRelationship } from '@/utils/format'
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { Loader2, ListChecks, Syringe, DoorOpen } from 'lucide-vue-next'

const router = useRouter()
const API = import.meta.env.VITE_API_URL || 'http://localhost:57147'

// ─────────────────────────────────────────────────────────────
// LIVE QUEUE — GET /api/Queue/today (every visit today, with the station
// and health worker the Admission Staff assigned), polled so the board
// stays in step with the staff dashboard.
// ─────────────────────────────────────────────────────────────
const queue        = ref([])
const myStation    = ref(null)
const loadingQueue = ref(false)
let pollHandle = null

onMounted(() => {
  refresh()
  pollHandle = setInterval(refresh, 5000)
})

onUnmounted(() => {
  if (pollHandle) clearInterval(pollHandle)
})

async function refresh() {
  loadingQueue.value = queue.value.length === 0
  try {
    const [q, station] = await Promise.all([
      axios.get(`${API}/api/Queue/today`),
      fetchMyStation(),
    ])
    queue.value = q.data
    myStation.value = station
  } catch (e) {
    console.error('refresh queue:', e)
  } finally {
    loadingQueue.value = false
  }
}

// ─────────────────────────────────────────────────────────────
// TABS — "InProgress" is the backend's canonical value (older rows
// may say "In Progress").
// ─────────────────────────────────────────────────────────────
const activeTab = ref('mine')

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
  if (b === 'inProgress') return 'At Station'
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
  const counts = { mine: 0, all: queue.value.length, waiting: 0, inProgress: 0, completed: 0 }
  for (const q of queue.value) {
    const b = bucketOf(q.status)
    if (b in counts) counts[b]++
    if (isAtMyStation(q)) counts.mine++
  }
  return [
    { key: 'mine',       label: 'My Station',  count: counts.mine },
    { key: 'all',        label: 'All',         count: counts.all },
    { key: 'waiting',    label: 'Waiting',     count: counts.waiting },
    { key: 'inProgress', label: 'At a Station', count: counts.inProgress },
    { key: 'completed',  label: 'Completed',   count: counts.completed },
  ]
})

const filteredQueue = computed(() => {
  if (activeTab.value === 'all') return queue.value
  if (activeTab.value === 'mine') return queue.value.filter(isAtMyStation)
  return queue.value.filter(q => bucketOf(q.status) === activeTab.value)
})

// Why a visit can't be started by this worker.
function actionHint(q) {
  const b = bucketOf(q.status)
  if (b === 'completed') return 'Completed'
  if (b === 'waiting') return 'Waiting for station assignment'
  return `With ${q.assignedWorkerName || 'another health worker'}`
}

// Opens the vaccination visit for a child at my station. The visit is
// already In Progress — the Admission Staff set that when assigning it.
function startVaccinating(q, child) {
  router.push(`/healthcare/vaccination/${q.queueID}?child=${child.childID}`)
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
function formatTime(value) {
  return new Date(value).toLocaleTimeString('en-PH', { hour: 'numeric', minute: '2-digit' })
}
</script>
