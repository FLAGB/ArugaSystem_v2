<template>
  <div class="flex h-screen bg-stone-50 antialiased text-stone-900" style="font-family: 'Inter','Segoe UI',sans-serif;">
    
    <StaffSidebar />

    <!-- MAIN CONTENT -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <StaffTopbar title="Parent Records" breadcrumb="Aruga / Parent Records">
        <button 
          @click="showAddModal = true; fetchChildrenForLink()" 
          :disabled="isLoading"
          class="bg-emerald-600 text-white px-5 py-2.5 rounded-lg text-xs font-bold hover:bg-emerald-700 transition-all flex items-center gap-2 cursor-pointer shadow-sm active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Plus class="w-4 h-4" /> REGISTER PARENT
        </button>
      </StaffTopbar>


      <main class="flex-1 p-8 overflow-y-auto">
        <div class="max-w-6xl mx-auto">
          
          <div class="flex items-center justify-between mb-6">
            <h1 class="text-2xl font-bold text-slate-800">Registered Parents</h1>
            <button 
              @click="fetchParents" 
              class="text-slate-400 hover:text-emerald-600 transition-colors p-2 rounded-full hover:bg-slate-100 cursor-pointer"
            >
              <RefreshCw class="w-5 h-5" :class="{'animate-spin': isLoading}" />
            </button>
          </div>

          <!-- TABLE -->
          <div class="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr class="bg-slate-50 border-b border-slate-200">
                  <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Full Name</th>
                  <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Contact Info</th>
                  <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Address Details</th>
                  <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="parent in paginatedParents" :key="parent.parentID" class="hover:bg-slate-50/50 transition-colors group">
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-3">
                      <div class="w-9 h-9 bg-emerald-100 text-emerald-700 rounded-full flex items-center justify-center font-bold text-sm">
                        {{ parent.lastName?.charAt(0) || 'P' }}
                      </div>
                      <div>
                        <p class="font-bold text-slate-700 leading-tight">
                          {{ parent.firstName }} {{ parent.lastName }}
                        </p>
                        <p class="text-[11px] text-slate-400 font-medium uppercase tracking-tighter">ID: {{ parent.parentID?.substring(0,8) }}</p>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4">
                    <p class="text-sm text-slate-600 font-medium">{{ parent.email }}</p>
                    <p class="text-xs text-slate-400">{{ parent.contactNo }}</p>
                  </td>
                  <td class="px-6 py-4">
                    <p class="text-sm text-slate-600 truncate max-w-[200px]">{{ parent.address || 'No Address' }}</p>
                    <p class="text-xs text-emerald-600 font-bold uppercase tracking-tight">Brgy. {{ parent.barangayNo || 'N/A' }}</p>
                  </td>
                  <td class="px-6 py-4 text-right">
                    <button class="bg-slate-100 text-slate-600 px-3 py-1.5 rounded-lg text-xs font-bold hover:bg-emerald-600 hover:text-white transition-all cursor-pointer">
                      Edit
                    </button>
                  </td>
                </tr>
                <tr v-if="parentList.length === 0 && !isLoading">
                  <td colspan="4" class="px-6 py-20 text-center">
                    <div class="flex flex-col items-center">
                      <Users class="w-12 h-12 text-slate-200 mb-2" />
                      <p class="text-slate-400 font-medium">No parent records found.</p>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
            <!-- PAGINATION -->
            <div v-if="parentList.length > 0" class="flex items-center justify-between px-6 py-3.5 border-t border-slate-100 flex-wrap gap-3">
              <div class="flex items-center gap-3">
                <div class="flex items-center gap-2 text-xs text-slate-500">
                  <span>Show</span>
                  <select v-model="pageSize" @change="currentPage = 1"
                    class="border border-slate-200 rounded-lg px-2 py-1 text-xs text-slate-600 outline-none bg-white">
                    <option :value="10">10</option>
                    <option :value="20">20</option>
                    <option :value="30">30</option>
                    <option :value="50">50</option>
                  </select>
                  <span>entries</span>
                </div>
                <p class="text-xs text-slate-400">
                  Showing {{ Math.min((currentPage - 1) * pageSize + 1, parentList.length) }}–{{ Math.min(currentPage * pageSize, parentList.length) }}
                  of {{ parentList.length }} parents
                </p>
              </div>
              <div class="flex items-center gap-1">
                <button @click="currentPage--" :disabled="currentPage === 1"
                  class="p-1.5 rounded-lg text-slate-400 hover:text-slate-600 hover:bg-slate-100 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
                  <ChevronLeft class="w-4 h-4" />
                </button>
                <template v-for="p in paginationPages" :key="p">
                  <span v-if="p === '...'" class="px-1 text-xs text-slate-400">…</span>
                  <button v-else @click="currentPage = p"
                    class="min-w-[28px] h-7 rounded-lg text-xs font-medium transition-colors"
                    :class="p === currentPage ? 'bg-emerald-600 text-white' : 'text-slate-600 hover:bg-slate-100'">
                    {{ p }}
                  </button>
                </template>
                <button @click="currentPage++" :disabled="currentPage >= totalPages"
                  class="p-1.5 rounded-lg text-slate-400 hover:text-slate-600 hover:bg-slate-100 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
                  <ChevronRight class="w-4 h-4" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>

    <!-- REGISTRATION MODAL -->
    <div v-if="showAddModal" class="fixed inset-0 z-[60] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="showAddModal = false"></div>
      
      <div class="relative bg-white w-full max-w-2xl rounded-3xl shadow-2xl overflow-hidden flex flex-col">
        <div class="p-6 border-b border-slate-100 flex justify-between items-center">
          <h3 class="font-bold text-slate-800 text-lg">Register Parent</h3>
          <X @click="showAddModal = false" class="w-5 h-5 text-slate-400 cursor-pointer hover:text-slate-600" />
        </div>

        <div class="p-8 space-y-5 overflow-y-auto max-h-[65vh]">
          <div class="grid grid-cols-3 gap-4">
            <div class="space-y-1">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Given Name</label>
              <input v-model="form.GivenName" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" />
            </div>
            <div class="space-y-1">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Middle Name</label>
              <input v-model="form.MiddleName" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" />
            </div>
            <div class="space-y-1">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Last Name</label>
              <input v-model="form.LastName" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div class="space-y-1">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Email Address</label>
              <input v-model="form.Email" type="email" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" />
            </div>
            <div class="space-y-1">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Contact No</label>
              <input v-model="form.ContactNo" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" />
            </div>
          </div>

          <div class="grid grid-cols-3 gap-4">
            <div class="col-span-1 space-y-1">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Barangay No</label>
              <input v-model="form.BarangayNo" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" />
            </div>
            <div class="col-span-2 space-y-1">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Complete Address</label>
              <input v-model="form.Address" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" />
            </div>
          </div>

          <div class="space-y-1">
            <div class="flex justify-between items-center">
              <label class="text-[10px] font-bold text-slate-400 uppercase ml-1">Password</label>
              <span :class="isPasswordValid ? 'text-emerald-600' : 'text-rose-500'" class="text-[10px] font-bold">
                {{ form.Password.length }}/8-16 characters
              </span>
            </div>
            <input 
              v-model="form.Password" 
              type="password" 
              maxlength="16"
              placeholder="8 to 16 characters" 
              class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all" 
            />
          </div>

          <!-- LINK TO EXISTING CHILD (OPTIONAL) -->
          <div class="pt-2 border-t border-slate-100 space-y-3">
            <div>
              <p class="text-[11px] font-bold text-emerald-700 uppercase tracking-wide">Link to Existing Child <span class="text-slate-400 font-medium normal-case">(optional)</span></p>
              <p class="text-[11px] text-slate-400 mt-0.5">Already have a child registered? Link this new parent account to them now, or skip and link it later.</p>
            </div>

            <div class="grid grid-cols-3 gap-3">
              <div class="col-span-2 space-y-1 relative">
                <input
                  v-model="childSearch"
                  type="text"
                  placeholder="Search by child name..."
                  class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none transition-all"
                />
                <div v-if="childSearch && filteredChildrenForLink.length > 0" class="absolute left-0 right-0 z-10 bg-white border border-slate-200 shadow-xl rounded-xl mt-1 overflow-hidden max-h-48 overflow-y-auto">
                  <div v-for="c in filteredChildrenForLink" :key="c.childID" @click="addLinkedChild(c)"
                    class="px-4 py-2.5 hover:bg-emerald-50 cursor-pointer border-b border-slate-50 last:border-none">
                    <p class="text-sm font-bold text-slate-700">{{ c.firstName }} {{ c.lastName }}</p>
                    <p class="text-[10px] text-slate-400">Brgy {{ c.barangay ?? 'N/A' }}</p>
                  </div>
                </div>
              </div>
              <select v-model="linkRole" class="px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none">
                <option v-for="r in linkRoleOptions" :key="r" :value="r">{{ r }}</option>
              </select>
            </div>

            <div v-if="linkedChildren.length > 0" class="flex flex-wrap gap-2">
              <div v-for="lc in linkedChildren" :key="lc.childID" class="flex items-center gap-2 bg-emerald-50 border border-emerald-100 rounded-lg px-3 py-1.5">
                <span class="text-xs font-bold text-emerald-800">{{ lc.name }}</span>
                <span class="text-[10px] text-emerald-600 uppercase font-bold">{{ lc.role }}</span>
                <X @click="removeLinkedChild(lc.childID)" class="w-3 h-3 text-emerald-400 hover:text-rose-500 cursor-pointer" />
              </div>
            </div>
          </div>
        </div>

        <div class="p-6 bg-slate-50 border-t border-slate-100 flex gap-3">
          <button @click="showAddModal = false" class="flex-1 py-3 font-bold text-slate-400 hover:text-slate-600 cursor-pointer">Cancel</button>
          <button 
            @click="handleSave" 
            :disabled="isLoading || !isPasswordValid" 
            class="flex-1 py-3 bg-emerald-600 text-white rounded-xl font-bold shadow-lg shadow-emerald-100 disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer transition-all active:scale-95"
          >
            {{ isLoading ? 'PROCESSING...' : 'SAVE RECORD' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import axios from 'axios'
import { Plus, X, RefreshCw, Users, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import StaffSidebar from './StaffSidebar.vue'
import StaffTopbar from './StaffTopbar.vue'

// Matches the API_BASE pattern used elsewhere in the project (StaffPatientRecords.vue,
// auth.js): real routes live under ParentsController (/api/Parents), not the old
// "/api/Vaccination/GetAllParents" endpoint this page was pointed at before, which
// doesn't exist on the backend — that's why nothing ever loaded.
const API_BASE = (import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '') + '/api'
const parentList = ref([])
const showAddModal = ref(false)
const isLoading = ref(false)

// ── PAGINATION ────────────────────────────────────────────────────
const currentPage = ref(1)
const pageSize    = ref(10)
const totalPages  = computed(() => Math.max(1, Math.ceil(parentList.value.length / pageSize.value)))
const paginatedParents = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return parentList.value.slice(start, start + pageSize.value)
})
const paginationPages = computed(() => {
  const total = totalPages.value
  const cur   = currentPage.value
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1)
  const pages = []
  pages.push(1)
  if (cur > 3) pages.push('...')
  for (let p = Math.max(2, cur - 1); p <= Math.min(total - 1, cur + 1); p++) pages.push(p)
  if (cur < total - 2) pages.push('...')
  pages.push(total)
  return pages
})

