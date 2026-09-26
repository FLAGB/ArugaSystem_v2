<template>
  <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

    <HealthcareSidebar />

    <!-- MAIN -->
    <div class="flex flex-col flex-1 overflow-hidden">

      <!-- TOP BAR -->
      <HealthcareHeader />

      <!-- PAGE CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">

        <template v-if="selectedChild">

          <div class="flex items-center justify-between mb-4">
            <button
              @click="closeRecord"
              class="flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-700 transition-colors"
            >
              <ArrowLeft class="w-4 h-4" />
              Back to Patients
            </button>
            <a :href="`/print/vaccination-card/${selectedChild.childID}`" target="_blank"
              class="text-xs border border-slate-200 text-slate-600 hover:bg-slate-50 px-3 py-1.5 rounded-lg font-medium transition-colors">
              Print Vaccination Card
            </a>
          </div>

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

                <div class="flex items-start gap-2.5">
                  <Users class="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />
                  <div>
                    <p class="text-xs text-slate-400">Parents / Guardians</p>
                    <p v-for="g in selectedChild.guardians" :key="g.name + g.relationship" class="text-slate-700">
                      {{ g.name }} <span class="text-slate-400">· {{ g.relationship || 'Guardian' }}<template v-if="g.isPrimary"> · primary</template></span>
                    </p>
                    <p v-if="!selectedChild.guardians?.length" class="text-slate-700">None linked</p>
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
                <div class="flex items-center gap-2">
                  <button v-if="!healthEdit" @click="startEditHealthNotes"
                    class="inline-flex items-center gap-1.5 text-xs font-medium px-2.5 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 transition-colors">
                    <Pencil class="w-3.5 h-3.5" /> Edit allergies / conditions
                  </button>
                  <div class="w-9 h-9 rounded-lg bg-emerald-50 flex items-center justify-center">
                    <HeartPulse class="w-4 h-4 text-emerald-600" />
                  </div>
                </div>
              </div>

              <div class="grid grid-cols-2 gap-4">
                <!-- Allergies and existing conditions: Doctors/Nurses can update
                     these (e.g. an allergy found at the station); the parent
                     gets a "record updated" notice. -->
                <template v-if="healthEdit">
                  <div>
                    <p class="text-xs text-slate-400 mb-1.5">Allergies</p>
                    <textarea v-model="healthEdit.allergies" rows="3" maxlength="500" placeholder="e.g. Egg (mild rash). Leave blank if none."
                      class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 resize-none"></textarea>
                  </div>
                  <div>
                    <p class="text-xs text-slate-400 mb-1.5">Existing Conditions</p>
                    <textarea v-model="healthEdit.existingConditions" rows="3" maxlength="500" placeholder="e.g. Asthma. Leave blank if none."
                      class="w-full px-3 py-2.5 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 resize-none"></textarea>
                  </div>
                  <div class="col-span-2 flex items-center justify-end gap-2 -mt-1">
                    <button @click="healthEdit = null" :disabled="healthEdit.saving"
                      class="text-xs font-medium px-3 py-1.5 rounded-lg text-slate-600 hover:bg-slate-100 transition-colors">Cancel</button>
                    <button @click="saveHealthNotes" :disabled="healthEdit.saving"
                      class="inline-flex items-center gap-1.5 text-xs font-semibold px-3 py-1.5 rounded-lg text-white bg-emerald-600 hover:bg-emerald-700 disabled:opacity-50 transition-colors">
                      <Loader2 v-if="healthEdit.saving" class="w-3.5 h-3.5 animate-spin" />
                      <Check v-else class="w-3.5 h-3.5" /> Save
                    </button>
                  </div>
                </template>
                <template v-else>
                  <div>
                    <p class="text-xs text-slate-400 mb-1.5">Allergies</p>
                    <div class="min-h-[48px] px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700 whitespace-pre-line">
                      {{ selectedChild.allergies || 'None reported' }}
                    </div>
                  </div>
                  <div>
                    <p class="text-xs text-slate-400 mb-1.5">Existing Conditions</p>
                    <div class="min-h-[48px] px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700 whitespace-pre-line">
                      {{ selectedChild.existingConditions || 'None reported' }}
                    </div>
                  </div>
                </template>
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
                <button @click="attemptVaccinateFromSearch(selectedChild)" title="Opens the vaccination visit when this child is at your station"
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

          <!-- ================= VACCINATION PROGRESS / WHAT'S NEXT ================= -->
          <div class="mt-5 bg-white rounded-xl border border-slate-200">
            <div class="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
              <div>
                <h3 class="font-semibold text-slate-800">Remaining Schedule</h3>
                <p class="text-xs text-slate-400 mt-0.5">Doses still to be given, with the dates the system has scheduled</p>
              </div>
              <span class="text-xs font-semibold text-emerald-700">
                {{ selectedChild.completedCount ?? 0 }} of {{ selectedChild.totalDoses ?? 0 }} doses completed
              </span>
            </div>
            <div class="px-5 pt-4">
              <div class="w-full h-2 rounded-full bg-slate-100 overflow-hidden">
                <div class="h-full bg-emerald-500 rounded-full"
                  :style="{ width: (selectedChild.totalDoses ? (selectedChild.completedCount / selectedChild.totalDoses) * 100 : 0) + '%' }"></div>
              </div>
            </div>
            <div v-if="remainingDoses.length === 0" class="px-5 py-6 text-sm text-slate-400">
              {{ loadingChildRecords ? 'Loading...' : 'No remaining doses — the vaccination series is complete.' }}
            </div>
            <div v-else class="p-5 grid grid-cols-2 lg:grid-cols-3 gap-2">
              <div v-for="d in remainingDoses" :key="d.timelineID"
                class="flex items-center justify-between rounded-lg border px-3 py-2.5"
                :class="d.label === 'Overdue' ? 'border-red-100 bg-red-50/50' : d.label === 'Due Today' ? 'border-amber-100 bg-amber-50/50' : 'border-slate-100'">
                <div class="min-w-0">
                  <p class="text-sm font-medium text-slate-800 truncate">{{ d.vaccineName }} — Dose {{ d.doseNumber }}</p>
                  <p class="text-xs text-slate-500">{{ formatDate(d.scheduledDate) }}</p>
                </div>
                <span class="text-[11px] font-semibold shrink-0 ml-2"
                  :class="d.label === 'Overdue' ? 'text-red-600' : d.label === 'Due Today' ? 'text-amber-600' : 'text-slate-400'">
                  {{ d.label }}
                </span>
              </div>
            </div>
          </div>

          <!-- ================= VACCINATION HISTORY ================= -->
          <div class="mt-5 bg-white rounded-xl border border-slate-200">

            <div class="px-5 py-4 border-b border-slate-100">
              <h3 class="font-semibold text-slate-800">Vaccination History</h3>
              <p class="text-xs text-slate-400 mt-0.5">Doses already given — check observations for past adverse reactions</p>
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
                    <th class="px-5 py-2.5 text-left font-medium">Remarks / Reactions</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="rec in childRecords" :key="rec.recordID"
                    class="border-b border-slate-50 hover:bg-slate-50 transition-colors align-top">
                    <td class="px-5 py-3 font-medium text-slate-700">{{ rec.vaccineName }}</td>
                    <td class="px-5 py-3 text-slate-500">Dose {{ rec.doseNumber }}</td>
                    <td class="px-5 py-3 text-slate-500">{{ rec.dateAdministered ? formatDate(rec.dateAdministered) : '—' }}</td>
                    <td class="px-5 py-3 text-slate-500">{{ rec.administeredByName || 'Historical record' }}</td>
                    <td class="px-5 py-3 text-slate-500 min-w-[260px]">

                      <!-- Remarks can be added/corrected later, e.g. when a
                           parent reports a reaction the day after -->
                      <div v-if="editingRemarksId === rec.recordID" class="flex items-start gap-2">
                        <textarea v-model="editRemarksText" rows="2"
                          placeholder="e.g. Mild fever the next day, resolved with paracetamol"
                          class="flex-1 px-2 py-1.5 text-xs border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-emerald-500 resize-none"
                        ></textarea>
                        <div class="flex flex-col gap-1 shrink-0">
                          <button @click="saveRemarks(rec)" :disabled="savingRemarks" title="Save"
                            class="p-1 rounded text-emerald-600 hover:bg-emerald-50 disabled:opacity-50 transition-colors">
                            <Loader2 v-if="savingRemarks" class="w-3.5 h-3.5 animate-spin" />
                            <Check v-else class="w-3.5 h-3.5" />
                          </button>
                          <button @click="cancelEditRemarks" title="Cancel"
                            class="p-1 rounded text-slate-400 hover:bg-slate-100 transition-colors">
                            <X class="w-3.5 h-3.5" />
                          </button>
                        </div>
                      </div>

                      <div v-else class="flex items-start justify-between gap-2 group">
                        <span :class="hasReaction(rec.nurseObservation) ? 'text-amber-700 font-medium' : ''">{{ rec.nurseObservation || '—' }}</span>
                        <button @click="startEditRemarks(rec)" title="Edit remarks"
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
            <input v-model="searchQuery" type="text" placeholder="Search by name, Family No., barangay, or parent..."
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
                  <th class="px-5 py-3.5 text-left font-medium">Family No.</th>
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
                  <td class="px-5 py-3.5 text-slate-600">{{ child.familyNo }}</td>
                  <td class="px-5 py-3.5 font-medium text-slate-800">{{ child.fullName }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ child.age }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ child.sex }}</td>
                  <td class="px-5 py-3.5 text-slate-500">{{ child.barangay }}</td>
                  <td class="px-5 py-3.5 text-slate-500">
                    {{ child.parentName }}<span v-if="child.guardians.length > 1" class="text-slate-400" :title="child.guardians.map(g => `${g.name} (${g.relationship})`).join(', ')"> +{{ child.guardians.length - 1 }}</span>
                  </td>
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
                      <button @click="attemptVaccinateFromSearch(child)" title="Opens the vaccination visit when this child is at your station"
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



    <!-- SUCCESS TOAST -->
    <Transition name="toast">
      <div v-if="toast.show"
        class="fixed bottom-6 right-6 z-50 bg-emerald-600 text-white px-5 py-3.5 rounded-xl shadow-lg flex items-center gap-3 text-sm font-medium">
        <CheckCircle class="w-5 h-5" />
        {{ toast.message }}
      </div>
    </Transition>


  </div>
