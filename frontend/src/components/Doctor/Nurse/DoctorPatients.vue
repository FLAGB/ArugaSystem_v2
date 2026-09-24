<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <DoctorSidebar />

    <!-- MAIN -->
    <div class="flex flex-col flex-1 overflow-hidden">

      <!-- TOP BAR -->
      <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 shrink-0">
        <div></div>
        <div class="flex items-center gap-4">
          <button @click="showNotifications = true" class="relative p-2 text-slate-400 hover:text-slate-600 transition-colors">
            <Bell class="w-5 h-5" />
            <span v-if="unreadCount > 0" class="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
          </button>
          <div class="flex items-center gap-3">
            <div class="text-right">
              <p class="text-sm font-semibold">{{ doctor.fullName }}</p>
              <p class="text-xs text-slate-400">{{ doctor.userType }}</p>
            </div>
            <div class="w-9 h-9 bg-emerald-700 rounded-full flex items-center justify-center text-white text-sm font-bold">
              {{ doctorInitials }}
            </div>
          </div>
        </div>
      </header>

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <template v-if="selectedChild">

          <button
            @click="closeRecord"
            class="flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-700 mb-4 transition-colors"
          >
            <ArrowLeft class="w-4 h-4" />
            Back to Patients
          </button>

          <div class="grid grid-cols-3 gap-5">

            <!-- ================= PROFILE CARD ================= -->
            <div class="bg-white rounded-xl border border-slate-200 p-5">

              <div
                class="w-16 h-16 rounded-full flex items-center justify-center text-xl font-bold text-white mb-3"
                :style="{ backgroundColor: avatarColor(selectedChild.fullName) }"
              >
                {{ initials(selectedChild.fullName) }}
              </div>

              <h2 class="text-lg font-bold text-slate-800">
                {{ selectedChild.fullName }}
              </h2>

              <p class="text-sm text-slate-400">
                {{ selectedChild.age || '—' }}
                ·
                {{ selectedChild.sex || '—' }}
                <template v-if="selectedChild.parentName && selectedChild.parentName !== '—'">
                  · Parent: {{ selectedChild.parentName }}
                </template>
              </p>

              <div class="mt-4 space-y-4 text-sm">

                <div class="flex items-start gap-2.5">
                  <Cake class="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />
                  <div>
                    <p class="text-xs text-slate-400">Birthdate</p>
                    <p class="text-slate-700">
                      {{ selectedChild.birthDate ? formatDate(selectedChild.birthDate) : '—' }}
                    </p>
                  </div>
                </div>

                <div class="flex items-start gap-2.5">
                  <MapPin class="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />
                  <div>
                    <p class="text-xs text-slate-400">Barangay</p>
                    <p class="text-slate-700">{{ selectedChild.barangay || '—' }}</p>
                  </div>
                </div>

                <div class="flex items-start gap-2.5">
                  <Phone class="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />
                  <div>
                    <p class="text-xs text-slate-400">Primary Contact No.</p>
                    <p class="text-slate-700">{{ selectedChild.primaryContactNo || 'Not available' }}</p>
                  </div>
                </div>

              </div>
            </div>

            <!-- ================= MEDICAL RECORDS ================= -->
            <div class="col-span-2 bg-white rounded-xl border border-slate-200 p-5">

              <div class="flex items-center justify-between mb-5">
                <div>
                  <h3 class="font-semibold text-slate-800">Medical Records</h3>
                  <p class="text-xs text-slate-400 mt-0.5">Medical information and health measurements</p>
                </div>
                <div class="w-9 h-9 rounded-lg bg-emerald-50 flex items-center justify-center">
                  <HeartPulse class="w-4 h-4 text-emerald-600" />
                </div>
              </div>

              <div class="grid grid-cols-2 gap-4">
                <div>
                  <p class="text-xs text-slate-400 mb-1.5">Allergies</p>
                  <div class="min-h-[48px] px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                    {{ selectedChild.allergies || 'None reported' }}
                  </div>
                </div>
                <div>
                  <p class="text-xs text-slate-400 mb-1.5">Existing Conditions</p>
                  <div class="min-h-[48px] px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                    {{ selectedChild.existingConditions || 'None reported' }}
                  </div>
                </div>
                <div>
                  <p class="text-xs text-slate-400 mb-1.5">Birth Height</p>
                  <div class="px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                    {{ selectedChild.birthHeight || 'Not available' }}
                  </div>
                </div>
                <div>
                  <p class="text-xs text-slate-400 mb-1.5">Birth Weight</p>
                  <div class="px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                    {{ selectedChild.birthWeight || 'Not available' }}
                  </div>
                </div>
              </div>
            </div>

            <!-- ================= PATIENT DETAILS ================= -->
            <div class="bg-white rounded-xl border border-slate-200 p-5">
              <h3 class="font-semibold text-slate-800 mb-4">Patient Details</h3>
              <div class="space-y-3 text-sm">
                <div>
                  <p class="text-xs text-slate-400">Full Name</p>
                  <p class="text-slate-700">{{ selectedChild.fullName }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-400">Sex</p>
                  <p class="text-slate-700">{{ selectedChild.sex || '—' }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-400">Age</p>
                  <p class="text-slate-700">{{ selectedChild.age || '—' }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-400 mb-1">Vaccine Status</p>
                  <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                    :class="statusClass(selectedChild.vaccineStatus)">
                    {{ selectedChild.vaccineStatus }}
                  </span>
                </div>
              </div>
            </div>

            <!-- ================= NEXT VACCINATION ================= -->
            <div class="col-span-2 bg-white rounded-xl border border-slate-200 p-5">
              <div class="flex items-center justify-between mb-4">
                <h3 class="font-semibold text-slate-800">Next Vaccination</h3>
                <button @click="attemptVaccinateFromSearch(selectedChild)" title="Recording still only happens from Home when this child is Now Serving"
                  class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-slate-500 bg-slate-100 hover:bg-slate-200 rounded-lg transition-colors">
                  <Syringe class="w-3.5 h-3.5" /> Record Vaccine
                </button>
              </div>

              <div v-if="selectedChild.nextDueDate" class="grid grid-cols-3 gap-4">
                <div>
                  <p class="text-xs text-slate-400">Vaccine</p>
                  <p class="font-semibold text-slate-800 mt-1">{{ selectedChild.nextVaccineName }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-400">Scheduled Date</p>
                  <p class="text-sm text-slate-700 mt-1">{{ formatDate(selectedChild.nextDueDate) }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-400">Status</p>
                  <span class="inline-flex mt-1 px-2.5 py-1 rounded-full text-xs font-medium bg-amber-50 text-amber-700">
                    {{ selectedChild.vaccineStatus }}
                  </span>
                </div>
              </div>
              <div v-else class="text-sm text-slate-400">
                {{ selectedChild.vaccineStatus === 'Completed' ? 'Series complete.' : 'No upcoming vaccination scheduled.' }}
              </div>
            </div>

          </div>

          <!-- ================= VACCINATION HISTORY ================= -->
          <div class="mt-5 bg-white rounded-xl border border-slate-200">

            <div class="px-5 py-4 border-b border-slate-100">
              <h3 class="font-semibold text-slate-800">Vaccination History</h3>
            </div>

            <div v-if="loadingChildRecords" class="flex items-center justify-center py-10">
              <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
            </div>

            <div v-else-if="childRecords.length === 0" class="flex flex-col items-center justify-center py-10 text-slate-400">
              <Syringe class="w-7 h-7 mb-2 opacity-40" />
              <p class="text-sm">No vaccination records yet</p>
            </div>

            <div v-else class="overflow-x-auto">
              <table class="w-full text-sm">
                <thead>
                  <tr class="text-xs text-slate-400 uppercase tracking-wide border-b border-slate-100">
                    <th class="px-5 py-2.5 text-left font-medium">Vaccine</th>
                    <th class="px-5 py-2.5 text-left font-medium">Dose</th>
                    <th class="px-5 py-2.5 text-left font-medium">Date</th>
                    <th class="px-5 py-2.5 text-left font-medium">Administered By</th>
                    <th class="px-5 py-2.5 text-left font-medium">Observation</th>
                    <th class="px-5 py-2.5 text-left font-medium">Doctor Diagnosis</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="rec in childRecords" :key="rec.recordID"
                    class="border-b border-slate-50 hover:bg-slate-50 transition-colors align-top">
                    <td class="px-5 py-3 font-medium text-slate-700">{{ rec.vaccineName }}</td>
                    <td class="px-5 py-3 text-slate-500">Dose {{ rec.doseNumber }}</td>
                    <td class="px-5 py-3 text-slate-500">{{ rec.dateAdministered ? formatDate(rec.dateAdministered) : '—' }}</td>
                    <td class="px-5 py-3 text-slate-500">{{ rec.administeredByName || '—' }}</td>
                    <td class="px-5 py-3 text-slate-500">{{ rec.nurseObservation || '—' }}</td>
                    <td class="px-5 py-3 text-slate-500 min-w-[220px]">

                      <div v-if="editingDiagnosisId === rec.recordID" class="flex items-start gap-2">
                        <textarea v-model="editDiagnosisText" rows="2"
                          placeholder="Clinical notes or diagnosis for this visit"
                          class="flex-1 px-2 py-1.5 text-xs border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 resize-none"
                        ></textarea>
                        <div class="flex flex-col gap-1 shrink-0">
                          <button @click="saveDiagnosis(rec)" :disabled="savingDiagnosis" title="Save"
                            class="p-1 rounded text-emerald-600 hover:bg-emerald-50 disabled:opacity-50 transition-colors">
                            <Loader2 v-if="savingDiagnosis" class="w-3.5 h-3.5 animate-spin" />
                            <Check v-else class="w-3.5 h-3.5" />
                          </button>
                          <button @click="cancelEditDiagnosis" title="Cancel"
                            class="p-1 rounded text-slate-400 hover:bg-slate-100 transition-colors">
                            <X class="w-3.5 h-3.5" />
                          </button>
                        </div>
                      </div>

                      <div v-else class="flex items-start justify-between gap-2 group">
                        <span>{{ rec.doctorDiagnosis || '—' }}</span>
                        <button @click="startEditDiagnosis(rec)" title="Edit diagnosis"
                          class="p-1 rounded text-slate-300 hover:text-emerald-600 hover:bg-emerald-50 opacity-0 group-hover:opacity-100 transition-opacity shrink-0">
                          <Pencil class="w-3.5 h-3.5" />
                        </button>
                      </div>

                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

        </template>

        <template v-else>

        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Patients (Children)</h1>
          <p class="text-sm text-slate-500 mt-1">Manage pediatric patient records and vaccinations</p>
        </div>

        <!-- FILTER ROW -->
        <div class="flex items-center gap-3 mb-5">
          <div class="relative flex-1 max-w-sm">
            <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
            <input v-model="searchQuery" type="text" placeholder="Search by name, ID, or parent..."
              class="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white" />
          </div>
          <select v-model="statusFilter"
            class="text-sm border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white text-slate-600">
            <option value="">All Status</option>
            <option value="Due Soon">Due Soon</option>
            <option value="Overdue">Overdue</option>
            <option value="Up to Date">Up to Date</option>
            <option value="Completed">Completed</option>
            <option value="Not Started">Not Started</option>
          </select>
          <select v-model="barangayFilter"
            class="text-sm border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white text-slate-600">
            <option value="">All Barangays</option>
            <option v-for="b in uniqueBarangays" :key="b" :value="b">{{ b }}</option>
          </select>
        </div>

        <!-- TABLE -->
        <div class="bg-white rounded-xl border border-slate-200 overflow-hidden">
          <div v-if="loading" class="flex items-center justify-center py-16">
            <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
            <span class="ml-2 text-sm text-slate-400">Loading patients...</span>
          </div>

          <div v-else-if="filteredPatients.length === 0" class="flex flex-col items-center justify-center py-16 text-slate-400">
            <Users class="w-10 h-10 mb-3 opacity-30" />
            <p class="text-sm">No patients found</p>
          </div>

          <div v-else class="overflow-x-auto">
            <table class="w-full text-sm">
              <thead class="bg-slate-50 border-b border-slate-200">
                <tr class="text-xs text-slate-500 uppercase tracking-wide">
                  <th class="px-5 py-3.5 text-left font-medium">Profile</th>
                  <th class="px-5 py-3.5 text-left font-medium text-slate-400">ID</th>
                  <th class="px-5 py-3.5 text-left font-medium cursor-pointer select-none hover:text-slate-700 transition-colors" @click="toggleSort('fullName')">
                    <div class="flex items-center gap-1">
                      Full Name
                      <component :is="sortIcon('fullName')" class="w-3 h-3" :class="sortField === 'fullName' ? 'text-emerald-600' : 'text-slate-300'" />
                    </div>
                  </th>
                  <th class="px-5 py-3.5 text-left font-medium cursor-pointer select-none hover:text-slate-700 transition-colors" @click="toggleSort('age')">
                    <div class="flex items-center gap-1">
                      Age
                      <component :is="sortIcon('age')" class="w-3 h-3" :class="sortField === 'age' ? 'text-emerald-600' : 'text-slate-300'" />
                    </div>
                  </th>
                  <th class="px-5 py-3.5 text-left font-medium">Sex</th>
                  <th class="px-5 py-3.5 text-left font-medium">Barangay</th>
                  <th class="px-5 py-3.5 text-left font-medium">Parent Name</th>
                  <th class="px-5 py-3.5 text-left font-medium">Next Dose Due</th>
                  <th class="px-5 py-3.5 text-left font-medium">Status</th>
                  <th class="px-5 py-3.5 text-left font-medium">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="child in paginatedPatients" :key="child.childID"
                  class="hover:bg-slate-50 transition-colors">
                  <td class="px-5 py-3.5">
                    <div class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold text-white"
                      :style="{ backgroundColor: avatarColor(child.fullName) }">
                      {{ initials(child.fullName) }}
                    </div>
                  </td>
                  <td class="px-5 py-3.5 text-slate-400 font-mono text-[11px]">
                    {{ child.childID.substring(0, 8).toUpperCase() }}
                  </td>
                  <td class="px-5 py-3.5 font-medium text-slate-800">{{ child.fullName }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ child.age }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ child.sex }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ child.barangay }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ child.parentName }}</td>
                  <td class="px-5 py-3.5 text-slate-500 text-xs">
                    <span v-if="child.nextDueDate">{{ formatDate(child.nextDueDate) }}</span>
                    <span v-else-if="child.vaccineStatus === 'Completed'" class="text-emerald-600 font-medium">Series complete</span>
                    <span v-else class="text-slate-400">—</span>
                  </td>
                  <td class="px-5 py-3.5">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="statusClass(child.vaccineStatus)">
                      {{ child.vaccineStatus }}
                    </span>
                  </td>
                  <td class="px-5 py-3.5">
                    <div class="flex items-center gap-2">
                      <button @click="openRecord(child)" title="View Profile"
                        class="p-1.5 text-slate-400 hover:text-emerald-600 hover:bg-emerald-50 rounded-lg transition-colors">
                        <Eye class="w-4 h-4" />
                      </button>
                      <button @click="attemptVaccinateFromSearch(child)" title="Recording still only happens from Home when this child is Now Serving"
                        class="p-1.5 text-slate-400 hover:text-emerald-600 hover:bg-emerald-50 rounded-lg transition-colors">
                        <Syringe class="w-4 h-4" />
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- PAGINATION -->
          <div v-if="filteredPatients.length > 0" class="flex items-center justify-between px-5 py-3.5 border-t border-slate-100 flex-wrap gap-3">
            <!-- Left: entries selector + count -->
            <div class="flex items-center gap-3">
              <div class="flex items-center gap-2 text-xs text-slate-500">
                <span>Show</span>
                <select v-model="pageSize" @change="currentPage = 1"
                  class="border border-slate-200 rounded-lg px-2 py-1 text-xs text-slate-600 outline-none bg-white">
                  <option :value="10">10</option>
                  <option :value="20">20</option>
                  <option :value="50">50</option>
                  <option :value="100">100</option>
                </select>
                <span>entries</span>
              </div>
              <p class="text-xs text-slate-400">
                Showing {{ (currentPage - 1) * pageSize + 1 }}–{{ Math.min(currentPage * pageSize, filteredPatients.length) }}
                of {{ filteredPatients.length }} patients
              </p>
            </div>
            <!-- Right: numbered pages -->
            <div class="flex items-center gap-1">
              <button @click="currentPage--" :disabled="currentPage === 1"
                class="p-1.5 rounded-lg text-slate-400 hover:text-slate-600 hover:bg-slate-100 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
                <ChevronLeft class="w-4 h-4" />
              </button>
              <template v-for="p in paginationPages" :key="p">
                <span v-if="p === '...'" class="px-1 text-xs text-slate-400">…</span>
                <button v-else @click="currentPage = p"
                  class="min-w-[28px] h-7 rounded-lg text-xs font-medium transition-colors"
                  :class="p === currentPage
                    ? 'bg-emerald-600 text-white'
                    : 'text-slate-600 hover:bg-slate-100'">
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

        <!-- STATUS LEGEND -->
        <div class="mt-4 flex flex-wrap items-center gap-4 text-xs text-slate-500">
          <span class="font-medium text-slate-400">Status guide:</span>
          <span class="flex items-center gap-1.5"><span class="w-2 h-2 rounded-full bg-amber-400"></span>Due Soon — next dose within 14 days</span>
          <span class="flex items-center gap-1.5"><span class="w-2 h-2 rounded-full bg-red-400"></span>Overdue — past 14-day catch-up window</span>
          <span class="flex items-center gap-1.5"><span class="w-2 h-2 rounded-full bg-blue-400"></span>Up to Date — next dose not yet due</span>
          <span class="flex items-center gap-1.5"><span class="w-2 h-2 rounded-full bg-emerald-500"></span>Completed — full vaccine series done</span>
          <span class="flex items-center gap-1.5"><span class="w-2 h-2 rounded-full bg-slate-400"></span>Not Started — no vaccines recorded yet</span>
        </div>
        </template>

      </main>
    </div>


    <!-- ── RECORD VACCINATION MODAL (Option B) ─────────────────────────────── -->
    <Transition name="modal">
      <div v-if="showVaccinateModal && selectedChild"
        class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm"
        @click.self="showVaccinateModal = false">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md overflow-hidden">

          <div class="bg-emerald-600 px-6 py-5 flex items-center justify-between">
            <div>
              <p class="text-white font-bold leading-tight">Record Vaccination</p>
              <p class="text-emerald-200 text-xs mt-0.5">{{ selectedChild.fullName }}</p>
            </div>
            <button @click="showVaccinateModal = false" class="text-white/70 hover:text-white text-xl">✕</button>
          </div>

          <div class="p-6 space-y-4">

            <!-- Patient summary -->
            <div class="bg-slate-50 rounded-xl p-4 flex items-center gap-3">
              <div class="w-10 h-10 rounded-full flex items-center justify-center text-sm font-bold text-white"
                :style="{ backgroundColor: avatarColor(selectedChild.fullName) }">
                {{ initials(selectedChild.fullName) }}
              </div>
              <div>
                <p class="font-semibold text-slate-800">{{ selectedChild.fullName }}</p>
                <p class="text-xs text-slate-500">{{ selectedChild.age }} · {{ selectedChild.sex }} · Parent: {{ selectedChild.parentName }}</p>
              </div>
            </div>

            <!-- VACCINE selector (Option B: pick from all vaccines + doses) -->
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

            <!-- DOSE NUMBER selector — filtered to selected vaccine -->
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

            <div>
              <label class="text-xs font-medium text-slate-600 uppercase tracking-wide block mb-1.5">
                Doctor Diagnosis
              </label>
              <textarea v-model="vaccinateForm.diagnosis" rows="2"
                placeholder="Optional — clinical notes or diagnosis for this visit"
                class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 resize-none">
              </textarea>
            </div>

            <!-- Accountability notice -->
            <div class="flex items-start gap-2 p-3 bg-amber-50 rounded-lg border border-amber-100">
              <Info class="w-4 h-4 text-amber-500 shrink-0 mt-0.5" />
              <p class="text-xs text-amber-700">
                This will be permanently saved under <strong>{{ doctor.fullName }}</strong>
                and visible to the parent.
              </p>
            </div>
          </div>

          <div class="px-6 pb-5 flex gap-3">
            <button @click="showVaccinateModal = false"
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
    </Transition>

    <!-- SUCCESS TOAST -->
    <Transition name="toast">
      <div v-if="toast.show"
        class="fixed bottom-6 right-6 z-50 bg-emerald-600 text-white px-5 py-3.5 rounded-xl shadow-lg flex items-center gap-3 text-sm font-medium">
        <CheckCircle class="w-5 h-5" />
        {{ toast.message }}
      </div>
    </Transition>

    <!-- NOTIFICATION PANEL -->
    <Transition name="notif-panel">
      <div v-if="showNotifications" class="fixed inset-0 z-[200] flex justify-end" @click.self="showNotifications = false">
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm" @click="showNotifications = false"></div>
        <div class="relative w-full max-w-[400px] h-full bg-white shadow-2xl flex flex-col">
          <div class="bg-slate-800 px-6 py-5 flex items-center justify-between text-white shrink-0">
            <div class="flex items-center gap-3">
              <Bell class="w-5 h-5" />
              <span class="font-bold text-base">Notifications</span>
              <span v-if="unreadCount > 0" class="bg-red-500 text-[10px] px-2.5 py-1 rounded-full font-bold">{{ unreadCount }} NEW</span>
            </div>
            <div class="flex items-center gap-3">
              <button v-if="unreadCount > 0" @click="markAllRead" class="text-[10px] font-bold text-white/70 hover:text-white uppercase tracking-wide transition-colors">Mark all read</button>
              <button @click="showNotifications = false" class="w-8 h-8 rounded-lg bg-white/10 hover:bg-white/20 flex items-center justify-center text-lg transition-colors">✕</button>
            </div>
          </div>
          <div class="flex-1 overflow-y-auto">
            <div v-if="notifsLoading" class="flex flex-col items-center justify-center h-40 text-slate-400">
              <p class="text-2xl mb-2 animate-pulse">🔔</p><p class="text-xs font-medium">Loading notifications...</p>
            </div>
            <div v-else-if="notifications.length === 0" class="flex flex-col items-center justify-center h-40 text-slate-400 px-8 text-center">
              <p class="text-3xl mb-3">✅</p><p class="text-sm font-bold text-slate-600">All caught up!</p><p class="text-xs text-slate-400 mt-1">No notifications.</p>
            </div>
            <div v-else class="p-3 space-y-2">
              <div v-for="notif in notifications" :key="notif.notificationID" @click="markRead(notif)"
                   :class="notif.isRead ? 'bg-white border-transparent' : 'bg-emerald-50 border-emerald-200'"
                   class="p-4 rounded-xl border cursor-pointer hover:shadow-sm transition-all">
                <div class="flex items-start gap-3">
                  <div class="w-10 h-10 rounded-lg bg-slate-100 flex items-center justify-center text-lg shrink-0">🔔</div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-start justify-between gap-2">
                      <p class="text-xs font-bold text-slate-800 leading-tight">{{ notif.title }}</p>
                      <span v-if="!notif.isRead" class="w-2 h-2 rounded-full bg-emerald-500 shrink-0 mt-1"></span>
                    </div>
                    <p class="text-[11px] text-slate-500 mt-1 leading-relaxed">{{ notif.message }}</p>
                    <p class="text-[9px] text-slate-400 mt-2 font-bold uppercase tracking-wide">{{ formatRelativeTime(notif.createdAt) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>

  </div>
</template>

<script setup>
import { getUser } from '@/utils/auth'
import DoctorSidebar from './DoctorSidebar.vue'
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import axios from 'axios'
import {
  Home, Users, Calendar, Syringe, FileText, Settings,
  Search, Bell, LogOut, Loader2, X, CheckCircle, Info,
  Eye, ChevronLeft, ChevronRight, ArrowUp, ArrowDown, ArrowUpDown,
  ArrowLeft, Cake, MapPin, Phone, HeartPulse, Pencil, Check,
  ListChecks
} from 'lucide-vue-next'

const API = import.meta.env.VITE_API_URL || 'http://localhost:57147'
const router = useRouter()
const route  = useRoute()

// ── Auth ──────────────────────────────────────────────────────────────────
const doctor = ref({ userId: '', fullName: '', userType: '', prcNo: '' })
onMounted(() => {
  const u = getUser()
if (!u) { router.push('/'); return }
  doctor.value = {
    userId:   u.UserID,
    fullName: `${u.FirstName} ${u.LastName}`,
    userType: u.UserType,
    prcNo:    u.PRCNo || '',
  }
  fetchPatients()
  fetchVaccines()   // ← preload vaccines for the modal
  fetchNotifications()
})

// ── Notifications ────────────────────────────────────────────
const showNotifications = ref(false)
const notifications     = ref([])
const notifsLoading     = ref(false)
const unreadCount        = computed(() => notifications.value.filter(n => !n.isRead).length)

function formatRelativeTime(dateStr) {
  if (!dateStr) return ''
  const diff  = Date.now() - new Date(dateStr).getTime()
  const mins  = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days  = Math.floor(diff / 86400000)
  if (mins < 1)   return 'Just now'
  if (mins < 60)  return `${mins}m ago`
  if (hours < 24) return `${hours}h ago`
  if (days < 7)   return `${days}d ago`
  return new Date(dateStr).toLocaleDateString('en-PH', { month: 'short', day: 'numeric' })
}

async function fetchNotifications() {
  if (!doctor.value.userId) return
  notifsLoading.value = true
  try {
    const res = await axios.get(`${API}/api/Notifications/user/${doctor.value.userId}`)
    notifications.value = res.data
  } catch {
    notifications.value = []
  } finally {
    notifsLoading.value = false
  }
}

async function markRead(notif) {
  if (notif.isRead) return
  notif.isRead = true
  try { await axios.patch(`${API}/api/Notifications/mark-read/${notif.notificationID}`) } catch {}
}

async function markAllRead() {
  notifications.value.forEach(n => { n.isRead = true })
  try { await axios.patch(`${API}/api/Notifications/mark-all-read/user/${doctor.value.userId}`) } catch {}
}

const doctorInitials = computed(() =>
  doctor.value.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
)

// ── Nav ───────────────────────────────────────────────────────────────────
// ── DOH Vaccine master ────────────────────────────────────────────────────
const VACCINE_MASTER = [
  { vaccineId: 1, name: 'BCG Vaccine',                      totalDoses: 1 },
  { vaccineId: 2, name: 'Hepatitis B Vaccine',              totalDoses: 1 },
  { vaccineId: 3, name: 'Pentavalent (DPT-Hep B-HIB)',      totalDoses: 3, gap: 28 },
  { vaccineId: 4, name: 'Oral Polio Vaccine (OPV)',         totalDoses: 3, gap: 28 },
  { vaccineId: 5, name: 'Inactivated Polio Vaccine (IPV)',  totalDoses: 2, gap: 165 },
  { vaccineId: 6, name: 'Pneumococcal Conj. Vaccine (PCV)', totalDoses: 3, gap: 28 },
  { vaccineId: 7, name: 'MMR Vaccine',                      totalDoses: 2, gap: 90 },
]
const TOTAL_DOSES = VACCINE_MASTER.reduce((sum, v) => sum + v.totalDoses, 0)

// ── Data ──────────────────────────────────────────────────────────────────
const patients       = ref([])
const loading        = ref(false)
const searchQuery    = ref('')
const statusFilter   = ref('')
const barangayFilter = ref('')
const currentPage    = ref(1)
const pageSize       = ref(10)

// ── Sorting ───────────────────────────────────────────────────────────────
const sortField = ref(null)      // 'fullName' | 'age' | null
const sortDir   = ref('asc')     // 'asc' | 'desc'

function toggleSort(field) {
  if (sortField.value === field) {
    sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortDir.value = 'asc'
  }
  currentPage.value = 1
}

function sortIcon(field) {
  if (sortField.value !== field) return ArrowUpDown
  return sortDir.value === 'asc' ? ArrowUp : ArrowDown
}

async function fetchPatients() {
  loading.value = true
  try {
    const childrenRes = await axios.get(`${API}/api/Children/all`)
    const children    = childrenRes.data

    let allRecords = []
    try {
      const recRes = await axios.get(`${API}/api/VaccinationRecords/all`)
      allRecords = recRes.data
    } catch {
      for (const c of children) {
        try {
          const r = await axios.get(`${API}/api/VaccinationRecords/child/${c.childID}`)
          allRecords.push(...r.data.map(rec => ({ ...rec, childID: c.childID })))
        } catch {}
      }
    }

    const today = new Date()
    today.setHours(0, 0, 0, 0)

    patients.value = await Promise.all(children.map(async c => {
      const childRecords = allRecords.filter(r =>
        (r.childID ?? r.ChildID)?.toString().toLowerCase() === c.childID?.toString().toLowerCase()
      )

      const parentName = c.parentName || '—'

      const hasAnyRecords  = childRecords.length > 0
      const completedCount = childRecords.filter(r => r.status === 'Completed').length
      const allDone        = hasAnyRecords && completedCount >= TOTAL_DOSES

      // GET /api/VaccinationRecords/all (ShapeRecords) sends vaccinationDate;
      // the per-child fallback below (/api/VaccinationRecords/child/{id})
      // sends scheduledDate for not-yet-given doses. Check both so this
      // works no matter which endpoint actually supplied the record —
      // previously this only checked scheduledDate, which the primary
      // "/all" path never sends, so pendingRecords was always empty and
      // every patient showed "No upcoming vaccination scheduled".
      const pendingRecords = childRecords
        .filter(r => r.status !== 'Completed' && (r.vaccinationDate ?? r.scheduledDate))
        .sort((a, b) => new Date(a.vaccinationDate ?? a.scheduledDate) - new Date(b.vaccinationDate ?? b.scheduledDate))

      const nextRecord      = pendingRecords[0] ?? null
      const nextDueDate     = nextRecord ? new Date(nextRecord.vaccinationDate ?? nextRecord.scheduledDate) : null
      const nextVaccineName = nextRecord
        ? (nextRecord.vaccineName ?? `Vaccine Dose ${nextRecord.doseNumber}`)
        : null

      let vaccineStatus = 'Not Started'
      if (!hasAnyRecords) {
        vaccineStatus = 'Not Started'
      } else if (allDone) {
        vaccineStatus = 'Completed'
      } else if (nextDueDate) {
        const diffDays = Math.floor((nextDueDate - today) / 86400000)
        if (diffDays < -14)      vaccineStatus = 'Overdue'
        else if (diffDays <= 14) vaccineStatus = 'Due Soon'
        else                     vaccineStatus = 'Up to Date'
      } else {
        vaccineStatus = 'Up to Date'
      }

      return {
        childID:        c.childID,
        fullName:       `${c.firstName} ${c.lastName}`,
        age:            computeAge(c.birthDate),
        ageMonths:      computeAgeMonths(c.birthDate),
        sex:            c.sex ?? '—',
        barangay:       c.barangay ?? c.Barangay ?? '—',
        parentName,
        birthDate:      c.birthDate,
        nextDueDate,
        nextVaccineName,
        vaccineStatus,
        completedCount,
      }
    }))
  } catch (err) {
    console.error('fetchPatients error:', err)
  } finally {
    loading.value = false
  }
}

function computeAge(birthDate) {
  if (!birthDate) return '—'
  const birth = new Date(birthDate)
  const now   = new Date()
  let years   = now.getFullYear() - birth.getFullYear()
  let months  = now.getMonth() - birth.getMonth()
  if (months < 0) { years--; months += 12 }
  if (years === 0) return `${months} mo${months !== 1 ? 's' : ''}`
  if (months === 0) return `${years} yr${years !== 1 ? 's' : ''}`
  return `${years} yr${years !== 1 ? 's' : ''} ${months} mo${months !== 1 ? 's' : ''}`
}

// Numeric age-in-months, used for sorting the Age column (the display
// string "X yrs Y mos" isn't directly comparable).
function computeAgeMonths(birthDate) {
  if (!birthDate) return -1
  const birth = new Date(birthDate)
  const now   = new Date()
  return (now.getFullYear() - birth.getFullYear()) * 12 + (now.getMonth() - birth.getMonth())
}

// ── Filters / Sort / Pagination ──────────────────────────────────────────
const uniqueBarangays = computed(() =>
  [...new Set(patients.value.map(c => c.barangay).filter(b => b && b !== '—'))].sort()
)

const filteredPatients = computed(() => {
  let list = patients.value
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    list = list.filter(c =>
      c.fullName.toLowerCase().includes(q) ||
      c.childID.toLowerCase().includes(q) ||
      c.parentName.toLowerCase().includes(q)
    )
  }
  if (statusFilter.value)
    list = list.filter(c => c.vaccineStatus === statusFilter.value)
  if (barangayFilter.value)
    list = list.filter(c => c.barangay === barangayFilter.value)
  return list
})

const sortedPatients = computed(() => {
  if (!sortField.value) return filteredPatients.value
  const dir = sortDir.value === 'asc' ? 1 : -1
  const list = [...filteredPatients.value]
  if (sortField.value === 'fullName') {
    list.sort((a, b) => a.fullName.localeCompare(b.fullName) * dir)
  } else if (sortField.value === 'age') {
    list.sort((a, b) => (a.ageMonths - b.ageMonths) * dir)
  }
  return list
})

const totalPages = computed(() => Math.ceil(filteredPatients.value.length / pageSize.value))
const paginatedPatients = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return sortedPatients.value.slice(start, start + pageSize.value)
})
const paginationPages = computed(() => {
  const total = totalPages.value
  const cur = currentPage.value
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1)
  const pages = []
  pages.push(1)
  if (cur > 3) pages.push('...')
  for (let p = Math.max(2, cur - 1); p <= Math.min(total - 1, cur + 1); p++) pages.push(p)
  if (cur < total - 2) pages.push('...')
  pages.push(total)
  return pages
})

