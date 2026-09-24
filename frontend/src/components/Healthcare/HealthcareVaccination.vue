<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar @logout="logout" />

    <div class="flex flex-col flex-1 overflow-hidden">
      <HealthcareHeader :worker="worker" title="Record Vaccination" />

      <main class="flex-1 overflow-y-auto p-6">

        <!-- BACK -->
        <button
          @click="router.push('/healthcare/queue')"
          class="flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-700 mb-4 transition-colors"
        >
          <ArrowLeft class="w-4 h-4" /> Back to Queue
        </button>

        <!-- LOAD ERROR (missing/invalid child, child fetch failed) -->
        <div v-if="loadError" class="mb-4 flex items-start gap-2 p-4 bg-red-50 border border-red-100 rounded-xl text-sm text-red-700">
          <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
          <span>{{ loadError }}</span>
        </div>

        <!-- LOADING CHILD -->
        <div v-if="loadingChild" class="flex items-center justify-center py-20">
          <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
          <span class="ml-2 text-sm text-slate-400">Loading patient...</span>
        </div>

        <!-- FORM -->
        <div v-else-if="child" class="max-w-xl">
          <div class="bg-white rounded-2xl border border-slate-200 overflow-hidden">

            <div class="bg-emerald-600 px-6 py-5 flex items-center justify-between">
              <div>
                <p class="text-white font-bold leading-tight">Record Vaccination</p>
                <p class="text-emerald-200 text-xs mt-0.5">{{ child.name }} · Queue #{{ queueNumber || queueId }}</p>
              </div>
            </div>

            <div class="p-6 space-y-4">

              <!-- Patient summary -->
              <div class="bg-slate-50 rounded-xl p-4 flex items-center gap-3">
                <div class="w-10 h-10 rounded-full flex items-center justify-center text-sm font-bold text-white"
                  :style="{ backgroundColor: avatarColor(child.name) }">
                  {{ initials(child.name) }}
                </div>
                <div>
                  <p class="font-semibold text-slate-800">{{ child.name }}</p>
                  <p class="text-xs text-slate-500">
                    {{ child.ageLabel || '—' }} · {{ child.sex || '—' }}
                    <span v-if="child.parentName"> · Parent: {{ child.parentName }}</span>
                  </p>
                </div>
              </div>

              <!-- VACCINE selector -->
              <div>
                <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                  Vaccine <span class="text-red-400">*</span>
                </label>
                <div v-if="loadingVaccines" class="text-xs text-slate-400 py-2">Loading vaccines...</div>
                <select v-else v-model="vaccinateForm.vaccineId"
                  @change="vaccinateForm.doseNumber = ''; fetchInventoryFor(vaccinateForm.vaccineId)"
                  class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500">
                  <option value="">Select a vaccine...</option>
                  <option v-for="v in vaccineList" :key="v.vaccineID" :value="v.vaccineID">
                    {{ v.vaccineName }}
                  </option>
                </select>
              </div>

              <!-- DOSE NUMBER selector -->
              <div>
                <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                  Dose Number <span class="text-red-400">*</span>
                </label>
                <select v-model="vaccinateForm.doseNumber"
                  :disabled="!vaccinateForm.vaccineId"
                  class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 disabled:opacity-50 disabled:cursor-not-allowed">
                  <option value="">Select dose...</option>
                  <option v-for="d in selectedVaccineDoses" :key="d.doseNumber" :value="d.doseNumber">
                    Dose {{ d.doseNumber }}
                  </option>
                </select>
              </div>

              <div>
                <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                  Date Administered <span class="text-red-400">*</span>
                </label>
                <input type="date" v-model="vaccinateForm.dateAdministered"
                  class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500" />
              </div>

              <div>
                <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                  Vaccine Batch <span class="text-red-400">*</span>
                </label>
                <select v-model="vaccinateForm.inventoryId"
                  class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500">
                  <option value="" disabled>{{ loadingInventory ? 'Loading batches...' : 'Select a batch...' }}</option>
                  <option v-for="inv in inventoryOptions" :key="inv.inventoryId" :value="inv.inventoryId">
                    {{ inv.lotNumber }} — exp {{ new Date(inv.expirationDate).toLocaleDateString() }} ({{ inv.currentQuantity }} left)
                  </option>
                </select>
                <p v-if="!loadingInventory && !vaccinateForm.vaccineId" class="text-xs text-slate-400 mt-1">Select a vaccine first.</p>
                <p v-else-if="!loadingInventory && vaccinateForm.vaccineId && inventoryOptions.length === 0" class="text-xs text-red-500 mt-1">No usable stock for this vaccine — check inventory.</p>
              </div>

              <div>
                <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                  Remarks / Adverse Reactions
                </label>
                <textarea v-model="vaccinateForm.remarks" rows="2"
                  placeholder="e.g. No adverse reaction"
                  class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 resize-none">
                </textarea>
              </div>

              <!-- Accountability notice -->
              <div class="flex items-start gap-2 p-3 bg-amber-50 rounded-lg border border-amber-100">
                <Info class="w-4 h-4 text-amber-500 shrink-0 mt-0.5" />
                <p class="text-xs text-amber-700">
                  This will be permanently saved under <strong>{{ worker.fullName }}</strong>
                  and visible to the parent.
                </p>
              </div>

              <!-- SUBMIT ERROR -->
              <div v-if="submitError" class="flex items-start gap-2 p-3 bg-red-50 rounded-lg border border-red-100">
                <AlertCircle class="w-4 h-4 text-red-500 shrink-0 mt-0.5" />
                <p class="text-xs text-red-700">{{ submitError }}</p>
              </div>
            </div>

            <div class="px-6 pb-6 flex gap-3">
              <button @click="router.push('/healthcare/queue')"
                class="flex-1 px-4 py-2.5 text-sm font-medium text-slate-600 border border-slate-200 rounded-lg hover:bg-slate-50 transition-colors">
                Cancel
              </button>
              <button @click="submitVaccination"
                :disabled="submitting || !vaccinateForm.vaccineId || !vaccinateForm.doseNumber || !vaccinateForm.dateAdministered || !vaccinateForm.inventoryId"
                class="flex-1 px-4 py-2.5 text-sm font-medium text-white bg-emerald-600 rounded-lg hover:bg-emerald-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2">
                <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
                <CheckCircle v-else class="w-4 h-4" />
                {{ submitting ? 'Saving...' : 'Confirm & Save' }}
              </button>
            </div>
          </div>
        </div>

      </main>
    </div>

    <!-- TOAST -->
    <Transition name="fade">
      <div v-if="toast.show"
        class="fixed bottom-6 right-6 bg-slate-800 text-white text-sm px-4 py-3 rounded-lg shadow-lg z-50">
        {{ toast.message }}
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { getUser, getToken, logout as clearSession } from '@/utils/auth'
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ArrowLeft, AlertCircle, Loader2, CheckCircle, Info } from 'lucide-vue-next'
import HealthcareSidebar from '@/components/Healthcare/Components/HealtcareSidebar.vue'
import HealthcareHeader from '@/components/Healthcare/Components/HealthcareHeader.vue'

