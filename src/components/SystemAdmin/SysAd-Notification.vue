<script setup>
import { ref, computed, reactive } from 'vue'
import AppSidebar from './Components/AppSidebar.vue'
import AppHeader from './Components/AppHeader.vue'

/* ----------------------------- Sidebar state ------------------------------ */
const isCollapsed = ref(false)
const toggleSidebar = () => (isCollapsed.value = !isCollapsed.value)

const navItems = [
  { label: 'Dashboard', icon: '🏠' },
  { label: 'User Management', icon: '👥' },
  { label: 'Patient Management', icon: '🧒' },
  { label: 'Vaccine Management', icon: '💉' },
  { label: 'Inventory', icon: '📦' },
  { label: 'Notifications', icon: '🔔' },
  { label: 'Reports', icon: '📊' },
  { label: 'Audit Logs', icon: '📋' },
  { label: 'Settings', icon: '⚙️' },
]
const activeNav = ref('Notifications')

/* -------------------------------- Status meta -------------------------------- */
const statusMeta = {
  Sent: { tint: 'bg-emerald-50', text: 'text-emerald-700', dot: 'bg-emerald-500' },
  Scheduled: { tint: 'bg-sky-50', text: 'text-sky-700', dot: 'bg-sky-500' },
  Pending: { tint: 'bg-amber-50', text: 'text-amber-700', dot: 'bg-amber-500' },
  Failed: { tint: 'bg-rose-50', text: 'text-rose-600', dot: 'bg-rose-500' },
}

const typeOptions = ['Vaccination Reminder', 'Upcoming Schedule', 'Missed Vaccination', 'Queue Notification', 'Announcement', 'Manual Notification']

/* -------------------------------- Notification data -------------------------------- */
const notifications = ref([
  {
    id: 1, title: 'Pentavalent Dose 2 Reminder', recipient: 'Marites Santos', child: 'Miguel Bautista',
    type: 'Vaccination Reminder', scheduledDate: 'Jul 12, 2026', sentDate: 'Jul 09, 2026', status: 'Sent',
    message: 'This is a reminder that Miguel is due for the 2nd dose of Pentavalent vaccine on Jul 12, 2026. Please visit the clinic during operating hours.',
    deliveryMethod: 'In-App', opened: true,
  },
  {
    id: 2, title: 'OPV Dose 1 Upcoming Schedule', recipient: 'Liza Domingo', child: 'Sofia Domingo',
    type: 'Upcoming Schedule', scheduledDate: 'Jul 20, 2026', sentDate: '—', status: 'Scheduled',
    message: 'Sofia has an upcoming vaccination schedule for OPV Dose 1 on Jul 20, 2026.',
    deliveryMethod: 'In-App', opened: false,
  },
  {
    id: 3, title: 'Missed BCG Vaccination', recipient: 'Angeli Torres', child: 'Carlos Torres',
    type: 'Missed Vaccination', scheduledDate: 'Jul 03, 2026', sentDate: 'Jul 10, 2026', status: 'Sent',
    message: 'Carlos missed his scheduled BCG vaccination on Jul 03, 2026. Please schedule a catch-up visit as soon as possible.',
    deliveryMethod: 'In-App', opened: true,
  },
  {
    id: 4, title: 'Queue Confirmed – Window 2', recipient: 'Elena Cruz', child: 'Isabela Aquino',
    type: 'Queue Notification', scheduledDate: '—', sentDate: 'Jul 11, 2026', status: 'Sent',
    message: 'Your queue number Q-014 has been confirmed at Window 2. Please proceed when called.',
    deliveryMethod: 'In-App', opened: true,
  },
  {
    id: 5, title: 'Clinic Closed – National Holiday', recipient: 'All Parents', child: '—',
    type: 'Announcement', scheduledDate: '—', sentDate: 'Jul 08, 2026', status: 'Sent',
    message: 'Please be informed that the clinic will be closed on Jul 27, 2026 in observance of a national holiday. Regular operations resume the following day.',
    deliveryMethod: 'In-App', opened: false,
  },
  {
    id: 6, title: 'MMR Booster Reminder', recipient: 'Bea Fernandez', child: 'Diego Ramos',
    type: 'Vaccination Reminder', scheduledDate: 'Jul 15, 2026', sentDate: '—', status: 'Pending',
    message: 'Diego is due for MMR Booster on Jul 15, 2026. Reminder delivery is queued and pending dispatch.',
    deliveryMethod: 'In-App', opened: false,
  },
  {
    id: 7, title: 'DPT Booster Dose Reminder', recipient: 'Renz Villanueva', child: 'Andrea Villanueva',
    type: 'Vaccination Reminder', scheduledDate: 'Jul 05, 2026', sentDate: 'Jul 05, 2026', status: 'Failed',
    message: 'Delivery failed — recipient device could not be reached. Retry recommended.',
    deliveryMethod: 'In-App', opened: false,
  },
  {
    id: 8, title: 'New Immunization Guidelines Released', recipient: 'All Parents', child: '—',
    type: 'Manual Notification', scheduledDate: '—', sentDate: 'Jul 01, 2026', status: 'Sent',
    message: 'The Department of Health has released updated immunization guidelines for 2026. Tap to view the full advisory.',
    deliveryMethod: 'In-App', opened: true,
  },
  {
    id: 9, title: 'Queue Next-in-Line Alert', recipient: 'Jhun Aquino', child: 'Isabela Aquino',
    type: 'Queue Notification', scheduledDate: '—', sentDate: 'Jul 11, 2026', status: 'Sent',
    message: 'You are next in line at Window 1. Please proceed to the counter.',
    deliveryMethod: 'In-App', opened: true,
  },
])

