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
          <div class="animate-in fade-in zoom-in-95 duration-500 space-y-4">

            <!-- Children load error -->
            <div v-if="childrenError" class="bg-red-50 border border-red-200 rounded-xl p-5 flex items-center gap-3">
              <span>⚠️</span>
              <p class="text-[11px] text-red-700 font-bold">{{ childrenError }}</p>
            </div>

            <!-- No linked children -->
            <div v-else-if="!childrenLoading && children.length === 0" class="bg-amber-50 border border-amber-200 rounded-xl p-5 flex items-center gap-3">
              <span>⚠️</span>
              <p class="text-[11px] text-amber-700 font-bold">No children are linked to this account yet. Please contact the clinic to link a child before checking in.</p>
            </div>

            <!-- ═══════════════ SUCCESS / READY FOR QUEUE STATE ═══════════════ -->
            <div v-if="checkInConfirmed" class="bg-white rounded-xl border border-slate-200 shadow-sm p-8">
              <div class="text-center mb-6">
                <div class="w-16 h-16 rounded-full bg-emerald-50 flex items-center justify-center text-3xl mx-auto mb-3">✅</div>
                <h2 class="text-xl font-bold text-slate-800">Checked In</h2>
                <p v-if="myQueueNumber" class="text-2xl font-black text-emerald-600 mt-2">Queue #{{ String(myQueueNumber).padStart(3, '0') }}</p>
                <p class="text-xs text-slate-400 mt-1">You're in the queue — please wait to be called.</p>
              </div>

              <div class="space-y-3 mb-6">
                <div v-for="item in confirmedChildren" :key="item.child.childID"
                     class="p-4 bg-slate-50 rounded-xl border border-slate-100 flex items-center justify-between">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-xl bg-emerald-50 flex items-center justify-center text-xl shrink-0">
                      {{ item.child.sex === 'Female' ? '👧' : '👶' }}
                    </div>
                    <div>
                      <p class="text-xs font-bold text-slate-800">{{ item.child.firstName }} {{ item.child.lastName }}</p>
                      <p v-if="item.nextDose" class="text-[10px] text-slate-400 mt-0.5">
                        Next: {{ item.nextDose.name }} · Dose {{ item.nextDose.doseNumber }} · {{ formatDisplayDate(item.nextDose.scheduledDate) }}
                      </p>
                      <p v-else class="text-[10px] text-slate-400 mt-0.5">No pending doses on file</p>
                    </div>
                  </div>
                </div>
              </div>

              <button
                @click="resetCheckIn"
                class="w-full px-10 py-3.5 bg-slate-800 hover:bg-slate-900 text-white rounded-lg font-medium text-sm transition-all"
              >
                Start a New Check-In
              </button>
            </div>

            <!-- ═══════════════ DEMO CHECK-IN (QR step skipped) ═══════════════ -->
            <div v-else class="bg-slate-800 rounded-xl p-10 text-center text-white relative overflow-hidden shadow-2xl">
              <div class="absolute inset-0 opacity-5" style="background-image: radial-gradient(circle, #10b981 1px, transparent 1px); background-size: 24px 24px;"></div>
              <div class="relative">
                <h2 class="text-2xl font-bold mb-2">Check In</h2>
                <p class="text-white/50 text-xs mb-8">Tap below to check in your child for today's visit</p>

                <div class="w-24 h-24 mx-auto mb-8 rounded-full bg-emerald-500/10 border-2 border-emerald-500 flex items-center justify-center text-5xl">
                  🩺
                </div>

                <button
                  @click="startDemoCheckIn"
                  :disabled="childrenLoading || children.length === 0"
                  class="px-10 py-3.5 bg-emerald-600 hover:bg-emerald-700 disabled:opacity-40 disabled:cursor-not-allowed text-white rounded-lg font-medium text-sm transition-all shadow-lg"
                >
                  Check In Now
                </button>

                <p v-if="qrError" class="text-red-300 text-xs font-bold mt-3">{{ qrError }}</p>

                <p class="text-white/30 text-[10px] font-medium mt-6">Check-in is only available during clinic hours · Mon, Wed, Fri · 8:00 AM – 12:00 PM</p>
              </div>
            </div>
          </div>
        </main>
      </div>

      <!-- ═══════════════ CHILD SELECTION MODAL ═══════════════ -->
      <div v-if="showChildSelection" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4">
        <div class="bg-white rounded-xl w-full max-w-md p-6 shadow-2xl">
          <h3 class="text-lg font-bold text-slate-800 mb-1">Who are you checking in for?</h3>
          <p class="text-xs text-slate-400 mb-5">Select one or more children linked to your account.</p>

          <div class="space-y-2 max-h-80 overflow-y-auto mb-5">
            <button
              v-for="child in children"
              :key="child.childID"
              type="button"
              @click="toggleChildSelection(child)"
              class="w-full flex items-center gap-3 p-3 rounded-xl border transition-all text-left"
              :class="isChildSelected(child) ? 'border-emerald-500 bg-emerald-50' : 'border-slate-100 hover:border-slate-200'"
            >
              <div class="w-9 h-9 rounded-xl bg-white border border-slate-100 flex items-center justify-center text-lg shrink-0">
                {{ child.sex === 'Female' ? '👧' : '👶' }}
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-xs font-bold text-slate-800 truncate">{{ child.firstName }} {{ child.lastName }}</p>
                <p class="text-[10px] text-slate-400">{{ child.relationshipType || 'Linked child' }}</p>
              </div>
              <div
                class="w-5 h-5 rounded-md border-2 flex items-center justify-center shrink-0"
                :class="isChildSelected(child) ? 'bg-emerald-500 border-emerald-500' : 'border-slate-300'"
              >
                <span v-if="isChildSelected(child)" class="text-white text-[10px] font-bold">✓</span>
              </div>
            </button>
          </div>

          <p v-if="selectedChildren.length === 0" class="text-[10px] text-slate-400 font-bold mb-3">Select at least one child to continue.</p>
          <p v-if="checkInError" class="text-[10px] text-red-500 font-bold mb-3">{{ checkInError }}</p>

          <div class="flex gap-3">
            <button
              @click="cancelChildSelection"
              class="flex-1 px-4 py-2.5 text-sm font-medium text-slate-600 border border-slate-200 rounded-lg hover:bg-slate-50 transition-colors"
            >
              Cancel
            </button>
            <button
              @click="confirmChildren"
              :disabled="selectedChildren.length === 0 || confirmLoading"
              class="flex-1 px-4 py-2.5 text-sm font-medium text-white bg-emerald-600 hover:bg-emerald-700 disabled:opacity-50 disabled:cursor-not-allowed rounded-lg transition-colors"
            >
              {{ confirmLoading ? 'Confirming…' : 'Confirm' }}
            </button>
          </div>
        </div>
      </div>

      <ProfileModal v-if="showProfile" :parent-data="parentData" :children="children" @close="showProfile = false" />
      <NotificationPanel v-if="showNotifications" :parent-data="parentData" @close="showNotifications = false" />

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { addDays, format, isMonday, isWednesday, isFriday } from 'date-fns'

