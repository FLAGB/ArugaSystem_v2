<template>
  <div class="flex h-screen bg-[#F8FAFC] font-sans antialiased text-slate-900">
    
    <!-- SIDEBAR -->
    <aside class="w-64 bg-white border-r border-slate-200 flex flex-col shrink-0">
      <div class="p-6">
        <div class="flex items-center gap-3 px-2">
          <div class="w-8 h-8 bg-emerald-600 rounded flex items-center justify-center text-white font-bold">A</div>
          <span class="text-xl font-bold tracking-tight text-slate-800">Aruga</span>
        </div>
      </div>
      
      <nav class="flex-1 px-3 space-y-1 overflow-y-auto custom-scrollbar">
        <button v-for="item in navItems" :key="item.id" 
          @click="$router.push(item.path)"
          class="w-full flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-colors cursor-pointer"
          :class="$route.path === item.path ? 'bg-emerald-50 text-emerald-700' : 'text-slate-500 hover:bg-slate-50 hover:text-slate-900'">
          <component :is="item.icon" class="w-4 h-4" />
          {{ item.label }}
        </button>
      </nav>
    </aside>

    <!-- MAIN SECTION -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <header class="h-16 bg-white border-b border-slate-200 px-8 flex items-center justify-between shrink-0">
        <div>
          <h2 class="font-bold text-[10px] tracking-widest text-slate-400 uppercase">Front Desk Operations</h2>
          <p class="text-lg font-bold text-slate-800">Admission & Queuing</p>
        </div>
        <div class="flex gap-3">
          <button @click="fetchData" class="p-2.5 text-slate-400 hover:text-emerald-600 transition-colors bg-slate-50 rounded-lg border border-slate-200">
            <RefreshCw class="w-4 h-4" :class="{'animate-spin': isLoading}" />
          </button>
          <button @click="showAddModal = true" class="bg-emerald-600 text-white px-5 py-2.5 rounded-lg text-xs font-bold hover:bg-emerald-700 transition-all flex items-center gap-2 shadow-sm active:scale-95">
            <Plus class="w-4 h-4" /> WALK-IN REGISTRATION
          </button>
        </div>
      </header>

      <main class="flex-1 p-8 overflow-y-auto custom-scrollbar">
        <div class="max-w-[1600px] mx-auto space-y-8">
          
          <!-- TOP ROW: STATION STATUS -->
          <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
            <div v-for="station in activeStations" :key="station.roomId" 
                 class="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between relative overflow-hidden transition-all"
                 :class="station.isOccupied ? 'border-orange-200 bg-orange-50/20' : 'border-emerald-200 bg-emerald-50/20'">
              <div class="flex items-center gap-4">
                <div class="w-10 h-10 rounded-xl flex items-center justify-center transition-colors" 
                     :class="station.isOccupied ? 'bg-orange-100 text-orange-600' : 'bg-emerald-100 text-emerald-600'">
                  <UserRound v-if="station.isOccupied" class="w-5 h-5 animate-pulse" />
                  <Stethoscope v-else class="w-5 h-5" />
                </div>
                <div>
                  <p class="text-[10px] font-bold text-slate-400 uppercase tracking-tight">{{ station.roomName }}</p>
                  <p class="text-sm font-bold text-slate-800">{{ station.doctorName }}</p>
                </div>
              </div>
              <button v-if="station.isOccupied" @click="completeSession(station)" 
                      class="text-[9px] font-black uppercase px-2 py-1 rounded-md bg-orange-200 text-orange-700 hover:bg-emerald-600 hover:text-white transition-all">
                Mark Done
              </button>
              <span v-else class="text-[9px] font-black uppercase px-2 py-1 rounded-md bg-emerald-100 text-emerald-700">Available</span>
            </div>
          </div>

          <!-- OPERATIONAL GRID -->
          <div class="grid grid-cols-12 gap-8 items-start">
            
            <!-- LEFT: LIVE QUEUE -->
            <section class="col-span-12 lg:col-span-8 space-y-4">
              <div class="flex items-center justify-between px-1">
                <h3 class="font-bold text-slate-800 flex items-center gap-2">
                  <RefreshCw class="w-4 h-4 text-emerald-600" /> Current Queue
                </h3>
              </div>

              <div class="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
                <table class="w-full text-left">
                  <thead class="bg-slate-50/50 border-b border-slate-200">
                    <tr>
                      <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">No.</th>
                      <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Patient</th>
                      <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Assign Station</th>
                      <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest text-right">Action</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-100">
                    <tr v-for="(q, index) in activeQueue" :key="q.id">
                      <td class="px-6 py-5 text-sm font-black text-slate-300">#{{ index + 1 }}</td>
                      <td class="px-6 py-5">
                        <p class="text-sm font-bold text-slate-700">{{ q.firstName }} {{ q.lastName }}</p>
                        <p class="text-[10px] font-medium text-slate-400 uppercase">Arrived: {{ q.arrivalTime }}</p>
                      </td>
                      <td class="px-6 py-5">
                        <select v-model="q.assignedStationId" 
                          class="w-full bg-slate-50 border border-slate-200 text-xs rounded-lg px-3 py-2 outline-none focus:border-emerald-500">
                          <option value="">-- Choose Station --</option>
                          <option v-for="s in activeStations" :key="s.roomId" :value="s.roomId" :disabled="s.isOccupied">
                            {{ s.roomName }} ({{ s.doctorName }}) {{ s.isOccupied ? '— BUSY' : '' }}
                          </option>
                        </select>
                      </td>
                      <td class="px-6 py-5 text-right">
                        <button :disabled="!q.assignedStationId" @click="sendToRoom(q)"
                          class="bg-emerald-600 text-white px-4 py-2 rounded-xl text-[10px] font-bold hover:bg-emerald-700 disabled:opacity-20 uppercase">
                          Call Next
                        </button>
                      </td>
                    </tr>
                    <tr v-if="activeQueue.length === 0">
                      <td colspan="4" class="px-6 py-12 text-center text-[10px] font-bold text-slate-400 uppercase tracking-widest">No patients waiting</td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <!-- NEW TABLE: VACCINATED / DONE -->
              <div class="mt-12 space-y-4">
                <h3 class="font-bold text-slate-800 flex items-center gap-2">
                  <Syringe class="w-4 h-4 text-slate-400" /> Completed Today
                </h3>
                <div class="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
                  <table class="w-full text-left">
                    <thead class="bg-slate-50/30 border-b border-slate-200">
                      <tr>
                        <th class="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase">Patient</th>
                        <th class="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase">Vaccine</th>
                        <th class="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase">Medical Staff</th>
                        <th class="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase text-right">Time Done</th>
                      </tr>
                    </thead>
                    <tbody class="divide-y divide-slate-100">
                      <tr v-for="v in vaccinatedToday" :key="v.id" class="opacity-70 group hover:opacity-100 transition-opacity">
                        <td class="px-6 py-4">
                          <p class="text-xs font-bold text-slate-700">{{ v.firstName }} {{ v.lastName }}</p>
                        </td>
                        <td class="px-6 py-4">
                          <span class="text-[10px] font-bold text-slate-500 bg-slate-100 px-2 py-0.5 rounded">{{ v.vaccineName }}</span>
                        </td>
                        <td class="px-6 py-4">
                          <p class="text-xs text-slate-600">{{ v.doctorName }}</p>
                        </td>
                        <td class="px-6 py-4 text-right">
                          <p class="text-[10px] font-bold text-emerald-600">{{ v.doneTime }}</p>
                        </td>
                      </tr>
                      <tr v-if="vaccinatedToday.length === 0">
                        <td colspan="4" class="px-6 py-8 text-center text-[10px] text-slate-300 uppercase">No completed records yet</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </section>

            <!-- RIGHT: EXPECTED -->
            <section class="col-span-12 lg:col-span-4 space-y-4">
              <h3 class="font-bold text-slate-800 flex items-center gap-2 px-1">
                <Calendar class="w-4 h-4 text-emerald-600" /> Expected Today
              </h3>
              <div class="space-y-3 max-h-[70vh] overflow-y-auto pr-2 custom-scrollbar">
                <div v-for="p in expectedPatients" :key="p.id" 
                  class="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm flex justify-between items-center group">
                  <div>
                    <p class="font-bold text-slate-700 text-sm leading-tight">{{ p.firstName }} {{ p.lastName }}</p>
                    <p class="text-[9px] font-bold text-emerald-600 uppercase mt-1">{{ p.vaccineName }}</p>
                  </div>
                  <button @click="markAsArrived(p)" 
                    class="bg-slate-50 text-slate-500 px-3 py-1.5 rounded-lg text-[10px] font-black hover:bg-emerald-600 hover:text-white transition-colors">
                    Arrived
                  </button>
                </div>
              </div>
            </section>

          </div>
        </div>
      </main>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useRoute, useRouter } from 'vue-router' // Ensure router is usable
