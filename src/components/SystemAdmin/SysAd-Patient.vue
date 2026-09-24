  <script setup>
  import { ref, computed, reactive } from 'vue'
import AppHeader from './Components/AppHeader.vue'
import AppSidebar from './Components/AppSidebar.vue'
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
  const activeNav = ref('Patient Management')

  /* -------------------------------- Status meta -------------------------------- */
  const vaccMeta = {
    'Fully Vaccinated': { tint: 'bg-emerald-50', text: 'text-emerald-700', dot: 'bg-emerald-500' },
    'Upcoming': { tint: 'bg-amber-50', text: 'text-amber-700', dot: 'bg-amber-500' },
    'Partially Vaccinated': { tint: 'bg-orange-50', text: 'text-orange-700', dot: 'bg-orange-500' },
    'Delayed': { tint: 'bg-rose-50', text: 'text-rose-600', dot: 'bg-rose-500' },
  }

  const patientStatusMeta = {
    Active: { tint: 'bg-emerald-50', text: 'text-emerald-700', dot: 'bg-emerald-500' },
    Inactive: { tint: 'bg-slate-100', text: 'text-slate-600', dot: 'bg-slate-400' },
    Archived: { tint: 'bg-slate-200', text: 'text-slate-800', dot: 'bg-slate-600' },
  }

  /* -------------------------------- Patient data -------------------------------- */
  const patients = ref([
    {
      id: 1, patientId: 'PT-00231', firstName: 'Miguel', middleName: 'Santos', lastName: 'Bautista',
      birthDate: 'Sep 03, 2024', age: '10 mos', sex: 'Male', birthWeight: '3.2 kg', bloodType: 'O+',
      address: 'Blk 4 Lot 12, Brgy. San Isidro, Quezon City',
      mother: 'Marites Santos', father: 'Ronaldo Bautista', guardian: '—', relationship: 'Mother',
      contactNumbers: '+63 920 441 8832', vaccStatus: 'Fully Vaccinated', patientStatus: 'Active',
      lastVaccination: 'Jun 18, 2026', latestVaccine: 'Pentavalent (3rd dose)', nextVaccine: 'MMR (1st dose)',
      completion: 100, registeredDate: 'Sep 10, 2024', statusReason: '',
    },
    {
      id: 2, patientId: 'PT-00232', firstName: 'Sofia', middleName: 'Reyes', lastName: 'Domingo',
      birthDate: 'Jan 22, 2025', age: '5 mos', sex: 'Female', birthWeight: '2.9 kg', bloodType: 'A+',
      address: 'Purok 3, Brgy. Malinis, Marikina City',
      mother: 'Liza Domingo', father: '—', guardian: '—', relationship: 'Mother',
      contactNumbers: '+63 919 205 6671', vaccStatus: 'Delayed', patientStatus: 'Active',
      lastVaccination: 'Mar 02, 2026', latestVaccine: 'BCG', nextVaccine: 'Pentavalent (1st dose)',
      completion: 35, registeredDate: 'Jan 28, 2025', statusReason: '',
    },
    {
      id: 3, patientId: 'PT-00233', firstName: 'Carlos', middleName: 'Fernandez', lastName: 'Torres',
      birthDate: 'Nov 11, 2023', age: '1 yr 8 mos', sex: 'Male', birthWeight: '3.4 kg', bloodType: 'B+',
      address: '25 Mabini St., Brgy. Poblacion, Pasig City',
      mother: 'Angeli Torres', father: 'Carlo Reyes', guardian: '—', relationship: 'Father',
      contactNumbers: '+63 921 336 9081', vaccStatus: 'Partially Vaccinated', patientStatus: 'Active',
      lastVaccination: 'May 14, 2026', latestVaccine: 'OPV (2nd dose)', nextVaccine: 'OPV (3rd dose)',
      completion: 62, registeredDate: 'Nov 20, 2023', statusReason: '',
    },
    {
      id: 4, patientId: 'PT-00234', firstName: 'Isabela', middleName: 'Cruz', lastName: 'Aquino',
      birthDate: 'Jul 07, 2026', age: '4 days', sex: 'Female', birthWeight: '3.0 kg', bloodType: 'O-',
      address: 'Blk 9 Lot 3, Brgy. Bagong Silang, Caloocan City',
      mother: 'Elena Cruz', father: 'Jhun Aquino', guardian: '—', relationship: 'Mother',
      contactNumbers: '+63 917 200 1145', vaccStatus: 'Upcoming', patientStatus: 'Active',
      lastVaccination: '—', latestVaccine: '—', nextVaccine: 'BCG',
      completion: 0, registeredDate: 'Jul 08, 2026', statusReason: '',
    },
    {
      id: 5, patientId: 'PT-00198', firstName: 'Diego', middleName: 'Miguel', lastName: 'Ramos',
      birthDate: 'Feb 14, 2023', age: '2 yrs 5 mos', sex: 'Male', birthWeight: '3.1 kg', bloodType: 'AB+',
      address: '18 Rizal Ave., Brgy. Sto. Niño, Marikina City',
      mother: '—', father: '—', guardian: 'Bea Fernandez', relationship: 'Guardian',
      contactNumbers: '+63 918 774 2201', vaccStatus: 'Fully Vaccinated', patientStatus: 'Inactive',
      lastVaccination: 'Dec 09, 2025', latestVaccine: 'MMR (2nd dose)', nextVaccine: '—',
      completion: 100, registeredDate: 'Feb 20, 2023', statusReason: 'Family relocated outside service area',
    },
    {
      id: 6, patientId: 'PT-00104', firstName: 'Andrea', middleName: 'Lopez', lastName: 'Villanueva',
      birthDate: 'May 30, 2022', age: '4 yrs 1 mo', sex: 'Female', birthWeight: '3.3 kg', bloodType: 'A-',
      address: '7 Kalayaan St., Brgy. Malaya, Quezon City',
      mother: 'Renz Villanueva', father: '—', guardian: '—', relationship: 'Mother',
      contactNumbers: '+63 917 663 0072', vaccStatus: 'Delayed', patientStatus: 'Archived',
      lastVaccination: 'Aug 21, 2024', latestVaccine: 'DPT Booster', nextVaccine: 'MMR Booster',
      completion: 78, registeredDate: 'Jun 02, 2022', statusReason: 'Transferred to another clinic',
    },
  ])

  /* ---------------------------- Toolbar / filters ---------------------------- */
  const searchQuery = ref('')
  const vaccFilter = ref('All')
  const statusFilter = ref('All')

  const filteredPatients = computed(() =>
    patients.value.filter((p) => {
      const q = searchQuery.value.trim().toLowerCase()
      const fullName = `${p.firstName} ${p.lastName}`.toLowerCase()
      const parentName = [p.mother, p.father, p.guardian].filter(Boolean).join(' ').toLowerCase()
      const matchesSearch =
        !q || fullName.includes(q) || p.patientId.toLowerCase().includes(q) || parentName.includes(q)
      const matchesVacc = vaccFilter.value === 'All' || p.vaccStatus === vaccFilter.value
      const matchesStatus = statusFilter.value === 'All' || p.patientStatus === statusFilter.value
      return matchesSearch && matchesVacc && matchesStatus
    })
  )

  /* -------------------------------- Summary ---------------------------------- */
  const summary = computed(() => ({
    total: patients.value.length,
    active: patients.value.filter((p) => p.patientStatus === 'Active').length,
    inactive: patients.value.filter((p) => p.patientStatus === 'Inactive').length,
    archived: patients.value.filter((p) => p.patientStatus === 'Archived').length,
    dueThisWeek: patients.value.filter((p) => p.vaccStatus === 'Upcoming').length,
    delayed: patients.value.filter((p) => p.vaccStatus === 'Delayed').length,
  }))

  const initials = (p) => `${p.firstName[0] ?? ''}${p.lastName[0] ?? ''}`.toUpperCase()

  /* ------------------------------ Row actions menu ---------------------------- */
  const openMenuId = ref(null)
  const toggleMenu = (id) => (openMenuId.value = openMenuId.value === id ? null : id)
  const closeMenu = () => (openMenuId.value = null)

  /* -------------------------------- Profile drawer ---------------------------- */
  const showDrawer = ref(false)
  const selectedPatient = ref(null)
  const openDrawer = (patient) => {
    selectedPatient.value = patient
    showDrawer.value = true
    closeMenu()
  }
  const closeDrawer = () => (showDrawer.value = false)

  /* --------------------------------- Register modal --------------------------------- */
  const showRegisterModal = ref(false)
  const parentMode = ref('search')
  const registerForm = reactive({
    firstName: '', middleName: '', lastName: '', birthDate: '', sex: 'Male',
    birthWeight: '', bloodType: 'O+', address: '',
    parentSearch: '', newParentName: '', newParentContact: '',
    relationship: 'Mother', emergencyContact: '',
  })
  const openRegisterModal = () => {
    Object.assign(registerForm, {
      firstName: '', middleName: '', lastName: '', birthDate: '', sex: 'Male',
      birthWeight: '', bloodType: 'O+', address: '',
      parentSearch: '', newParentName: '', newParentContact: '',
      relationship: 'Mother', emergencyContact: '',
    })
    parentMode.value = 'search'
    showRegisterModal.value = true
  }
  const registerPatient = () => {
    patients.value.unshift({
      id: Date.now(),
      patientId: `PT-0${Math.floor(1000 + Math.random() * 8999)}`,
      firstName: registerForm.firstName || 'New',
      middleName: registerForm.middleName,
      lastName: registerForm.lastName || 'Patient',
      birthDate: registerForm.birthDate || '—',
      age: '0 mos',
      sex: registerForm.sex,
      birthWeight: registerForm.birthWeight || '—',
      bloodType: registerForm.bloodType,
      address: registerForm.address || '—',
      mother: registerForm.relationship === 'Mother' ? (registerForm.newParentName || registerForm.parentSearch) : '—',
      father: registerForm.relationship === 'Father' ? (registerForm.newParentName || registerForm.parentSearch) : '—',
      guardian: registerForm.relationship === 'Guardian' ? (registerForm.newParentName || registerForm.parentSearch) : '—',
      relationship: registerForm.relationship,
      contactNumbers: registerForm.emergencyContact || '—',
      vaccStatus: 'Upcoming',
      patientStatus: 'Active',
      lastVaccination: '—',
      latestVaccine: '—',
      nextVaccine: 'BCG',
      completion: 0,
      registeredDate: 'Today',
      statusReason: '',
    })
    showRegisterModal.value = false
  }

  /* --------------------------------- Change status modal --------------------------------- */
  const showStatusModal = ref(false)
  const statusForm = reactive({ id: null, status: 'Active', reason: '' })
  const openStatusModal = (patient) => {
    Object.assign(statusForm, { id: patient.id, status: patient.patientStatus, reason: patient.statusReason || '' })
    showStatusModal.value = true
    closeMenu()
  }
  const saveStatus = () => {
    const p = patients.value.find((x) => x.id === statusForm.id)
    if (p) {
      p.patientStatus = statusForm.status
      p.statusReason = statusForm.status === 'Active' ? '' : statusForm.reason
    }
    showStatusModal.value = false
  }
  const statusFormValid = computed(() => statusForm.status === 'Active' || statusForm.reason.trim().length > 0)
  </script>

  <template>
    <div class="min-h-screen bg-slate-50 flex text-slate-900" @click="closeMenu">
      <!-- ============================ SIDEBAR ============================ -->
   <AppSidebar />
      <!-- ============================ MAIN ============================ -->
      <div class="flex-1 min-w-0 flex flex-col">
        <!-- Top navbar -->
        <AppHeader
  title="Patient Management"
  breadcrumb="System Administration / Patient Management"
  user-initials="RM"