</template>

<script setup>
import { API_ORIGIN } from '@/utils/apiBase'
import { getUser } from '@/utils/auth'
import HealthcareSidebar from './Components/HealthcareSidebar.vue'
import HealthcareHeader from './Components/HealthcareHeader.vue'
import { isAtMyStation } from './Components/station.js'
import { guardiansOf } from '@/utils/format'
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

const API = API_ORIGIN
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
})


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
    // GET /api/Children/overview — one row per child with dose counts and
    // the next due dose already worked out from the vaccination timeline.
    // (This used to look for the next dose inside VaccinationRecords, which
    // only ever holds doses already given, so "Next Dose Due" was always
    // blank and most children showed the wrong status.)
    const res = await axios.get(`${API}/api/Children/overview`)

    const today = new Date()
    today.setHours(0, 0, 0, 0)

    patients.value = res.data.map(c => {
      const nextDueDate = c.nextDueDate ? new Date(c.nextDueDate) : null

      // Same rules as the status guide under the table: Overdue only once
      // the earliest missed dose is past the 14-day catch-up window.
      const daysUntilNext = nextDueDate ? Math.floor((nextDueDate - today) / 86400000) : null
      let vaccineStatus
      if (c.totalDoses > 0 && c.completedDoses === c.totalDoses) {
        vaccineStatus = 'Completed'
      } else if (daysUntilNext !== null && daysUntilNext < -14) {
        vaccineStatus = 'Overdue'
      } else if (daysUntilNext !== null && daysUntilNext <= 14) {
        vaccineStatus = 'Due Soon'
      } else if (c.completedDoses === 0) {
        vaccineStatus = 'Not Started'
      } else {
        vaccineStatus = 'Up to Date'
      }

      return {
        childID:         c.childID,
        fullName:        `${c.firstName} ${c.lastName}`,
        age:             computeAge(c.birthDate),
        ageMonths:       computeAgeMonths(c.birthDate),
        sex:             c.sex ?? '—',
        barangay:        c.barangay ?? '—',
        familyNo:        c.familyNo || '—',
        parentName:      c.parentName || '—',
        parentContact:   c.parentContact || null,
        // Everyone linked to the child (mother, grandmother, uncle...), primary first
        guardians:       guardiansOf(c.parents),
        birthDate:       c.birthDate,
        nextDueDate,
        nextVaccineName: c.nextVaccine,
        vaccineStatus,
        completedCount:  c.completedDoses,
        totalDoses:      c.totalDoses,
      }
    })
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
      c.familyNo.toLowerCase().includes(q) ||
      String(c.barangay).toLowerCase().includes(q) ||
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
const childTimeline       = ref([])