watch([searchQuery, statusFilter, barangayFilter], () => { currentPage.value = 1 })

// ── PATIENT RECORD (full-page, mirrors the Nurse view) ─────────────────────
const selectedChild       = ref(null)
const childRecords        = ref([])
const loadingChildRecords = ref(false)

function closeRecord() {
  selectedChild.value = null
  childRecords.value = []
}

async function openRecord(child) {
  selectedChild.value = child
  loadingChildRecords.value = true

  // Pull the full child record (allergies, conditions, birth measurements,
  // contact) the same way the Nurse view does — the list endpoint doesn't
  // carry these fields, only /api/Children/{id} does.
  try {
    const childRes = await axios.get(`${API}/api/Children/${child.childID}`)
    const childData = childRes.data
    selectedChild.value = {
      ...selectedChild.value,
      allergies:          childData.allergies ?? childData.Allergies ?? null,
      existingConditions: childData.existingConditions ?? childData.ExistingConditions ?? null,
      birthHeight:         childData.birthHeight ?? childData.BirthHeight ?? null,
      birthWeight:         childData.birthWeight ?? childData.BirthWeight ?? null,
      primaryContactNo:    childData.primaryContactNo ?? childData.PrimaryContactNo ?? childData.contactNo ?? childData.ContactNo ?? null,
    }
  } catch (e) {
    console.error('openRecord child details:', e)
  }

  try {
    const res = await axios.get(`${API}/api/VaccinationRecords/child/${child.childID}`)
    // Backend returns vaccinationDate/vaccinationRecordID — normalize to the
    // dateAdministered/recordID names this modal's template already expects,
    // and build administeredByName from whichever shape the API gives us
    // (mirrors the Nurse view's mapping).
    childRecords.value = res.data
      .map(r => ({
        ...r,
        recordID:         r.recordID ?? r.vaccinationRecordID ?? r.VaccinationRecordID ?? null,
        vaccineName:       r.vaccineName ?? r.VaccineName ?? r.vaccine?.vaccineName ?? r.Vaccine?.VaccineName ?? 'Unknown',
        doseNumber:        r.doseNumber ?? r.DoseNumber,
        dateAdministered:  r.dateAdministered ?? r.vaccinationDate ?? r.VaccinationDate ?? null,
        nurseObservation:  r.nurseObservation ?? r.NurseObservation ?? '',
        doctorDiagnosis:   r.doctorDiagnosis ?? r.DoctorDiagnosis ?? '',
        administeredByName:
          r.administeredByName ?? r.AdministeredByName ??
          (r.administeredBy
            ? `${r.administeredBy.firstName ?? r.administeredBy.FirstName ?? ''} ${r.administeredBy.lastName ?? r.administeredBy.LastName ?? ''}`.trim()
            : (r.AdministeredBy
                ? `${r.AdministeredBy.FirstName ?? r.AdministeredBy.firstName ?? ''} ${r.AdministeredBy.LastName ?? r.AdministeredBy.lastName ?? ''}`.trim()
                : '')),
      }))
      .sort((a, b) =>
        new Date(a.dateAdministered ?? a.scheduledDate) - new Date(b.dateAdministered ?? b.scheduledDate)
      )
  } catch { childRecords.value = [] }
  finally { loadingChildRecords.value = false }
}