import { 
  LayoutDashboard, Users, UserRound, Stethoscope, Syringe, 
  Calendar, RefreshCw, Plus, FileText 
} from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()

const API_BASE = 'http://localhost:57147/api/Vaccination'
const isLoading = ref(false)
const activeStations = ref([])
const activeQueue = ref([])
const expectedPatients = ref([])
const vaccinatedToday = ref([])

// Added showAddModal state as your template references it
const showAddModal = ref(false)

const fetchData = async () => {
  isLoading.value = true
  try {
    const res = await axios.get(`${API_BASE}/GetRooms`)
    activeStations.value = res.data || []
    
    expectedPatients.value = [
      { id: 101, firstName: 'Marco', lastName: 'Polo', vaccineName: 'BCG' },
      { id: 102, firstName: 'Elena', lastName: 'Gilbert', vaccineName: 'HepB' }
    ]
  } catch (err) {
    console.error("Connection to API failed. Using empty room list.", err)
    activeStations.value = []
  } finally {
    isLoading.value = false
  }
}

const markAsArrived = (p) => {
  activeQueue.value.push({ ...p, arrivalTime: new Date().toLocaleTimeString(), assignedStationId: '' })
  expectedPatients.value = expectedPatients.value.filter(x => x.id !== p.id)
}

