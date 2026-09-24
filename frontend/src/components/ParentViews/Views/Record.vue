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
          <div class="space-y-5 animate-in fade-in slide-in-from-right-4 duration-500">
            <div class="grid grid-cols-3 gap-4">
              <div class="bg-emerald-600 p-6 rounded-xl text-center text-white shadow-sm"><h3 class="text-4xl font-black">{{ recordStats.completed }}</h3><p class="text-[9px] uppercase font-bold opacity-70 mt-1 tracking-widest">Completed</p></div>
              <div class="bg-slate-600 p-6 rounded-xl text-center text-white shadow-sm"><h3 class="text-4xl font-black">{{ recordStats.scheduled }}</h3><p class="text-[9px] uppercase font-bold opacity-70 mt-1 tracking-widest">Scheduled</p></div>
              <div class="bg-red-500 p-6 rounded-xl text-center text-white shadow-sm"><h3 class="text-4xl font-black">{{ recordStats.overdue }}</h3><p class="text-[9px] uppercase font-bold opacity-70 mt-1 tracking-widest">Overdue</p></div>
            </div>
            <div class="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
              <div class="px-8 py-6 border-b border-slate-50 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div>
                  <h2 class="text-base font-bold text-slate-800">Vaccination History</h2>
                  <p class="text-[10px] text-slate-400 mt-0.5">{{ selectedChild?.firstName }} {{ selectedChild?.lastName }} <span class="text-slate-200 mx-1">·</span> {{ vaccinationHistory.length }} record{{ vaccinationHistory.length !== 1 ? 's' : '' }}</p>
                </div>
                <div class="flex gap-1 bg-slate-50 p-1 rounded-xl">
                  <button v-for="f in ['All', 'Completed', 'Scheduled', 'Overdue']" :key="f" @click="recordFilter = f"
                          :class="recordFilter === f ? 'bg-white shadow-sm text-emerald-600 font-bold' : 'text-slate-400 hover:text-slate-600'"
                          class="px-3 py-1.5 rounded-lg text-[9px] uppercase font-bold transition-all">{{ f }}</button>
                </div>
              </div>
              <div v-if="recordsLoading" class="py-16 text-center text-slate-400"><p class="text-2xl mb-2 animate-pulse">💉</p><p class="text-xs font-bold">Loading records...</p></div>
              <div v-else-if="filteredRecords.length === 0" class="py-16 text-center text-slate-400"><p class="text-2xl mb-2">📋</p><p class="text-xs font-bold">No {{ recordFilter === 'All' ? '' : recordFilter.toLowerCase() + ' ' }}vaccination records found.</p></div>
              <div v-else class="overflow-x-auto">
                <table class="w-full text-left text-[11px] border-collapse">
                  <thead><tr class="bg-slate-50 text-slate-400 font-medium uppercase tracking-wide text-[9px]">
                    <th class="px-6 py-3">Date</th>
                    <th class="px-6 py-3">Vaccine</th>
                    <th class="px-6 py-3">Dose</th>
                    <th class="px-6 py-3">Status</th>
                    <th class="px-6 py-3">Given By</th>
                    <th class="px-6 py-3">Lot #</th>
                  </tr></thead>
                  <tbody>
                    <tr v-for="(rec, i) in filteredRecords" :key="i" class="border-t border-slate-50 hover:bg-slate-50 transition-colors">
                      <td class="px-6 py-4 whitespace-nowrap text-slate-500">
                        {{ rec.dateAdministered
                            ? formatDisplayDate(new Date(rec.dateAdministered))
                            : rec.scheduledDate
                              ? formatDisplayDate(new Date(rec.scheduledDate)) + ' (scheduled)'
                              : '—' }}
                      </td>
                      <td class="px-6 py-4 font-bold text-slate-800">{{ rec.vaccineName }}</td>
                      <td class="px-6 py-4 text-slate-500">Dose {{ rec.doseNumber }}</td>
                      <td class="px-6 py-4"><span :class="getStatusClass(rec.status)" class="px-3 py-1 rounded-full text-[9px] font-bold uppercase">{{ rec.status }}</span></td>
                      <td class="px-6 py-4 text-slate-500 italic">{{ rec.administeredByName || '—' }}</td>
                      <td class="px-6 py-4 font-mono text-[10px] text-slate-400">{{ rec.lotNumber || '—' }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="px-8 py-5 bg-slate-50 border-t border-slate-100 flex justify-between items-center">
                <p class="text-[10px] text-slate-400 font-medium">Full history available as PDF</p>
                <button class="bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors">Download PDF</button>
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
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { addDays, format, isMonday, isWednesday, isFriday } from 'date-fns'
import HeaderNav from '../Components/Headernav.vue'
import ChildSidebar from '../Components/Childsidebar.vue'
import ProfileModal from '../Components/Profilemodal.vue'
import NotificationPanel from '../Components/Notificationpanel.vue'
import { getAccount, logout as authLogout } from '@/utils/auth'
import api from '../Composables/api.js'

const router = useRouter()

// ── State ──────────────────────────────────────────────────────────────────
const parentData        = ref(null)
const children           = ref([])
const selectedChild      = ref(null)
const showProfile        = ref(false)
const showNotifications  = ref(false)
const unreadCount        = ref(0)

// Records
// completedRecords = raw Completed rows from DB
// vaccinationHistory = completedRecords + computed upcoming/overdue rows
const completedRecords   = ref([])
const vaccinationHistory = ref([])
const recordsLoading     = ref(false)
const recordFilter       = ref('All')
const recordStats        = ref({ completed: 0, scheduled: 0, overdue: 0 })

// ── Vaccine master (DOH schedule) — duplicated per-page, see HARD RULE 1 ───
// vaccineId values below are the real ArugaSystemDB.dbo.Vaccines.VaccineID
// rows (confirmed 2026-09-22): BCG=5, Hepatitis B=6, Pentavalent=7, OPV=8,
// IPV=9, PCV=10, MMR=11. If that table's IDs ever get re-seeded, these need
// to be updated to match — the completed-record lookup below depends on it.
const VACCINE_MASTER = [
  { vaccineId: 5,  name: 'BCG Vaccine',                      doses: [{ n: 1, gap: 0   }] },
  { vaccineId: 6,  name: 'Hepatitis B Vaccine',              doses: [{ n: 1, gap: 0   }] },
  { vaccineId: 7,  name: 'Pentavalent (DPT-Hep B-HIB)',      doses: [{ n: 1, gap: 45  }, { n: 2, gap: 28 }, { n: 3, gap: 28 }] },
  { vaccineId: 8,  name: 'Oral Polio Vaccine (OPV)',         doses: [{ n: 1, gap: 45  }, { n: 2, gap: 28 }, { n: 3, gap: 28 }] },
  { vaccineId: 9,  name: 'Inactivated Polio Vaccine (IPV)',  doses: [{ n: 1, gap: 105 }, { n: 2, gap: 165}] },
  { vaccineId: 10, name: 'Pneumococcal Conj. Vaccine (PCV)', doses: [{ n: 1, gap: 45  }, { n: 2, gap: 28 }, { n: 3, gap: 28 }] },
  { vaccineId: 11, name: 'MMR Vaccine',                      doses: [{ n: 1, gap: 270 }, { n: 2, gap: 90 }] },
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
function getStatusClass(status) {
  if (status === 'Completed') return 'bg-emerald-100 text-emerald-700'
  if (status === 'Overdue')   return 'bg-red-100 text-red-700'
  if (status === 'Scheduled') return 'bg-blue-100 text-blue-700'
  return 'bg-amber-100 text-amber-700'
}

const computedVaccineList = computed(() => {
  if (!selectedChild.value?.birthDate) return []
  const birth = new Date(selectedChild.value.birthDate)
  const result = []

  for (const vaccine of VACCINE_MASTER) {
    let prevActualDate   = null
    let prevOriginalDate = null

    for (const dose of vaccine.doses) {
      // Match by vaccineId now that VACCINE_MASTER's ids above are the real
      // backend Vaccines.VaccineID values, not frontend guesses. ID matching
      // is exact; matching on vaccineName (the previous approach) turned out
      // to be fragile — e.g. this page's hardcoded "Pentavalent (DPT-Hep
      // B-HIB)" vs. the backend's "Pentavalent (DPT-HepB-Hib)" differ by a
      // single space, which silently failed a string-equality match and
      // still produced a duplicate row.
      const record = completedRecords.value.find(r =>
        Number(r.vaccineID ?? r.vaccineId ?? r.VaccineID) === vaccine.vaccineId &&
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
      const wasLate = record ? administeredDate > originalDueDate : false
      const daysLate = wasLate
        ? Math.max(0, Math.round((administeredDate - originalDueDate) / 86400000))
        : 0

      prevOriginalDate = originalDueDate
      prevActualDate   = administeredDate ?? null

      result.push({
        doseId:          `${vaccine.vaccineId}-${dose.n}`,
        vaccineId:       vaccine.vaccineId,
        name:            vaccine.name,
        doseNumber:      dose.n,
        scheduledDate,
        originalDueDate,
        isCompleted:     !!record,
        administeredDate,
        wasLate,
        daysLate,
      })
    }
  }
  return result
})

const filteredRecords = computed(() => {
  if (recordFilter.value === 'All') return vaccinationHistory.value
  return vaccinationHistory.value.filter(r => r.status === recordFilter.value)
})

// ── Actions ────────────────────────────────────────────────────────────────
function handleLogout() {
  authLogout()
  router.push('/')
}

function handleSelectChild(child) {
  selectedChild.value = child
  localStorage.setItem('selectedParentChild', JSON.stringify(child))
  fetchRecords(child.childID)
}

async function fetchRecords(childId) {
  if (!childId) return
  recordsLoading.value = true
  try {
    const res = await api.get(`/VaccinationRecords/child/${childId}`)
    // Backend returns vaccinationDate/vaccinationRecordID — normalize to the
    // dateAdministered/recordId names the rest of this page already expects.
    completedRecords.value = res.data.map(r => ({
      ...r,
      recordId:         r.recordId ?? r.vaccinationRecordID ?? null,
      dateAdministered: r.dateAdministered ?? r.vaccinationDate ?? null,
    }))
    await new Promise(r => setTimeout(r, 0))

    const today = new Date()
    const upcomingRows = computedVaccineList.value
      .filter(v => !v.isCompleted)
      .map(v => ({
        vaccineID:          v.vaccineId,
        vaccineName:        v.name,
        doseNumber:         v.doseNumber,
        dateAdministered:   null,
        scheduledDate:      v.scheduledDate,
        status:             v.scheduledDate < today ? 'Overdue' : 'Scheduled',
        administeredByName: null,
        lotNumber:          null,
      }))

    vaccinationHistory.value = [...completedRecords.value, ...upcomingRows].sort((a, b) => {
      const dA = a.dateAdministered ? new Date(a.dateAdministered) : new Date(a.scheduledDate)
      const dB = b.dateAdministered ? new Date(b.dateAdministered) : new Date(b.scheduledDate)
      return dA - dB
    })

    recordStats.value = {
      completed: completedRecords.value.length,
      scheduled: upcomingRows.filter(r => r.status === 'Scheduled').length,
      overdue:   upcomingRows.filter(r => r.status === 'Overdue').length,
    }
  } catch (err) {
    console.error('fetchRecords error:', err)
    completedRecords.value   = []
    vaccinationHistory.value = []
    recordStats.value = { completed: 0, scheduled: 0, overdue: 0 }
  } finally {
    recordsLoading.value = false
  }
}

async function fetchUnreadCount() {
  if (!parentData.value?.parentID) return
  try {
    const res = await api.get(`/Notifications/parent/${parentData.value.parentID}`)
    unreadCount.value = res.data.filter(n => !n.isRead).length
  } catch {
    unreadCount.value = 0
  }
}

// ── Lifecycle ──────────────────────────────────────────────────────────────
onMounted(async () => {
  const savedAccount = getAccount()
  if (!savedAccount) { router.push('/'); return }

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
    return
  }

  try {
    const res = await api.get(`/Parents/dashboard/${parentData.value.parentID}`)
    children.value = res.data.map(child => {
      const relationshipType = child.relationshipType ?? child.RelationshipType
      const relatedParentName = child.parentFullName ?? child.ParentFullName

      return {
        childID: child.ChildID ?? child.childID,
        firstName: child.FirstName ?? child.firstName,
        middleName: child.MiddleName ?? child.middleName,
        lastName: child.LastName ?? child.lastName,
        birthDate: child.BirthDate ?? child.birthDate,
        placeOfBirth: child.PlaceOfBirth ?? child.placeOfBirth,
        sex: child.Sex ?? child.sex,
        barangay: child.Barangay ?? child.barangay,
        address: child.Address ?? child.address,
        healthCenter: child.HealthCenter ?? child.healthCenter,
        relationshipType,
        isPrimaryContact: child.IsPrimaryContact ?? child.isPrimaryContact,
        canReceiveNotifications: child.CanReceiveNotifications ?? child.canReceiveNotifications,

        // The dashboard endpoint doesn't return MotherName/FatherName/GuardianName
        // fields directly — it only returns relationshipType + parentFullName for
        // the currently logged-in parent, so derive which of the three this is.
        motherName: relationshipType === 'Mother' ? relatedParentName : undefined,
        fatherName: relationshipType === 'Father' ? relatedParentName : undefined,
        guardianName: relationshipType === 'Guardian' ? relatedParentName : undefined,
      }
    })
  } catch (err) {
    console.error('Error fetching parent dashboard:', err)
    children.value = []
  }

  const savedChildRaw = localStorage.getItem('selectedParentChild')
  if (savedChildRaw) {
    try {
      const savedChild = JSON.parse(savedChildRaw)
      selectedChild.value = children.value.find(c => c.childID === savedChild.childID) || children.value[0] || null
    } catch {
      selectedChild.value = children.value[0] || null
    }
  } else {
    selectedChild.value = children.value[0] || null
  }

  if (selectedChild.value) {
    localStorage.setItem('selectedParentChild', JSON.stringify(selectedChild.value))
    await fetchRecords(selectedChild.value.childID)
  }

  await fetchUnreadCount()
})
</script>