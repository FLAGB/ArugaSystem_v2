<template>
  <div class="flex h-screen bg-stone-50 antialiased text-stone-900" style="font-family: 'Inter','Segoe UI',sans-serif;">
    
    <StaffSidebar />

    <!-- MAIN SECTION -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <StaffTopbar title="Clinic Reports" breadcrumb="Aruga / Reports">
        <select class="bg-white border border-slate-200 text-xs font-bold px-3 py-2 rounded-lg outline-none focus:border-emerald-500">
          <option>Year 2024</option>
          <option>Year 2025</option>
          <option>Year 2026</option>
        </select>
        <button class="bg-emerald-600 text-white px-4 py-2 rounded-lg text-xs font-bold hover:bg-emerald-700 transition-colors flex items-center gap-2">
          <FileDown class="w-3.5 h-3.5" /> DOWNLOAD PDF
        </button>
      </StaffTopbar>

      <main class="flex-1 p-8 overflow-y-auto custom-scrollbar">
        <div class="max-w-7xl mx-auto space-y-6">
          
          <!-- TOP ROW: TWO CHARTS -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            
            <!-- 1. VACCINATION PER AGE -->
            <div class="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
              <div class="flex justify-between items-start mb-6">
                <div>
                  <h3 class="font-bold text-slate-800">Vaccination per Age</h3>
                  <p class="text-xs text-slate-500">Distribution across age groups</p>
                </div>
                <BarChart3 class="w-5 h-5 text-emerald-500" />
              </div>
              <!-- Chart Visual Placeholder -->
              <div class="h-64 flex items-end justify-around gap-2 px-2">
                <div v-for="(val, i) in [40, 70, 90, 50, 30]" :key="i" 
                  class="w-full bg-emerald-100 rounded-t-lg relative group transition-all hover:bg-emerald-500" 
                  :style="{ height: val + '%' }">
                  <span class="absolute -top-6 left-1/2 -translate-x-1/2 text-[10px] font-bold text-slate-600 opacity-0 group-hover:opacity-100">{{ val }}%</span>
                </div>
              </div>
              <div class="flex justify-around mt-4 text-[10px] font-bold text-slate-400 uppercase">
                <span>0-1y</span><span>2-3y</span><span>4-6y</span><span>7-10y</span><span>11y+</span>
              </div>
            </div>

            <!-- 2. VACCINE COVERAGE REPORT -->
            <div class="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
              <div class="flex justify-between items-start mb-6">
                <div>
                  <h3 class="font-bold text-slate-800">Vaccine Coverage Report</h3>
                  <p class="text-xs text-slate-500">Total doses per vaccine type</p>
                </div>
                <Syringe class="w-5 h-5 text-emerald-500" />
              </div>
              <!-- Chart Visual Placeholder -->
              <div class="h-64 flex items-end justify-around gap-4 px-2">
                <div v-for="(val, i) in [85, 60, 45, 95]" :key="i" 
                  class="w-full bg-slate-100 rounded-t-lg relative group transition-all hover:bg-emerald-500" 
                  :style="{ height: val + '%' }">
                </div>
              </div>
              <div class="flex justify-around mt-4 text-[10px] font-bold text-slate-400 uppercase">
                <span>BCG</span><span>Polio</span><span>MMR</span><span>HepB</span>
              </div>
            </div>
          </div>

          <!-- BOTTOM ROW: FULL WIDTH CHART -->
          <!-- 3. VACCINATION PER MONTH (YEAR) -->
          <div class="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
            <div class="flex justify-between items-start mb-6">
              <div>
                <h3 class="font-bold text-slate-800">Vaccination per Month (2026)</h3>
                <p class="text-xs text-slate-500">Monthly administrative growth</p>
              </div>
              <CalendarDays class="w-5 h-5 text-emerald-500" />
            </div>
            <!-- Chart Visual Placeholder -->
            <div class="h-64 flex items-end justify-between gap-2 px-2 border-b border-slate-100">
              <div v-for="(val, i) in [30, 45, 35, 60, 80, 55, 90, 75, 40, 50, 65, 85]" :key="i" 
                class="w-full bg-emerald-600/10 rounded-t flex flex-col justify-end group transition-all hover:bg-emerald-600" 
                :style="{ height: val + '%' }">
              </div>
            </div>
            <div class="flex justify-between mt-4 text-[10px] font-bold text-slate-400 uppercase px-1">
              <span v-for="m in ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']" :key="m">{{m}}</span>
            </div>
          </div>

        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { Syringe, FileDown, BarChart3, CalendarDays } from 'lucide-vue-next'
import StaffSidebar from './StaffSidebar.vue'
import StaffTopbar from './StaffTopbar.vue'
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #E2E8F0; border-radius: 10px; }
</style>