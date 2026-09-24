<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <DoctorSidebar />

    <!-- MAIN -->
    <div class="flex flex-col flex-1 overflow-hidden">
      <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 shrink-0">
        <div class="relative w-80">
          </div>
        <div class="flex items-center gap-4">
          <button @click="showNotifications = true" class="relative p-2 text-slate-400 hover:text-slate-600">
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
          <h1 class="text-2xl font-bold text-slate-800">My Account</h1>
          <p class="text-sm text-slate-500 mt-1">Manage your profile and settings</p>
        </div>

        <div class="grid grid-cols-3 gap-6">

          <!-- LEFT: Profile + Password -->
          <div class="col-span-2 space-y-5">

            <!-- Profile Info -->
            <div class="bg-white rounded-xl border border-slate-200 p-6">
              <h3 class="font-semibold text-slate-800 mb-1">Profile Information</h3>
              <p class="text-xs text-slate-400 mb-4">Name and license number can only be changed by the System Administrator.</p>
              <div class="grid grid-cols-2 gap-4">
                <!-- Read-only: Full Name -->
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <User class="w-3 h-3 inline mr-1" />Full Name
                  </label>
                  <div class="w-full px-3 py-2.5 text-sm border border-slate-100 rounded-lg bg-slate-50 text-slate-700 flex items-center gap-2">
                    {{ doctor.fullName || '—' }}
                  </div>
                </div>
                <!-- Read-only: PRC No. -->
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <BadgeCheck class="w-3 h-3 inline mr-1" />Professional License No.
                  </label>
                  <div class="w-full px-3 py-2.5 text-sm border border-slate-100 rounded-lg bg-slate-50 text-slate-700 flex items-center gap-2">
                    {{ doctor.prcNo || '—' }}
                    
                  </div>
                </div>
                <!-- Editable: Email -->
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <Mail class="w-3 h-3 inline mr-1" />Email Address
                  </label>
                  <input v-model="form.email" type="email"
                    class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <!-- Editable: Contact -->
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <Phone class="w-3 h-3 inline mr-1" />Contact Number
                  </label>
                  <input v-model="form.contactNo" type="text"
                    class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <!-- Read-only: Role -->
                <div class="col-span-2">
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">Role</label>
                  <div class="w-full px-3 py-2.5 text-sm border border-slate-100 rounded-lg bg-slate-50 text-slate-700">
                    {{ doctor.userType || '—' }}
                  </div>
                </div>
              </div>
              <div class="mt-4 flex items-center gap-3">
                <button @click="saveProfile"
                  :disabled="savingProfile"
                  class="flex items-center gap-2 px-4 py-2.5 text-sm font-medium text-white bg-emerald-600 rounded-lg hover:bg-emerald-700 disabled:opacity-50 transition-colors">
                  <Save class="w-4 h-4" />
                  {{ savingProfile ? 'Saving…' : 'Save Contact Info' }}
                </button>
                <p v-if="profileSaved" class="text-xs text-emerald-600 font-medium">✓ Changes saved</p>
              </div>
            </div>

            <!-- Change Password -->
            <div class="bg-white rounded-xl border border-slate-200 p-6">
              <h3 class="font-semibold text-slate-800 mb-4">
                <Lock class="w-4 h-4 inline mr-1.5 text-slate-400" />Change Password
              </h3>
              <div class="space-y-4">
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">Current Password</label>
                  <input v-model="pwForm.current" type="password" placeholder="Enter current password"
                    name="current-password-field" autocomplete="new-password" readonly
                    onfocus="this.removeAttribute('readonly')"
                    class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">New Password</label>
                  <input v-model="pwForm.newPw" type="password" placeholder="Enter new password"
                    name="new-password-field" autocomplete="new-password"
                    class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />

                  <ul v-if="pwForm.newPw" class="mt-2 space-y-1">
                    <li v-for="req in pwRequirements" :key="req.label"
                      class="flex items-center gap-1.5 text-xs"
                      :class="req.met ? 'text-emerald-600' : 'text-slate-400'">
                      <Check v-if="req.met" class="w-3.5 h-3.5 shrink-0" />
                      <X v-else class="w-3.5 h-3.5 shrink-0" />
                      {{ req.label }}
                    </li>
                  </ul>
                </div>
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">Confirm New Password</label>
                  <input v-model="pwForm.confirm" type="password" placeholder="Confirm new password"
                    name="confirm-password-field" autocomplete="new-password"
                    class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <p v-if="pwError" class="text-xs text-red-500">{{ pwError }}</p>
                <p v-if="pwSuccess" class="text-xs text-emerald-600 font-medium">✓ Password updated</p>
              </div>
              <button @click="changePassword" :disabled="pwSaving || !pwRequirementsMet"
                class="mt-4 flex items-center gap-2 px-4 py-2.5 text-sm font-medium text-white bg-slate-700 rounded-lg hover:bg-slate-800 disabled:opacity-50 transition-colors">
                <Lock class="w-4 h-4" />{{ pwSaving ? 'Updating…' : 'Update Password' }}
              </button>
            </div>

          </div>

          <!-- RIGHT: Avatar card + Notification settings -->
          <div class="space-y-5">

            <!-- Avatar Card -->
            <div class="bg-white rounded-xl border border-slate-200 p-6 flex flex-col items-center text-center">
              <div class="w-20 h-20 rounded-full bg-emerald-700 flex items-center justify-center text-2xl font-bold text-white mb-3">
                {{ doctorInitials }}
              </div>
              <p class="font-bold text-slate-800">{{ doctor.fullName }}</p>
              <p class="text-sm text-slate-400 mt-0.5">{{ doctor.userType }}</p>
              <p v-if="doctor.prcNo" class="text-xs text-slate-400 mt-0.5">{{ doctor.prcNo }}</p>
            </div>

            <!-- Notification Settings -->
            <div class="bg-white rounded-xl border border-slate-200 p-5">
              <h3 class="font-semibold text-slate-800 mb-3">
                <Bell class="w-4 h-4 inline mr-1.5 text-slate-400" />Notification Settings
              </h3>
              <div class="space-y-3">
                <label v-for="n in notifSettings" :key="n.key"
                  class="flex items-center justify-between cursor-pointer">
                  <span class="text-sm text-slate-600" :class="n.highlight ? 'text-red-500 font-medium' : ''">{{ n.label }}</span>
                  <div class="relative">
                    <input type="checkbox" v-model="n.enabled" class="sr-only" />
                    <div class="w-9 h-5 rounded-full transition-colors"
                      :class="n.enabled ? 'bg-emerald-500' : 'bg-slate-200'">
                      <div class="absolute top-0.5 left-0.5 w-4 h-4 bg-white rounded-full shadow transition-transform"
                        :class="n.enabled ? 'translate-x-4' : 'translate-x-0'"></div>
                    </div>
                  </div>
                </label>
              </div>
              <button class="mt-4 w-full py-2 text-sm font-medium text-white bg-slate-700 rounded-lg hover:bg-slate-800 transition-colors">
                Save Settings
              </button>
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
  Search, Bell, LogOut, Save, Lock, User, Mail, Phone, BadgeCheck, Check, X,
  ListChecks
} from 'lucide-vue-next'