/* ---------------------------- Toolbar / filters ---------------------------- */
const searchQuery = ref('')
const typeFilter = ref('All')
const statusFilter = ref('All')

const filteredNotifications = computed(() =>
  notifications.value.filter((n) => {
    const q = searchQuery.value.trim().toLowerCase()
    const matchesSearch =
      !q || n.recipient.toLowerCase().includes(q) || n.child.toLowerCase().includes(q) || n.title.toLowerCase().includes(q)
    const matchesType = typeFilter.value === 'All' || n.type === typeFilter.value
    const matchesStatus = statusFilter.value === 'All' || n.status === statusFilter.value
    return matchesSearch && matchesType && matchesStatus
  })
)

/* -------------------------------- Summary ---------------------------------- */
const summary = computed(() => {
  const sentToday = notifications.value.filter((n) => n.sentDate === 'Jul 11, 2026').length
  const scheduled = notifications.value.filter((n) => n.status === 'Scheduled').length
  const pending = notifications.value.filter((n) => n.status === 'Pending').length
  const failed = notifications.value.filter((n) => n.status === 'Failed').length
  const manual = notifications.value.filter((n) => n.type === 'Manual Notification' || n.type === 'Announcement').length
  const sentTotal = notifications.value.filter((n) => n.status === 'Sent' || n.status === 'Failed').length
  const successRate = sentTotal ? Math.round((notifications.value.filter((n) => n.status === 'Sent').length / sentTotal) * 100) : 0
  return { sentToday, scheduled, pending, failed, manual, successRate }
})

/* ------------------------------ Row actions menu ---------------------------- */
const openMenuId = ref(null)
const toggleMenu = (id) => (openMenuId.value = openMenuId.value === id ? null : id)
const closeMenu = () => (openMenuId.value = null)

const resend = (n) => {
  n.status = 'Sent'
  n.sentDate = 'Jul 11, 2026'
  closeMenu()
}
const cancelScheduled = (n) => {
  n.status = 'Failed'
  closeMenu()
}
const deleteAnnouncement = (n) => {
  notifications.value = notifications.value.filter((x) => x.id !== n.id)
  closeMenu()
}