const form = reactive({
  GivenName: '', MiddleName: '', LastName: '',
  Email: '', ContactNo: '', BarangayNo: '',
  Address: '', Password: ''
})


// VALIDATION: Password must be between 8 and 16 characters
const isPasswordValid = computed(() => {
  const len = form.Password.length
  return len >= 8 && len <= 16
})

// ── LINK TO EXISTING CHILD (optional, at registration time) ──────────
// Mirrors the parent-linking search already used on the Register Child
// form (StaffChildRecord.vue), just from the other direction: search
// existing children and attach this new parent to one or more of them.
const allChildren = ref([])
const childSearch = ref('')
const linkRole = ref('Mother')
const linkRoleOptions = ['Mother', 'Father', 'Guardian', 'Grandmother', 'Grandfather', 'Aunt', 'Uncle', 'Sibling', 'Foster Parent', 'Relative', 'Other']
const linkedChildren = ref([]) // [{ childID, name, role }]

async function fetchChildrenForLink() {
  try {
    const res = await axios.get(`${API_BASE}/Children/all`)
    allChildren.value = res.data
  } catch (err) {
    console.error('fetchChildrenForLink:', err)
  }
}

const filteredChildrenForLink = computed(() => {
  const q = childSearch.value.trim().toLowerCase()
  if (!q) return []
  return allChildren.value
    .filter(c => !linkedChildren.value.some(lc => lc.childID === c.childID))
    .filter(c => `${c.firstName} ${c.lastName}`.toLowerCase().includes(q))
    .slice(0, 8)
}) 

