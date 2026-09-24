<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <DoctorSidebar />

    <!-- MAIN -->
    <div class="flex flex-col flex-1 overflow-hidden">
      <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 shrink-0">
        <div class="relative w-80">
          </div>
        <div class="flex items-center gap-4">
          <button @click="showNotifications = true" class="relative p-2 text-slate-400 hover:text-slate-600 transition-colors">
            <Bell class="w-5 h-5" />
            <span v-if="unreadCount > 0" class="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
          </button>
          <div class="flex items-center gap-3">
            <div class="text-right">
              <p class="text-sm font-semibold">{{ doctor.fullName }}</p>
              <p class="text-xs text-slate-400">{{ doctor.userType }}</p>
            </div>
            <div class="w-9 h-9 bg-emerald-700 rounded-full flex items-center justify-center text-white text-sm font-bold">
              {{ doctorInitials }}
            </div>
          </div>
        </div>
      </header>

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

    <!-- NOTIFICATION PANEL -->
    <Transition name="notif-panel">
      <div v-if="showNotifications" class="fixed inset-0 z-[200] flex justify-end" @click.self="showNotifications = false">
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm" @click="showNotifications = false"></div>
        <div class="relative w-full max-w-[400px] h-full bg-white shadow-2xl flex flex-col">
          <div class="bg-slate-800 px-6 py-5 flex items-center justify-between text-white shrink-0">
            <div class="flex items-center gap-3">
              <Bell class="w-5 h-5" />
              <span class="font-bold text-base">Notifications</span>
              <span v-if="unreadCount > 0" class="bg-red-500 text-[10px] px-2.5 py-1 rounded-full font-bold">{{ unreadCount }} NEW</span>
            </div>
            <div class="flex items-center gap-3">
              <button v-if="unreadCount > 0" @click="markAllRead" class="text-[10px] font-bold text-white/70 hover:text-white uppercase tracking-wide transition-colors">Mark all read</button>
              <button @click="showNotifications = false" class="w-8 h-8 rounded-lg bg-white/10 hover:bg-white/20 flex items-center justify-center text-lg transition-colors">✕</button>
            </div>
          </div>
          <div class="flex-1 overflow-y-auto">
            <div v-if="notifsLoading" class="flex flex-col items-center justify-center h-40 text-slate-400">
              <p class="text-2xl mb-2 animate-pulse">🔔</p><p class="text-xs font-medium">Loading notifications...</p>
            </div>
            <div v-else-if="notifications.length === 0" class="flex flex-col items-center justify-center h-40 text-slate-400 px-8 text-center">
              <p class="text-3xl mb-3">✅</p><p class="text-sm font-bold text-slate-600">All caught up!</p><p class="text-xs text-slate-400 mt-1">No notifications.</p>
            </div>
            <div v-else class="p-3 space-y-2">
              <div v-for="notif in notifications" :key="notif.notificationID" @click="markRead(notif)"
                   :class="notif.isRead ? 'bg-white border-transparent' : 'bg-emerald-50 border-emerald-200'"
                   class="p-4 rounded-xl border cursor-pointer hover:shadow-sm transition-all">
                <div class="flex items-start gap-3">
                  <div class="w-10 h-10 rounded-lg bg-slate-100 flex items-center justify-center text-lg shrink-0">🔔</div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-start justify-between gap-2">
                      <p class="text-xs font-bold text-slate-800 leading-tight">{{ notif.title }}</p>
                      <span v-if="!notif.isRead" class="w-2 h-2 rounded-full bg-emerald-500 shrink-0 mt-1"></span>
                    </div>
                    <p class="text-[11px] text-slate-500 mt-1 leading-relaxed">{{ notif.message }}</p>
                    <p class="text-[9px] text-slate-400 mt-2 font-bold uppercase tracking-wide">{{ formatRelativeTime(notif.createdAt) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>

  </div>
</template>

<script setup>
import { getUser } from '@/utils/auth'
import DoctorSidebar from './DoctorSidebar.vue'
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import axios from 'axios'
import {
  Home, Users, Calendar, Syringe, FileText, Settings,
  Search, Bell, LogOut, TrendingUp, AlertCircle, Clock, UserCheck,
  ListChecks
} from 'lucide-vue-next'

const API = import.meta.env.VITE_API_URL || 'http://localhost:57147'
const router = useRouter()
const route  = useRoute()

const doctor = ref({ userId: '', fullName: '', userType: '' })
onMounted(() => {
  const u = getUser()
if (!u) { router.push('/'); return }
  doctor.value = { userId: u.UserID, fullName: `${u.FirstName} ${u.LastName}`, userType: u.UserType }
  fetchNotifications()
  fetchRecords()
})
const doctorInitials = computed(() =>
  doctor.value.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
)

// ── Notifications ────────────────────────────────────────────
const showNotifications = ref(false)
const notifications     = ref([])
const notifsLoading     = ref(false)
const unreadCount        = computed(() => notifications.value.filter(n => !n.isRead).length)

function formatRelativeTime(dateStr) {
  if (!dateStr) return ''
  const diff  = Date.now() - new Date(dateStr).getTime()
  const mins  = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days  = Math.floor(diff / 86400000)
  if (mins < 1)   return 'Just now'
  if (mins < 60)  return `${mins}m ago`
  if (hours < 24) return `${hours}h ago`
  if (days < 7)   return `${days}d ago`
  return new Date(dateStr).toLocaleDateString('en-PH', { month: 'short', day: 'numeric' })
}

async function fetchNotifications() {
  if (!doctor.value.userId) return
  notifsLoading.value = true
  try {
    const res = await axios.get(`${API}/api/Notifications/user/${doctor.value.userId}`)
    notifications.value = res.data
  } catch {
    notifications.value = []
  } finally {
    notifsLoading.value = false
  }
}

async function markRead(notif) {
  if (notif.isRead) return
  notif.isRead = true
  try { await axios.patch(`${API}/api/Notifications/mark-read/${notif.notificationID}`) } catch {}
}

async function markAllRead() {
  notifications.value.forEach(n => { n.isRead = true })
  try { await axios.patch(`${API}/api/Notifications/mark-all-read/user/${doctor.value.userId}`) } catch {}
}

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
.notif-panel-enter-active, .notif-panel-leave-active { transition: opacity 0.25s ease; }
.notif-panel-enter-active > div:last-child, .notif-panel-leave-active > div:last-child { transition: transform 0.3s cubic-bezier(0.4,0,0.2,1); }
.notif-panel-enter-from, .notif-panel-leave-to { opacity: 0; }
.notif-panel-enter-from > div:last-child, .notif-panel-leave-to > div:last-child { transform: translateX(100%); }
</style>