// ── EDIT DOCTOR DIAGNOSIS (inline, on an existing vaccination record) ──────
// NOTE: PATCH /api/VaccinationRecords/{id}/diagnosis does not exist on the
// backend yet — this needs a small addition to VaccinationRecordsController.cs
// (update DoctorDiagnosis, and ideally stamp DoctorDiagnosedByUserID with the
// logged-in doctor) before this will actually persist. Frontend is wired and
// ready for it.
const editingDiagnosisId = ref(null)
const editDiagnosisText  = ref('')
const savingDiagnosis    = ref(false)

function startEditDiagnosis(rec) {
  editingDiagnosisId.value = rec.recordID
  editDiagnosisText.value  = rec.doctorDiagnosis || ''
}

function cancelEditDiagnosis() {
  editingDiagnosisId.value = null
  editDiagnosisText.value  = ''
}

async function saveDiagnosis(rec) {
  savingDiagnosis.value = true
  try {
    await axios.patch(`${API}/api/VaccinationRecords/${rec.recordID}/diagnosis`, {
      doctorDiagnosis:   editDiagnosisText.value || null,
      diagnosedByUserID: doctor.value.userId,
    })
    rec.doctorDiagnosis = editDiagnosisText.value
    editingDiagnosisId.value = null
    showToast('Diagnosis updated')
  } catch (err) {
    console.error('saveDiagnosis error:', err)
    showToast(err.response?.data?.message || 'Failed to save diagnosis — backend endpoint may not exist yet.')
  } finally {
    savingDiagnosis.value = false
  }
}

