<template>
  <div class="flex h-screen bg-stone-50 antialiased text-stone-900" style="font-family: 'Inter','Segoe UI',sans-serif;">
    
    <StaffSidebar />

    <!-- MAIN SECTION -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <StaffTopbar title="Doctor / Staff" breadcrumb="Aruga / Doctor &amp; Staff">
        <button @click="showAddModal = true" class="bg-slate-900 text-white px-5 py-2.5 rounded-lg text-xs font-bold hover:bg-slate-800 transition-all flex items-center gap-2 shadow-sm">
          <Plus class="w-4 h-4" /> ADD NEW EMPLOYEE
        </button>
      </StaffTopbar>

      <main class="flex-1 p-8 overflow-y-auto custom-scrollbar">
        <div class="max-w-7xl mx-auto">
          
          <!-- FILTERS & SEARCH -->
          <div class="mb-6 flex justify-between items-center">
            <div class="relative">
              <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input type="text" placeholder="Search by name or PRC..." class="pl-10 pr-4 py-2 bg-white border border-slate-200 rounded-xl text-sm focus:border-emerald-500 outline-none w-80 transition-all">
            </div>
            <div class="flex gap-2">
              <button class="px-4 py-2 text-xs font-bold bg-white border border-slate-200 rounded-lg text-slate-600 hover:bg-slate-50">All</button>
              <button class="px-4 py-2 text-xs font-bold bg-white border border-slate-200 rounded-lg text-slate-600 hover:bg-slate-50">Doctors</button>
              <button class="px-4 py-2 text-xs font-bold bg-white border border-slate-200 rounded-lg text-slate-600 hover:bg-slate-50">Nurses</button>
            </div>
          </div>

          <!-- STAFF TABLE -->
          <div class="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
            <table class="w-full text-left text-sm">
              <thead class="bg-slate-50/50 border-b border-slate-200 text-[11px] uppercase font-bold text-slate-500">
                <tr>
                  <th class="px-6 py-4">Employee Name</th>
                  <th class="px-6 py-4">User Type</th>
                  <th class="px-6 py-4">Contact Number</th>
                  <th class="px-6 py-4">PRC License No.</th>
                  <th class="px-6 py-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="staff in staffMembers" :key="staff.id" class="hover:bg-slate-50/50 transition-colors">
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-3">
                      <div class="w-9 h-9 rounded-full bg-slate-100 flex items-center justify-center text-slate-500 font-bold text-xs border border-slate-200">
                        {{ staff.firstName[0] }}{{ staff.lastName[0] }}
                      </div>
                      <div>
                        <p class="font-bold text-slate-800">{{ staff.lastName }}, {{ staff.firstName }} {{ staff.middleName }}</p>
                        <p class="text-[10px] text-slate-400 font-medium tracking-wide">EMP-{{ staff.id }}</p>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4">
                    <span :class="staff.type === 'Doctor' ? 'bg-indigo-50 text-indigo-600 border-indigo-100' : 'bg-emerald-50 text-emerald-600 border-emerald-100'" 
                      class="px-2.5 py-1 rounded-full text-[10px] font-bold border uppercase">
                      {{ staff.type }}
                    </span>
                  </td>
                  <td class="px-6 py-4 text-slate-600 font-medium">
                    {{ staff.contact }}
                  </td>
                  <td class="px-6 py-4">
                    <code class="text-xs font-bold text-slate-500 bg-slate-100 px-2 py-1 rounded tracking-widest">
                      {{ staff.prcNo }}
                    </code>
                  </td>
                  <td class="px-6 py-4 text-right">
                    <button class="p-2 text-slate-400 hover:text-slate-600 transition-colors"><MoreHorizontal class="w-5 h-5" /></button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </main>

      <!-- ADD EMPLOYEE MODAL (Simplified) -->
      <div v-if="showAddModal" class="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
        <div class="bg-white w-full max-w-lg rounded-2xl shadow-2xl overflow-hidden border border-slate-200">
          <div class="p-6 border-b border-slate-100 flex justify-between items-center">
            <h3 class="font-bold text-slate-800">Add New Employee</h3>
            <button @click="showAddModal = false" class="text-slate-400 hover:text-slate-600"><X class="w-5 h-5" /></button>
          </div>
          
          <div class="p-6 space-y-4">
            <div class="grid grid-cols-3 gap-3">
              <div class="col-span-1">
                <label class="block text-[10px] font-bold text-slate-400 uppercase mb-1">First Name</label>
                <input type="text" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:border-emerald-500 outline-none">
              </div>
              <div class="col-span-1">
                <label class="block text-[10px] font-bold text-slate-400 uppercase mb-1">Middle Name</label>
                <input type="text" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:border-emerald-500 outline-none">
              </div>
              <div class="col-span-1">
                <label class="block text-[10px] font-bold text-slate-400 uppercase mb-1">Last Name</label>
                <input type="text" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:border-emerald-500 outline-none">
              </div>
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="block text-[10px] font-bold text-slate-400 uppercase mb-1">Contact No.</label>
                <input type="text" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:border-emerald-500 outline-none">
              </div>
              <div>
                <label class="block text-[10px] font-bold text-slate-400 uppercase mb-1">User Type</label>
                <select class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:border-emerald-500 outline-none appearance-none">
                  <option>Doctor</option>
                  <option>Nurse</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-[10px] font-bold text-slate-400 uppercase mb-1">PRC License No.</label>
              <input type="text" placeholder="0000000" class="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:border-emerald-500 outline-none">
            </div>
          </div>

          <div class="p-4 bg-slate-50 border-t border-slate-100 flex gap-2">
             <button @click="showAddModal = false" class="flex-1 py-2.5 text-xs font-bold text-slate-500 hover:bg-slate-100 rounded-lg transition-all">CANCEL</button>
             <button class="flex-1 py-2.5 text-xs font-bold bg-emerald-600 text-white hover:bg-emerald-700 rounded-lg shadow-sm transition-all">SAVE EMPLOYEE</button>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { Plus, Search, MoreHorizontal, X } from 'lucide-vue-next'
import StaffSidebar from './StaffSidebar.vue'
import StaffTopbar from './StaffTopbar.vue'

const showAddModal = ref(false)

// Mock Data for Staff
const staffMembers = ref([
  { id: '2024-001', firstName: 'Elena', middleName: 'M.', lastName: 'Rodriguez', type: 'Doctor', contact: '0917-555-0123', prcNo: '1234567' },
  { id: '2024-002', firstName: 'Marcus', middleName: 'L.', lastName: 'Pineda', type: 'Nurse', contact: '0918-444-9876', prcNo: '7654321' },
  { id: '2024-003', firstName: 'Sarah', middleName: 'G.', lastName: 'Concepcion', type: 'Doctor', contact: '0919-333-5566', prcNo: '2468135' },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #E2E8F0; border-radius: 10px; }
</style>