const sendToRoom = async (patient) => {
  try {
    await axios.post(`${API_BASE}/AssignRoom`, {
      queueId: patient.id,
      roomId: patient.assignedStationId
    })
    activeQueue.value = activeQueue.value.filter(q => q.id !== patient.id)
    fetchData()
  } catch (err) { 
    console.error(err)
    alert("Room Error") 
  }
}

const completeSession = async (station) => {
  try {
    await axios.post(`${API_BASE}/CompleteSession/${station.roomID}`)
    vaccinatedToday.value.unshift({
      firstName: 'Patient', lastName: 'Done', 
      doctorName: station.doctorName, vaccineName: 'Administered',
      doneTime: new Date().toLocaleTimeString()
    })
    fetchData()
  } catch (err) {
    console.error(err)
  }
}

onMounted(fetchData)

const navItems = [
  { id: 'dashboard', label: 'Dashboard', icon: LayoutDashboard, path: '/AdminHome' },
  { id: 'parent', label: 'Parent', icon: Users, path: '/StaffParent' },
  { id: 'records', label: 'Children Record', icon: UserRound, path: '/StaffChild' },
  { id: 'staff', label: 'Doctor / Staff', icon: Stethoscope, path: '/StaffDocNurse' },
  { id: 'vaccines', label: 'Vaccines', icon: Syringe, path: '/StaffVaccine' },
  { id: 'calendar', label: 'Calendar', icon: Calendar, path: '/calendar' },
  { id: 'reports', label: 'Reports', icon: FileText, path: '/StaffReport' },
]
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #E2E8F0; border-radius: 10px; }
</style>