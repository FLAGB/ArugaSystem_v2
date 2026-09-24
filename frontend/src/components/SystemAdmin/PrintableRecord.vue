<script setup>
/**
 * Child Immunization Record — Phase 1
 * -------------------------------------------------
 * This is a BLANK, PRINTABLE immunization card template.
 * It is NOT wired to any child/patient data.
 *
 * Data sources (the only two endpoints this page calls):
 *   GET /api/Vaccines
 *   GET /api/VaccineDoses
 *
 * Every vaccine row and every date box on the page is generated
 * from those two endpoints. Nothing is hardcoded. If a vaccine or
 * dose is added, removed, or changed in the database, this page
 * reflects it automatically with no component changes.
 *
 * Phase 2 (later, NOT part of this file) will wire this template
 * to a real child's record and auto-fill the blanks below.
 */
import axios from "axios"
import { ref, computed, onMounted } from "vue"

/* ------------------------------- API config ------------------------------- */
const vaccineApi = "http://localhost:57147/api/Vaccines"
const doseApi = "http://localhost:57147/api/VaccineDoses"

/* ------------------------------ Vaccine data ------------------------------ */
const vaccines = ref([])
const allDoses = ref([])
const loading = ref(true)
const loadError = ref("")

async function loadVaccineData() {
  loading.value = true
  loadError.value = ""
  try {
    const [vaccineRes, doseRes] = await Promise.all([
      axios.get(vaccineApi),
      axios.get(doseApi),
    ])
    vaccines.value = vaccineRes.data ?? []
    allDoses.value = doseRes.data ?? []
  } catch (err) {
    loadError.value = "Unable to load vaccine schedule from the server."
  } finally {
    loading.value = false
  }
}

// Group doses by vaccine, sorted by dose number — no fixed dose count anywhere.
const dosesByVaccine = computed(() => {
  const map = {}
  for (const d of allDoses.value) {
    if (!map[d.vaccineID]) map[d.vaccineID] = []
    map[d.vaccineID].push(d)
  }
  Object.values(map).forEach((list) => list.sort((a, b) => a.doseNumber - b.doseNumber))
  return map
})

// One row per vaccine, each carrying its own dynamically-sized dose list.
// The number of date boxes rendered always equals the number of VaccineDose
// rows for that vaccine — this is the only thing that drives box count.
const scheduleRows = computed(() =>
  vaccines.value.map((v) => ({
    ...v,
    doses: dosesByVaccine.value[v.vaccineID] || [],
  }))
)

onMounted(() => {
  loadVaccineData()
})

function printRecord() {
  window.print()
}
</script>

