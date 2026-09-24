<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar @logout="logout" />

    <div class="flex flex-col flex-1 overflow-hidden">
      <HealthcareHeader :worker="worker" title="Vaccination Records" />

      <main class="flex-1 overflow-y-auto p-6">

        <div class="mb-6">
          <p class="text-sm text-slate-500">View vaccination history across all patients</p>
        </div>

        <!-- STAT CARDS -->
        <div class="grid grid-cols-4 gap-4 mb-6">
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <div class="flex items-center justify-between mb-3">
              <Syringe class="w-5 h-5 text-emerald-600" />
            </div>
            <p class="text-3xl font-bold text-slate-800">{{ stats.total }}</p>
            <p class="text-xs text-slate-400 mt-1">Total Records</p>
          </div>
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <div class="flex items-center justify-between mb-3">
              <CheckCircle class="w-5 h-5 text-emerald-500" />
            </div>
            <p class="text-3xl font-bold text-slate-800">{{ stats.completed }}</p>
            <p class="text-xs text-slate-400 mt-1">Completed</p>
          </div>
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <div class="flex items-center justify-between mb-3">
              <Clock class="w-5 h-5 text-amber-500" />
            </div>
            <p class="text-3xl font-bold text-slate-800">{{ stats.pending }}</p>
            <p class="text-xs text-slate-400 mt-1">Pending</p>
          </div>
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <div class="flex items-center justify-between mb-3">
              <AlertCircle class="w-5 h-5 text-red-500" />
            </div>
            <p class="text-3xl font-bold text-slate-800">{{ stats.missed }}</p>
            <p class="text-xs text-slate-400 mt-1">Missed / Overdue</p>
          </div>
        </div>

        <!-- FILTER ROW -->
        <div class="flex items-center gap-3 mb-4 flex-wrap">
          <div class="relative flex-1 max-w-md">
            <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
            <input v-model="searchQuery" type="text"
              placeholder="Search by child name, vaccine, or batch number..."
              class="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white" />
          </div>

          <select v-model="statusFilter"
            class="text-sm border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white text-slate-600">
            <option value="">All Status</option>
            <option value="Completed">Completed</option>
            <option value="Pending">Pending</option>
            <option value="Missed">Missed</option>
          </select>

          <select v-model="vaccineFilter"
            class="text-sm border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white text-slate-600">
            <option value="">All Vaccines</option>
            <option v-for="v in vaccineOptions" :key="v" :value="v">{{ v }}</option>
          </select>

          <input v-model="dateFrom" type="date"
            class="text-sm border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white text-slate-600" />
          <span class="text-slate-400 text-sm">to</span>
          <input v-model="dateTo" type="date"
            class="text-sm border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white text-slate-600" />

          <button v-if="hasActiveFilters" @click="clearFilters"
            class="flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-700 px-3 py-2 rounded-lg hover:bg-slate-100 transition-colors">
            <X class="w-4 h-4" /> Clear
          </button>
        </div>

        <p class="text-xs text-slate-400 mb-3">
          Showing {{ filteredRecords.length }} of {{ records.length }} records
        </p>

        <!-- LOAD ERROR -->
        <div v-if="loadError" class="mb-4 flex items-start gap-2 p-4 bg-red-50 border border-red-100 rounded-xl text-sm text-red-700">
          <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
          <span>{{ loadError }}</span>
        </div>

        <!-- TABLE -->
        <div class="bg-white rounded-xl border border-slate-200 overflow-hidden">

          <div v-if="loading" class="flex items-center justify-center py-16">
            <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
            <span class="ml-2 text-sm text-slate-400">Loading records...</span>
          </div>

          <div v-else-if="filteredRecords.length === 0" class="flex flex-col items-center justify-center py-16 text-slate-400">
            <FileText class="w-10 h-10 mb-3 opacity-30" />
            <p class="text-sm">No vaccination records found</p>
            <p v-if="hasActiveFilters" class="text-xs mt-1">Try clearing your filters</p>
          </div>

          <div v-else class="overflow-x-auto">
            <table class="w-full text-sm">
              <thead class="bg-slate-50 border-b border-slate-200">
                <tr class="text-xs text-slate-500 uppercase tracking-wide">
                  <th class="px-5 py-3.5 text-left font-medium">Child Name</th>
                  <th class="px-5 py-3.5 text-left font-medium">Parent</th>
                  <th class="px-5 py-3.5 text-left font-medium">Vaccine Name</th>
                  <th class="px-5 py-3.5 text-left font-medium">Dose</th>
                  <th class="px-5 py-3.5 text-left font-medium">Date Given</th>
                  <th class="px-5 py-3.5 text-left font-medium">Batch No.</th>
                  <th class="px-5 py-3.5 text-left font-medium">Administered By</th>
                  <th class="px-5 py-3.5 text-left font-medium">Status</th>
                  <th class="px-5 py-3.5 text-left font-medium">Remarks</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="rec in paginatedRecords" :key="rec.recordId"
                  class="hover:bg-slate-50 transition-colors">

                  <td class="px-5 py-3.5">
                    <div class="flex items-center gap-2.5">
                      <div class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                        :style="{ backgroundColor: avatarColor(rec.childName) }">
                        {{ initials(rec.childName) }}
                      </div>
                      <span class="font-medium text-slate-800">{{ rec.childName }}</span>
                    </div>
                  </td>

                  <td class="px-5 py-3.5 text-slate-500">{{ rec.parentName }}</td>
                  <td class="px-5 py-3.5 font-medium text-slate-700">{{ rec.vaccineName }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ rec.doseNumber }}{{ ordinal(rec.doseNumber) }}</td>

                  <td class="px-5 py-3.5">
                    <div v-if="rec.dateAdministered">
                      <p class="text-slate-700">{{ formatDate(rec.dateAdministered) }}</p>
                      <p v-if="rec.daysLate > 0" class="text-xs text-amber-600 font-medium mt-0.5">
                        {{ rec.daysLate }}d late
                      </p>
                    </div>
                    <span v-else class="text-slate-400">—</span>
                  </td>

                  <td class="px-5 py-3.5 font-mono text-xs text-slate-500">{{ rec.lotNumber || '—' }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ rec.administeredByName || '—' }}</td>

                  <td class="px-5 py-3.5">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="statusClass(rec.status)">
                      {{ rec.status }}
                    </span>
                  </td>

                  <td class="px-5 py-3.5 text-slate-500 max-w-[160px] truncate" :title="rec.remarks">
                    {{ rec.remarks || '—' }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- PAGINATION -->
          <div v-if="filteredRecords.length > 0" class="flex items-center justify-between px-5 py-3.5 border-t border-slate-100">
            <p class="text-xs text-slate-400">Page {{ currentPage }} of {{ totalPages }}</p>
            <div class="flex items-center gap-2">
              <button @click="currentPage = Math.max(1, currentPage - 1)" :disabled="currentPage === 1"
                class="p-1.5 text-slate-400 hover:text-slate-700 disabled:opacity-30 disabled:cursor-not-allowed rounded-lg hover:bg-slate-100 transition-colors">
                <ChevronLeft class="w-4 h-4" />
              </button>
              <button @click="currentPage = Math.min(totalPages, currentPage + 1)" :disabled="currentPage === totalPages"
                class="p-1.5 text-slate-400 hover:text-slate-700 disabled:opacity-30 disabled:cursor-not-allowed rounded-lg hover:bg-slate-100 transition-colors">
                <ChevronRight class="w-4 h-4" />
              </button>
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
  Syringe, CheckCircle, Clock, AlertCircle, Search, X, Loader2,
  FileText, ChevronLeft, ChevronRight
} from 'lucide-vue-next'
import HealthcareSidebar from '@/components/Healthcare/Components/HealtcareSidebar.vue'
import HealthcareHeader from '@/components/Healthcare/Components/HealthcareHeader.vue'

