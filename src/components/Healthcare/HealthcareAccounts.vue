<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar @logout="logout" />

    <div class="flex flex-col flex-1 overflow-hidden">
      <HealthcareHeader :worker="worker" title="My Account" />

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
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <User class="w-3 h-3 inline mr-1" />Full Name
                  </label>
                  <div class="w-full px-3 py-2.5 text-sm border border-slate-100 rounded-lg bg-slate-50 text-slate-700 flex items-center gap-2">
                    {{ worker.fullName || '—' }}
                  </div>
                </div>
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <BadgeCheck class="w-3 h-3 inline mr-1" />Professional License No.
                  </label>
                  <div class="w-full px-3 py-2.5 text-sm border border-slate-100 rounded-lg bg-slate-50 text-slate-700 flex items-center gap-2">
                    {{ worker.prcNo || '—' }}
                  </div>
                </div>
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <Mail class="w-3 h-3 inline mr-1" />Email Address
                  </label>
                  <input v-model="form.email" type="email"
                    class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <div>
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">
                    <Phone class="w-3 h-3 inline mr-1" />Contact Number
                  </label>
                  <input v-model="form.contactNo" type="text"
                    class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <div class="col-span-2">
                  <label class="text-xs font-medium text-slate-500 uppercase tracking-wide block mb-1.5">Role</label>
                  <div class="w-full px-3 py-2.5 text-sm border border-slate-100 rounded-lg bg-slate-50 text-slate-700">
                    {{ worker.userType || '—' }}
                  </div>
                </div>
              </div>
              <div class="mt-4 flex items-center gap-3">
                <button @click="saveProfile"
                  :disabled="savingProfile"
                  class="flex items-center gap-2 px-4 py-2.5 text-sm font-medium text-white bg-emerald-600 rounded-lg hover:bg-emerald-700 disabled:opacity-50 transition-colors">
                  <Save class="w-4 h-4" />
                  {{ savingProfile ? 'Saving…' : 'Save Changes' }}
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
                {{ workerInitials }}
              </div>
              <p class="font-bold text-slate-800">{{ worker.fullName }}</p>
              <p class="text-sm text-slate-400 mt-0.5">{{ worker.userType }}</p>
              <p v-if="form.prcNo" class="text-xs text-slate-400 mt-0.5">{{ form.prcNo }}</p>
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
  </div>
</template>

<script setup>
import { getUser, getToken, logout as clearSession } from '@/utils/auth'
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Save, Lock, User, Mail, Phone, BadgeCheck, Bell, Check, X } from 'lucide-vue-next'
import HealthcareSidebar from '@/components/Healthcare/Components/HealtcareSidebar.vue'
import HealthcareHeader from '@/components/Healthcare/Components/HealthcareHeader.vue'

const router = useRouter()

// VITE_API_URL is the bare host (no /api) per the project's existing
// convention — /api is appended here, not stored in the env var.
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

const worker = ref({ userId: '', fullName: '', userType: '', prcNo: '' })
const form   = ref({ email: '', contactNo: '' })
const pwForm = ref({ current: '', newPw: '', confirm: '' })
const pwError       = ref('')
const pwSaving      = ref(false)
const pwSuccess     = ref(false)

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
const savingProfile = ref(false)
const profileSaved  = ref(false)

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
    prcNo:    u.PRCNo || u.prcNo || '',
  }
})

const workerInitials = computed(() =>
  worker.value.fullName.split(' ').filter(Boolean).map(n => n[0]).join('').toUpperCase().slice(0, 2)
)

const notifSettings = ref([
  { key: 'email', label: 'Email Alerts',  enabled: true,  highlight: false },
  { key: 'sms',   label: 'SMS Alerts',    enabled: false, highlight: false },
  { key: 'daily', label: 'Daily Reports', enabled: true,  highlight: false },
])

async function saveProfile() {
  savingProfile.value = true
  await new Promise(r => setTimeout(r, 800)) // simulate API call
  // await fetch(`${API_BASE}/Users/${worker.value.userId}`, { method: 'PATCH', headers: authHeaders(true), body: ... })
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
    const res = await fetch(`${API_BASE}/Users/${worker.value.userId}/change-password`, {
      method: 'PATCH',
      headers: authHeaders(true),
      body: JSON.stringify({
        currentPassword: pwForm.value.current,
        newPassword: pwForm.value.newPw,
      }),
    })
    if (!res.ok) {
      const data = await res.json().catch(() => null)
      throw new Error(data?.message || `Change password failed (${res.status})`)
    }
    pwSuccess.value = true
    pwForm.value = { current: '', newPw: '', confirm: '' }
    setTimeout(() => pwSuccess.value = false, 3000)
  } catch (err) {
    pwError.value = err.message || 'Current password is incorrect, or the update failed.'
  } finally {
    pwSaving.value = false
  }
}

function logout() {
  clearSession()
  router.push('/')
}
</script>