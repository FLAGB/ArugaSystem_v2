<!--
  HealthcareHeader.vue
  ====================
  Shared top bar for every Healthcare Worker (Doctor / Nurse) page:
  page title, notification bell + slide-out panel, and the signed-in
  worker's name/role badge.

  Before the Doctor and Healthcare folders were merged, every page carried
  its own copy of this header AND its own copy of the notification panel
  and fetch logic — this is now the single place for both.

  Usage:  <HealthcareHeader title="Patients (Children)" />
-->
<template>
  <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 shrink-0">
    <h1 v-if="title" class="font-semibold text-slate-800">{{ title }}</h1>
    <div v-else></div>

    <div class="flex items-center gap-4">
      <button @click="openPanel" class="relative p-2 text-slate-400 hover:text-slate-600 transition-colors" aria-label="Notifications">
        <Bell class="w-5 h-5" />
        <span v-if="unreadCount > 0" class="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
      </button>
      <div class="flex items-center gap-3">
        <div class="text-right">
          <p class="text-sm font-semibold">{{ worker.fullName }}</p>
          <p class="text-xs text-slate-400">{{ worker.roleLabel }}</p>
        </div>
        <div class="w-9 h-9 bg-emerald-700 rounded-full flex items-center justify-center text-white text-sm font-bold">
          {{ worker.initials }}
        </div>
      </div>
    </div>
  </header>

  <!-- NOTIFICATION PANEL -->
  <Teleport to="body">
    <Transition name="notif-panel">
      <div v-if="showPanel" class="fixed inset-0 z-[200] flex justify-end" @click.self="showPanel = false">
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm" @click="showPanel = false"></div>
        <div class="relative w-full max-w-[400px] h-full bg-white shadow-2xl flex flex-col">
          <div class="bg-slate-800 px-6 py-5 flex items-center justify-between text-white shrink-0">
            <div class="flex items-center gap-3">
              <Bell class="w-5 h-5" />
              <span class="font-bold text-base">Notifications</span>
              <span v-if="unreadCount > 0" class="bg-red-500 text-[10px] px-2.5 py-1 rounded-full font-bold">{{ unreadCount }} NEW</span>
            </div>
            <div class="flex items-center gap-3">
              <button v-if="unreadCount > 0" @click="markAllRead" class="text-[10px] font-bold text-white/70 hover:text-white uppercase tracking-wide transition-colors">Mark all read</button>
              <button @click="showPanel = false" class="w-8 h-8 rounded-lg bg-white/10 hover:bg-white/20 flex items-center justify-center text-lg transition-colors">✕</button>
            </div>
          </div>
          <div class="flex-1 overflow-y-auto">
            <div v-if="loading" class="flex flex-col items-center justify-center h-40 text-slate-400">
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
                  <div class="w-10 h-10 rounded-lg bg-slate-100 flex items-center justify-center text-lg shrink-0">{{ iconFor(notif.type) }}</div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-start justify-between gap-2">
                      <p class="text-xs font-bold text-slate-800 leading-tight">{{ notif.title }}</p>
                      <span v-if="!notif.isRead" class="w-2 h-2 rounded-full bg-emerald-500 shrink-0 mt-1"></span>
                    </div>
                    <p class="text-[11px] text-slate-500 mt-1 leading-relaxed whitespace-pre-line">{{ notif.message }}</p>
                    <p class="text-[9px] text-slate-400 mt-2 font-bold uppercase tracking-wide">{{ formatRelativeTime(notif.createdAt) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { API_ORIGIN } from '@/utils/apiBase'
import { ref, computed, onMounted, onUnmounted } from 'vue'
import axios from 'axios'
import { Bell } from 'lucide-vue-next'
import { getUser } from '@/utils/auth'

defineProps({
  title: { type: String, default: '' },
})

const API = API_ORIGIN

// ── Signed-in worker ─────────────────────────────────────────
// UserType holds the position ("Doctor" / "Nurse") for accounts created
// through User Management; older sessions may still hold "Healthcare".
const worker = computed(() => {
  const u = getUser() || {}
  const fullName = `${u.FirstName || ''} ${u.LastName || ''}`.trim()
  const type = u.UserType
  return {
    userId: u.UserID,
    fullName,
    roleLabel: type && type !== 'Healthcare' ? type : 'Healthcare Worker',
    initials: fullName.split(' ').filter(Boolean).map(n => n[0]).join('').toUpperCase().slice(0, 2),
  }
})

// ── Notifications ────────────────────────────────────────────
const showPanel     = ref(false)
const notifications = ref([])
const loading       = ref(false)
const unreadCount   = computed(() => notifications.value.filter(n => !n.isRead).length)
let pollHandle = null

onMounted(() => {
  fetchNotifications()
  // Low-stock alerts can arrive while a worker is mid-shift.
  pollHandle = setInterval(fetchNotifications, 60000)
})

onUnmounted(() => {
  if (pollHandle) clearInterval(pollHandle)
})

async function fetchNotifications() {
  if (!worker.value.userId) return
  loading.value = notifications.value.length === 0
  try {
    const res = await axios.get(`${API}/api/Notifications/user/${worker.value.userId}`)
    notifications.value = res.data
  } catch {
    notifications.value = []
  } finally {
    loading.value = false
  }
}

function openPanel() {
  showPanel.value = true
  fetchNotifications()
}

async function markRead(notif) {
  if (notif.isRead) return
  notif.isRead = true
  try { await axios.patch(`${API}/api/Notifications/mark-read/${notif.notificationID}`) } catch {}
}

async function markAllRead() {
  notifications.value.forEach(n => { n.isRead = true })
  try { await axios.patch(`${API}/api/Notifications/mark-all-read/user/${worker.value.userId}`) } catch {}
}

function iconFor(type) {
  if (type === 'LowStock') return '📦'
  if (type === 'Announcement') return '📢'
  return '🔔'
}

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
</script>

<style scoped>
.notif-panel-enter-active, .notif-panel-leave-active { transition: opacity 0.25s ease; }
.notif-panel-enter-active > div:last-child, .notif-panel-leave-active > div:last-child { transition: transform 0.3s cubic-bezier(0.4,0,0.2,1); }
.notif-panel-enter-from, .notif-panel-leave-to { opacity: 0; }
.notif-panel-enter-from > div:last-child, .notif-panel-leave-to > div:last-child { transform: translateX(100%); }
</style>