import HeaderNav from '../Components/Headernav.vue'
import ChildSidebar from '../Components/Childsidebar.vue'
import ProfileModal from '../Components/Profilemodal.vue'
import NotificationPanel from '../Components/Notificationpanel.vue'

import { getAccount, logout as authLogout } from '@/utils/auth'
import api from '../Composables/api.js'

const router = useRouter()

// =====================================================
// SESSION / PARENT (same pattern as ParentOverview.vue)
// =====================================================

const parentData = ref(null)

function loadParentSession() {
  const savedAccount = getAccount()

  if (!savedAccount) {
    router.push('/')
    return false
  }

  // Support both possible backend response structures
  const rawUser = savedAccount.user ?? savedAccount

  // Backend now returns PascalCase (ParentID, FirstName, LastName, Email, ...).
  // Normalize to the camelCase shape every Parent component expects.
  parentData.value = {
    ...rawUser,
    parentID: rawUser.parentID ?? rawUser.ParentID,
    firstName: rawUser.firstName ?? rawUser.FirstName,
    middleName: rawUser.middleName ?? rawUser.MiddleName,
    lastName: rawUser.lastName ?? rawUser.LastName,
    email: rawUser.email ?? rawUser.Email,
  }

  if (!parentData.value?.parentID) {
    console.error('Invalid parent session:', savedAccount)
    router.push('/')
    return false
  }

  return true
}