const router = useRouter()

const API_BASE = `${(import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '')}/api`

function authHeaders() {
  return { Authorization: `Bearer ${getToken()}` }
}

// ─────────────────────────────────────────────────────────────
// AUTH — same pattern as HealthcareQueue.vue / HealthcarePatient.vue
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

  fetchRecords()
})

function logout() {
  clearSession()
  router.push('/')
}

// ─────────────────────────────────────────────────────────────
// RECORDS — GET /api/VaccinationRecords/all, same shaped DTO
// endpoint already confirmed working on the Doctor side
// (childName, vaccineName, parentName, lotNumber,
// administeredByName all present). Read-only here — nurses/
// midwives create records via the Queue → Start Vaccinating
// flow, not from this list.
// ─────────────────────────────────────────────────────────────
const records    = ref([])
const loading    = ref(false)
const loadError  = ref('')

function computeDaysLate(dateAdministered, scheduledDate) {
  if (!dateAdministered || !scheduledDate) return 0
  return Math.max(0, Math.round(
    (new Date(dateAdministered) - new Date(scheduledDate)) / 86400000
  ))
}

async function fetchRecords() {
  loading.value = true
  loadError.value = ''
  try {
    const res = await fetch(`${API_BASE}/VaccinationRecords/all`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`VaccinationRecords request failed (${res.status})`)
    const data = await res.json()
    records.value = data.map(r => ({
      recordId:           r.vaccinationRecordID ?? r.recordID ?? r.RecordID,
      childId:            r.childID    ?? r.ChildID,
      childName:          r.childName  ?? r.ChildName  ?? '—',
      parentName:         r.parentName ?? r.ParentName ?? '—',
      vaccineId:          r.vaccineID  ?? r.VaccineID,
      vaccineName:        r.vaccineName ?? r.VaccineName ?? `Vaccine ${r.vaccineID ?? r.VaccineID}`,
      doseNumber:         r.doseNumber ?? r.DoseNumber,
      dateAdministered:   r.vaccinationDate ?? r.dateAdministered ?? r.DateAdministered ?? null,
      scheduledDate:      r.scheduledDate ?? r.ScheduledDate ?? null,
      lotNumber:          r.lotNumber  ?? r.LotNumber  ?? null,
      status:             r.status     ?? r.Status     ?? 'Pending',
      administeredByName: r.administeredByName ?? r.AdministeredByName ?? null,
      remarks:            r.nurseObservation ?? r.remarks ?? r.Remarks ?? null,
      daysLate:           computeDaysLate(
                            r.vaccinationDate ?? r.dateAdministered ?? r.DateAdministered,
                            r.scheduledDate    ?? r.ScheduledDate
                          ),
    }))
  } catch (err) {
    console.error('fetchRecords error:', err)
    loadError.value = 'Could not load vaccination records. Please refresh.'
  } finally {
    loading.value = false
  }
}