const router = useRouter()
const route  = useRoute()

// VITE_API_URL is the bare host (no /api) per the project's existing
// convention — /api is appended here, not stored in the env var.
const API_BASE = `${(import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '')}/api`

function authHeaders(json = false) {
  const h = { Authorization: `Bearer ${getToken()}` }
  if (json) h['Content-Type'] = 'application/json'
  return h
}

// ─────────────────────────────────────────────────────────────
// AUTH — same pattern as HealthcareQueue.vue / HealthcarePatient.vue
// ─────────────────────────────────────────────────────────────
const SKIP_AUTH = false
const worker = ref({ userId: '', fullName: '', userType: '' })

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
  }

  if (!queueId.value || !childId.value) {
    loadError.value = 'Missing queue or patient reference — go back to the Queue and start vaccinating from there.'
    loadingChild.value = false
    return
  }

  fetchChild()
  fetchVaccines()
})

function logout() {
  clearSession()
  router.push('/')
}

// ─────────────────────────────────────────────────────────────
// ROUTE PARAMS — /healthcare/vaccination/:queueId?child=<childID>
// ─────────────────────────────────────────────────────────────
const queueId     = computed(() => route.params.queueId)
const childId     = computed(() => route.query.child)
const queueNumber = ref('')

// ─────────────────────────────────────────────────────────────
// CHILD — GET /api/Children/{childID}, confirmed real endpoint
// (parentName isn't on this payload yet — GET /api/Children/all
// still needs the parentName join fixed; shown only if present)
// ─────────────────────────────────────────────────────────────
const child        = ref(null)
const loadingChild  = ref(true)
const loadError     = ref('')