/* -------------------------------- Details drawer ---------------------------- */
const showDrawer = ref(false)
const selectedNotification = ref(null)
const openDrawer = (n) => {
  selectedNotification.value = n
  showDrawer.value = true
  closeMenu()
}
const closeDrawer = () => (showDrawer.value = false)

/* --------------------------------- Create announcement modal --------------------------------- */
const showAnnounceModal = ref(false)
const announceForm = reactive({
  title: '', message: '', recipients: 'All Parents', schedule: 'Immediately',
  scheduleDate: '', method: 'In-App',
})
const openAnnounceModal = () => {
  Object.assign(announceForm, { title: '', message: '', recipients: 'All Parents', schedule: 'Immediately', scheduleDate: '', method: 'In-App' })
  showAnnounceModal.value = true
}
const sendAnnouncement = () => {
  notifications.value.unshift({
    id: Date.now(),
    title: announceForm.title || 'Untitled Announcement',
    recipient: announceForm.recipients,
    child: '—',
    type: 'Announcement',
    scheduledDate: announceForm.schedule === 'Later' ? (announceForm.scheduleDate || '—') : '—',
    sentDate: announceForm.schedule === 'Immediately' ? 'Jul 11, 2026' : '—',
    status: announceForm.schedule === 'Immediately' ? 'Sent' : 'Scheduled',
    message: announceForm.message,
    deliveryMethod: announceForm.method,
    opened: false,
  })
  showAnnounceModal.value = false
}

