<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar @logout="logout" />

    <div class="flex flex-col flex-1 overflow-hidden">
      <HealthcareHeader :worker="worker" title="Today's Queue" />

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <!-- LOAD ERROR -->
        <div v-if="loadError" class="mb-4 flex items-start gap-2 p-4 bg-red-50 border border-red-100 rounded-xl text-sm text-red-700">
          <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
          <span>{{ loadError }}</span>
        </div>

        <!-- ACTION ERROR (e.g. status update failed) -->
        <div v-if="actionError" class="mb-4 flex items-start gap-2 p-4 bg-red-50 border border-red-100 rounded-xl text-sm text-red-700">
          <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
          <span>{{ actionError }}</span>
        </div>

        <!--
          STATUS TABS
          Only "Waiting", "In Progress", "Completed" are confirmed statuses
          from the backend (see QueueController). Anything else the backend
          returns is grouped under "Other" instead of being invented (no
          confirmed "Ready" / "Late" status exists yet).
        -->
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
          <div v-for="q in filteredQueue" :key="q.queueID" class="bg-white rounded-xl border border-slate-200 p-5">
            <div class="flex items-start justify-between mb-3">
              <div>
                <div class="flex items-center gap-2">
                  <span class="text-sm font-bold text-slate-700">Queue #{{ q.queueNumber }}</span>
                  <span class="text-xs px-2 py-0.5 rounded-full font-medium" :class="statusBadgeClass(q.status)">
                    {{ q.status }}
                  </span>
                </div>
                <p class="text-xs text-slate-400 mt-1">
                  Requested by {{ q.requestBy }} · {{ formatDate(q.queueDate) }}
                </p>
              </div>
            </div>

            <div class="divide-y divide-slate-50">
              <div
                v-for="c in q.children"
                :key="c.childID"
                class="flex items-center justify-between py-3 first:pt-0 last:pb-0"
              >
                <div class="flex items-center gap-2.5">
                  <div class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                    :style="{ backgroundColor: avatarColor(c.name) }">
                    {{ initials(c.name) }}
                  </div>
                  <span class="text-sm font-medium text-slate-800">{{ c.name }}</span>
                </div>

                <button
                  @click="startVaccinating(q, c)"
                  :disabled="bucketOf(q.status) === 'completed' || startingKey === (q.queueID + c.childID)"
                  class="text-xs bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-1.5 rounded-lg font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-1.5"
                >
                  <Loader2 v-if="startingKey === (q.queueID + c.childID)" class="w-3.5 h-3.5 animate-spin" />
                  <Syringe v-else class="w-3.5 h-3.5" />
                  {{ bucketOf(q.status) === 'completed' ? 'Completed' : 'Start Vaccinating' }}
                </button>
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
import { Loader2, AlertCircle, ListChecks, Syringe } from 'lucide-vue-next'
import HealthcareSidebar from '@/components/Healthcare/Components/HealtcareSidebar.vue'
import HealthcareHeader from '@/components/Healthcare/Components/HealthcareHeader.vue'

const router = useRouter()

const API_BASE = `${(import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '')}/api`

function authHeaders(json = false) {
  const h = { Authorization: `Bearer ${getToken()}` }
  if (json) h['Content-Type'] = 'application/json'
  return h
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
    userType: u.UserType || u.userType || u.role,
  }

  fetchQueue()
})

function logout() {
  clearSession()
  router.push('/')
}

// ─────────────────────────────────────────────────────────────
// QUEUE
// GET /api/Queue — no confirmed date filter param, so "today"
// is filtered client-side against QueueDate.
// ─────────────────────────────────────────────────────────────
const rawQueue      = ref([])
const loadingQueue  = ref(false)
const loadError     = ref('')
const actionError   = ref('')

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
    rawQueue.value = data.filter(q => isToday(q.queueDate ?? q.QueueDate))
  } catch (e) {
    console.error('fetchQueue:', e)
    loadError.value = 'Could not load today\'s queue. Please refresh.'
  } finally {
    loadingQueue.value = false
  }
}

const todaysQueue = computed(() =>
  rawQueue.value.map(q => ({
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

// ─────────────────────────────────────────────────────────────
// TABS (only confirmed statuses get their own bucket)
// ─────────────────────────────────────────────────────────────
const activeTab = ref('all')

// Accepts both "InProgress" (backend canonical, written by CallNext
// and matched by GetBoard) and "In Progress" (written by older rows),
// so legacy data still buckets correctly.
function bucketOf(status) {
  const s = (status || '').toLowerCase().replace(/\s+/g, '')
  if (s === 'waiting') return 'waiting'
  if (s === 'inprogress') return 'inProgress'
  if (s === 'completed') return 'completed'
  return 'other'
}

const tabs = computed(() => {
  const counts = { all: todaysQueue.value.length, waiting: 0, inProgress: 0, completed: 0, other: 0 }
  for (const q of todaysQueue.value) counts[bucketOf(q.status)]++
  return [
    { key: 'all',        label: 'All',         count: counts.all },
    { key: 'waiting',    label: 'Waiting',     count: counts.waiting },
    { key: 'inProgress', label: 'In Progress', count: counts.inProgress },
    { key: 'completed',  label: 'Completed',   count: counts.completed },
    { key: 'other',      label: 'Other',       count: counts.other },
  ]
})

const filteredQueue = computed(() => {
  if (activeTab.value === 'all') return todaysQueue.value
  return todaysQueue.value.filter(q => bucketOf(q.status) === activeTab.value)
})

function statusBadgeClass(status) {
  const b = bucketOf(status)
  if (b === 'waiting')    return 'bg-amber-50 text-amber-700'
  if (b === 'inProgress') return 'bg-blue-50 text-blue-700'
  if (b === 'completed')  return 'bg-emerald-50 text-emerald-700'
  return 'bg-slate-100 text-slate-500'
}

// ─────────────────────────────────────────────────────────────
// START VACCINATING
// Updates queue status to "In Progress", then routes into the
// vaccination workflow for the selected child. Route is
// /healthcare/vaccination/:queueId as specified; the child is
// passed as a ?child= query param since a Queue entry can hold
// more than one child.
// ─────────────────────────────────────────────────────────────
const startingKey = ref('')

async function startVaccinating(queue, child) {
  actionError.value = ''
  const key = queue.queueID + child.childID
  startingKey.value = key

  try {
    // Backend canonical value is "InProgress" (no space) — CallNext
    // writes it and GetBoard filters on it. Writing "In Progress"
    // here would hide the visit from the Doctor dashboard board.
    if (bucketOf(queue.status) !== 'inProgress') {
      const res = await fetch(`${API_BASE}/Queue/${queue.queueID}/status`, {
        method: 'PUT',
        headers: authHeaders(true),
        body: JSON.stringify({ status: 'InProgress' }),
      })
      if (!res.ok) throw new Error(`Status update failed (${res.status})`)
      queue.status = 'InProgress'
    }

    router.push(`/healthcare/vaccination/${queue.queueID}?child=${child.childID}`)
  } catch (e) {
    console.error('startVaccinating:', e)
    actionError.value = 'Could not start vaccination for this patient. Please try again.'
  } finally {
    startingKey.value = ''
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
function formatDate(dateStr) {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('en-PH', { month: 'short', day: 'numeric', year: 'numeric' })
}
</script>