// Not-yet-given doses from the child's vaccination timeline (already
// recalculated by the backend when an earlier dose was given late).
const remainingDoses = computed(() => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return childTimeline.value
    .filter(t => t.status === 'Pending' || t.status === 'Missed')
    .map(t => {
      const d = new Date(t.scheduledDate)
      d.setHours(0, 0, 0, 0)
      const label = d < today ? 'Overdue' : d.getTime() === today.getTime() ? 'Due Today' : 'Upcoming'
      return { ...t, label }
    })
    .sort((a, b) => new Date(a.scheduledDate) - new Date(b.scheduledDate))
})

function closeRecord() {
  selectedChild.value = null
  childRecords.value = []
  childTimeline.value = []
}

async function openRecord(child) {
  selectedChild.value = child
  healthEdit.value = null
  loadingChildRecords.value = true
  childTimeline.value = []
  axios.get(`${API}/api/VaccinationTimeline/child/${child.childID}`)
    .then(res => { childTimeline.value = res.data })
    .catch(e => console.error('openRecord timeline:', e))

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
      primaryContactNo:    childData.primaryContactNo ?? childData.PrimaryContactNo ?? childData.contactNo ?? childData.ContactNo ?? child.parentContact ?? null,
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

// ── EDIT REMARKS (inline, on a dose already given) ────────────────────────
// Remarks are where health workers note complications / adverse reactions,
// and are what they check before giving the next dose. They can be added
// or corrected after the visit — e.g. a parent reports a fever the next
// day. PATCH /api/VaccinationRecords/{id}/remarks (logged in the audit trail).
const editingRemarksId = ref(null)
const editRemarksText  = ref('')
const savingRemarks    = ref(false)

function startEditRemarks(rec) {
  editingRemarksId.value = rec.recordID
  editRemarksText.value  = rec.nurseObservation || ''
}

function cancelEditRemarks() {
  editingRemarksId.value = null
  editRemarksText.value  = ''
}

async function saveRemarks(rec) {
  savingRemarks.value = true
  try {
    const { data } = await axios.patch(`${API}/api/VaccinationRecords/${rec.recordID}/remarks`, {
      remarks:         editRemarksText.value || null,
      updatedByUserID: doctor.value.userId,
    })
    rec.nurseObservation = data.remarks || ''
    editingRemarksId.value = null
    showToast('Remarks updated')
  } catch (err) {
    console.error('saveRemarks error:', err)
    showToast(err.response?.data?.message || 'Failed to save remarks. Please try again.')
  } finally {
    savingRemarks.value = false
  }
}

// ── EDIT ALLERGIES / EXISTING CONDITIONS ──────────────────────────────────
// PATCH /api/Children/{id}/health-notes: only these two fields; the rest of
// the profile is edited by the Admission Staff. Logged in the audit trail,
// and the parent gets a "record updated" notice.
const healthEdit = ref(null)   // { allergies, existingConditions, saving }

function startEditHealthNotes() {
  healthEdit.value = {
    allergies:          selectedChild.value?.allergies || '',
    existingConditions: selectedChild.value?.existingConditions || '',
    saving: false,
  }
}

async function saveHealthNotes() {
  const h = healthEdit.value
  h.saving = true
  try {
    const { data } = await axios.patch(`${API}/api/Children/${selectedChild.value.childID}/health-notes`, {
      allergies:          h.allergies.trim() || null,
      existingConditions: h.existingConditions.trim() || null,
    })
    selectedChild.value = { ...selectedChild.value, allergies: data.allergies, existingConditions: data.existingConditions }
    healthEdit.value = null
    showToast(data.changed?.length ? "Saved. The parent was notified of the change." : 'No changes')
  } catch (err) {
    console.error('saveHealthNotes error:', err)
    showToast(err.response?.data?.message || 'Could not save. Please try again.')
    h.saving = false
  }
}

// Remarks that mention anything other than "no reaction" are highlighted.
function hasReaction(text) {
  const t = (text || '').toLowerCase()
  return !!t && !/no (adverse )?reaction/.test(t)
}

// ── Toast ─────────────────────────────────────────────────────────────────
const toast = ref({ show: false, message: '' })
function showToast(msg) {
  toast.value = { show: true, message: msg }
  setTimeout(() => toast.value.show = false, 3500)
}

// ─────────────────────────────────────────────────────────────
// RECORD VACCINE — only for the child the Admission Staff sent to
// MY station. If they are, open their vaccination visit; otherwise
// explain where the child is instead of recording anything here.
// ─────────────────────────────────────────────────────────────
async function attemptVaccinateFromSearch(childArg) {
  const child = childArg || selectedChild.value
  const childId = child?.childID ?? child?.childId
  if (!childId) return

  try {
    const res = await axios.get(`${API}/api/Queue/today`)
    const visit = res.data.find(q =>
      q.children.some(c => c.childID.toString().toLowerCase() === childId.toString().toLowerCase()))

    if (!visit) {
      showToast(`${child.fullName} hasn't checked in today. The Admission Staff adds them to the queue first.`)
    } else if (isAtMyStation(visit)) {
      router.push(`/healthcare/vaccination/${visit.queueID}?child=${childId}`)
    } else if ((visit.status || '').toLowerCase() === 'completed') {
      showToast(`${child.fullName}'s visit today is already completed.`)
    } else if (visit.assignedRoomID) {
      showToast(`${child.fullName} is at ${visit.stationName} with ${visit.assignedWorkerName || 'another health worker'}.`)
    } else {
      showToast(`${child.fullName} is #${visit.queueNumber} in the queue, waiting to be sent to a station.`)
    }
  } catch {
    showToast('Could not check the queue. Please try again.')
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

</style>