/* --------------------------------- Notification settings modal --------------------------------- */
const showSettingsModal = ref(false)
const settings = reactive({
  reminder7: true, reminder3: true, reminder1: true,
  missed1: false, missed3: true, missed7: true,
  queueConfirmed: true, queueNextInLine: true,
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex text-slate-900" @click="closeMenu">
    <!-- ============================ SIDEBAR ============================ -->
    <AppSidebar/>

    <!-- ============================ MAIN ============================ -->
    <div class="flex-1 min-w-0 flex flex-col">
      <!-- Top navbar -->
      <AppHeader
  title="Notification Logs"
  breadcrumb="System Administration / Notification Logs"
  user-initials="RM"
/>

      <!-- Content -->
      <main class="p-6 space-y-6">
        <!-- Summary cards -->
        <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Sent Today</p>
              <div class="bg-emerald-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">📨</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.sentToday }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Scheduled</p>
              <div class="bg-sky-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">🗓️</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.scheduled }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Pending</p>
              <div class="bg-amber-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">⏳</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.pending }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Failed</p>
              <div class="bg-rose-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">⚠️</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.failed }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Manual Announcements</p>
              <div class="bg-teal-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">📢</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.manual }}</p>
          </div>
          <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
            <div class="flex items-start justify-between gap-2">
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Reminder Success</p>
              <div class="bg-violet-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">📈</div>
            </div>
            <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.successRate }}%</p>
          </div>
        </section>

        <!-- Toolbar -->
        <section class="bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
          <div class="flex flex-col lg:flex-row lg:items-center gap-3">
            <div class="relative flex-1 min-w-0">
              <span class="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-sm" aria-hidden="true">🔍</span>
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search by parent name, child name, or notification title..."
                class="w-full pl-9 pr-3 py-2 text-sm rounded-lg border border-slate-200 bg-slate-50 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
              />
            </div>

            <select
              v-model="typeFilter"
              class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
            >
              <option value="All">All Types</option>
              <option v-for="t in typeOptions" :key="t">{{ t }}</option>
            </select>

            <select
              v-model="statusFilter"
              class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
            >
              <option value="All">All Status</option>
              <option>Sent</option>
              <option>Scheduled</option>
              <option>Pending</option>
              <option>Failed</option>
            </select>

            <div class="flex items-center gap-2 shrink-0 flex-wrap">
              <button @click="showSettingsModal = true" class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500">
                Notification Settings
              </button>
              <button class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500">
                Export Logs
              </button>
              <button
                @click="openAnnounceModal"
                class="text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
              >
                + Create Announcement
              </button>
            </div>
          </div>
        </section>

        <!-- Notification table -->
        <section class="bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-sm">
              <thead>
                <tr class="border-b border-slate-200 bg-slate-50/60">
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Notification Title</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Recipient</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Type</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Scheduled Date</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Sent Date</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Status</th>
                  <th class="text-right font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="n in filteredNotifications"
                  :key="n.id"
                  class="border-b border-slate-100 last:border-0 hover:bg-slate-50 transition-colors"
                >
                  <td class="px-5 py-3">
                    <p class="font-semibold text-slate-900 whitespace-nowrap">{{ n.title }}</p>
                    <p class="text-xs text-slate-400 whitespace-nowrap">{{ n.child !== '—' ? n.child : 'Broadcast' }}</p>
                  </td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ n.recipient }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ n.type }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ n.scheduledDate }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ n.sentDate }}</td>
                  <td class="px-3 py-3">
                    <span :class="[statusMeta[n.status].tint, statusMeta[n.status].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full whitespace-nowrap">
                      <span :class="statusMeta[n.status].dot" class="w-1.5 h-1.5 rounded-full"></span>
                      {{ n.status }}
                    </span>
                  </td>
                  <td class="px-5 py-3 text-right relative">
                    <button
                      @click.stop="toggleMenu(n.id)"
                      class="text-slate-400 hover:text-slate-700 hover:bg-slate-100 rounded-lg w-8 h-8 inline-flex items-center justify-center transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
                    >
                      ⋮
                    </button>

                    <div
                      v-if="openMenuId === n.id"
                      @click.stop
                      class="absolute right-5 top-11 z-30 w-52 bg-white border border-slate-200 rounded-lg shadow-md py-1 text-left"
                    >
                      <button @click="openDrawer(n)" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">View Notification</button>
                      <button @click="resend(n)" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">Resend</button>
                      <button v-if="n.status === 'Scheduled'" @click="cancelScheduled(n)" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">Cancel Scheduled Notification</button>
                      <div class="my-1 border-t border-slate-100"></div>
                      <button v-if="n.type === 'Announcement' || n.type === 'Manual Notification'" @click="deleteAnnouncement(n)" class="w-full text-left px-3.5 py-2 text-sm text-rose-600 hover:bg-rose-50 transition-colors">Delete Announcement</button>
                    </div>
                  </td>
                </tr>

                <tr v-if="filteredNotifications.length === 0">
                  <td colspan="7" class="px-5 py-12 text-center text-sm text-slate-400">No notifications match your search or filters.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <!-- Notification History -->
        <section class="bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
          <div class="px-5 py-4 border-b border-slate-200">
            <h2 class="text-sm font-bold text-slate-900">Notification History</h2>
            <p class="text-xs text-slate-500 mt-0.5">Complete delivery log across all notification types.</p>
          </div>
          <div class="overflow-x-auto">
            <table class="w-full text-sm">
              <thead>
                <tr class="border-b border-slate-200 bg-slate-50/60">
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Recipient</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Title</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Date</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Status</th>
                  <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Opened</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="n in notifications"
                  :key="'h-' + n.id"
                  class="border-b border-slate-100 last:border-0 hover:bg-slate-50 transition-colors"
                >
                  <td class="px-5 py-3 text-slate-700 whitespace-nowrap">{{ n.recipient }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ n.title }}</td>
                  <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ n.sentDate !== '—' ? n.sentDate : n.scheduledDate }}</td>
                  <td class="px-3 py-3">
                    <span :class="[statusMeta[n.status].tint, statusMeta[n.status].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full whitespace-nowrap">
                      <span :class="statusMeta[n.status].dot" class="w-1.5 h-1.5 rounded-full"></span>
                      {{ n.status }}
                    </span>
                  </td>
                  <td class="px-5 py-3">
                    <span :class="n.opened ? 'text-emerald-700' : 'text-slate-400'" class="text-xs font-semibold">{{ n.opened ? 'Yes' : 'No' }}</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </main>
    </div>

    <!-- ============================ NOTIFICATION DETAILS DRAWER ============================ -->
    <transition name="fade">
      <div v-if="showDrawer" class="fixed inset-0 bg-slate-900/30 z-40" @click="closeDrawer"></div>
    </transition>
    <transition name="slide">
      <aside v-if="showDrawer" class="fixed top-0 right-0 h-screen w-full max-w-sm bg-white border-l border-slate-200 shadow-lg z-50 flex flex-col">
        <div class="h-[70px] flex items-center justify-between px-5 border-b border-slate-200 shrink-0">
          <h2 class="text-sm font-bold text-slate-900">Notification Details</h2>
          <button @click="closeDrawer" class="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-50 transition-colors">✕</button>
        </div>

        <div v-if="selectedNotification" class="flex-1 overflow-y-auto p-6 space-y-6">
          <div>
            <span :class="[statusMeta[selectedNotification.status].tint, statusMeta[selectedNotification.status].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full mb-3">
              <span :class="statusMeta[selectedNotification.status].dot" class="w-1.5 h-1.5 rounded-full"></span>
              {{ selectedNotification.status }}
            </span>
            <p class="text-base font-bold text-slate-900">{{ selectedNotification.title }}</p>
          </div>

          <div>
            <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Message Content</h3>
            <p class="text-sm text-slate-700 leading-relaxed bg-slate-50 rounded-lg p-4">{{ selectedNotification.message }}</p>
          </div>

          <div class="bg-slate-50 rounded-lg divide-y divide-slate-200">
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Recipient</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedNotification.recipient }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Child</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedNotification.child }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Notification Type</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedNotification.type }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Scheduled Date</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedNotification.scheduledDate }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Sent Date</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedNotification.sentDate }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Delivery Method</span>
              <span class="text-sm font-medium text-slate-900">{{ selectedNotification.deliveryMethod }}</span>
            </div>
            <div class="flex items-center justify-between px-4 py-3">
              <span class="text-xs text-slate-500">Opened</span>
              <span :class="selectedNotification.opened ? 'text-emerald-700' : 'text-slate-400'" class="text-sm font-semibold">{{ selectedNotification.opened ? 'Yes' : 'No' }}</span>
            </div>
          </div>
        </div>

        <div class="border-t border-slate-200 p-4 flex items-center gap-2 shrink-0">
          <button @click="resend(selectedNotification)" class="flex-1 text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors">Resend</button>
          <button @click="closeDrawer" class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Close</button>
        </div>
      </aside>
    </transition>

    <!-- ============================ CREATE ANNOUNCEMENT MODAL ============================ -->
    <transition name="fade">
      <div v-if="showAnnounceModal" class="fixed inset-0 bg-slate-900/40 z-40 flex items-center justify-center p-4" @click.self="showAnnounceModal = false">
        <div class="bg-white rounded-xl shadow-lg w-full max-w-2xl max-h-[90vh] overflow-y-auto">
          <div class="flex items-center justify-between px-6 py-4 border-b border-slate-200">
            <h2 class="text-base font-bold text-slate-900">Create Announcement</h2>
            <button @click="showAnnounceModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-50 transition-colors">✕</button>
          </div>

          <div class="p-6 space-y-4">
            <div>
              <label class="block text-xs font-semibold text-slate-500 mb-1.5">Title</label>
              <input v-model="announceForm.title" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
            </div>
            <div>
              <label class="block text-xs font-semibold text-slate-500 mb-1.5">Message</label>
              <textarea v-model="announceForm.message" rows="4" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors resize-none"></textarea>
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Recipients</label>
                <select v-model="announceForm.recipients" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                  <option>All Parents</option>
                  <option>Selected Parents</option>
                </select>
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Notification Method</label>
                <select v-model="announceForm.method" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                  <option>In-App</option>
                  <option disabled>SMS (Future)</option>
                  <option disabled>Email (Future)</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-xs font-semibold text-slate-500 mb-1.5">Schedule</label>
              <div class="grid grid-cols-2 gap-2 mb-3">
                <button
                  @click="announceForm.schedule = 'Immediately'"
                  :class="announceForm.schedule === 'Immediately' ? 'bg-emerald-600 text-white' : 'border border-slate-200 text-slate-600 hover:bg-slate-50'"
                  class="text-sm font-semibold px-3 py-2 rounded-lg transition-colors"
                >Immediately</button>
                <button
                  @click="announceForm.schedule = 'Later'"
                  :class="announceForm.schedule === 'Later' ? 'bg-emerald-600 text-white' : 'border border-slate-200 text-slate-600 hover:bg-slate-50'"
                  class="text-sm font-semibold px-3 py-2 rounded-lg transition-colors"
                >Later</button>
              </div>
              <input
                v-if="announceForm.schedule === 'Later'"
                v-model="announceForm.scheduleDate"
                type="datetime-local"
                class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
              />
            </div>
          </div>

          <div class="flex items-center justify-end gap-2 px-6 py-4 border-t border-slate-200">
            <button @click="showAnnounceModal = false" class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Cancel</button>
            <button @click="sendAnnouncement" class="text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors">Send</button>
          </div>
        </div>
      </div>
    </transition>

    <!-- ============================ NOTIFICATION SETTINGS MODAL ============================ -->
    <transition name="fade">
      <div v-if="showSettingsModal" class="fixed inset-0 bg-slate-900/40 z-40 flex items-center justify-center p-4" @click.self="showSettingsModal = false">
        <div class="bg-white rounded-xl shadow-lg w-full max-w-lg max-h-[90vh] overflow-y-auto">
          <div class="flex items-center justify-between px-6 py-4 border-b border-slate-200">
            <h2 class="text-base font-bold text-slate-900">Notification Settings</h2>
            <button @click="showSettingsModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-50 transition-colors">✕</button>
          </div>

          <div class="p-6 space-y-6">
            <div>
              <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Vaccination Reminder</h3>
              <p class="text-xs text-slate-500 mb-3">Send reminder notifications before a scheduled vaccination.</p>
              <div class="space-y-2">
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.reminder7" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">7 days before</span>
                </label>
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.reminder3" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">3 days before</span>
                </label>
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.reminder1" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">1 day before</span>
                </label>
              </div>
            </div>

            <div>
              <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Missed Vaccination Reminder</h3>
              <p class="text-xs text-slate-500 mb-3">Send follow-up reminders after a missed schedule.</p>
              <div class="space-y-2">
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.missed1" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">Send after 1 day</span>
                </label>
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.missed3" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">Send after 3 days</span>
                </label>
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.missed7" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">Send after 7 days</span>
                </label>
              </div>
            </div>

            <div>
              <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Queue Notification</h3>
              <div class="space-y-2">
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.queueConfirmed" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">Notify when queue is confirmed</span>
                </label>
                <label class="flex items-center gap-3 bg-slate-50 rounded-lg px-4 py-2.5 cursor-pointer">
                  <input v-model="settings.queueNextInLine" type="checkbox" class="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
                  <span class="text-sm text-slate-700">Notify when next in line</span>
                </label>
              </div>
            </div>
          </div>

          <div class="flex items-center justify-end gap-2 px-6 py-4 border-t border-slate-200">
            <button @click="showSettingsModal = false" class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Cancel</button>
            <button @click="showSettingsModal = false" class="text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors">Save Settings</button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<style scoped>
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
.slide-enter-active, .slide-leave-active { transition: transform 0.25s ease; }
.slide-enter-from, .slide-leave-to { transform: translateX(100%); }

@media (prefers-reduced-motion: reduce) {
  * { transition-duration: 0.01ms !important; }
}
</style>