// ── VACCINE LIST (Option B) ───────────────────────────────────────────────
// vaccineList shape: [{ vaccineID, vaccineName, doses: [{ doseNumber, minIntervalDays }] }]
const vaccineList    = ref([])
const loadingVaccines = ref(false)

async function fetchVaccines() {
  loadingVaccines.value = true
  try {
    const res = await axios.get(`${API}/api/Vaccines/with-doses`)
    vaccineList.value = res.data
  } catch (err) {
    console.error('fetchVaccines error:', err)
    vaccineList.value = []
  } finally {
    loadingVaccines.value = false
  }
}

// Doses for whichever vaccine is currently selected in the form
const selectedVaccineDoses = computed(() => {
  if (!vaccinateForm.value.vaccineId) return []
  const match = vaccineList.value.find(v => v.vaccineID === vaccinateForm.value.vaccineId)
  return match ? match.doses : []
})

// ── VACCINATE MODAL (Option B) ────────────────────────────────────────────
const showVaccinateModal = ref(false)
const submitting         = ref(false)
const vaccinateForm      = ref({
  vaccineId:        '',
  doseNumber:       '',
  dateAdministered: '',
  inventoryId:      '',
  remarks:          '',
  diagnosis:        '',
})
const inventoryOptions = ref([])
const loadingInventory = ref(false)