function handleLogout() {
  authLogout()
  router.push('/')
}

// =====================================================
// CHILDREN (retrieved from the backend — DB relationship
// is the source of truth, not localStorage)
// =====================================================

const children = ref([])
const childrenLoading = ref(true)
const childrenError = ref('')

// Kept only for the sidebar/header "currently viewing" UX —
// this selection has no bearing on who can check in.
const selectedChild = ref(null)

async function fetchChildren() {
  if (!parentData.value?.parentID) {
    console.error('No ParentID available')
    childrenLoading.value = false
    return
  }

  childrenLoading.value = true
  childrenError.value = ''

  try {
    const response = await api.get(`/Parents/dashboard/${parentData.value.parentID}`)

    children.value = (response.data ?? []).map(child => {
      const relationshipType = child.relationshipType ?? child.RelationshipType
      const relatedParentName = child.parentFullName ?? child.ParentFullName

      return {
        childID: child.childID ?? child.ChildID,
        firstName: child.firstName ?? child.FirstName,
        middleName: child.middleName ?? child.MiddleName,
        lastName: child.lastName ?? child.LastName,
        birthDate: child.birthDate ?? child.BirthDate,
        placeOfBirth: child.placeOfBirth ?? child.PlaceOfBirth,
        sex: child.sex ?? child.Sex,
        barangay: child.barangay ?? child.Barangay,
        address: child.address ?? child.Address,
        healthCenter: child.healthCenter ?? child.HealthCenter,
        relationshipType,
        isPrimaryContact: child.isPrimaryContact ?? child.IsPrimaryContact,
        canReceiveNotifications: child.canReceiveNotifications ?? child.CanReceiveNotifications,

        // Same derivation as ParentOverview.vue — the dashboard endpoint only
        // returns the relationship for the currently logged-in parent, so we
        // can only fill in whichever of these matches their own relationshipType.
        motherName: relationshipType === 'Mother' ? relatedParentName : undefined,
        fatherName: relationshipType === 'Father' ? relatedParentName : undefined,
        guardianName: relationshipType === 'Guardian' ? relatedParentName : undefined,
      }
    })
  } catch (err) {
    console.error('Failed to load parent children:', err)
    children.value = []
    childrenError.value = 'Unable to load your children right now. Please try again later.'
  } finally {
    childrenLoading.value = false
  }
}

function handleSelectChild(child) {
  // Purely a sidebar/header viewing convenience — does NOT
  // determine who is eligible to check in.
  selectedChild.value = child
}

// =====================================================
// NOTIFICATIONS
// =====================================================

const unreadCount = ref(0)

async function fetchUnreadCount() {
  if (!parentData.value?.parentID) return

  try {
    const response = await api.get(`/Notifications/parent/${parentData.value.parentID}`)
    unreadCount.value = (response.data ?? []).filter(n => !n.isRead).length
  } catch (err) {
    console.error('Failed to load notification count:', err)
    unreadCount.value = 0
  }
}

// =====================================================
// UI PANELS
// =====================================================

const showProfile = ref(false)
const showNotifications = ref(false)

// =====================================================
// DEMO CHECK-IN (QR scan step skipped for demo purposes —
// TODO before real deployment: bring back QR verification,
// e.g. await api.get(`/Queue/validate-qr?code=...`))
// =====================================================

const qrError = ref('')

