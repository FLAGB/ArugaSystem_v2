<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar />

    <!-- MAIN -->
    <div class="flex flex-col flex-1 overflow-hidden">
      <HealthcareHeader />

      <main class="flex-1 overflow-y-auto p-6">
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Reports</h1>
          <p class="text-sm text-slate-500 mt-1">Vaccination statistics and analytics</p>
        </div>

        <!-- STAT CARDS -->
        <div class="grid grid-cols-4 gap-4 mb-6">
          <div v-for="stat in statCards" :key="stat.label"
            class="bg-white rounded-xl border border-slate-200 p-5">
            <div class="mb-3">
              <div class="w-9 h-9 rounded-lg flex items-center justify-center" :class="stat.bg">
                <component :is="stat.icon" class="w-4 h-4" :class="stat.color" />
              </div>
            </div>
            <p class="text-3xl font-bold text-slate-800">{{ stat.value }}</p>
            <p class="text-xs text-slate-400 mt-1">{{ stat.label }}</p>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-6">

          <!-- BAR CHART: Vaccinations Per Week -->
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <h3 class="font-semibold text-slate-800 mb-4">Vaccinations Per Week</h3>
            <div class="flex items-end gap-2 h-40">
              <div v-for="bar in weeklyBars" :key="bar.day"
                class="flex-1 flex flex-col items-center gap-1">
                <span class="text-xs text-slate-400">{{ bar.count }}</span>
                <div class="w-full rounded-t-md bg-emerald-500 transition-all duration-500"
                  :style="{ height: (bar.count / maxBar * 100) + '%' }"></div>
                <span class="text-xs text-slate-400">{{ bar.day }}</span>
              </div>
            </div>
          </div>

          <!-- PIE CHART: Most Given Vaccines -->
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <h3 class="font-semibold text-slate-800 mb-4">Most Given Vaccines</h3>
            <div class="flex items-center gap-6">
              <!-- SVG Pie -->
              <svg viewBox="0 0 100 100" class="w-36 h-36 -rotate-90 shrink-0">
                <circle v-for="(seg, i) in pieSegments" :key="i"
                  cx="50" cy="50" r="40"
                  fill="none"
                  :stroke="seg.color"
                  stroke-width="20"
                  :stroke-dasharray="`${seg.dash} ${251.2 - seg.dash}`"
                  :stroke-dashoffset="-seg.offset" />
              </svg>
              <!-- Legend -->
              <div class="space-y-2">
                <div v-for="v in vaccineBreakdown" :key="v.name" class="flex items-center justify-between gap-8">
                  <div class="flex items-center gap-2">
                    <div class="w-2.5 h-2.5 rounded-full shrink-0" :style="{ backgroundColor: v.color }"></div>
                    <span class="text-xs text-slate-600">{{ v.name }}</span>
                  </div>
                  <span class="text-xs font-medium text-slate-700">{{ v.pct }}%</span>
                </div>
              </div>
            </div>
          </div>

          <!-- STATUS BREAKDOWN -->
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <h3 class="font-semibold text-slate-800 mb-4">Vaccination Status Breakdown</h3>
            <div class="space-y-3">
              <div v-for="s in statusBreakdown" :key="s.label">
                <div class="flex items-center justify-between mb-1">
                  <span class="text-xs text-slate-600">{{ s.label }}</span>
                  <span class="text-xs font-medium text-slate-700">{{ s.count }} ({{ s.pct }}%)</span>
                </div>
                <div class="h-2 bg-slate-100 rounded-full overflow-hidden">
                  <div class="h-full rounded-full transition-all duration-500"
                    :class="s.color" :style="{ width: s.pct + '%' }"></div>
                </div>
              </div>
            </div>
          </div>

          <!-- RECENT ACTIVITY -->
          <div class="bg-white rounded-xl border border-slate-200 p-5">
            <h3 class="font-semibold text-slate-800 mb-4">Recent Vaccinations</h3>
            <div class="space-y-3">
              <div v-for="rec in recentActivity" :key="rec.id"
                class="flex items-center gap-3">
                <div class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                  :style="{ backgroundColor: avatarColor(rec.name) }">
                  {{ initials(rec.name) }}
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-medium text-slate-800 truncate">{{ rec.name }}</p>
                  <p class="text-xs text-slate-400">{{ rec.vaccine }} · {{ rec.date }}</p>
                </div>
                <span class="text-xs text-emerald-600 font-medium shrink-0">Done</span>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>


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
  Search, Bell, LogOut, TrendingUp, AlertCircle, Clock, UserCheck,
  ListChecks
} from 'lucide-vue-next'