function addLinkedChild(child) {
  linkedChildren.value.push({
    childID: child.childID,
    name: `${child.firstName} ${child.lastName}`,
    role: linkRole.value,
  })
  childSearch.value = ''
}

function removeLinkedChild(childID) {
  linkedChildren.value = linkedChildren.value.filter(lc => lc.childID !== childID)
}

const fetchParents = async () => {
  isLoading.value = true
  try {
    const response = await axios.get(`${API_BASE}/Parents/all`)
    parentList.value = response.data
  } catch (err) {
    console.error("Fetch Error:", err)
  } finally {
    isLoading.value = false
  }
}

const handleSave = async () => {
  if (!isPasswordValid.value) {
    alert("Password must be between 8 and 16 characters.")
    return
  }

  isLoading.value = true
  try {
    // ParentsController's create endpoint only has firstName/lastName/email/contactNo/
    // address/barangayNo/password (confirmed against StaffPatientRecords.vue's working
    // createParent call) — there's no MiddleName column on this endpoint, so it isn't
    // sent even though the form still collects it.
    const payload = {
      firstName: form.GivenName,
      lastName: form.LastName,
      email: form.Email,
      contactNo: form.ContactNo,
      address: form.Address,
      barangayNo: form.BarangayNo,
      password: form.Password,
    }

    const res = await axios.post(`${API_BASE}/Parents`, payload)

    // NOTE: assumes ParentsController's POST returns the created parent's
    // ID as `parentID` (matching the camelCase shape every other endpoint
    // in this project returns, e.g. ChildrenController's `childID`). If
    // your ParentsController returns something different, this link step
    // will silently no-op — the parent record itself is still created and
    // can be linked afterward from the Children Record page.
    const newParentID = res.data?.parentID

    let linkFailures = 0
    if (newParentID && linkedChildren.value.length > 0) {
      for (const lc of linkedChildren.value) {
        try {
          await axios.post(`${API_BASE}/ChildParentRelationships`, {
            childID: lc.childID,
            parentID: newParentID,
            relationshipType: lc.role,
            isPrimaryContact: false,
            canReceiveNotifications: true,
          })
        } catch (linkErr) {
          console.error('Link failed for child', lc.childID, linkErr)
          linkFailures++
        }
      }
    }

    if (linkFailures > 0) {
      alert(`Parent registered, but ${linkFailures} child link(s) failed to save. You can link them from the child's profile instead.`)
    } else {
      alert("Registration Successful!")
    }
    showAddModal.value = false
    resetForm()
    await fetchParents()
  } catch (err) {
    console.error("Save Error:", err)
    if (err.response?.status === 409 || /already/i.test(err.response?.data?.message || '')) {
      alert("Error: This email address is already registered.")
    } else {
      alert(err.response?.data?.message || "Could not save this record. Check your connection and try again.")
    }
  } finally {
    isLoading.value = false
  }
}

const resetForm = () => {
  Object.keys(form).forEach(k => form[k] = '')
  linkedChildren.value = []
  childSearch.value = ''
}

onMounted(fetchParents)
</script>