<template>
  <div class="flex h-screen bg-stone-50 antialiased text-stone-900" style="font-family: 'Inter','Segoe UI',sans-serif;">
    
    <StaffSidebar />

    <!-- MAIN CONTENT -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <StaffTopbar title="Children Records" breadcrumb="Aruga / Children Record">
        <button @click="openAddModal" :disabled="isLoading" class="bg-emerald-600 text-white px-5 py-2.5 rounded-lg text-xs font-bold hover:bg-emerald-700 transition-all flex items-center gap-2 shadow-sm active:scale-95 disabled:opacity-50">
          <Plus class="w-4 h-4" /> REGISTER CHILD
        </button>
      </StaffTopbar>

      <main class="flex-1 p-8 overflow-y-auto custom-scrollbar">
        <div class="max-w-6xl mx-auto">
          <div class="flex items-center justify-between mb-6">
            <h1 class="text-2xl font-bold text-slate-800">Children Records</h1>
            <button @click="fetchChildren" class="text-slate-400 hover:text-emerald-600 transition-colors p-2 rounded-full hover:bg-slate-100">
              <RefreshCw class="w-5 h-5" :class="{'animate-spin': isLoading}" />
            </button>
          </div>

          <!-- TABLE -->
          <div class="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr class="bg-slate-50 border-b border-slate-200">
                  <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Full Name</th>
                  <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Parent / Brgy</th>
                  <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest text-center">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="child in paginatedChildren" :key="child.childID" class="hover:bg-slate-50/50 transition-colors">
                  <td class="px-6 py-4">
                    <p class="font-bold text-slate-700">{{ child.firstName }} {{ child.lastName }}</p>
                    <p class="text-[11px] text-slate-400">#{{ child.childID?.substring(0,8) }}</p>
                  </td>
                  <td class="px-6 py-4">
                    <p class="text-sm text-slate-600">{{ child.parentName || 'No primary contact' }}</p>
                    <p class="text-[10px] text-emerald-600 font-bold uppercase">Brgy {{ child.barangay ?? '—' }}</p>
                  </td>
                  <td class="px-6 py-4 text-center">
                    <button @click="viewProfile(child)" class="bg-white border border-slate-200 text-slate-600 px-4 py-2 rounded-xl text-xs font-bold hover:bg-emerald-600 hover:text-white transition-all shadow-sm">
                      VIEW PROFILE
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
            <!-- PAGINATION -->
            <div v-if="childrenList.length > 0" class="flex items-center justify-between px-6 py-3.5 border-t border-slate-100 flex-wrap gap-3">
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
                  Showing {{ Math.min((currentPage - 1) * pageSize + 1, childrenList.length) }}–{{ Math.min(currentPage * pageSize, childrenList.length) }}
                  of {{ childrenList.length }} children
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
      
      <div class="relative bg-white w-full max-w-4xl rounded-3xl shadow-2xl overflow-hidden flex flex-col max-h-[95vh]">
        <!-- Header -->
        <div class="p-6 border-b border-slate-100 flex justify-between items-center bg-white sticky top-0 z-10">
          <div>
            <h3 class="font-bold text-slate-800 text-lg">Register New Child</h3>
            <p class="text-[11px] text-slate-400 uppercase font-bold tracking-wider">Health Intake Form</p>
          </div>
          <X @click="showAddModal = false" class="w-5 h-5 text-slate-400 cursor-pointer hover:text-slate-600" />
        </div>

        <!-- Scrollable Form Content -->
        <div class="p-8 space-y-10 overflow-y-auto custom-scrollbar">
          
          <!-- STEP 1: PARENT INFORMATION (LINK OR MANUAL) -->
          <div class="p-6 bg-emerald-50 rounded-2xl border border-emerald-100 space-y-4">
            <div class="flex justify-between items-end">
                <label class="text-[10px] font-bold text-emerald-700 uppercase tracking-widest">Step 1: Parent/Guardian Information</label>
                <span class="text-[9px] text-emerald-600 bg-white px-2 py-1 rounded border border-emerald-100">Search to link existing account</span>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-emerald-800 ml-1">Assign To Role</label>
                <select v-model="selectedRole" class="w-full px-4 py-2.5 bg-white border border-emerald-200 rounded-xl text-sm outline-none">
                  <option value="Mother">Mother</option>
                  <option value="Father">Father</option>
                  <option value="Guardian">Guardian</option>
                </select>
              </div>
              <div class="space-y-1 relative">
                <label class="text-[11px] font-bold text-emerald-800 ml-1">Type Name or Search Email</label>
                <input 
                    v-model="parentSearch" 
                    @input="handleManualNameEntry"
                    type="text" 
                    placeholder="Type name manually or search account..." 
                    class="w-full px-4 py-2.5 bg-white border border-emerald-200 rounded-xl text-sm outline-none" 
                />
                
                <div v-if="filteredParents.length > 0" class="absolute left-0 right-0 z-[70] bg-white border border-slate-200 shadow-xl rounded-xl mt-1 overflow-hidden">
                  <div v-for="p in filteredParents" :key="p.email" @click="linkExistingAccount(p)" class="px-4 py-3 hover:bg-emerald-50 cursor-pointer border-b border-slate-50 last:border-none">
                    <p class="text-sm font-bold text-slate-700">{{ p.lastName }}, {{ p.firstName }}</p>
                    <p class="text-[10px] text-emerald-600 font-bold">LINK ACCOUNT: {{ p.email }}</p>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Selection Status Preview -->
            <div class="grid grid-cols-3 gap-3 pt-2">
                <div v-for="role in ['Mother', 'Father', 'Guardian']" :key="role" 
                     class="p-3 rounded-xl border transition-all"
                     :class="childForm[role+'Name'] ? 'bg-white border-emerald-200 shadow-sm' : 'bg-emerald-50/50 border-dashed border-emerald-200'">
                    <div class="flex justify-between items-start mb-1">
                        <span class="text-[9px] font-black uppercase text-emerald-800">{{ role }}</span>
                        <button v-if="childForm[role+'Name']" @click="clearParentRole(role)" class="text-emerald-300 hover:text-red-400">
                            <X class="w-3 h-3"/>
                        </button>
                    </div>
                    <p class="text-xs font-bold text-slate-700 truncate">
                        {{ childForm[role+'Name'] || 'Empty' }}
                    </p>
                    <p v-if="childForm[role+'Email']" class="text-[8px] text-emerald-500 font-bold uppercase mt-1">
                        Linked: {{ childForm[role+'Email'] }}
                    </p>
                    <p v-else-if="childForm[role+'Name']" class="text-[8px] text-slate-400 font-medium uppercase mt-1">
                        Manual Entry
                    </p>
                </div>
            </div>
          </div>

          <!-- STEP 2: CHILD IDENTITY -->
          <div class="space-y-4">
            <label class="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Step 2: Child Identity</label>
            <div class="grid grid-cols-3 gap-4">
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">First Name</label>
                <input v-model="childForm.FirstName" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none focus:border-emerald-500" />
              </div>
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Middle Name</label>
                <input v-model="childForm.MiddleName" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none focus:border-emerald-500" />
              </div>
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Last Name</label>
                <input v-model="childForm.LastName" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none focus:border-emerald-500" />
              </div>
            </div>

            <div class="grid grid-cols-3 gap-4">
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Birth Date</label>
                <input v-model="childForm.BirthDate" type="date" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
              </div>
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Sex</label>
                <select v-model="childForm.Sex" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none">
                  <option value="Male">Male</option>
                  <option value="Female">Female</option>
                </select>
              </div>
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Place of Birth</label>
                <input v-model="childForm.PlaceOfBirth" type="text" placeholder="City/Hospital" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
              </div>
            </div>
          </div>

          <!-- STEP 3: PHYSICAL METRICS & ADDRESS -->
          <div class="space-y-4">
            <label class="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Step 3: Medical & Location</label>
            <div class="grid grid-cols-4 gap-4">
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Birth Weight (kg)</label>
                <input v-model="childForm.BirthWeight" type="number" step="0.01" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
              </div>
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Birth Height (cm)</label>
                <input v-model="childForm.BirthHeight" type="number" step="0.1" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
              </div>
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Barangay</label>
                <input v-model="childForm.Barangay" type="text" placeholder="e.g. 704" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
              </div>
              <div class="space-y-1">
                <label class="text-[11px] font-bold text-slate-500 ml-1">Family No.</label>
                <input v-model="childForm.FamilyNo" type="text" class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
              </div>
            </div>
            <div class="space-y-1">
              <label class="text-[11px] font-bold text-slate-500 ml-1">Full Home Address</label>
              <input v-model="childForm.Address" type="text" placeholder="Street Name, Building, etc." class="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
            </div>
          </div>
        </div>

        <!-- Footer Actions -->
        <div class="p-6 bg-slate-50 border-t border-slate-100 flex gap-3 sticky bottom-0 z-10">
          <button @click="showAddModal = false" class="flex-1 py-3 font-bold text-slate-400 hover:text-slate-600 transition-colors">Cancel</button>
          <button @click="handleSave" :disabled="isLoading" class="flex-1 py-3 bg-emerald-600 text-white rounded-xl font-bold shadow-lg hover:bg-emerald-700 active:scale-95 transition-all flex items-center justify-center">
            <span v-if="isLoading" class="animate-spin mr-2"><RefreshCw class="w-4 h-4" /></span>
            {{ isLoading ? 'SAVING...' : 'REGISTER CHILD' }}
          </button>
        </div>  
      </div>
    </div>

    <!-- VIEW / EDIT PROFILE MODAL -->
    <div v-if="showProfileModal" class="fixed inset-0 z-[60] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="showProfileModal = false"></div>
      
      <div class="relative bg-white w-full max-w-4xl rounded-3xl shadow-2xl overflow-hidden flex flex-col max-h-[95vh]">
        <div class="p-6 border-b border-slate-100 flex justify-between items-center bg-white sticky top-0 z-10">
          <div>
            <h3 class="font-bold text-slate-800 text-lg">{{ isEditing ? 'Edit Profile' : 'Child Profile' }}</h3>
            <p class="text-[11px] text-slate-400 uppercase font-bold tracking-wider">
              {{ isEditing ? 'Modify health records' : 'Information Overview' }}
            </p>
          </div>
          <div class="flex items-center gap-3">
            <button @click="isEditing = !isEditing" class="text-xs font-bold px-3 py-1.5 rounded-lg border border-slate-200 hover:bg-slate-50 transition-colors">
              {{ isEditing ? 'CANCEL EDIT' : 'EDIT PROFILE' }}
            </button>
            <X @click="showProfileModal = false" class="w-5 h-5 text-slate-400 cursor-pointer hover:text-slate-600" />
          </div>
        </div>

        <div class="p-8 space-y-6 overflow-y-auto custom-scrollbar">
          <!-- Identity Section -->
          <div class="grid grid-cols-3 gap-6">
            <div v-for="field in ['firstName', 'middleName', 'lastName']" :key="field" class="space-y-1">
              <label class="text-[11px] font-bold text-slate-400 uppercase">{{ field.replace(/([A-Z])/g, ' $1') }}</label>
              <input v-if="isEditing" v-model="selectedChild[field]" class="w-full px-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none focus:border-emerald-500" />
              <p v-else class="text-sm font-bold text-slate-700">{{ selectedChild[field] || '---' }}</p>
            </div>
          </div>

          <div class="grid grid-cols-3 gap-6">
            <div class="space-y-1">
              <label class="text-[11px] font-bold text-slate-400 uppercase">Birth Date</label>
              <input v-if="isEditing" type="date" v-model="formattedBirthDate" class="w-full px-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none" />
              <p v-else class="text-sm font-bold text-slate-700">{{ selectedChild.birthDate ? new Date(selectedChild.birthDate).toLocaleDateString() : '---' }}</p>
            </div>
            <div class="space-y-1">
              <label class="text-[11px] font-bold text-slate-400 uppercase">Sex</label>
              <select v-if="isEditing" v-model="selectedChild.sex" class="w-full px-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm outline-none">
                <option value="Male">Male</option>
                <option value="Female">Female</option>
              </select>
              <p v-else class="text-sm font-bold text-slate-700">{{ selectedChild.sex }}</p>
            </div>
            <div class="space-y-1">
              <label class="text-[11px] font-bold text-slate-400 uppercase">Barangay</label>
              <input v-if="isEditing" v-model="selectedChild.barangay" type="number" class="w-full px-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm" />
              <p v-else class="text-sm font-bold text-slate-700">{{ selectedChild.barangay ?? 'N/A' }}</p>
            </div>
          </div>

          <hr class="border-slate-100" />

          <!-- Linked Parents/Guardians — read from the child's real `parents` relationship
               array (relationshipType/parentName/isPrimaryContact), not editable here.
               Adding/removing a link is a separate action against
               ChildParentRelationshipsController, not part of this simple edit form. -->
          <div class="grid grid-cols-3 gap-6">
            <div v-for="role in ['Mother', 'Father', 'Guardian']" :key="role" class="space-y-1">
              <label class="text-[11px] font-bold text-slate-400 uppercase">{{ role }}</label>
              <p class="text-sm font-bold text-slate-700">{{ parentForRole(role)?.parentName || 'Not Linked' }}</p>
              <p v-if="parentForRole(role)?.isPrimaryContact" class="text-[9px] text-emerald-600 font-bold uppercase">Primary Contact</p>
            </div>
          </div>
        </div>

        <div v-if="isEditing" class="p-6 bg-slate-50 border-t border-slate-100 sticky bottom-0">
          <button @click="handleUpdate" :disabled="isLoading" class="w-full py-3 bg-emerald-600 text-white rounded-xl font-bold shadow-lg hover:bg-emerald-700 transition-all">
            {{ isLoading ? 'UPDATING...' : 'SAVE CHANGES' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'

const route = useRoute()
import { Plus, X, RefreshCw, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import StaffSidebar from './StaffSidebar.vue'
import StaffTopbar from './StaffTopbar.vue'

// Real routes live under ChildrenController (/api/Children) and ParentsController
// (/api/Parents) — confirmed against StaffPatientRecords.vue's working calls. The old
// "/api/Vaccination/GetAllChildren" / "GetAllParents" endpoints this page called don't
// exist on the backend, which is why nothing ever loaded.
const API_BASE = (import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '') + '/api'

// ==========================================
// STATE MANAGEMENT
// ==========================================
const childrenList = ref([])

// ── PAGINATION ────────────────────────────────────────────────────
const currentPage = ref(1)
const pageSize    = ref(10)
const totalPages  = computed(() => Math.max(1, Math.ceil(childrenList.value.length / pageSize.value)))
const paginatedChildren = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return childrenList.value.slice(start, start + pageSize.value)
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
const parentOptions = ref([]) 
const isLoading = ref(false)

// Registration Modal State
const showAddModal = ref(false)
const parentSearch = ref('')
const selectedRole = ref('Mother')
const childForm = reactive({
  FirstName: '', MiddleName: '', LastName: '', BirthDate: '', Sex: 'Male',
  PlaceOfBirth: '', Address: '', 
  MotherName: '', MotherEmail: '', MotherID: null,
  FatherName: '', FatherEmail: '', FatherID: null, 
  GuardianName: '', GuardianEmail: '', GuardianID: null, 
  BirthWeight: null, BirthHeight: null, 
  HealthCenter: 'Leveriza Health Center', Barangay: '', FamilyNo: ''
})

// Profile & Edit Modal State
const showProfileModal = ref(false)
const isEditing = ref(false)
const selectedChild = ref(null)


// ==========================================
// COMPUTED PROPERTIES
// ==========================================

// Search Logic for Parent Linking
const filteredParents = computed(() => {
  if (!parentSearch.value) return []
  const q = parentSearch.value.toLowerCase()
  return parentOptions.value.filter(p => 
    p.email?.toLowerCase().includes(q) || 
    p.firstName?.toLowerCase().includes(q) || 
    p.lastName?.toLowerCase().includes(q)
  ).slice(0, 5)
})

// Formats SQL Date (ISO) to HTML Input Date (YYYY-MM-DD)
const formattedBirthDate = computed({
  get: () => selectedChild.value?.birthDate ? selectedChild.value.birthDate.split('T')[0] : '',
  set: (val) => { if(selectedChild.value) selectedChild.value.birthDate = val }
})

// Looks up a linked parent by role from the child's real `parents` array
// (GET /api/Children/all and /{id} both return `parents: [{ parentID,
// parentName, relationshipType, isPrimaryContact, canReceiveNotifications }]`).
const parentForRole = (role) =>
  selectedChild.value?.parents?.find(p => p.relationshipType === role)

// ==========================================
// METHODS
// ==========================================

const fetchChildren = async () => {
  isLoading.value = true
  try {
    const res = await axios.get(`${API_BASE}/Children/all`)
    childrenList.value = res.data
  } catch (err) { 
    console.error("Fetch Error:", err) 
  } finally { 
    isLoading.value = false 
  }
}

const fetchParents = async () => {
  try {
    const res = await axios.get(`${API_BASE}/Parents/all`)
    parentOptions.value = res.data
  } catch (err) { 
    console.error(err) 
  }
}

// Logic to open the Profile (View Mode)
const viewProfile = (child) => {
  selectedChild.value = { ...child } 
  isEditing.value = false
  showProfileModal.value = true
  fetchParents() // Load parents list for potential editing
}

// --- PARENT LINKING LOGIC ---

// 1. Handle manual typing
const handleManualNameEntry = () => {
  const role = selectedRole.value;
  childForm[`${role}Name`] = parentSearch.value;
  childForm[`${role}ID`] = null;     // Manual entry = No ID
  childForm[`${role}Email`] = '';    // Manual entry = No Email
}

// 2. Handle selecting from search (Link Account)
const linkExistingAccount = (parent) => {
  const role = selectedRole.value;
  const fullName = `${parent.firstName} ${parent.lastName}`;
  
  childForm[`${role}Name`] = fullName;
  childForm[`${role}ID`] = parent.parentID;
  childForm[`${role}Email`] = parent.email;
  
  parentSearch.value = ''; // Reset search input
}

// 3. Clear selected role
const clearParentRole = (role) => {
  childForm[`${role}Name`] = '';
  childForm[`${role}ID`] = null;
  childForm[`${role}Email`] = '';
}

// CREATE: Save New Child
//
// ChildrenController.CreateChild's real DTO takes a Parents ARRAY (confirmed
// from the controller source: dto.Parents.Any() is required, and
// dto.Parents.First().ParentID is read) — so this form's three-role picker
// (Mother/Father/Guardian) can genuinely all be saved, one entry per role
// that got LINKED to an existing account. A role typed in manually has no
// ParentID and can't be included — the backend only accepts real linked
// accounts here, not free-text guardian names. The first linked role is
// marked the primary contact.
const handleSave = async () => {
  if (!childForm.FirstName || !childForm.LastName) {
    alert("Please enter Child's Name.")
    return
  }
  if (!childForm.BirthDate) {
    alert("Please enter Birth Date.")
    return
  }
  const parents = []
  for (const role of ['Mother', 'Father', 'Guardian']) {
    const parentID = childForm[`${role}ID`]
    if (parentID) {
      parents.push({
        parentID,
        relationshipType: role,
        isPrimaryContact: parents.length === 0,
        canReceiveNotifications: true,
      })
    }
  }
  if (parents.length === 0) {
    alert("Please link at least one parent/guardian to an existing account (search and select from the results) — a manually-typed name with no account can't be saved yet.")
    return
  }
  isLoading.value = true
  try {
    // NOTE: BirthWeight/BirthHeight/FamilyNo aren't columns on the backend's
    // Child model at all — they're left out of the request; the inputs are
    // kept in the form as a placeholder in case those get added later.
    const payload = {
      firstName: childForm.FirstName,
      middleName: childForm.MiddleName || null,
      lastName: childForm.LastName,
      birthDate: childForm.BirthDate,
      placeOfBirth: childForm.PlaceOfBirth || null,
      sex: childForm.Sex,
      barangay: childForm.Barangay ? Number(childForm.Barangay) : null,
      address: childForm.Address || null,
      healthCenter: childForm.HealthCenter || null,
      parents,
    }
    await axios.post(`${API_BASE}/Children`, payload)
    alert("Child registered successfully!")
    showAddModal.value = false
    fetchChildren()
  } catch (err) {
    console.error(err)
    alert(err.response?.data?.message || "Error saving record.")
  } finally { 
    isLoading.value = false 
  }
}

// UPDATE: Save Changes to Existing Profile
//
// Only scalar Child fields go here — linked parents are managed separately
// via ChildParentRelationshipsController (create/delete), not through this
// PUT, so selectedChild.parents is intentionally not sent.
const handleUpdate = async () => {
  if (!selectedChild.value.firstName || !selectedChild.value.lastName) {
    alert("Name fields are required.")
    return
  }

  isLoading.value = true
  try {
    const c = selectedChild.value
    const payload = {
      firstName: c.firstName,
      middleName: c.middleName || null,
      lastName: c.lastName,
      birthDate: c.birthDate,
      placeOfBirth: c.placeOfBirth || null,
      sex: c.sex || null,
      barangay: c.barangay ? Number(c.barangay) : null,
      address: c.address || null,
      healthCenter: c.healthCenter || null,
    }
    await axios.put(`${API_BASE}/Children/${c.childID}`, payload)
    alert("Profile updated successfully!")
    showProfileModal.value = false
    isEditing.value = false
    fetchChildren()
  } catch (err) {
    console.error(err)
    alert(err.response?.data?.message || "Failed to update profile.")
  } finally {
    isLoading.value = false
  }
}

const openAddModal = () => {
  Object.assign(childForm, {
    FirstName: '', MiddleName: '', LastName: '', BirthDate: '', Sex: 'Male',
    PlaceOfBirth: '', Address: '', 
    MotherName: '', MotherEmail: '', MotherID: null,
    FatherName: '', FatherEmail: '', FatherID: null, 
    GuardianName: '', GuardianEmail: '', GuardianID: null, 
    BirthWeight: null, BirthHeight: null, 
    HealthCenter: 'Leveriza Health Center', Barangay: '', FamilyNo: ''
  })
  parentSearch.value = '';
  showAddModal.value = true;
  fetchParents();
}

onMounted(() => {
  fetchChildren()
  // Arrived from Dashboard's "Register Child" / "Add Walk-in Patient" quick
  // actions. `walkin` isn't handled distinctly yet — both just open the
  // registration modal for now.
  if (route.query.openRegister) {
    openAddModal()
  }
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #E2E8F0; border-radius: 10px; }
</style>