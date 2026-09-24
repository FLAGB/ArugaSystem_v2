<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <DoctorSidebar />

    <!-- MAIN CONTENT -->
    <div class="flex flex-col flex-1 overflow-hidden">

      <!-- TOP BAR -->
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

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <!-- WELCOME -->
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Welcome back, {{ doctor.fullName }}</h1>
          <p class="text-sm text-slate-500 mt-1">{{ todayFormatted }}</p>
        </div>

        <!-- LIVE QUEUE -->
        <div class="bg-white rounded-xl border border-slate-200 mb-6">
          <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
            <div>
              <h2 class="font-semibold text-slate-800">Today's Queue</h2>
              <p class="text-xs text-slate-400 mt-0.5">Live check-in queue — updates automatically</p>
            </div>
            <button @click="$router.push('/doctor/queue')"
              class="text-xs bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-1.5 rounded-lg font-medium transition-colors flex items-center gap-1.5">
              <ListChecks class="w-3.5 h-3.5" />
              Go to Queue
            </button>
          </div>

          <div class="flex divide-x divide-slate-100">
            <!-- Now serving -->
            <div class="w-56 shrink-0 p-5 flex flex-col items-center justify-center text-center">
              <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest mb-1">Now Serving</p>
              <p class="text-4xl font-black text-slate-800">{{ nowServing ? `#${nowServing.queueNumber}` : '—' }}</p>
              <p v-if="nowServing" class="text-xs text-slate-500 mt-1 truncate max-w-full">{{ nowServing.childName || nowServing.parentName }}</p>
              <button v-if="nowServing" @click="completeQueueEntry(nowServing)" :disabled="completingQueue"
                class="mt-3 text-xs bg-slate-800 hover:bg-slate-900 disabled:opacity-40 text-white px-3 py-1.5 rounded-lg font-medium transition-colors">
                Mark Complete
              </button>
              <p v-if="nowServing && nowServingRemaining > 0" class="text-[11px] text-amber-600 font-medium mt-1.5">
                {{ nowServingRemaining }} more due today
              </p>
            </div>

            <!-- Waiting list -->
            <div class="flex-1 p-5">
              <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest mb-3">Waiting ({{ waitingQueue.length }})</p>
              <div v-if="loadingQueue" class="text-sm text-slate-400 py-4 text-center">Loading...</div>
              <div v-else-if="waitingQueue.length === 0" class="text-sm text-slate-400 py-4 text-center">No one else waiting</div>
              <div v-else class="flex flex-wrap gap-2">
                <div v-for="q in waitingQueue" :key="q.queueID"
                  class="px-3 py-2 bg-slate-50 border border-slate-100 rounded-lg text-xs">
                  <span class="font-bold text-slate-700">#{{ q.queueNumber }}</span>
                  <span class="text-slate-400 ml-1.5">{{ q.childName || q.parentName || 'Walk-in' }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- STAT CARDS -->
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

          <!-- PENDING VACCINATIONS TODAY -->
          <div class="col-span-2 bg-white rounded-xl border border-slate-200">
            <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
              <div>
                <h2 class="font-semibold text-slate-800">Pending Vaccinations Today</h2>
                <p class="text-xs text-slate-400 mt-0.5">Children scheduled for vaccination today</p>
              </div>
              <button class="text-xs text-emerald-600 hover:underline font-medium" @click="$router.push('/doctor/records')">
                View All Records →
              </button>
            </div>

            <div v-if="loadingPending" class="flex items-center justify-center py-12">
              <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
              <span class="ml-2 text-sm text-slate-400">Loading...</span>
            </div>

            <div v-else-if="pendingToday.length === 0" class="flex flex-col items-center justify-center py-12 text-slate-400">
              <CheckCircle class="w-8 h-8 mb-2 opacity-40" />
              <p class="text-sm">All vaccinations for today are done</p>
            </div>

            <div v-else class="overflow-x-auto">
              <table class="w-full text-sm">
                <thead>
                  <tr class="text-xs text-slate-400 uppercase tracking-wide">
                    <th class="px-5 py-3 text-left font-medium">Child</th>
                    <th class="px-5 py-3 text-left font-medium">Parent</th>
                    <th class="px-5 py-3 text-left font-medium">Vaccine</th>
                    <th class="px-5 py-3 text-left font-medium">Dose</th>
                    <th class="px-5 py-3 text-left font-medium">Action</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr v-for="item in pendingToday" :key="item.timelineId ?? item.recordId" class="hover:bg-slate-50 transition-colors">
                    <td class="px-5 py-3">
                      <div class="flex items-center gap-2.5">
                        <div class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold text-white shrink-0"
                          :style="{ backgroundColor: avatarColor(item.childName) }">
                          {{ initials(item.childName) }}
                        </div>
                        <span class="font-medium text-slate-800">{{ item.childName }}</span>
                      </div>
                    </td>
                    <td class="px-5 py-3 text-slate-500">{{ item.parentName }}</td>
                    <td class="px-5 py-3 text-slate-700">{{ item.vaccineName }}</td>
                    <td class="px-5 py-3 text-slate-500">Dose {{ item.doseNumber }}</td>
                    <td class="px-5 py-3">
                      <button
                        @click="openVaccinateModal(item)"
                        class="text-xs bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-1.5 rounded-lg font-medium transition-colors"
                      >
                        Mark Vaccinated
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
            <div class="bg-white rounded-xl border border-slate-200 p-5 flex-1">
              <h2 class="font-semibold text-slate-800 mb-3">Vaccinated Today</h2>
              <div v-if="recentlyVaccinated.length === 0" class="text-sm text-slate-400 text-center py-4">
                No vaccinations recorded yet today
              </div>
              <div v-else class="space-y-3">
                <div v-for="rec in recentlyVaccinated" :key="rec.recordId" class="flex items-start gap-3">
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

    <!-- MARK AS VACCINATED MODAL -->
    <Transition name="modal">
      <div v-if="showModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm" @click="closeModal"></div>
        <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-md p-6 z-10">

          <div class="flex items-center justify-between mb-5">
            <div>
              <h3 class="text-lg font-bold text-slate-800">Confirm Vaccination</h3>
              <p class="text-sm text-slate-400 mt-0.5">{{ selectedItem?.childName }}</p>
            </div>
            <button @click="closeModal" class="text-slate-400 hover:text-slate-600 transition-colors">
              <X class="w-5 h-5" />
            </button>
          </div>

          <!-- Patient Summary -->
          <div class="bg-slate-50 rounded-xl p-4 mb-5 flex items-center gap-3">
            <div class="w-10 h-10 rounded-full flex items-center justify-center text-sm font-bold text-white"
              :style="{ backgroundColor: avatarColor(selectedItem?.childName || '') }">
              {{ initials(selectedItem?.childName || '') }}
            </div>
            <div>
              <p class="font-semibold text-slate-800">{{ selectedItem?.childName }}</p>
              <p v-if="selectedItem?.vaccineId" class="text-sm text-slate-500">
                {{ selectedItem?.vaccineName }} — Dose {{ selectedItem?.doseNumber }}
              </p>
              <p v-else class="text-xs text-amber-600 font-medium mt-0.5">
                No scheduled dose found — select vaccine manually
              </p>
              <p class="text-xs text-slate-400">Parent: {{ selectedItem?.parentName }}</p>
            </div>
          </div>

          <!-- Form Fields -->
          <div class="space-y-4">

            <!-- Manual vaccine/dose picker — only shown when the vaccine truly isn't known yet (no vaccineId) -->
            <template v-if="!selectedItem?.vaccineId">
              <div>
                <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                  Vaccine <span class="text-red-400">*</span>
                </label>
                <select v-model="form.vaccineId" @change="fetchInventoryFor(form.vaccineId)"
                  class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500">
                  <option value="" disabled>Select a vaccine...</option>
                  <option v-for="v in vaccineOptions" :key="v.vaccineId" :value="v.vaccineId">{{ v.name }}</option>
                </select>
              </div>
              <div>
                <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                  Dose Number <span class="text-red-400">*</span>
                </label>
                <select v-model="form.doseNumber"
                  class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500">
                  <option value="" disabled>Select dose...</option>
                  <option v-for="d in selectedVaccineDoses" :key="d.doseNumber" :value="d.doseNumber">Dose {{ d.doseNumber }}</option>
                </select>
              </div>
            </template>

            <div>
              <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                Date Administered <span class="text-red-400">*</span>
              </label>
              <input type="date" v-model="form.dateAdministered"
                class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                Vaccine Batch <span class="text-red-400">*</span>
              </label>
              <select v-model="form.inventoryId"
                class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500">
                <option value="" disabled>{{ loadingInventory ? 'Loading batches...' : 'Select a batch...' }}</option>
                <option v-for="inv in inventoryOptions" :key="inv.inventoryId" :value="inv.inventoryId">
                  {{ inv.lotNumber }} — exp {{ new Date(inv.expirationDate).toLocaleDateString() }} ({{ inv.currentQuantity }} left)
                </option>
              </select>
              <p v-if="!loadingInventory && form.vaccineId === '' && !selectedItem?.vaccineId" class="text-xs text-slate-400 mt-1">Select a vaccine first.</p>
              <p v-else-if="!loadingInventory && inventoryOptions.length === 0" class="text-xs text-red-500 mt-1">No usable stock for this vaccine — check inventory.</p>
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                Remarks / Adverse Reactions
              </label>
              <textarea v-model="form.remarks" rows="3"
                placeholder="e.g. No adverse reaction, mild fever possible..."
                class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 resize-none">
              </textarea>
            </div>
          </div>

          <!-- Accountability Notice -->
          <div class="flex items-start gap-2 mt-4 p-3 bg-amber-50 rounded-lg border border-amber-100">
            <Info class="w-4 h-4 text-amber-500 shrink-0 mt-0.5" />
            <p class="text-xs text-amber-700">
              This record will be permanently saved under <strong>{{ doctor.fullName }}</strong>
              and will be visible to the child's parent.
            </p>
          </div>

          <!-- Actions -->
          <div class="flex gap-3 mt-5">
            <button @click="closeModal"
              class="flex-1 px-4 py-2.5 text-sm font-medium text-slate-600 border border-slate-200 rounded-lg hover:bg-slate-50 transition-colors">
              Cancel
            </button>
            <button @click="submitVaccination"
              :disabled="submitting || !form.dateAdministered || !form.inventoryId || (!selectedItem?.vaccineId && (!form.vaccineId || !form.doseNumber))"
              class="flex-1 px-4 py-2.5 text-sm font-medium text-white bg-emerald-600 rounded-lg hover:bg-emerald-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2">
              <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
              <CheckCircle v-else class="w-4 h-4" />
              {{ submitting ? 'Saving...' : 'Confirm & Save' }}
            </button>
          </div>

        </div>
      </div>
    </Transition>

    <!-- WALK-IN: ADD ANOTHER VACCINE OR COMPLETE VISIT -->
    <Transition name="modal">
      <div v-if="showWalkInMoreModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm"></div>
        <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-sm p-6 z-10 text-center">
          <div class="w-12 h-12 rounded-full bg-emerald-50 flex items-center justify-center mx-auto mb-3">
            <CheckCircle class="w-6 h-6 text-emerald-600" />
          </div>
          <h3 class="text-lg font-bold text-slate-800">Dose recorded</h3>
          <p class="text-sm text-slate-500 mt-1 mb-5">
            Any other vaccines to give {{ pendingWalkInChild?.childName }} in this same visit?
          </p>
          <div class="flex gap-3">
            <button @click="walkInCompleteVisit" :disabled="completingQueue"
              class="flex-1 px-4 py-2.5 text-sm font-medium text-slate-600 border border-slate-200 rounded-lg hover:bg-slate-50 disabled:opacity-50 transition-colors">
              No, complete visit
            </button>
            <button @click="walkInAddAnother"
              class="flex-1 px-4 py-2.5 text-sm font-medium text-white bg-emerald-600 rounded-lg hover:bg-emerald-700 transition-colors">
              Yes, add another
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- SUCCESS TOAST -->
    <Transition name="toast">
      <div v-if="toast.show"
        class="fixed bottom-6 right-6 z-50 bg-emerald-600 text-white px-5 py-3.5 rounded-xl shadow-lg flex items-center gap-3 text-sm font-medium">
        <CheckCircle class="w-5 h-5" />
        {{ toast.message }}
      </div>
    </Transition>

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
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import axios from 'axios'
import {
  Home, Users, Calendar, Syringe, FileText, Settings,
  Search, Bell, LogOut, Loader2, X, Info, CheckCircle,
  Clock, AlertCircle, TrendingUp, ListChecks
} from 'lucide-vue-next'

const router = useRouter()
const route  = useRoute()
const API = import.meta.env.VITE_API_URL || 'http://localhost:57147'

// ─────────────────────────────────────────────────────────────
// AUTH
// Set localStorage during login with these exact keys
// matching dbo.Users columns
// ─────────────────────────────────────────────────────────────
const doctor = ref({ userId: '', fullName: '', userType: '', prcNo: '' })

onMounted(() => {
  const u = getUser()
if (!u) { router.push('/'); return }
  doctor.value = {
    userId:   u.UserID,
    fullName: `${u.FirstName} ${u.LastName}`,
    userType: u.UserType,  // 'Doctor' or 'Nurse'
    prcNo:    u.PRCNo || '',
  }
  fetchStats()
  fetchPendingToday()
  fetchRecentlyVaccinated()
  fetchQueue()
  fetchVaccineOptions()
  queuePollHandle = setInterval(fetchQueue, 5000)
  fetchNotifications()
})

onUnmounted(() => {
  if (queuePollHandle) clearInterval(queuePollHandle)
})

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

// ─────────────────────────────────────────────────────────────
// LIVE QUEUE
// C# endpoints: GET /api/Queue/board, PATCH /api/Queue/call-next,
//               PATCH /api/Queue/{queueId}/complete
// ─────────────────────────────────────────────────────────────
const queueBoard    = ref([])
const loadingQueue  = ref(false)
const completingQueue = ref(false)
let queuePollHandle  = null

const nowServing   = computed(() => queueBoard.value.find(q => q.queueStatus === 'InProgress') || null)
const waitingQueue = computed(() => queueBoard.value.filter(q => q.queueStatus === 'Waiting'))

// How many more vaccines (besides the one currently linked to this queue
// slot) are still pending today for the child now being served — shown next
// to Mark Complete so the doctor can see up front that the visit isn't done
// yet, rather than finding out only after submitting.
const nowServingRemaining = computed(() => {
  if (!nowServing.value) return 0
  const childId = nowServing.value.childId ?? nowServing.value.childID
  return countPendingForChild(childId, nowServing.value.recordId)
})

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

// "Call Next" now lives on the Staff/Admission Queue page — Staff calls
// the next patient forward and marks a session complete once the doctor
// is done or the room is vacant. The doctor's board here (and the new
// Queue tab) is read-only: it mirrors the same live queue for visibility.

// "Mark Complete" no longer closes the queue slot directly — a queue slot
// being closed with no vaccine/lot/remarks captured was the original bug.
// Instead it opens the same Confirm Vaccination modal used by Pending
// Vaccinations Today, pre-filled from the queue entry's linked record
// (queueEntry.recordId) when one exists, or blank for a manual pick
// when it doesn't (e.g. walk-in, no dose scheduled today).
function completeQueueEntry(queueEntry) {
  openVaccinateModal({
    recordId:    queueEntry.recordId ?? null,
    childId:     queueEntry.childId ?? null,
    childName:   queueEntry.childName,
    parentName:  queueEntry.parentName,
    vaccineName: queueEntry.vaccineName ?? null,
    doseNumber:  queueEntry.doseNumber ?? null,
  }, queueEntry.queueID)
}

// The actual server call that closes a queue slot — now triggered from
// inside submitVaccination() once the vaccination record is saved, so the
// two are never out of sync.
async function completeQueueEntryOnServer(queueId) {
  completingQueue.value = true
  try {
    await axios.patch(`${API}/api/Queue/${queueId}/complete`)
  } catch (e) { console.error('completeQueueEntryOnServer:', e) }
  finally { completingQueue.value = false }
}

// Doctor said "yes, one more" after a walk-in dose — reopen the same
// Confirm Vaccination modal for the same child/queue slot, blank so they
// can pick the next vaccine manually.
function walkInAddAnother() {
  const queueId = pendingWalkInQueueId.value
  const child = pendingWalkInChild.value
  showWalkInMoreModal.value = false
  pendingWalkInQueueId.value = null
  pendingWalkInChild.value = null
  if (queueId && child) {
    openVaccinateModal({
      recordId: null,
      childId: child.childId,
      childName: child.childName,
      parentName: child.parentName,
    }, queueId)
  }
}

// Doctor said the visit is done — close the queue slot now, same as the
// scheduled-dose path does automatically once nothing is left pending.
async function walkInCompleteVisit() {
  const queueId = pendingWalkInQueueId.value
  showWalkInMoreModal.value = false
  pendingWalkInQueueId.value = null
  pendingWalkInChild.value = null
  if (queueId) {
    await completeQueueEntryOnServer(queueId)
    await fetchQueue()
  }
}

const doctorInitials = computed(() =>
  doctor.value.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
)

// ─────────────────────────────────────────────────────────────
// NAVIGATION
// ─────────────────────────────────────────────────────────────
const quickActions = [
  { label: 'Patient List',        path: '/doctor/patients', icon: Users    },
  { label: 'View Calendar',       path: '/doctor/calendar', icon: Calendar },
  { label: 'Vaccination Records', path: '/doctor/records',  icon: Syringe  },
  { label: 'Daily Report',        path: '/doctor/reports',  icon: FileText },
]

// ─────────────────────────────────────────────────────────────
// STATS
// C# endpoint: GET /api/VaccinationRecords/stats
// ─────────────────────────────────────────────────────────────
const stats = ref({ vaccinatedToday: 0, pendingToday: 0, missedTotal: 0, weeklyTotal: 0 })

const statCards = computed(() => [
  { label: 'Vaccinated Today',  value: stats.value.vaccinatedToday, icon: Syringe,     iconColor: 'text-emerald-500' },
  { label: 'Pending Today',     value: stats.value.pendingToday,    icon: Clock,        iconColor: 'text-amber-500'   },
  { label: 'Missed Follow-ups', value: stats.value.missedTotal,     icon: AlertCircle,  iconColor: 'text-red-400'     },
  { label: 'Weekly Total',      value: stats.value.weeklyTotal,     icon: TrendingUp,   iconColor: 'text-blue-400'    },
])

async function fetchStats() {
  try {
    const res = await axios.get(`${API}/api/VaccinationRecords/stats`)
    stats.value = res.data
  } catch (e) { console.error('fetchStats:', e) }
}

// ─────────────────────────────────────────────────────────────
// PENDING VACCINATIONS TODAY
// C# endpoint: GET /api/VaccinationTimeline/due-today
// (was GET /api/VaccinationRecords/pending-today — that endpoint reads
// VaccinationRecords, which nothing in the app ever populates with
// Status "Scheduled", so it always returned []. VaccinationTimeline
// (Status "Pending") is the real source of what's due today.)
//
// Items from this endpoint have no recordId (there's no
// VaccinationRecords row yet for a Pending dose) — they carry
// timelineId instead. recordId stays null so downstream logic that
// checks "does a record already exist to PATCH /complete" still works;
// vaccineId/doseNumber being present is what tells the modal the dose
// is already known and the manual picker isn't needed.
// ─────────────────────────────────────────────────────────────
const pendingToday  = ref([])
const loadingPending = ref(false)

async function fetchPendingToday() {
  loadingPending.value = true
  try {
    const res = await axios.get(`${API}/api/VaccinationTimeline/due-today`)
    pendingToday.value = res.data.map(t => ({
      recordId: null,
      timelineId: t.timelineId,
      childId: t.childId,
      childName: t.childName,
      parentName: t.parentName,
      vaccineId: t.vaccineId,
      vaccineName: t.vaccineName,
      doseNumber: t.doseNumber,
    }))
  } catch (e) { console.error('fetchPendingToday:', e) }
  finally { loadingPending.value = false }
}

// ─────────────────────────────────────────────────────────────
// VACCINATED TODAY (sidebar)
// C# endpoint: GET /api/VaccinationRecords/completed-today
// ─────────────────────────────────────────────────────────────
const recentlyVaccinated = ref([])

async function fetchRecentlyVaccinated() {
  try {
    const res = await axios.get(`${API}/api/VaccinationRecords/completed-today`)
    recentlyVaccinated.value = res.data
  } catch (e) { console.error('fetchRecentlyVaccinated:', e) }
}

// ─────────────────────────────────────────────────────────────
// MARK AS VACCINATED
// C# endpoints:
//   PATCH /api/VaccinationRecords/{recordId}/complete  (existing Pending record)
//   POST  /api/VaccinationRecords                      (walk-in, no existing record)
// ─────────────────────────────────────────────────────────────
const showModal     = ref(false)
const selectedItem  = ref(null)
const submitting    = ref(false)
const activeQueueId = ref(null)   // set when the modal was opened from the Queue, so submit can also close out that queue slot
const form = ref({ dateAdministered: '', inventoryId: '', remarks: '', vaccineId: '', doseNumber: '' })

// Walk-in vaccines (no linked pendingToday record) can't be counted by
// countPendingForChild, so after a walk-in submit we ask the doctor
// directly instead of guessing whether the visit is done.
const showWalkInMoreModal = ref(false)
const pendingWalkInQueueId = ref(null)
const pendingWalkInChild = ref(null)
const inventoryOptions = ref([])
const loadingInventory = ref(false)

// Fetches available batches for a vaccine, FIFO-sorted by expiry,
// filters out inactive/expired/out-of-stock ones, and auto-selects
// the earliest-expiring valid batch.
async function fetchInventoryFor(vaccineId) {
  form.value.inventoryId = ''
  inventoryOptions.value = []
  if (!vaccineId) return
  loadingInventory.value = true
  try {
    const res = await axios.get(`${API}/api/VaccineInventory/vaccine/${vaccineId}`)
    const today = new Date().setHours(0,0,0,0)
    inventoryOptions.value = res.data
      .filter(i => (i.status ?? i.Status) && (i.currentQuantity ?? i.CurrentQuantity) > 0
        && new Date(i.expirationDate ?? i.ExpirationDate).setHours(0,0,0,0) >= today)
      .sort((a, b) => new Date(a.expirationDate ?? a.ExpirationDate) - new Date(b.expirationDate ?? b.ExpirationDate))
      .map(i => ({
        inventoryId: i.inventoryID ?? i.InventoryID,
        lotNumber: i.lotNumber ?? i.LotNumber,
        expirationDate: i.expirationDate ?? i.ExpirationDate,
        currentQuantity: i.currentQuantity ?? i.CurrentQuantity,
      }))
    if (inventoryOptions.value.length) form.value.inventoryId = inventoryOptions.value[0].inventoryId
  } catch (e) { console.error('fetchInventoryFor:', e) }
  finally { loadingInventory.value = false }
}

// ─────────────────────────────────────────────────────────────
// VACCINE OPTIONS (walk-in picker only — used when a queue entry
// has no linked VaccinationRecords row, e.g. no scheduled dose today)
// Pulled from the real catalog, same endpoint DoctorPatients.vue
// already uses, instead of a hardcoded 5-item stand-in.
// ─────────────────────────────────────────────────────────────
const vaccineOptions = ref([])

async function fetchVaccineOptions() {
  try {
    const res = await axios.get(`${API}/api/Vaccines/with-doses`)
    vaccineOptions.value = res.data.map(v => ({
      vaccineId: v.vaccineID,
      name: v.vaccineName,
      doses: v.doses || [],
    }))
  } catch (e) { console.error('fetchVaccineOptions:', e) }
}

const selectedVaccineDoses = computed(() => {
  const v = vaccineOptions.value.find(v => v.vaccineId === form.value.vaccineId)
  return v?.doses?.length ? v.doses : [{ doseNumber: 1 }]
})

// How many other pending-today items belong to this same child — used to
// decide whether "Mark Complete" should actually close the queue slot, and
// to tell the doctor how many doses are left when it doesn't. excludeRecordId
// leaves out the item that was just submitted (still in pendingToday.value
// at the point this is called, since it's filtered out afterward).
function countPendingForChild(childId, excludeKey = null) {
  if (!childId) return 0
  return pendingToday.value.filter(i => {
    const iKey = i.timelineId ?? i.recordId
    if (excludeKey && iKey === excludeKey) return false
    const iChildId = i.childId ?? i.childID ?? i.ChildID
    return iChildId && String(iChildId).toLowerCase() === String(childId).toLowerCase()
  }).length
}

// item: { recordId, timelineId, childName, parentName, vaccineName, doseNumber, childId, vaccineId }
//   - recordId present   → an existing VaccinationRecords row; submit PATCHes it complete
//   - recordId null      → no record yet; submit POSTs a new one
//   - vaccineId present  → the dose is already known (from Pending Vaccinations'
//                          timelineId, or a legacy scheduled record) — no manual picker shown
//   - vaccineId null     → true walk-in with nothing scheduled; doctor picks vaccine/dose manually
// queueId: pass this only when the modal was opened from "Mark Complete" on the queue,
//          so that on submit we also close out that queue slot.
function openVaccinateModal(item, queueId = null) {
  selectedItem.value = item
  activeQueueId.value = queueId
  form.value = { dateAdministered: new Date().toISOString().split('T')[0], inventoryId: '', remarks: '', vaccineId: '', doseNumber: '' }
  inventoryOptions.value = []
  if (item.vaccineId) fetchInventoryFor(item.vaccineId)
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  selectedItem.value = null
  activeQueueId.value = null
}

async function submitVaccination() {
  const isWalkIn = !selectedItem.value.recordId
  // A dose is "known" (no manual pick needed) whether it came from an
  // existing VaccinationRecords row (recordId) or a VaccinationTimeline
  // entry (timelineId, vaccineId/doseNumber already set) — only a true
  // walk-in with neither has to fall back to the manual form fields.
  const hasKnownDose = !!selectedItem.value.vaccineId
  if (!form.value.dateAdministered || !form.value.inventoryId) return
  if (isWalkIn && !hasKnownDose && (!form.value.vaccineId || !form.value.doseNumber)) return

  submitting.value = true
  try {
    let vaccineName = selectedItem.value.vaccineName
    let doseNumber  = selectedItem.value.doseNumber
    let response

    if (isWalkIn) {
      const vaccineId = hasKnownDose ? selectedItem.value.vaccineId : Number(form.value.vaccineId)
      doseNumber = hasKnownDose ? selectedItem.value.doseNumber : Number(form.value.doseNumber)
      if (!hasKnownDose) {
        const chosenVaccine = vaccineOptions.value.find(v => v.vaccineId === form.value.vaccineId)
        vaccineName = chosenVaccine?.name
      }

      response = await axios.post(`${API}/api/VaccinationRecords`, {
        childID: selectedItem.value.childId,
        vaccineID: Number(vaccineId),
        doseNumber: Number(doseNumber),
        vaccinationDate: form.value.dateAdministered,
        inventoryID: Number(form.value.inventoryId),
        administeredByUserID: doctor.value.userId,
        nurseObservation: form.value.remarks || null,
      })
    } else {
      response = await axios.patch(`${API}/api/VaccinationRecords/${selectedItem.value.recordId}/complete`, {
        inventoryID: Number(form.value.inventoryId),
        administeredByUserID: doctor.value.userId,
        vaccinationDate: form.value.dateAdministered,
        nurseObservation: form.value.remarks || null,
      })
    }

    // A queue slot represents the whole visit, and a child can have several
    // vaccines due the same day (that's the actual scenario Mark Complete
    // needs to handle correctly) — so only close the slot once nothing else
    // is still pending for this same child today. Otherwise the visit stays
    // InProgress and the doctor records the next dose the same way.
    //
    // Walk-ins (no linked pendingToday record) aren't tracked by
    // countPendingForChild at all — there's no scheduled row to count — so
    // "remaining" is meaningless for them. Ask the doctor explicitly instead
    // of silently closing the visit after just one unscheduled dose.
    const isWalkInQueueClose = activeQueueId.value && isWalkIn
    if (activeQueueId.value && !isWalkIn) {
      const childId = selectedItem.value.childId ?? selectedItem.value.childID
      const remaining = countPendingForChild(childId, selectedItem.value.recordId)

      if (remaining > 0) {
        showToast(`${selectedItem.value.childName} recorded — still ${remaining} more due today, visit stays open.`)
      } else {
        await completeQueueEntryOnServer(activeQueueId.value)
      }
    }

    const submittedKey = selectedItem.value.timelineId ?? selectedItem.value.recordId
    pendingToday.value = pendingToday.value.filter(i => (i.timelineId ?? i.recordId) !== submittedKey)
    recentlyVaccinated.value.unshift({
      recordId: response.data.vaccinationRecordID,
      childName: selectedItem.value.childName,
      vaccineName,
      doseNumber,
    })
    stats.value.vaccinatedToday++
    stats.value.pendingToday = Math.max(0, stats.value.pendingToday - 1)

    const name = selectedItem.value.childName
    const childIdForWalkIn = selectedItem.value.childId ?? selectedItem.value.childID
    const parentNameForWalkIn = selectedItem.value.parentName
    const queueIdForWalkIn = activeQueueId.value

    closeModal()
    showToast(`${name}'s vaccination recorded successfully`)
    await fetchQueue()

    if (isWalkInQueueClose) {
      pendingWalkInQueueId.value = queueIdForWalkIn
      pendingWalkInChild.value = { childId: childIdForWalkIn, childName: name, parentName: parentNameForWalkIn }
      showWalkInMoreModal.value = true
    }
  } catch (e) {
    console.error('submitVaccination:', e)
    showToast(e.response?.data?.message || 'Failed to save. Please try again.')
  } finally { submitting.value = false }
}

// ─────────────────────────────────────────────────────────────
// TOAST & HELPERS
// ─────────────────────────────────────────────────────────────
const toast = ref({ show: false, message: '' })
function showToast(msg) {
  toast.value = { show: true, message: msg }
  setTimeout(() => toast.value.show = false, 3500)
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

const todayFormatted = computed(() =>
  new Date().toLocaleDateString('en-PH', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })
)

</script>

<style scoped>
.modal-enter-active, .modal-leave-active { transition: opacity 0.2s ease; }
.modal-enter-from, .modal-leave-to       { opacity: 0; }
.toast-enter-active, .toast-leave-active { transition: all 0.3s ease; }
.toast-enter-from  { opacity: 0; transform: translateY(12px); }
.toast-leave-to    { opacity: 0; transform: translateY(12px); }

.notif-panel-enter-active, .notif-panel-leave-active { transition: opacity 0.25s ease; }
.notif-panel-enter-active > div:last-child, .notif-panel-leave-active > div:last-child { transition: transform 0.3s cubic-bezier(0.4,0,0.2,1); }
.notif-panel-enter-from, .notif-panel-leave-to { opacity: 0; }
.notif-panel-enter-from > div:last-child, .notif-panel-leave-to > div:last-child { transform: translateX(100%); }
</style>