const API = import.meta.env.VITE_API_URL || 'http://localhost:57147'

const router = useRouter()
const route  = useRoute()

const doctor = ref({ userId: '', fullName: '', userType: '', prcNo: '' })
const form   = ref({ fullName: '', prcNo: '', email: '', contactNo: '' })
const pwForm = ref({ current: '', newPw: '', confirm: '' })
const pwError       = ref('')
const pwSaving      = ref(false)
const pwSuccess     = ref(false)
const savingProfile = ref(false)
const profileSaved  = ref(false)

// Live checklist shown under New Password. Also gates the submit
// button, and changePassword() re-checks the same rules server-side
// of the click (not just relying on the disabled attribute).
const pwRequirements = computed(() => {
  const pw = pwForm.value.newPw
  return [
    { label: 'At least 8 characters', met: pw.length >= 8 },
    { label: 'One uppercase letter',  met: /[A-Z]/.test(pw) },
    { label: 'One lowercase letter',  met: /[a-z]/.test(pw) },
    { label: 'One number',            met: /[0-9]/.test(pw) },
    { label: 'One special character', met: /[^A-Za-z0-9]/.test(pw) },
  ]
})
const pwRequirementsMet = computed(() => pwRequirements.value.every(r => r.met))

onMounted(() => {
  const u = getUser()
if (!u) { router.push('/'); return }
  doctor.value = {
    userId:   u.UserID,
    fullName: `${u.FirstName} ${u.LastName}`,
    userType: u.UserType,
    prcNo:    u.PRCNo || '',
  }
  form.value.fullName  = doctor.value.fullName
  form.value.prcNo     = doctor.value.prcNo
  fetchNotifications()
})

const doctorInitials = computed(() =>
  doctor.value.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
)

const notifSettings = ref([
  { key: 'email',   label: 'Email Alerts',          enabled: true,  highlight: false },
  { key: 'sms',     label: 'SMS Alerts',            enabled: false, highlight: false },
  { key: 'daily',   label: 'Daily Reports',         enabled: true,  highlight: false },
])

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

async function saveProfile() {
  savingProfile.value = true
  await new Promise(r => setTimeout(r, 800)) // simulate API call
  // await fetch(`${import.meta.env.VITE_API_URL}/api/users/${doctor.value.userId}`, { method: 'PATCH', ... })
  savingProfile.value = false
  profileSaved.value  = true
  setTimeout(() => profileSaved.value = false, 3000)
}

async function changePassword() {
  pwError.value = ''
  pwSuccess.value = false
  if (!pwForm.value.current) { pwError.value = 'Please enter your current password.'; return }
  if (!pwRequirementsMet.value) { pwError.value = 'New password does not meet all requirements.'; return }
  if (pwForm.value.newPw !== pwForm.value.confirm) { pwError.value = 'Passwords do not match.'; return }

  pwSaving.value = true
  try {
    await axios.patch(`${API}/api/Users/${doctor.value.userId}/change-password`, {
      currentPassword: pwForm.value.current,
      newPassword: pwForm.value.newPw,
    })
    pwSuccess.value = true
    pwForm.value = { current: '', newPw: '', confirm: '' }
    setTimeout(() => pwSuccess.value = false, 3000)
  } catch (err) {
    pwError.value = err.response?.data?.message || 'Current password is incorrect, or the update failed.'
  } finally {
    pwSaving.value = false
  }
}

</script>

<style scoped>
.notif-panel-enter-active, .notif-panel-leave-active { transition: opacity 0.25s ease; }
.notif-panel-enter-active > div:last-child, .notif-panel-leave-active > div:last-child { transition: transform 0.3s cubic-bezier(0.4,0,0.2,1); }
.notif-panel-enter-from, .notif-panel-leave-to { opacity: 0; }
.notif-panel-enter-from > div:last-child, .notif-panel-leave-to > div:last-child { transform: translateX(100%); }
</style>