// ── STATS ─────────────────────────────────────────────────────────
const stats = computed(() => ({
  total:     records.value.length,
  completed: records.value.filter(r => r.status === 'Completed').length,
  pending:   records.value.filter(r => r.status === 'Pending').length,
  missed:    records.value.filter(r => r.status === 'Missed').length,
}))

// ── FILTERS ───────────────────────────────────────────────────────
const searchQuery   = ref('')
const statusFilter  = ref('')
const vaccineFilter = ref('')
const dateFrom      = ref('')
const dateTo        = ref('')

const vaccineOptions = computed(() =>
  [...new Set(records.value.map(r => r.vaccineName))].sort()
)

const hasActiveFilters = computed(() =>
  searchQuery.value || statusFilter.value || vaccineFilter.value || dateFrom.value || dateTo.value
)

function clearFilters() {
  searchQuery.value   = ''
  statusFilter.value  = ''
  vaccineFilter.value = ''
  dateFrom.value      = ''
  dateTo.value         = ''
}

const filteredRecords = computed(() => {
  let list = records.value
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    list = list.filter(r =>
      r.childName?.toLowerCase().includes(q)  ||
      r.parentName?.toLowerCase().includes(q) ||
      r.vaccineName?.toLowerCase().includes(q)||
      r.lotNumber?.toLowerCase().includes(q)
    )
  }
  if (statusFilter.value)  list = list.filter(r => r.status === statusFilter.value)
  if (vaccineFilter.value) list = list.filter(r => r.vaccineName === vaccineFilter.value)
  if (dateFrom.value)      list = list.filter(r => r.dateAdministered >= dateFrom.value)
  if (dateTo.value)        list = list.filter(r => r.dateAdministered <= dateTo.value)

  return [...list].sort((a, b) => {
    if (a.status === 'Pending' && b.status !== 'Pending') return -1
    if (b.status === 'Pending' && a.status !== 'Pending') return 1
    const da = a.dateAdministered || a.scheduledDate || ''
    const db = b.dateAdministered || b.scheduledDate || ''
    return db.localeCompare(da)
  })
})

// ── PAGINATION ────────────────────────────────────────────────────
const currentPage = ref(1)
const pageSize    = ref(10)

const totalPages = computed(() =>
  Math.max(1, Math.ceil(filteredRecords.value.length / pageSize.value))
)
const paginatedRecords = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredRecords.value.slice(start, start + pageSize.value)
})

// ── HELPERS ───────────────────────────────────────────────────────
function formatDate(dateStr) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('en-PH', {
    month: 'short', day: 'numeric', year: 'numeric'
  })
}

function ordinal(n) {
  const s = ['th', 'st', 'nd', 'rd'], v = n % 100
  return s[(v - 20) % 10] || s[v] || s[0]
}

function statusClass(status) {
  return {
    Completed: 'bg-emerald-100 text-emerald-700',
    Pending:   'bg-amber-100 text-amber-700',
    Missed:    'bg-red-100 text-red-600',
    Scheduled: 'bg-blue-100 text-blue-700',
  }[status] || 'bg-slate-100 text-slate-600'
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