function ageLabelFromDOB(dobStr) {
  const dob = new Date(dobStr)
  if (isNaN(dob)) return null
  const now = new Date()
  let months = (now.getFullYear() - dob.getFullYear()) * 12 + (now.getMonth() - dob.getMonth())
  if (now.getDate() < dob.getDate()) months--
  if (months < 0) return null
  if (months < 24) return `${months} month${months === 1 ? '' : 's'}`
  const years = Math.floor(months / 12)
  return `${years} yr${years === 1 ? '' : 's'}`
}

async function fetchChild() {
  loadingChild.value = true
  loadError.value = ''
  try {
    const res = await fetch(`${API_BASE}/Children/${childId.value}`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`Children request failed (${res.status})`)
    const c = await res.json()
    const dob = c.birthDate ?? c.BirthDate ?? null
    child.value = {
      childID:    c.childID ?? c.ChildID,
      name:       `${c.firstName ?? c.FirstName ?? ''} ${c.lastName ?? c.LastName ?? ''}`.trim(),
      sex:        c.sex ?? c.Sex ?? null,
      dateOfBirth: dob,
      ageLabel:   dob ? ageLabelFromDOB(dob) : null,
      parentName: c.parentName ?? c.ParentName ?? null,
    }
  } catch (e) {
    console.error('fetchChild:', e)
    loadError.value = 'Could not load this patient. Go back to the Queue and try again.'
  } finally {
    loadingChild.value = false
  }
}

// ─────────────────────────────────────────────────────────────
// VACCINE LIST — same as DoctorPatients.vue's Record Vaccination modal
// vaccineList shape: [{ vaccineID, vaccineName, doses: [{ doseNumber, minIntervalDays }] }]
// ─────────────────────────────────────────────────────────────
const vaccineList     = ref([])
const loadingVaccines = ref(false)

async function fetchVaccines() {
  loadingVaccines.value = true
  try {
    const res = await fetch(`${API_BASE}/Vaccines/with-doses`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`Vaccines request failed (${res.status})`)
    vaccineList.value = await res.json()
  } catch (err) {
    console.error('fetchVaccines error:', err)
    vaccineList.value = []
  } finally {
    loadingVaccines.value = false
  }
}

const selectedVaccineDoses = computed(() => {
  if (!vaccinateForm.value.vaccineId) return []
  const match = vaccineList.value.find(v => v.vaccineID === vaccinateForm.value.vaccineId)
  return match ? match.doses : []
})

// ─────────────────────────────────────────────────────────────
// FORM
// ─────────────────────────────────────────────────────────────
const submitting  = ref(false)
const submitError = ref('')
const vaccinateForm = ref({
  vaccineId:        '',
  doseNumber:       '',
  dateAdministered: new Date().toISOString().split('T')[0],
  inventoryId:      '',
  remarks:          '',
})
const inventoryOptions = ref([])
const loadingInventory = ref(false)