const API = API_ORIGIN
const router = useRouter()
const route  = useRoute()

const doctor = ref({ userId: '', fullName: '', userType: '' })
onMounted(() => {
  const u = getUser()
if (!u) { router.push('/'); return }
  doctor.value = { userId: u.UserID, fullName: `${u.FirstName} ${u.LastName}`, userType: u.UserType }
  fetchRecords()
})
const doctorInitials = computed(() =>
  doctor.value.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
)


// ── REAL DATA ────────────────────────────────────────────────
// Reuses the same endpoint as Vaccination Records (GET /api/VaccinationRecords/all)
// so this page always reflects what's actually in the database, not mock numbers.
const records      = ref([])
const recordsReady = ref(false)

async function fetchRecords() {
  try {
    const res = await axios.get(`${API}/api/VaccinationRecords/all`)
    records.value = res.data.map(r => ({
      recordId:         r.recordID   ?? r.RecordID,
      childId:          r.childID    ?? r.ChildID,
      childName:        r.childName  ?? r.ChildName  ?? '—',
      vaccineName:      r.vaccineName ?? r.VaccineName ?? `Vaccine ${r.vaccineID ?? r.VaccineID}`,
      dateAdministered: r.vaccinationDate ?? r.dateAdministered ?? r.DateAdministered ?? null,
      scheduledDate:    r.scheduledDate ?? r.ScheduledDate ?? null,
      status:           r.status     ?? r.Status     ?? 'Pending',
    }))
  } catch (err) {
    console.error('DoctorReports fetchRecords error:', err)
    records.value = []
  } finally {
    recordsReady.value = true
  }
}

// ── Date helpers ─────────────────────────────────────────────
function toDate(dateStr) {
  if (!dateStr) return null
  const d = new Date(dateStr)
  return isNaN(d.getTime()) ? null : d
}
function isSameDay(a, b) {
  return a && b && a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() && a.getDate() === b.getDate()
}
// Monday-start week containing `ref`
function weekBounds(ref) {
  const day   = (ref.getDay() + 6) % 7 // Mon=0 ... Sun=6
  const start = new Date(ref); start.setHours(0,0,0,0); start.setDate(ref.getDate() - day)
  const end   = new Date(start); end.setDate(start.getDate() + 6); end.setHours(23,59,59,999)
  return { start, end }
}

const today = new Date()
const { start: weekStart, end: weekEnd } = weekBounds(today)

const completedRecords = computed(() => records.value.filter(r => r.status === 'Completed'))

// ── STAT CARDS ───────────────────────────────────────────────
const vaccinatedTodayCount = computed(() =>
  completedRecords.value.filter(r => isSameDay(toDate(r.dateAdministered), today)).length
)
const weeklyTotalCount = computed(() =>
  completedRecords.value.filter(r => {
    const d = toDate(r.dateAdministered)
    return d && d >= weekStart && d <= weekEnd
  }).length
)
const missedCount = computed(() => records.value.filter(r => r.status === 'Missed').length)
// Follow-up Cases = distinct children who still have a Pending record (need a return visit)
const followUpCount = computed(() =>
  new Set(records.value.filter(r => r.status === 'Pending').map(r => r.childId)).size
)

