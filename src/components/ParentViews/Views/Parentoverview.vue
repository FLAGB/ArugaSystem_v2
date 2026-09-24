<template>
  <div class="w-full min-h-screen bg-slate-50 flex justify-center font-sans antialiased text-slate-900">
    <div class="w-full max-w-312.5 px-6 py-6">

      <HeaderNav
        :parent-data="parentData"
        :children="children"
        :unread-count="unreadCount"
        @open-profile="showProfile = true"
        @open-notifications="showNotifications = true"
        @logout="handleLogout"
        @select-child="handleSelectChild"
      />

      <div class="grid grid-cols-12 gap-8">

        <ChildSidebar :children="children" :selected-child="selectedChild" @select-child="handleSelectChild" />

        <main class="col-span-12 lg:col-span-9">
          <div class="space-y-5 animate-in slide-in-from-bottom-4 duration-500">
            <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-8 flex flex-col md:flex-row justify-between items-center gap-6">
              <div>
                <p class="text-[10px] font-semibold text-slate-400 uppercase tracking-wide mb-2">Today's Priority Ticket</p>
                <h2 v-if="queueStatus.checkedIn" class="text-4xl font-black text-slate-800">You are Queue <span class="text-emerald-500">#{{ String(queueStatus.myQueueNumber).padStart(3, '0') }}</span></h2>
                <h2 v-else class="text-2xl font-black text-slate-800">Not checked in today</h2>
                <p class="text-sm font-bold text-slate-500 mt-1">Active for {{ selectedChild ? `${selectedChild.firstName} ${selectedChild.lastName}` : 'Select a child' }}</p>
              </div>
              <div class="bg-emerald-50 px-10 py-6 rounded-xl border border-emerald-500/20 text-center min-w-45">
                <p class="text-[10px] font-black text-emerald-600 uppercase mb-1">Now Serving</p>
                <p class="text-5xl font-black text-slate-800">{{ queueStatus.nowServingNumber ? '#' + String(queueStatus.nowServingNumber).padStart(3, '0') : '—' }}</p>
                <p v-if="queueStatus.positionInLine" class="text-[10px] font-bold text-emerald-600 mt-2 uppercase">{{ queueStatus.positionInLine }}{{ queueStatus.positionInLine === 1 ? 'st' : queueStatus.positionInLine === 2 ? 'nd' : queueStatus.positionInLine === 3 ? 'rd' : 'th' }} in line</p>
              </div>
            </div>

            <div v-if="selectedChild" class="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
              <div class="bg-emerald-600 px-8 py-5 flex items-center gap-5">
                <div class="w-14 h-14 rounded-xl bg-white/20 flex items-center justify-center text-3xl shadow">{{ selectedChild.sex === 'Female' ? '👧' : '👶' }}</div>
                <div>
                  <p class="text-white font-bold text-lg leading-tight">{{ selectedChild.firstName }} {{ selectedChild.lastName }}</p>
                  <p class="text-emerald-200 text-[10px] font-bold uppercase mt-0.5">{{ selectedChild.healthCenter || 'Leveriza Health Center' }}</p>
                </div>
                <div class="ml-auto text-right hidden sm:block">
                  <p class="text-[10px] text-white/60 font-black uppercase">Patient ID</p>
                  <p class="text-white font-mono text-xs font-bold">#{{ selectedChild.childID.substring(0, 8).toUpperCase() }}</p>
                </div>
              </div>
              <div class="grid grid-cols-2 sm:grid-cols-3 divide-x divide-y divide-slate-50">
                <div class="px-6 py-5"><p class="text-[9px] font-semibold text-slate-400 uppercase tracking-wide mb-1">Date of Birth</p><p class="text-sm font-bold text-slate-800">{{ formatDisplayDate(new Date(selectedChild.birthDate)) }}</p></div>
                <div class="px-6 py-5"><p class="text-[9px] font-semibold text-slate-400 uppercase tracking-wide mb-1">Age</p><p class="text-sm font-bold text-slate-800">{{ childAge }}</p></div>
                <div class="px-6 py-5"><p class="text-[9px] font-semibold text-slate-400 uppercase tracking-wide mb-1">Sex</p><p class="text-sm font-bold text-slate-800">{{ selectedChild.sex || '—' }}</p></div>
                <div class="px-6 py-5"><p class="text-[9px] font-semibold text-slate-400 uppercase tracking-wide mb-1">Birth Weight</p><p class="text-sm font-bold text-slate-800">{{ selectedChild.birthWeight ? selectedChild.birthWeight + ' kg' : '—' }}</p></div>
                <div class="px-6 py-5"><p class="text-[9px] font-semibold text-slate-400 uppercase tracking-wide mb-1">Birth Height</p><p class="text-sm font-bold text-slate-800">{{ selectedChild.birthHeight ? selectedChild.birthHeight + ' cm' : '—' }}</p></div>
                <div class="px-6 py-5">
                  <p class="text-[9px] font-semibold text-slate-400 uppercase tracking-wide mb-1">Barangay</p>
                  <p class="text-sm font-bold text-slate-800">{{ selectedChild.barangay || selectedChild.barangayNo || selectedChild.Barangay || '—' }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white rounded-xl border border-slate-200 p-7 shadow-sm">
              <div class="flex justify-between items-center mb-5">
                <h3 class="text-base font-bold text-slate-800">Upcoming Doses</h3>
                <router-link to="/ParentSchedule" class="text-[10px] font-semibold text-emerald-600 uppercase tracking-wide hover:text-emerald-600 transition-colors">View Schedule →</router-link>
              </div>
              <div class="space-y-3">
                <div v-for="vax in upcomingDoses.slice(0, 3)" :key="vax.doseId"
                    class="p-4 bg-slate-50 rounded-xl border border-slate-100 flex items-center justify-between">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 bg-emerald-600 rounded-xl flex items-center justify-center text-white text-sm">💉</div>
                    <div>
                      <p class="text-xs font-bold text-slate-800">{{ vax.name }} · Dose {{ vax.doseNumber }}</p>
                      <p class="text-[10px] text-slate-400 mt-0.5">{{ formatDisplayDate(vax.scheduledDate) }}</p>
                    </div>
                  </div>
                  <span class="px-3 py-1 bg-white rounded-lg text-[9px] font-black text-emerald-600 border border-slate-200 uppercase">Scheduled</span>
                </div>
                <p v-if="upcomingDoses.length > 3" class="text-center text-[10px] text-slate-400 font-bold pt-1">
                  +{{ upcomingDoses.length - 3 }} more doses scheduled
                </p>
              </div>
            </div>
          </div>
        </main>
      </div>

      <ProfileModal v-if="showProfile" :parent-data="parentData" :children="children" @close="showProfile = false" />
      <NotificationPanel v-if="showNotifications" :parent-data="parentData" @close="showNotifications = false" />

    </div>
  </div>
</template>
<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { addDays, isMonday, isWednesday, isFriday } from 'date-fns'

import HeaderNav from '../Components/Headernav.vue'
import ChildSidebar from '../Components/Childsidebar.vue'
import ProfileModal from '../Components/Profilemodal.vue'
import NotificationPanel from '../Components/Notificationpanel.vue'

import { getAccount, logout as authLogout } from '@/utils/auth'
import api from '../Composables/api.js'

const router = useRouter()

// =====================================================
// SESSION
// =====================================================

const account = ref(null)
const parentData = ref(null)


// =====================================================
// PARENT / CHILDREN
// =====================================================

const children = ref([])
const selectedChild = ref(null)


// =====================================================
// VACCINATION (client-side DOH cascade — see Scheduled.vue /
// Checkin.vue for the same logic; kept in sync across pages so
// "upcoming doses" always means the same thing everywhere)
// =====================================================

const completedRecords = ref([])
const timelineLoading = ref(false)

const VACCINE_MASTER = [
  { vaccineId: 1, name: 'BCG Vaccine',                      doses: [{ n: 1, gap: 0   }] },
  { vaccineId: 2, name: 'Hepatitis B Vaccine',              doses: [{ n: 1, gap: 0   }] },
  { vaccineId: 3, name: 'Pentavalent (DPT-Hep B-HIB)',      doses: [{ n: 1, gap: 45  }, { n: 2, gap: 28 }, { n: 3, gap: 28 }] },
  { vaccineId: 4, name: 'Oral Polio Vaccine (OPV)',         doses: [{ n: 1, gap: 45  }, { n: 2, gap: 28 }, { n: 3, gap: 28 }] },
  { vaccineId: 5, name: 'Inactivated Polio Vaccine (IPV)',  doses: [{ n: 1, gap: 105 }, { n: 2, gap: 165}] },
  { vaccineId: 6, name: 'Pneumococcal Conj. Vaccine (PCV)', doses: [{ n: 1, gap: 45  }, { n: 2, gap: 28 }, { n: 3, gap: 28 }] },
  { vaccineId: 7, name: 'MMR Vaccine',                      doses: [{ n: 1, gap: 270 }, { n: 2, gap: 90 }] },
]

function snapToClinicDay(date) {
  let d = new Date(date)
  while (!(isMonday(d) || isWednesday(d) || isFriday(d))) d = addDays(d, 1)
  return d
}

const computedVaccineList = computed(() => {
  if (!selectedChild.value?.birthDate) return []
  const birth = new Date(selectedChild.value.birthDate)
  const result = []

  for (const vaccine of VACCINE_MASTER) {
    let prevActualDate = null
    let prevOriginalDate = null

    for (const dose of vaccine.doses) {
      const record = completedRecords.value.find(r =>
        Number(r.vaccineID ?? r.vaccineId) === vaccine.vaccineId &&
        Number(r.doseNumber ?? r.DoseNumber) === dose.n &&
        r.status === 'Completed' &&
        r.dateAdministered
      )

      const originalDueDate = dose.n === 1
        ? snapToClinicDay(addDays(birth, dose.gap))
        : snapToClinicDay(addDays(prevOriginalDate ?? birth, dose.gap))

      let scheduledDate
      if (record) {
        scheduledDate = new Date(record.dateAdministered)
      } else if (dose.n === 1) {
        scheduledDate = snapToClinicDay(addDays(birth, dose.gap))
      } else {
        const base = prevActualDate ?? prevOriginalDate ?? birth
        scheduledDate = snapToClinicDay(addDays(base, dose.gap))
      }

      const administeredDate = record ? new Date(record.dateAdministered) : null

      prevOriginalDate = originalDueDate
      prevActualDate = administeredDate ?? null

      result.push({
        doseId: `${vaccine.vaccineId}-${dose.n}`,
        vaccineId: vaccine.vaccineId,
        name: vaccine.name,
        doseNumber: dose.n,
        scheduledDate,
        isCompleted: !!record,
      })
    }
  }
  return result
})

async function fetchCompletedRecords(childId) {
  if (!childId) { completedRecords.value = []; return }
  timelineLoading.value = true
  try {
    const response = await api.get(`/VaccinationRecords/child/${childId}`)
    completedRecords.value = response.data ?? []
  } catch (error) {
    console.error('Failed to load vaccination records:', error)
    completedRecords.value = []
  } finally {
    timelineLoading.value = false
  }
}

// =====================================================
// QUEUE TICKET (Today's Priority Ticket card)
// =====================================================

const queueStatus = ref({ checkedIn: false, myQueueNumber: null, myStatus: null, nowServingNumber: null, positionInLine: null })
let queuePollHandle = null

async function fetchQueueStatus() {
  if (!parentData.value?.parentID) return
  try {
    const response = await api.get(`/Queue/my-status/${parentData.value.parentID}`)
    queueStatus.value = {
      checkedIn: response.data.checkedIn ?? response.data.CheckedIn,
      myQueueNumber: response.data.myQueueNumber ?? response.data.MyQueueNumber,
      myStatus: response.data.myStatus ?? response.data.MyStatus,
      nowServingNumber: response.data.nowServingNumber ?? response.data.NowServingNumber,
      positionInLine: response.data.positionInLine ?? response.data.PositionInLine,
    }
  } catch (error) {
    console.error('Failed to load queue status:', error)
  }
}


// =====================================================
// NOTIFICATIONS
// =====================================================

const unreadCount = ref(0)


// =====================================================
// UI
// =====================================================

const showProfile = ref(false)
const showNotifications = ref(false)


// =====================================================
// SELECTED CHILD
// =====================================================

const selectedChildStorageKey = 'selectedParentChild'


// =====================================================
// HELPERS
// =====================================================

function formatDisplayDate(date) {
  if (!date) return '—'

  const parsed = new Date(date)

  if (Number.isNaN(parsed.getTime())) {
    return '—'
  }

  return parsed.toLocaleDateString('en-PH', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}


function calculateAge(birthDate) {
  if (!birthDate) return '—'

  const birth = new Date(birthDate)
  const today = new Date()

  if (Number.isNaN(birth.getTime())) {
    return '—'
  }

  let years =
    today.getFullYear() -
    birth.getFullYear()

  let months =
    today.getMonth() -
    birth.getMonth()

  if (
    months < 0 ||
    (
      months === 0 &&
      today.getDate() < birth.getDate()
    )
  ) {
    years--
  }

  months =
    (
      today.getMonth() -
      birth.getMonth() +
      12
    ) % 12

  if (years === 0) {
    return `${months} month${months !== 1 ? 's' : ''}`
  }

  if (months === 0) {
    return `${years} year${years !== 1 ? 's' : ''}`
  }

  return `${years} year${years !== 1 ? 's' : ''} ${months} month${months !== 1 ? 's' : ''}`
}


// =====================================================
// COMPUTED
// =====================================================

const childAge = computed(() => {
  return calculateAge(
    selectedChild.value?.birthDate
  )
})


const upcomingDoses = computed(() => {
  return computedVaccineList.value
    .filter(item => !item.isCompleted)
    .sort((a, b) => a.scheduledDate - b.scheduledDate)
})


// =====================================================
// LOAD LOGGED-IN PARENT
// =====================================================

function loadParentSession() {
  const savedAccount = getAccount()

  if (!savedAccount) {
    router.push('/')
    return false
  }

  account.value = savedAccount

  // Support both possible backend response structures
  const rawUser = savedAccount.user ?? savedAccount

  // Backend now returns PascalCase (ParentID, FirstName, LastName, Email, ...).
  // Normalize to the camelCase shape every Parent component (Headernav,
  // ChildSidebar, ProfileModal, etc.) already expects, same ?? pattern used
  // for the children map below.
  parentData.value = {
    ...rawUser,
    parentID: rawUser.parentID ?? rawUser.ParentID,
    firstName: rawUser.firstName ?? rawUser.FirstName,
    middleName: rawUser.middleName ?? rawUser.MiddleName,
    lastName: rawUser.lastName ?? rawUser.LastName,
    email: rawUser.email ?? rawUser.Email,
  }

  if (!parentData.value?.parentID) {
    console.error(
      'Invalid parent session:',
      savedAccount
    )

    router.push('/')
    return false
  }

  return true
}

// =====================================================
// LOAD CHILDREN
// =====================================================

async function fetchChildren() {

  if (!parentData.value?.parentID) {
    console.error('No ParentID available')
    return
  }

  try {

    console.log(
      'Fetching children for ParentID:',
      parentData.value.parentID
    )

    const response = await api.get(
      `/Parents/dashboard/${parentData.value.parentID}`
    )

    console.log(
      'Parent dashboard response:',
      response.data
    )

    children.value =
      (response.data ?? []).map(child => {
        const relationshipType =
          child.relationshipType ??
          child.RelationshipType

        const relatedParentName =
          child.parentFullName ??
          child.ParentFullName

        return {
          childID:
            child.childID ??
            child.ChildID,

          firstName:
            child.firstName ??
            child.FirstName,

          middleName:
            child.middleName ??
            child.MiddleName,

          lastName:
            child.lastName ??
            child.LastName,

          birthDate:
            child.birthDate ??
            child.BirthDate,

          placeOfBirth:
            child.placeOfBirth ??
            child.PlaceOfBirth,

          sex:
            child.sex ??
            child.Sex,

          barangay:
            child.barangay ??
            child.Barangay,

          address:
            child.address ??
            child.Address,

          healthCenter:
            child.healthCenter ??
            child.HealthCenter,

          relationshipType,

          isPrimaryContact:
            child.isPrimaryContact ??
            child.IsPrimaryContact,

          canReceiveNotifications:
            child.canReceiveNotifications ??
            child.CanReceiveNotifications,

          // The dashboard endpoint only returns the relationship for the
          // currently logged-in parent (Maria), so we can only fill in
          // whichever one of these matches her own RelationshipType —
          // co-parents/guardians linked by someone else aren't in this
          // response and would need a separate lookup to display.
          motherName:   relationshipType === 'Mother'   ? relatedParentName : undefined,
          fatherName:   relationshipType === 'Father'   ? relatedParentName : undefined,
          guardianName: relationshipType === 'Guardian' ? relatedParentName : undefined,
        }
      })

    console.log(
      'Loaded children:',
      children.value
    )

  } catch (error) {

    console.error(
      'Failed to load parent children:',
      error
    )

    children.value = []
  }
}
// =====================================================
// RESTORE SELECTED CHILD
// =====================================================

function restoreSelectedChild() {

  if (children.value.length === 0) {
    selectedChild.value = null
    return
  }

  const saved =
    localStorage.getItem(
      selectedChildStorageKey
    )

  if (!saved) {
    selectedChild.value =
      children.value[0]

    return
  }

  try {

    const savedChild =
      JSON.parse(saved)

    selectedChild.value =
      children.value.find(
        child =>
          child.childID ===
          savedChild.childID
      ) ??
      children.value[0]

  } catch {

    selectedChild.value =
      children.value[0]
  }
}


// =====================================================
// SELECT CHILD
// =====================================================

async function handleSelectChild(child) {

  selectedChild.value = child

  localStorage.setItem(
    selectedChildStorageKey,
    JSON.stringify(child)
  )

  await fetchCompletedRecords(
    child.childID
  )
}


// =====================================================
// LOAD UNREAD NOTIFICATION COUNT
// =====================================================

async function fetchUnreadCount() {

  if (!parentData.value?.parentID) {
    return
  }

  try {

    const response = await api.get(
      `/Notifications/parent/${parentData.value.parentID}`
    )

    unreadCount.value =
      (response.data ?? [])
        .filter(notification =>
          !notification.isRead
        )
        .length

  } catch (error) {

    console.error(
      'Failed to load notification count:',
      error
    )

    unreadCount.value = 0
  }
}


// =====================================================
// LOGOUT
// =====================================================

function handleLogout() {

  authLogout()

  router.push('/')
}


// =====================================================
// INITIAL LOAD
// =====================================================

onMounted(async () => {

  // 1. Identify currently logged-in parent
  const validSession =
    loadParentSession()

  if (!validSession) {
    return
  }


  // 2. Load all children
  await fetchChildren()


  // 3. Restore selected child
  restoreSelectedChild()


  // 4. Load vaccination records for the selected child
  if (selectedChild.value) {

    await fetchCompletedRecords(
      selectedChild.value.childID
    )
  }


  // 5. Load unread notifications
  await fetchUnreadCount()

  // 6. Load queue ticket status, then poll so "Now Serving" stays live
  await fetchQueueStatus()
  queuePollHandle = setInterval(fetchQueueStatus, 5000)
})

onUnmounted(() => {
  if (queuePollHandle) clearInterval(queuePollHandle)
})
</script>