/>

        <!-- Content -->
        <main class="p-6 space-y-6">
          <!-- Summary cards -->
          <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
            <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
              <div class="flex items-start justify-between gap-2">
                <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Total Registered</p>
                <div class="bg-teal-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">🧒</div>
              </div>
              <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.total }}</p>
            </div>
            <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
              <div class="flex items-start justify-between gap-2">
                <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Active Patients</p>
                <div class="bg-emerald-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">✅</div>
              </div>
              <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.active }}</p>
            </div>
            <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
              <div class="flex items-start justify-between gap-2">
                <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Inactive Patients</p>
                <div class="bg-slate-100 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">💤</div>
              </div>
              <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.inactive }}</p>
            </div>
            <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
              <div class="flex items-start justify-between gap-2">
                <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Archived Patients</p>
                <div class="bg-slate-200 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">🗄️</div>
              </div>
              <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.archived }}</p>
            </div>
            <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
              <div class="flex items-start justify-between gap-2">
                <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Due This Week</p>
                <div class="bg-amber-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">📅</div>
              </div>
              <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.dueThisWeek }}</p>
            </div>
            <div class="min-w-0 bg-white border border-slate-200 rounded-xl p-4 shadow-sm">
              <div class="flex items-start justify-between gap-2">
                <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Delayed Vaccinations</p>
                <div class="bg-rose-50 w-8 h-8 rounded-lg flex items-center justify-center shrink-0 text-sm">⚠️</div>
              </div>
              <p class="mt-2 text-2xl font-extrabold text-slate-900">{{ summary.delayed }}</p>
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
                  placeholder="Search by child name, patient ID, or parent name..."
                  class="w-full pl-9 pr-3 py-2 text-sm rounded-lg border border-slate-200 bg-slate-50 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
                />
              </div>

              <select
                v-model="vaccFilter"
                class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
              >
                <option value="All">All Vaccination Status</option>
                <option>Fully Vaccinated</option>
                <option>Partially Vaccinated</option>
                <option>Delayed</option>
                <option>Upcoming</option>
              </select>

              <select
                v-model="statusFilter"
                class="text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-slate-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors"
              >
                <option value="All">All Patient Status</option>
                <option>Active</option>
                <option>Inactive</option>
                <option>Archived</option>
              </select>

              <div class="flex items-center gap-2 shrink-0">
                <button class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500">
                  Export
                </button>
                <button
                  @click="openRegisterModal"
                  class="text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
                >
                  + Register Child
                </button>
              </div>
            </div>
          </section>

          <!-- Patient table -->
          <section class="bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
            <div class="overflow-x-auto">
              <table class="w-full text-sm">
                <thead>
                  <tr class="border-b border-slate-200 bg-slate-50/60">
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Patient ID</th>
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Child Name</th>
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Age</th>
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Sex</th>
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Parent / Guardian</th>
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Vaccination Status</th>
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Patient Status</th>
                    <th class="text-left font-semibold text-slate-500 text-xs uppercase tracking-wide px-3 py-3">Last Vaccination</th>
                    <th class="text-right font-semibold text-slate-500 text-xs uppercase tracking-wide px-5 py-3">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="patient in filteredPatients"
                    :key="patient.id"
                    class="border-b border-slate-100 last:border-0 hover:bg-slate-50 transition-colors"
                  >
                    <td class="px-5 py-3 font-mono text-xs text-slate-500 whitespace-nowrap">{{ patient.patientId }}</td>
                    <td class="px-3 py-3 whitespace-nowrap">
                      <div class="flex items-center gap-3">
                        <div class="w-8 h-8 rounded-full bg-emerald-100 text-emerald-700 flex items-center justify-center text-xs font-bold shrink-0">
                          {{ initials(patient) }}
                        </div>
                        <span class="font-semibold text-slate-900">{{ patient.firstName }} {{ patient.lastName }}</span>
                      </div>
                    </td>
                    <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ patient.age }}</td>
                    <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ patient.sex }}</td>
                    <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ patient.mother !== '—' ? patient.mother : (patient.father !== '—' ? patient.father : patient.guardian) }}</td>
                    <td class="px-3 py-3">
                      <span :class="[vaccMeta[patient.vaccStatus].tint, vaccMeta[patient.vaccStatus].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full whitespace-nowrap">
                        <span :class="vaccMeta[patient.vaccStatus].dot" class="w-1.5 h-1.5 rounded-full"></span>
                        {{ patient.vaccStatus }}
                      </span>
                    </td>
                    <td class="px-3 py-3">
                      <span :class="[patientStatusMeta[patient.patientStatus].tint, patientStatusMeta[patient.patientStatus].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full whitespace-nowrap">
                        <span :class="patientStatusMeta[patient.patientStatus].dot" class="w-1.5 h-1.5 rounded-full"></span>
                        {{ patient.patientStatus }}
                      </span>
                    </td>
                    <td class="px-3 py-3 text-slate-500 whitespace-nowrap">{{ patient.lastVaccination }}</td>
                    <td class="px-5 py-3 text-right relative">
                      <button
                        @click.stop="toggleMenu(patient.id)"
                        class="text-slate-400 hover:text-slate-700 hover:bg-slate-100 rounded-lg w-8 h-8 inline-flex items-center justify-center transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
                      >
                        ⋮
                      </button>

                      <div
                        v-if="openMenuId === patient.id"
                        @click.stop
                        class="absolute right-5 top-11 z-30 w-56 bg-white border border-slate-200 rounded-lg shadow-md py-1 text-left"
                      >
                        <button @click="openDrawer(patient)" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">View Patient Profile</button>
                        <button @click="closeMenu" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">Edit Patient Information</button>
                        <button @click="closeMenu" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">View Vaccination Card</button>
                        <button @click="closeMenu" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">View Vaccination History</button>
                        <div class="my-1 border-t border-slate-100"></div>
                        <button @click="closeMenu" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">Link Parent</button>
                        <button @click="closeMenu" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">Unlink Parent</button>
                        <button @click="closeMenu" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">Transfer Guardian</button>
                        <div class="my-1 border-t border-slate-100"></div>
                        <button @click="openStatusModal(patient)" class="w-full text-left px-3.5 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">Change Patient Status</button>
                        <button @click="closeMenu" class="w-full text-left px-3.5 py-2 text-sm text-rose-600 hover:bg-rose-50 transition-colors">Archive Patient</button>
                      </div>
                    </td>
                  </tr>

                  <tr v-if="filteredPatients.length === 0">
                    <td colspan="9" class="px-5 py-12 text-center text-sm text-slate-400">No patients match your search or filters.</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </section>
        </main>
      </div>

      <!-- ============================ PATIENT PROFILE DRAWER ============================ -->
      <transition name="fade">
        <div v-if="showDrawer" class="fixed inset-0 bg-slate-900/30 z-40" @click="closeDrawer"></div>
      </transition>
      <transition name="slide">
        <aside v-if="showDrawer" class="fixed top-0 right-0 h-screen w-full max-w-xl bg-white border-l border-slate-200 shadow-lg z-50 flex flex-col">
          <div class="h-[70px] flex items-center justify-between px-6 border-b border-slate-200 shrink-0">
            <h2 class="text-sm font-bold text-slate-900">Patient Profile</h2>
            <button @click="closeDrawer" class="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-50 transition-colors">✕</button>
          </div>

          <div v-if="selectedPatient" class="flex-1 overflow-y-auto p-6 space-y-6">
            <!-- Section 1: Child Information -->
            <div class="bg-slate-50 rounded-xl border border-slate-200 p-5">
              <div class="flex items-center gap-4 mb-4">
                <div class="w-16 h-16 rounded-full bg-emerald-100 text-emerald-700 flex items-center justify-center text-lg font-bold shrink-0">
                  {{ initials(selectedPatient) }}
                </div>
                <div class="min-w-0">
                  <p class="text-base font-bold text-slate-900 truncate">{{ selectedPatient.firstName }} {{ selectedPatient.middleName }} {{ selectedPatient.lastName }}</p>
                  <p class="text-xs text-slate-500 font-mono">{{ selectedPatient.patientId }}</p>
                </div>
              </div>
              <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-2">Child Information</h3>
              <div class="grid grid-cols-2 gap-x-4 gap-y-3">
                <div><p class="text-xs text-slate-500">Birth Date</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.birthDate }}</p></div>
                <div><p class="text-xs text-slate-500">Age</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.age }}</p></div>
                <div><p class="text-xs text-slate-500">Sex</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.sex }}</p></div>
                <div><p class="text-xs text-slate-500">Birth Weight</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.birthWeight }}</p></div>
                <div><p class="text-xs text-slate-500">Blood Type</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.bloodType }}</p></div>
                <div class="col-span-2"><p class="text-xs text-slate-500">Address</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.address }}</p></div>
              </div>
            </div>

            <!-- Section 2: Parent / Guardian Information -->
            <div>
              <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Parent / Guardian Information</h3>
              <div class="bg-slate-50 rounded-lg divide-y divide-slate-200">
                <div class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Mother</span>
                  <span class="text-sm font-medium text-slate-900">{{ selectedPatient.mother }}</span>
                </div>
                <div class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Father</span>
                  <span class="text-sm font-medium text-slate-900">{{ selectedPatient.father }}</span>
                </div>
                <div class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Guardian</span>
                  <span class="text-sm font-medium text-slate-900">{{ selectedPatient.guardian }}</span>
                </div>
                <div class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Relationship</span>
                  <span class="text-sm font-medium text-slate-900">{{ selectedPatient.relationship }}</span>
                </div>
                <div class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Contact Numbers</span>
                  <span class="text-sm font-medium text-slate-900">{{ selectedPatient.contactNumbers }}</span>
                </div>
              </div>
              <div class="flex flex-wrap gap-2 mt-3">
                <button class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Link Parent</button>
                <button class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Change Relationship</button>
                <button class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Transfer Guardian</button>
                <button class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-rose-200 text-rose-600 hover:bg-rose-50 transition-colors">Unlink Parent</button>
              </div>
            </div>

            <!-- Section 3: Vaccination Overview -->
            <div>
              <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Vaccination Overview</h3>
              <div class="bg-slate-50 rounded-lg p-4 space-y-4">
                <div>
                  <div class="flex items-center justify-between mb-1.5">
                    <span class="text-xs text-slate-500">Vaccination Progress</span>
                    <span class="text-xs font-semibold text-emerald-700">{{ selectedPatient.completion }}%</span>
                  </div>
                  <div class="w-full h-2 rounded-full bg-slate-200 overflow-hidden">
                    <div class="h-full bg-emerald-500 rounded-full" :style="{ width: selectedPatient.completion + '%' }"></div>
                  </div>
                </div>
                <div class="grid grid-cols-2 gap-x-4 gap-y-3">
                  <div><p class="text-xs text-slate-500">Latest Vaccine</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.latestVaccine }}</p></div>
                  <div><p class="text-xs text-slate-500">Next Scheduled Vaccine</p><p class="text-sm font-medium text-slate-900">{{ selectedPatient.nextVaccine }}</p></div>
                </div>
              </div>
              <div class="flex flex-wrap gap-2 mt-3">
                <button class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">View Vaccine Card</button>
                <button class="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">View Full History</button>
              </div>
            </div>

            <!-- Section 4: Patient Status -->
            <div>
              <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Patient Status</h3>
              <div class="bg-slate-50 rounded-lg divide-y divide-slate-200">
                <div class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Current Status</span>
                  <span :class="[patientStatusMeta[selectedPatient.patientStatus].tint, patientStatusMeta[selectedPatient.patientStatus].text]" class="inline-flex items-center gap-1.5 text-xs font-semibold px-2.5 py-1 rounded-full">
                    <span :class="patientStatusMeta[selectedPatient.patientStatus].dot" class="w-1.5 h-1.5 rounded-full"></span>
                    {{ selectedPatient.patientStatus }}
                  </span>
                </div>
                <div v-if="selectedPatient.statusReason" class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Reason</span>
                  <span class="text-sm font-medium text-slate-900 text-right ml-4">{{ selectedPatient.statusReason }}</span>
                </div>
                <div class="flex items-center justify-between px-4 py-3">
                  <span class="text-xs text-slate-500">Registered Date</span>
                  <span class="text-sm font-medium text-slate-900">{{ selectedPatient.registeredDate }}</span>
                </div>
              </div>
            </div>
          </div>

          <div class="border-t border-slate-200 p-4 flex items-center gap-2 shrink-0">
            <button class="flex-1 text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors">Edit Information</button>
            <button @click="openStatusModal(selectedPatient)" class="flex-1 text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Change Status</button>
            <button class="text-sm font-semibold px-4 py-2 rounded-lg text-rose-600 hover:bg-rose-50 transition-colors">Archive</button>
          </div>
        </aside>
      </transition>

      <!-- ============================ REGISTER CHILD MODAL ============================ -->
      <transition name="fade">
        <div v-if="showRegisterModal" class="fixed inset-0 bg-slate-900/40 z-40 flex items-center justify-center p-4" @click.self="showRegisterModal = false">
          <div class="bg-white rounded-xl shadow-lg w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            <div class="flex items-center justify-between px-6 py-4 border-b border-slate-200">
              <h2 class="text-base font-bold text-slate-900">Register Child</h2>
              <button @click="showRegisterModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-50 transition-colors">✕</button>
            </div>

            <div class="p-6 space-y-6">
              <!-- Child Information -->
              <div>
                <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Child Information</h3>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">First Name</label>
                    <input v-model="registerForm.firstName" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Middle Name</label>
                    <input v-model="registerForm.middleName" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Last Name</label>
                    <input v-model="registerForm.lastName" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Birth Date</label>
                    <input v-model="registerForm.birthDate" type="date" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Sex</label>
                    <select v-model="registerForm.sex" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                      <option>Male</option>
                      <option>Female</option>
                    </select>
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Birth Weight</label>
                    <input v-model="registerForm.birthWeight" type="text" placeholder="e.g. 3.2 kg" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Blood Type</label>
                    <select v-model="registerForm.bloodType" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                      <option>O+</option><option>O-</option><option>A+</option><option>A-</option>
                      <option>B+</option><option>B-</option><option>AB+</option><option>AB-</option>
                    </select>
                  </div>
                  <div class="sm:col-span-2">
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Address</label>
                    <input v-model="registerForm.address" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                </div>
              </div>

              <!-- Parent Information -->
              <div>
                <h3 class="text-xs font-bold uppercase tracking-wide text-slate-500 mb-3">Parent Information</h3>
                <div class="flex gap-2 mb-3">
                  <button
                    @click="parentMode = 'search'"
                    :class="parentMode === 'search' ? 'bg-emerald-600 text-white' : 'border border-slate-200 text-slate-600 hover:bg-slate-50'"
                    class="text-xs font-semibold px-3 py-1.5 rounded-lg transition-colors"
                  >Search Existing Parent</button>
                  <button
                    @click="parentMode = 'new'"
                    :class="parentMode === 'new' ? 'bg-emerald-600 text-white' : 'border border-slate-200 text-slate-600 hover:bg-slate-50'"
                    class="text-xs font-semibold px-3 py-1.5 rounded-lg transition-colors"
                  >Register New Parent</button>
                </div>

                <div v-if="parentMode === 'search'">
                  <label class="block text-xs font-semibold text-slate-500 mb-1.5">Search Parent</label>
                  <input v-model="registerForm.parentSearch" type="text" placeholder="Search by name, username, or email..." class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                </div>
                <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Full Name</label>
                    <input v-model="registerForm.newParentName" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Contact Number</label>
                    <input v-model="registerForm.newParentContact" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                </div>

                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-4">
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Relationship</label>
                    <select v-model="registerForm.relationship" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                      <option>Mother</option>
                      <option>Father</option>
                      <option>Guardian</option>
                    </select>
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5">Emergency Contact</label>
                    <input v-model="registerForm.emergencyContact" type="text" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors" />
                  </div>
                </div>
              </div>
            </div>

            <div class="flex items-center justify-end gap-2 px-6 py-4 border-t border-slate-200">
              <button @click="showRegisterModal = false" class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Cancel</button>
              <button @click="registerPatient" class="text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors">Register Patient</button>
            </div>
          </div>
        </div>
      </transition>

      <!-- ============================ CHANGE STATUS MODAL ============================ -->
      <transition name="fade">
        <div v-if="showStatusModal" class="fixed inset-0 bg-slate-900/40 z-40 flex items-center justify-center p-4" @click.self="showStatusModal = false">
          <div class="bg-white rounded-xl shadow-lg w-full max-w-md">
            <div class="flex items-center justify-between px-6 py-4 border-b border-slate-200">
              <h2 class="text-base font-bold text-slate-900">Change Patient Status</h2>
              <button @click="showStatusModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-slate-400 hover:bg-slate-50 transition-colors">✕</button>
            </div>

            <div class="p-6 space-y-4">
              <div>
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Status</label>
                <select v-model="statusForm.status" class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors">
                  <option>Active</option>
                  <option>Inactive</option>
                  <option>Archived</option>
                </select>
              </div>
              <div v-if="statusForm.status !== 'Active'">
                <label class="block text-xs font-semibold text-slate-500 mb-1.5">Reason <span class="text-rose-500">*</span></label>
                <textarea
                  v-model="statusForm.reason"
                  rows="3"
                  placeholder="Please provide a reason for this status change..."
                  class="w-full text-sm rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:bg-white transition-colors resize-none"
                ></textarea>
                <p v-if="!statusFormValid" class="text-xs text-rose-500 mt-1">A reason is required for this status.</p>
              </div>
            </div>

            <div class="flex items-center justify-end gap-2 px-6 py-4 border-t border-slate-200">
              <button @click="showStatusModal = false" class="text-sm font-semibold px-4 py-2 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">Cancel</button>
              <button
                @click="saveStatus"
                :disabled="!statusFormValid"
                :class="statusFormValid ? 'bg-emerald-600 hover:bg-emerald-700' : 'bg-emerald-300 cursor-not-allowed'"
                class="text-sm font-semibold px-4 py-2 rounded-lg text-white transition-colors"
              >Save Status</button>
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