// Fetches available batches for a vaccine, FIFO-sorted by expiry,
// filters out inactive/expired/out-of-stock ones, and auto-selects
// the earliest-expiring valid batch. Mirrors DoctorHomepage.vue.
async function fetchInventoryFor(vaccineId) {
  vaccinateForm.value.inventoryId = ''
  inventoryOptions.value = []
  if (!vaccineId) return
  loadingInventory.value = true
  try {
    const res = await axios.get(`${API}/api/VaccineInventory/vaccine/${vaccineId}`)
    const today = new Date().setHours(0,0,0,0)
    inventoryOptions.value = res.data
      .filter(i => (i.status ?? i.Status) && (i.currentQuantity ?? i.CurrentQuantity) > 0
        && new Date(i.expirationDate ?? i.ExpirationDate).setHours(0,0,0,0) >= today)
      .sort((a, b) => new Date(a.expirationDate ?? a.ExpirationDate) - new Date(b.expirationDate ?? b.ExpirationDate))
      .map(i => ({
        inventoryId: i.inventoryID ?? i.InventoryID,
        lotNumber: i.lotNumber ?? i.LotNumber,
        expirationDate: i.expirationDate ?? i.ExpirationDate,
        currentQuantity: i.currentQuantity ?? i.CurrentQuantity,
      }))
    if (inventoryOptions.value.length) vaccinateForm.value.inventoryId = inventoryOptions.value[0].inventoryId
  } catch (e) { console.error('fetchInventoryFor:', e) }
  finally { loadingInventory.value = false }
}