function startDemoCheckIn() {
  qrError.value = ''

  if (children.value.length === 0) {
    qrError.value = 'No children are linked to this account.'
    return
  }

  selectedChildren.value = []
  showChildSelection.value = true
}

// =====================================================
// CHILD SELECTION MODAL
// =====================================================

const showChildSelection = ref(false)
const selectedChildren = ref([])
const confirmLoading = ref(false)
const checkInConfirmed = ref(false)
const confirmedChildren = ref([])

function toggleChildSelection(child) {
  const index = selectedChildren.value.findIndex(c => c.childID === child.childID)

  if (index >= 0) {
    selectedChildren.value.splice(index, 1)
  } else {
    selectedChildren.value.push(child)
  }
}

function isChildSelected(child) {
  return selectedChildren.value.some(c => c.childID === child.childID)
}

function cancelChildSelection() {
  selectedChildren.value = []
  showChildSelection.value = false
}

// =====================================================
// VACCINATION (DOH schedule) — loaded only for the
// children that were actually selected, after confirm.
// =====================================================

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

function formatDisplayDate(date) {
  if (!date) return '—'
  try { return format(new Date(date), 'MMM d, yyyy') } catch { return '—' }
}

async function fetchVaccinationRecords(childId) {
  if (!childId) return []
  try {
    const res = await api.get(`/VaccinationRecords/child/${childId}`)
    return res.data ?? []
  } catch (err) {
    console.error('fetchVaccinationRecords error:', err)
    return []
  }
}

// Pure function version of the old `computedVaccineList` — takes a
// specific child + their records instead of relying on a single
// page-level `selectedChild`, so it works for any number of children.
function computeUpcomingDosesFor(child, records) {
  if (!child?.birthDate) return []
  const birth = new Date(child.birthDate)
  const result = []

  for (const vaccine of VACCINE_MASTER) {
    let prevActualDate = null
    let prevOriginalDate = null

    for (const dose of vaccine.doses) {
      const record = records.find(r =>
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
    .filter(d => !d.isCompleted)
    .sort((a, b) => a.scheduledDate - b.scheduledDate)
}

// =====================================================
// CONFIRM CHILDREN → submit to the real Queue check-in
// endpoint (POST /api/Queue), then show the resulting
// queue number.
// =====================================================

const checkInError = ref('')
const myQueueNumber = ref(null)

async function confirmChildren() {
  if (selectedChildren.value.length === 0) return

  confirmLoading.value = true
  checkInError.value = ''

  try {
    const summaries = []

    for (const child of selectedChildren.value) {
      const records = await fetchVaccinationRecords(child.childID)
      const upcoming = computeUpcomingDosesFor(child, records)
      summaries.push({ child, nextDose: upcoming[0] ?? null })
    }

    try {
      const response = await api.post('/Queue', {
        ParentID: parentData.value.parentID,
        ChildIDs: selectedChildren.value.map(child => child.childID),
      })
      myQueueNumber.value = response.data.queueNumber ?? response.data.QueueNumber ?? null
    } catch (err) {
      if (err.response?.status === 409) {
        // Already checked in today — not an error, just show their existing ticket.
        myQueueNumber.value = err.response.data?.queueNumber ?? err.response.data?.QueueNumber ?? null
      } else {
        console.error('Check-in failed:', err)
        checkInError.value = err.response?.data?.message || 'Could not check in — please try again.'
        return
      }
    }

    confirmedChildren.value = summaries
    showChildSelection.value = false
    checkInConfirmed.value = true
  } finally {
    confirmLoading.value = false
  }
}

function resetCheckIn() {
  qrError.value = ''
  selectedChildren.value = []
  confirmedChildren.value = []
  checkInConfirmed.value = false
  showChildSelection.value = false
  checkInError.value = ''
  myQueueNumber.value = null
}

// =====================================================
// LIFECYCLE
// =====================================================

onMounted(async () => {
  const validSession = loadParentSession()
  if (!validSession) return

  await fetchChildren()
  await fetchUnreadCount()
})
</script>