const statCards = computed(() => [
  { label: 'Vaccinated Today', value: vaccinatedTodayCount.value, icon: Syringe,     color: 'text-emerald-600', bg: 'bg-emerald-50' },
  { label: 'Weekly Total',     value: weeklyTotalCount.value,     icon: TrendingUp,  color: 'text-blue-600',    bg: 'bg-blue-50'    },
  { label: 'Missed Vaccines',  value: missedCount.value,          icon: AlertCircle, color: 'text-red-500',     bg: 'bg-red-50'     },
  { label: 'Follow-up Cases',  value: followUpCount.value,        icon: UserCheck,   color: 'text-amber-600',   bg: 'bg-amber-50'   },
])

// ── BAR CHART: Vaccinations Per Week (Mon–Sun of current week) ─
const DAY_LABELS = ['Mon','Tue','Wed','Thu','Fri','Sat','Sun']
const weeklyBars = computed(() => {
  const counts = Array(7).fill(0)
  completedRecords.value.forEach(r => {
    const d = toDate(r.dateAdministered)
    if (d && d >= weekStart && d <= weekEnd) {
      counts[(d.getDay() + 6) % 7]++
    }
  })
  return DAY_LABELS.map((day, i) => ({ day, count: counts[i] }))
})
const maxBar = computed(() => Math.max(1, ...weeklyBars.value.map(b => b.count)))

// ── PIE CHART: Most Given Vaccines (from completed records) ────
const PALETTE = ['#1a3a2a', '#6b7c45', '#8b9a6b', '#c8d4b0', '#4a7c59', '#8b5e3c', '#4a6fa5', '#e8edd8']
const vaccineBreakdown = computed(() => {
  const total = completedRecords.value.length
  if (total === 0) return []
  const counts = {}
  completedRecords.value.forEach(r => {
    counts[r.vaccineName] = (counts[r.vaccineName] || 0) + 1
  })
  return Object.entries(counts)
    .sort((a, b) => b[1] - a[1])
    .map(([name, count], i) => ({
      name, pct: Math.round((count / total) * 100), color: PALETTE[i % PALETTE.length]
    }))
})

const pieSegments = computed(() => {
  const circumference = 251.2
  let offset = 0
  return vaccineBreakdown.value.map(v => {
    const dash = (v.pct / 100) * circumference
    const seg  = { color: v.color, dash, offset }
    offset += dash
    return seg
  })
})

// ── STATUS BREAKDOWN (built from whatever statuses actually exist) ─
const STATUS_COLORS = { Completed: 'bg-emerald-500', Pending: 'bg-amber-400', Scheduled: 'bg-blue-400', Missed: 'bg-red-400' }
const statusBreakdown = computed(() => {
  const total = records.value.length
  if (total === 0) return []
  const counts = {}
  records.value.forEach(r => { counts[r.status] = (counts[r.status] || 0) + 1 })
  return Object.entries(counts).map(([label, count]) => ({
    label, count, pct: Math.round((count / total) * 100), color: STATUS_COLORS[label] || 'bg-slate-400'
  }))
})

// ── RECENT ACTIVITY: last 5 completed, most recent first ───────
function formatRecordDate(dateStr) {
  const d = toDate(dateStr)
  if (!d) return '—'
  if (isSameDay(d, today)) return 'Today'
  const yesterday = new Date(today); yesterday.setDate(today.getDate() - 1)
  if (isSameDay(d, yesterday)) return 'Yesterday'
  return d.toLocaleDateString('en-PH', { month: 'short', day: 'numeric' })
}
const recentActivity = computed(() =>
  [...completedRecords.value]
    .sort((a, b) => (toDate(b.dateAdministered)?.getTime() || 0) - (toDate(a.dateAdministered)?.getTime() || 0))
    .slice(0, 5)
    .map(r => ({ id: r.recordId, name: r.childName, vaccine: r.vaccineName, date: formatRecordDate(r.dateAdministered) }))
)

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
</style>