function openVaccinateModal(child) {
  selectedChild.value = child
  vaccinateForm.value = {
    vaccineId:        '',
    doseNumber:       '',
    dateAdministered: new Date().toISOString().split('T')[0],
    inventoryId:      '',
    remarks:          '',
    diagnosis:        '',
  }
  inventoryOptions.value = []
  showVaccinateModal.value = true
}

async function submitVaccination() {
  const f = vaccinateForm.value
  if (!f.vaccineId || !f.doseNumber || !f.dateAdministered || !f.inventoryId) return

  submitting.value = true
  try {
    await axios.post(`${API}/api/VaccinationRecords`, {
      childID:              selectedChild.value.childID,
      vaccineID:            Number(f.vaccineId),
      doseNumber:           Number(f.doseNumber),
      vaccinationDate:      f.dateAdministered,
      inventoryID:          Number(f.inventoryId),
      administeredByUserID: doctor.value.userId,
      nurseObservation:     f.remarks || null,
      doctorDiagnosis:      f.diagnosis || null,
    })
    showVaccinateModal.value = false
    showToast(`Vaccination recorded for ${selectedChild.value.fullName}`)
    await fetchPatients()
  } catch (err) {
    console.error('submitVaccination error:', err)
    showToast(err.response?.data?.message || 'Failed to save. Please try again.')
  } finally {
    submitting.value = false
  }
}