// Fetches available batches for a vaccine, FIFO-sorted by expiry,
// filters out inactive/expired/out-of-stock ones, and auto-selects
// the earliest-expiring valid batch. Mirrors DoctorPatients.vue.
async function fetchInventoryFor(vaccineId) {
  vaccinateForm.value.inventoryId = ''
  inventoryOptions.value = []
  if (!vaccineId) return
  loadingInventory.value = true
  try {
    const res = await fetch(`${API_BASE}/VaccineInventory/vaccine/${vaccineId}`, { headers: authHeaders() })
    if (!res.ok) throw new Error(`VaccineInventory request failed (${res.status})`)
    const data = await res.json()
    const today = new Date().setHours(0, 0, 0, 0)
    inventoryOptions.value = data
      .filter(i => (i.status ?? i.Status) && (i.currentQuantity ?? i.CurrentQuantity) > 0
        && new Date(i.expirationDate ?? i.ExpirationDate).setHours(0, 0, 0, 0) >= today)
      .sort((a, b) => new Date(a.expirationDate ?? a.ExpirationDate) - new Date(b.expirationDate ?? b.ExpirationDate))
      .map(i => ({
        inventoryId: i.inventoryID ?? i.InventoryID,
        lotNumber: i.lotNumber ?? i.LotNumber,
        expirationDate: i.expirationDate ?? i.ExpirationDate,
        currentQuantity: i.currentQuantity ?? i.CurrentQuantity,
      }))
    if (inventoryOptions.value.length) vaccinateForm.value.inventoryId = inventoryOptions.value[0].inventoryId
  } catch (e) {
    console.error('fetchInventoryFor:', e)
  } finally {
    loadingInventory.value = false
  }
}

// ─────────────────────────────────────────────────────────────
// SUBMIT — POST /api/VaccinationRecords, then close out the queue
// entry. Uses PATCH /api/Queue/{queueId}/complete (confirmed in
// QueueController.cs) rather than PUT /{id}/status: /complete also
// auto-advances the next Waiting visit to InProgress, matching what
// the Doctor dashboard does. Note a Queues row is one PARENT VISIT,
// so completing it closes out every child in that visit together.
// ─────────────────────────────────────────────────────────────
async function submitVaccination() {
  const f = vaccinateForm.value
  if (!f.vaccineId || !f.doseNumber || !f.dateAdministered || !f.inventoryId) return

  submitting.value = true
  submitError.value = ''
  try {
    const res = await fetch(`${API_BASE}/VaccinationRecords`, {
      method: 'POST',
      headers: authHeaders(true),
      body: JSON.stringify({
        childID:              child.value.childID,
        vaccineID:            Number(f.vaccineId),
        doseNumber:           Number(f.doseNumber),
        vaccinationDate:      f.dateAdministered,
        inventoryID:          Number(f.inventoryId),
        administeredByUserID: worker.value.userId,
        nurseObservation:     f.remarks || null,
      }),
    })
    if (!res.ok) throw new Error(`VaccinationRecords POST failed (${res.status})`)

    try {
      await fetch(`${API_BASE}/Queue/${queueId.value}/complete`, {
        method: 'PATCH',
        headers: authHeaders(true),
      })
    } catch (e) {
      // Non-fatal — the vaccination record itself was already saved.
      console.error('Queue status update to Completed failed:', e)
    }

    showToast(`Vaccination recorded for ${child.value.name}`)
    setTimeout(() => router.push('/healthcare/queue'), 900)
  } catch (err) {
    console.error('submitVaccination error:', err)
    submitError.value = 'Failed to save. Please try again.'
  } finally {
    submitting.value = false
  }
}

// ─────────────────────────────────────────────────────────────
// TOAST
// ─────────────────────────────────────────────────────────────
const toast = ref({ show: false, message: '' })
function showToast(msg) {
  toast.value = { show: true, message: msg }
  setTimeout(() => toast.value.show = false, 3500)
}

// ─────────────────────────────────────────────────────────────
// HELPERS
// ─────────────────────────────────────────────────────────────
const AVATAR_COLORS = ['#4a7c59', '#6b7c45', '#8b5e3c', '#4a6fa5', '#7b4f8e', '#5a7a6b', '#8b6914']
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
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>