<template>
  <div class="min-h-screen bg-slate-100">
    <!-- ============================ ON-SCREEN TOOLBAR (hidden on print) ============================ -->
    <div class="no-print sticky top-0 z-20 bg-white border-b border-slate-200">
      <div class="max-w-[1100px] mx-auto px-6 py-4 flex items-center justify-between gap-3">
        <div>
          <h1 class="text-base font-bold text-slate-900">Child Immunization Record</h1>
          <p class="text-xs text-slate-500">Blank printable card — vaccine schedule generated from Vaccine Management.</p>
        </div>
        <button
          @click="printRecord"
          class="text-sm font-semibold px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 transition-colors shrink-0"
        >
          🖨️ Print
        </button>
      </div>
      <p v-if="loadError" class="max-w-[1100px] mx-auto px-6 pb-3 -mt-1 text-xs text-rose-600">{{ loadError }}</p>
    </div>

    <!-- ============================ PRINTABLE CARD ============================ -->
    <main class="max-w-[1100px] mx-auto px-6 py-8 print:p-0 print:max-w-none">
      <div id="immunization-card" class="bg-white border border-slate-200 rounded-xl shadow-sm print:shadow-none print:border-0 print:rounded-none p-8 print:p-6">
        <!-- Title -->
        <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight pb-5 border-b-2 border-slate-900">
          Child Immunization Record
        </h1>

        <!-- Blank child / parent / facility info — printable only, no data binding -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-x-10 gap-y-3 py-6 border-b-2 border-slate-900">
          <div class="space-y-3">
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Child's Name:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Date of Birth:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Place of Birth:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Address:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
          </div>

          <div class="space-y-3">
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Mother's Name:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Father's Name:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Birth Height:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Birth Weight:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-center gap-4 pt-1">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Sex:</span>
              <span class="inline-flex items-center gap-1.5 text-sm text-slate-900">
                <span class="w-3.5 h-3.5 rounded-full border border-slate-500 inline-block"></span>
                Male
              </span>
              <span class="inline-flex items-center gap-1.5 text-sm text-slate-900">
                <span class="w-3.5 h-3.5 rounded-full border border-slate-500 inline-block"></span>
                Female
              </span>
            </div>
          </div>

          <div class="space-y-3">
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Health Center:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Barangay:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
            <div class="flex items-baseline gap-2">
              <span class="text-sm font-semibold text-slate-700 shrink-0">Family No.:</span>
              <span class="flex-1 border-b border-slate-400 min-h-[20px]"></span>
            </div>
          </div>
        </div>

        <!-- Vaccine schedule — fully data-driven from /api/Vaccines + /api/VaccineDoses -->
        <div class="mt-6">
          <div v-if="loading" class="text-sm text-slate-400 text-center py-12">Loading vaccine schedule…</div>

          <div v-else-if="loadError" class="text-sm text-rose-600 text-center py-12">
            Unable to load vaccine schedule from the server.
          </div>

          <div v-else-if="scheduleRows.length === 0" class="text-sm text-slate-400 text-center py-12 whitespace-pre-line">
            No vaccines configured yet.
            Please add vaccines and doses in Vaccine Management.
          </div>

          <table v-else class="w-full border-collapse text-sm">
            <thead>
              <tr class="bg-amber-400 print:bg-amber-400">
                <th class="border border-slate-900 text-left font-bold text-slate-900 px-3 py-2 w-[190px]">Bakuna</th>
                <th class="border border-slate-900 text-center font-bold text-slate-900 px-3 py-2 w-[90px]">Doses</th>
                <th class="border border-slate-900 text-left font-bold text-slate-900 px-3 py-2">
                  Petsa ng bakuna
                  <span class="block text-[10px] font-medium">MM/DD/YY</span>
                </th>
                <th class="border border-slate-900 text-left font-bold text-slate-900 px-3 py-2 min-w-[160px]">Remarks</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="vaccine in scheduleRows"
                :key="vaccine.vaccineID"
                class="break-inside-avoid align-top"
              >
                <td class="border border-slate-300 px-3 py-2 align-middle">
                  <p class="font-semibold text-slate-900 leading-snug">{{ vaccine.vaccineName }}</p>
                  <p v-if="vaccine.abbreviation" class="text-xs text-slate-400">{{ vaccine.abbreviation }}</p>
                </td>

                <td class="border border-slate-300 px-3 py-2 align-middle text-center">
                  <span class="inline-flex items-center justify-center w-6 h-6 rounded-full bg-amber-400 text-slate-900 font-bold text-xs mb-1">
                    {{ vaccine.doses.length }}
                  </span>
                  <span v-if="vaccine.recommendedAge" class="block text-[11px] text-slate-500 leading-tight">
                    {{ vaccine.recommendedAge }}
                  </span>
                </td>

                <td class="border border-slate-300 px-2 py-2 align-middle">
                  <div v-if="vaccine.doses.length" class="flex flex-wrap gap-0">
                    <!-- One empty date box per VaccineDose row — count is never hardcoded -->
                    <div
                      v-for="dose in vaccine.doses"
                      :key="dose.doseID"
                      class="relative w-[78px] h-9 border border-slate-300 -ml-px first:ml-0 flex items-center justify-center shrink-0"
                    >
                      <span class="absolute top-0.5 left-1 text-[9px] text-slate-400 leading-none">{{ dose.doseNumber }}</span>
                    </div>
                  </div>
                  <span v-else class="text-xs text-slate-400 pl-1">No doses configured</span>
                </td>

                <td class="border border-slate-300 px-3 py-2 align-middle"></td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Footer note -->
        <p class="mt-6 text-[11px] leading-relaxed text-slate-500 border-t border-slate-200 pt-4">
          Sa column ng <strong>Petsa ng bakuna</strong>, isulat ang petsa ng pagbibigay ng bakuna ayon sa kung
          pang-ilang dose ito. Sa column ng <strong>Remarks</strong>, isulat ang petsa ng pagbalik para sa
          susunod na dose, o anumang mahalagang impormasyon na maaaring makaapekto sa pagbabakuna ng bata.
        </p>
      </div>
    </main>
  </div>
</template>

<style scoped>
@media print {
  .no-print {
    display: none !important;
  }

  @page {
    size: A4;
    margin: 12mm;
  }

  body,
  .min-h-screen {
    background: #fff !important;
  }

  table,
  th,
  td {
    -webkit-print-color-adjust: exact;
    print-color-adjust: exact;
  }

  #immunization-card {
    page-break-inside: avoid;
  }
}
</style>