// ── Toast ─────────────────────────────────────────────────────────────────
const toast = ref({ show: false, message: '' })
function showToast(msg) {
  toast.value = { show: true, message: msg }
  setTimeout(() => toast.value.show = false, 3500)
}

// ─────────────────────────────────────────────────────────────
// SAFETY RULE: same as the Nurse/Healthworker side — recording a
// vaccine still can only happen from the doctor's Now Serving card
// on Home (the only place with a "Mark Complete" action; Queue
// itself is read-only). This button never opens a modal here.
//
// It used to be permanently inert regardless of what was actually
// going on with the child, which meant a doctor with a patient
// sitting in "Now Serving" right now got the exact same generic
// "go to the queue" toast as a doctor whose patient has no queue
// entry at all — no way to tell from this page whether the child
// was actually ready. Now it checks live status and, when the
// child is genuinely Now Serving, sends the doctor straight to
// Home where the real action lives instead of making them
// re-navigate and re-find the child themselves.
// ─────────────────────────────────────────────────────────────
async function attemptVaccinateFromSearch(childArg) {
  const child = childArg || selectedChild.value
  const childId = child?.childID ?? child?.childId

  if (!childId) {
    showToast('This child must be started from the Queue once assigned to you.')
    return
  }

  try {
    const res = await axios.get(`${API}/api/Queue/board`)
    const entry = res.data.find(q =>
      (q.childId ?? q.childID)?.toString().toLowerCase() === childId.toString().toLowerCase()
    )

    if (entry?.queueStatus === 'InProgress') {
      showToast(`${child.fullName} is Now Serving — opening their queue card on Home`)
      router.push('/doctor/home')
      return
    }

    if (entry?.queueStatus === 'Waiting') {
      showToast(`${child.fullName} is #${entry.queueNumber} in queue, waiting to be called — not ready to record yet.`)
      return
    }

    showToast('This child must be started from the Queue once assigned to you.')
  } catch {
    showToast('This child must be started from the Queue once assigned to you.')
  }
}

// ── Helpers ───────────────────────────────────────────────────────────────
function statusClass(status) {
  return {
    'Due Soon':    'bg-amber-100 text-amber-700',
    'Overdue':     'bg-red-100 text-red-600',
    'Up to Date':  'bg-blue-100 text-blue-700',
    'Completed':   'bg-emerald-100 text-emerald-700',
    'Not Started': 'bg-slate-100 text-slate-500',
  }[status] || 'bg-slate-100 text-slate-600'
}

function formatDate(date) {
  if (!date) return '—'
  try { return new Date(date).toLocaleDateString('en-PH', { month: 'short', day: 'numeric', year: 'numeric' }) }
  catch { return '—' }
}

const AVATAR_COLORS = ['#4a7c59','#6b7c45','#8b5e3c','#4a6fa5','#7b4f8e','#5a7a6b','#8b6914']
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
.modal-enter-active, .modal-leave-active { transition: opacity 0.2s ease; }
.modal-enter-from, .modal-leave-to { opacity: 0; }
.modal-enter-active > div, .modal-leave-active > div { transition: transform 0.25s cubic-bezier(0.4,0,0.2,1); }
.modal-enter-from > div, .modal-leave-to > div { transform: scale(0.95); }

.toast-enter-active, .toast-leave-active { transition: all 0.3s ease; }
.toast-enter-from { opacity: 0; transform: translateY(12px); }
.toast-leave-to { opacity: 0; transform: translateY(12px); }

.notif-panel-enter-active, .notif-panel-leave-active { transition: opacity 0.25s ease; }
.notif-panel-enter-active > div:last-child, .notif-panel-leave-active > div:last-child { transition: transform 0.3s cubic-bezier(0.4,0,0.2,1); }
.notif-panel-enter-from, .notif-panel-leave-to { opacity: 0; }
.notif-panel-enter-from > div:last-child, .notif-panel-leave-to > div:last-child { transform: translateX(100%); }
</style>