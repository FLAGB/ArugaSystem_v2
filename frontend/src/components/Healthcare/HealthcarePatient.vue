<template>
    <div class="flex h-screen bg-slate-50 font-sans antialiased text-slate-900">

        <HealthcareSidebar @logout="logout" />

        <div class="flex flex-col flex-1 overflow-hidden">
        <HealthcareHeader :worker="worker" title="Patients (Children)" />

        <main class="flex-1 overflow-y-auto p-6">

            <!-- ═══════════════════════ CHILD RECORD VIEW ═══════════════════════ -->
            <template v-if="selectedChild">

    <!-- BACK -->
    <button
        @click="closeRecord"
        class="flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-700 mb-4 transition-colors"
    >
        <ArrowLeft class="w-4 h-4" />
        Back to Patients
    </button>

    <!-- TOP RECORD LAYOUT -->
    <div class="grid grid-cols-3 gap-5">

        <!-- ================= PROFILE CARD ================= -->
        <div class="bg-white rounded-xl border border-slate-200 p-5">

            <div
                class="w-16 h-16 rounded-full flex items-center justify-center text-xl font-bold text-white mb-3"
                :style="{ backgroundColor: avatarColor(selectedChild.name) }"
            >
                {{ initials(selectedChild.name) }}
            </div>

            <h2 class="text-lg font-bold text-slate-800">
                {{ selectedChild.name }}
            </h2>

            <p class="text-sm text-slate-400">
                {{ selectedChild.ageLabel || '—' }}
                ·
                {{ selectedChild.sex || '—' }}
                <template v-if="selectedChild.parentName && selectedChild.parentName !== '—'">
                    · Parent: {{ selectedChild.parentName }}
                </template>
            </p>

            <div class="mt-4 space-y-4 text-sm">

                <!-- Birthdate -->
                <div class="flex items-start gap-2.5">
                    <Cake class="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />

                    <div>
                        <p class="text-xs text-slate-400">
                            Birthdate
                        </p>

                        <p class="text-slate-700">
                            {{
                                selectedChild.dateOfBirth
                                    ? formatDate(selectedChild.dateOfBirth)
                                    : '—'
                            }}
                        </p>
                    </div>
                </div>

                <!-- Barangay -->
                <div class="flex items-start gap-2.5">
                    <MapPin class="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />

                    <div>
                        <p class="text-xs text-slate-400">
                            Barangay
                        </p>

                        <p class="text-slate-700">
                            {{ selectedChild.barangayNo || '—' }}
                        </p>
                    </div>
                </div>

                <!-- PRIMARY CONTACT -->
                <div class="flex items-start gap-2.5">
                    <Phone class="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />

                    <div>
                        <p class="text-xs text-slate-400">
                            Primary Contact No.
                        </p>

                        <p class="text-slate-700">
                            {{ selectedChild.primaryContactNo || 'Not available' }}
                        </p>
                    </div>
                </div>

            </div>
        </div>


        <!-- ================= MEDICAL RECORDS ================= -->
        <div class="col-span-2 bg-white rounded-xl border border-slate-200 p-5">

            <div class="flex items-center justify-between mb-5">

                <div>
                    <h3 class="font-semibold text-slate-800">
                        Medical Records
                    </h3>

                    <p class="text-xs text-slate-400 mt-0.5">
                        Medical information and health measurements
                    </p>
                </div>

                <div class="w-9 h-9 rounded-lg bg-emerald-50 flex items-center justify-center">
                    <HeartPulse class="w-4 h-4 text-emerald-600" />
                </div>

            </div>

            <div class="grid grid-cols-2 gap-4">

                <!-- Allergies -->
                <div>
                    <p class="text-xs text-slate-400 mb-1.5">
                        Allergies
                    </p>

                    <div class="min-h-[48px] px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                        {{ selectedChild.allergies || 'None reported' }}
                    </div>
                </div>

                <!-- Existing Conditions -->
                <div>
                    <p class="text-xs text-slate-400 mb-1.5">
                        Existing Conditions
                    </p>

                    <div class="min-h-[48px] px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                        {{ selectedChild.existingConditions || 'None reported' }}
                    </div>
                </div>

                <!-- Birth Height -->
                <div>
                    <p class="text-xs text-slate-400 mb-1.5">
                        Birth Height
                    </p>

                    <div class="px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                        {{ selectedChild.birthHeight || 'Not available' }}
                    </div>
                </div>

                <!-- Birth Weight -->
                <div>
                    <p class="text-xs text-slate-400 mb-1.5">
                        Birth Weight
                    </p>

                    <div class="px-3 py-2.5 border border-slate-200 rounded-lg bg-slate-50 text-sm text-slate-700">
                        {{ selectedChild.birthWeight || 'Not available' }}
                    </div>
                </div>

            </div>

            <div class="flex items-start gap-2 mt-4 p-3 bg-slate-50 border border-slate-200 rounded-lg text-xs text-slate-500">

                <Info class="w-4 h-4 shrink-0 mt-0.5" />

                <span>
                    Additional medical information and growth measurements
                    will appear here when available.
                </span>

            </div>

        </div>


        <!-- ================= EMPTY LEFT AREA / PROFILE EXTENSION ================= -->
        <div class="bg-white rounded-xl border border-slate-200 p-5">

            <h3 class="font-semibold text-slate-800 mb-4">
                Patient Details
            </h3>

            <div class="space-y-3 text-sm">

                <div>
                    <p class="text-xs text-slate-400">
                        Full Name
                    </p>

                    <p class="text-slate-700">
                        {{ selectedChild.name }}
                    </p>
                </div>

                <div>
                    <p class="text-xs text-slate-400">
                        Sex
                    </p>

                    <p class="text-slate-700">
                        {{ selectedChild.sex || '—' }}
                    </p>
                </div>

                <div>
                    <p class="text-xs text-slate-400">
                        Age
                    </p>

                    <p class="text-slate-700">
                        {{ selectedChild.ageLabel || '—' }}
                    </p>
                </div>

            </div>

        </div>


        <!-- ================= NEXT VACCINATION ================= -->
        <div class="col-span-2 bg-white rounded-xl border border-slate-200 p-5">

            <h3 class="font-semibold text-slate-800 mb-4">
                Next Vaccination
            </h3>

            <!-- Loading -->
            <div
                v-if="loadingHistory"
                class="flex items-center gap-2 text-sm text-slate-400"
            >
                <Loader2 class="w-4 h-4 animate-spin" />
                Loading schedule...
            </div>

            <!-- Next vaccine -->
            <div
                v-else-if="selectedChild.nextDueDate"
                class="grid grid-cols-3 gap-4"
            >

                <div>
                    <p class="text-xs text-slate-400">
                        Vaccine
                    </p>

                    <p class="font-semibold text-slate-800 mt-1">
                        {{ selectedChild.nextVaccineName }}
                    </p>
                </div>

                <div>
                    <p class="text-xs text-slate-400">
                        Scheduled Date
                    </p>

                    <p class="text-sm text-slate-700 mt-1">
                        {{ formatDate(selectedChild.nextDueDate) }}
                    </p>
                </div>

                <div>
                    <p class="text-xs text-slate-400">
                        Status
                    </p>

                    <span
                        class="inline-flex mt-1 px-2.5 py-1 rounded-full text-xs font-medium"
                        :class="statusClass(selectedChild.vaccineStatus)"
                    >
                        {{ selectedChild.vaccineStatus }}
                    </span>
                </div>

            </div>

            <!-- No pending vaccine -->
            <div
                v-else
                class="text-sm"
                :class="selectedChild.vaccineStatus === 'Completed' ? 'text-emerald-600 font-medium' : 'text-slate-400'"
            >
                {{ selectedChild.vaccineStatus === 'Completed' ? 'Series complete.' : 'No upcoming vaccination scheduled.' }}
            </div>

        </div>

    </div>


    <!-- ================= VACCINATION HISTORY ================= -->
    <div class="mt-5 bg-white rounded-xl border border-slate-200">

        <div class="px-5 py-4 border-b border-slate-100">

            <h3 class="font-semibold text-slate-800">
                Vaccination History
            </h3>

        </div>

        <!-- Loading -->
        <div
            v-if="loadingHistory"
            class="flex items-center justify-center py-10"
        >
            <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
        </div>

        <!-- Error -->
        <div
            v-else-if="historyError"
            class="flex items-start gap-2 m-5 p-3 bg-red-50 border border-red-100 rounded-lg text-sm text-red-700"
        >
            <AlertCircle class="w-4 h-4 shrink-0" />

            <span>
                {{ historyError }}
            </span>
        </div>

        <!-- Empty -->
        <div
            v-else-if="history.length === 0"
            class="flex flex-col items-center justify-center py-10 text-slate-400"
        >
            <Syringe class="w-7 h-7 mb-2 opacity-40" />

            <p class="text-sm">
                No vaccination records yet
            </p>
        </div>

        <!-- History -->
        <div
            v-else
            class="overflow-x-auto"
        >

            <table class="w-full text-sm">

                <thead>
                    <tr class="text-xs text-slate-400 uppercase tracking-wide border-b border-slate-100">

                        <th class="px-5 py-2.5 text-left font-medium">
                            Vaccine
                        </th>

                        <th class="px-5 py-2.5 text-left font-medium">
                            Dose
                        </th>

                        <th class="px-5 py-2.5 text-left font-medium">
                            Date
                        </th>

                        <th class="px-5 py-2.5 text-left font-medium">
                            Administered By
                        </th>

                        <th class="px-5 py-2.5 text-left font-medium">
                            Observation
                        </th>

                        <th class="px-5 py-2.5 text-left font-medium">
                            Doctor Diagnosis
                        </th>

                    </tr>
                </thead>

                <tbody>

                    <template
                        v-for="visit in groupedHistory"
                        :key="`visit-${visit.visitNumber}`"
                    >

                        <!-- VISIT DIVIDER -->
                        <tr>

                            <td
                                colspan="6"
                                class="px-5 py-3 bg-slate-50 border-y border-slate-100"
                            >

                                <div class="flex items-center gap-3">

                                    <div
                                        class="w-7 h-7 rounded-full bg-emerald-100 text-emerald-700 flex items-center justify-center text-xs font-bold"
                                    >
                                        {{ visit.visitNumber }}
                                    </div>

                                    <div>

                                        <p class="text-sm font-semibold text-slate-800">
                                            {{ visit.visitNumber }}{{
                                                visit.visitNumber === 1 ? 'st' :
                                                visit.visitNumber === 2 ? 'nd' :
                                                visit.visitNumber === 3 ? 'rd' : 'th'
                                            }} Visit
                                        </p>

                                        <p class="text-xs text-slate-400">
                                            {{ formatDate(visit.date) }}
                                        </p>

                                    </div>

                                </div>

                            </td>

                        </tr>


                        <!-- RECORDS -->
                        <tr
                            v-for="r in visit.records"
                            :key="r.vaccinationRecordID"
                            class="border-b border-slate-50 hover:bg-slate-50 transition-colors"
                        >

                            <td class="px-5 py-3 pl-16 font-medium text-slate-700">
                                {{ r.vaccineName }}
                            </td>

                            <td class="px-5 py-3 text-slate-500">
                                Dose {{ r.doseNumber }}
                            </td>

                            <td class="px-5 py-3 text-slate-500">
                                {{ formatDate(r.vaccinationDate) }}
                            </td>

                            <td class="px-5 py-3 text-slate-500">
                                {{ r.administeredByName || '—' }}
                            </td>

                            <td class="px-5 py-3 text-slate-500">
                                {{ r.nurseObservation || '—' }}
                            </td>

                            <td class="px-5 py-3 text-slate-500">
                                {{ r.doctorDiagnosis || '—' }}
                            </td>

                        </tr>

                    </template>

                </tbody>

            </table>

        </div>

    </div>

</template>

            <!-- ═══════════════════════ PATIENT LIST VIEW ═══════════════════════ -->
            <template v-else>
            <div class="mb-5">
                <h1 class="text-xl font-bold text-slate-800">Patients (Children)</h1>
                <p class="text-sm text-slate-500 mt-0.5">Manage pediatric patient records and vaccinations</p>
            </div>

            <!-- SEARCH + FILTERS -->
            <div class="flex items-center gap-3 mb-4">
                <div class="relative flex-1 max-w-md">
                <Search class="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
                <input
                    v-model="searchTerm"
                    type="text"
                    placeholder="Search by name..."
                    class="w-full pl-9 pr-3 py-2 text-sm border border-slate-200 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-emerald-500"
                />
                </div>

                <select
                    v-model="statusFilter"
                    class="text-sm border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-500 bg-white text-slate-600"
                >
                    <option value="">Status: All</option>
                    <option value="Due Soon">Due Soon</option>
                    <option value="Overdue">Overdue</option>
                    <option value="Up to Date">Up to Date</option>
                    <option value="Completed">Completed</option>
                    <option value="Not Started">Not Started</option>
                </select>
            </div>

            <!-- LOADING / ERROR / EMPTY -->
            <div v-if="loadingChildren" class="bg-white rounded-xl border border-slate-200 flex items-center justify-center py-16">
                <Loader2 class="w-5 h-5 animate-spin text-slate-400" />
                <span class="ml-2 text-sm text-slate-400">Loading patients...</span>
            </div>

            <div v-else-if="loadError" class="bg-white rounded-xl border border-slate-200 p-6">
                <div class="flex items-start gap-2 text-sm text-red-700">
                <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
                <span>{{ loadError }}</span>
                </div>
            </div>

            <div v-else-if="filteredChildren.length === 0" class="bg-white rounded-xl border border-slate-200 flex flex-col items-center justify-center py-16 text-slate-400">
                <UserX class="w-7 h-7 mb-2 opacity-40" />
                <p class="text-sm">{{ searchTerm ? 'No matching children' : 'No children found' }}</p>
            </div>

            <!-- TABLE -->
            <div v-else class="bg-white rounded-xl border border-slate-200 overflow-hidden">
                <table class="w-full text-sm">
                <thead>
                    <tr class="text-xs text-slate-400 uppercase tracking-wide border-b border-slate-100 bg-slate-50">
                    <th class="px-5 py-3 text-left font-medium">Profile</th>
                    <th class="px-3 py-3 text-left font-medium">Full Name</th>
                    <th class="px-3 py-3 text-left font-medium">Parent</th>
                    <th class="px-3 py-3 text-left font-medium">Brgy No.</th>
                    <th class="px-3 py-3 text-left font-medium">Age</th>
                    <th class="px-3 py-3 text-left font-medium">Sex</th>
                    <th class="px-3 py-3 text-left font-medium">Next Schedule</th>
                    <th class="px-3 py-3 text-left font-medium">Status</th>
                    <th class="px-3 py-3 text-left font-medium">Actions</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                    <tr v-for="c in pagedChildren" :key="c.childID" class="hover:bg-slate-50 transition-colors">
                    <td class="px-5 py-3">
                        <div class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold text-white"
                        :style="{ backgroundColor: avatarColor(c.name) }">
                        {{ initials(c.name) }}
                        </div>
                    </td>
                    <td class="px-3 py-3 font-medium text-slate-800">{{ c.name }}</td>
                    <td class="px-3 py-3 text-slate-500">{{ c.parentName || '—' }}</td>
                    <td class="px-3 py-3 text-slate-500">{{ c.barangayNo || '—' }}</td>
                    <td class="px-3 py-3 text-slate-500">{{ c.ageLabel || '—' }}</td>
                    <td class="px-3 py-3 text-slate-500">{{ c.sex || '—' }}</td>
                    <td class="px-3 py-3 text-slate-500 text-xs">
                        <span v-if="c.nextDueDate">{{ formatDate(c.nextDueDate) }}</span>
                        <span v-else-if="c.vaccineStatus === 'Completed'" class="text-emerald-600 font-medium">Series complete</span>
                        <span v-else class="text-slate-400">—</span>
                    </td>
                    <td class="px-3 py-3">
                        <span
                            class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="statusClass(c.vaccineStatus)"
                        >
                            {{ c.vaccineStatus }}
                        </span>
                    </td>
                    <td class="px-3 py-3">
                        <div class="flex items-center gap-1.5">
                        <button
                            @click="openRecord(c)"
                            title="View Record"
                            class="p-1.5 rounded-lg text-slate-500 hover:bg-slate-100 hover:text-emerald-700 transition-colors"
                        >
                            <Eye class="w-4 h-4" />
                        </button>
                        <button
                            @click="attemptVaccinateFromSearch"
                            title="Vaccination can only be started from an assigned Queue entry"
                            class="p-1.5 rounded-lg text-slate-300 cursor-not-allowed"
                        >
                            <Syringe class="w-4 h-4" />
                        </button>
                        </div>
                    </td>
                    </tr>
                </tbody>
                </table>

                <!-- PAGINATION -->
                <div class="flex items-center justify-between px-5 py-3 border-t border-slate-100 text-xs text-slate-400">
                <span>Showing {{ pageStart + 1 }}-{{ Math.min(pageStart + pageSize, filteredChildren.length) }} of {{ filteredChildren.length }} patients</span>
                <div class="flex items-center gap-4">
                    <div class="flex items-center gap-1.5">
                        <span>Show</span>
                        <select
                            v-model.number="pageSize"
                            class="border border-slate-200 rounded px-1.5 py-1 text-xs bg-white focus:outline-none focus:ring-2 focus:ring-emerald-500"
                        >
                            <option :value="10">10</option>
                            <option :value="20">20</option>
                            <option :value="30">30</option>
                            <option :value="50">50</option>
                        </select>
                        <span>entries</span>
                    </div>
                    <div class="flex items-center gap-1">
                        <button @click="currentPage--" :disabled="currentPage === 1" class="p-1 rounded disabled:opacity-30 hover:bg-slate-100">
                        <ChevronLeft class="w-3.5 h-3.5" />
                        </button>
                        <template v-for="(p, idx) in pageNumbers" :key="idx">
                            <span v-if="p === '…'" class="px-1.5">…</span>
                            <button
                                v-else
                                @click="currentPage = p"
                                class="min-w-[24px] px-1.5 py-1 rounded text-xs"
                                :class="p === currentPage ? 'bg-emerald-600 text-white font-medium' : 'text-slate-500 hover:bg-slate-100'"
                            >
                                {{ p }}
                            </button>
                        </template>
                        <button @click="currentPage++" :disabled="currentPage >= totalPages" class="p-1 rounded disabled:opacity-30 hover:bg-slate-100">
                        <ChevronRight class="w-3.5 h-3.5" />
                        </button>
                    </div>
                </div>
                </div>
            </div>
            </template>

            <!-- INLINE NOTICE (e.g. vaccinate-blocked message) -->
            <Transition name="toast">
            <div v-if="notice" class="fixed bottom-6 right-6 z-50 bg-slate-800 text-white px-5 py-3 rounded-xl shadow-lg text-sm max-w-sm">
                {{ notice }}
            </div>
            </Transition>

        </main>
        </div>
    </div>
    </template>

    <script setup>
    import { getUser, getToken, logout as clearSession } from '@/utils/auth'
    import { ref, computed, onMounted, watch } from 'vue'
    import { useRouter } from 'vue-router'
    import {
    Search, Loader2, UserX, Eye, Syringe, AlertCircle, ArrowLeft,
    Cake, MapPin, Info, ChevronLeft, ChevronRight
    } from 'lucide-vue-next'
    import HealthcareSidebar from '@/components/Healthcare/Components/HealtcareSidebar.vue'
    import HealthcareHeader from '@/components/Healthcare/Components/HealthcareHeader.vue'

    const router = useRouter()

    // VITE_API_URL is the bare host (no /api) per the project's existing
    // convention — /api is appended here, not stored in the env var.
    const API_BASE = `${(import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '')}/api`

    function authHeaders() {
    return { Authorization: `Bearer ${getToken()}` }
    }

    // ─────────────────────────────────────────────────────────────
    // AUTH
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

    fetchChildren()
    })

    function logout() {
    clearSession()
    router.push('/')
    }

    // ─────────────────────────────────────────────────────────────
    // CHILDREN LIST
    // CONFIRMED from ChildrenController.cs: GET /api/Children/all
    // (note: NOT a bare GET /api/Children — there's no GetAll on the
    // base route, only /all, /{id}, /parent/{parentId}). Confirmed
    // field names: childID, firstName, middleName, lastName, birthDate,
    // placeOfBirth, address, healthCenter, barangay, sex, parentName,
    // parents[] (parentID, relationshipType, isPrimaryContact,
    // canReceiveNotifications). parentName IS present on this response
    // (confirmed working on the Doctor side) — it just wasn't being
    // read here. No medical fields exist on this model at all,
    // confirming the Medical Information gap below.
    // ─────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────
    // DOH vaccine master — same list/dose counts used on the Doctor
    // side (DoctorPatients.vue) so Status/Next Schedule match there.
    // ─────────────────────────────────────────────────────────────
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

    // Shared with openRecord() below so the list and the detail view
    // always agree on a child's status/next dose, computed the same way.
    function computeVaccineSummary(childRecords, today) {
        const hasAnyRecords  = childRecords.length > 0
        const completedCount = childRecords.filter(r => (r.status ?? r.Status) === 'Completed').length
        const allDone        = hasAnyRecords && completedCount >= TOTAL_DOSES

        const pendingRecords = childRecords
            .filter(r => (r.status ?? r.Status) !== 'Completed' && (r.vaccinationDate ?? r.VaccinationDate ?? r.scheduledDate ?? r.ScheduledDate))
            .sort((a, b) =>
                new Date(a.vaccinationDate ?? a.VaccinationDate ?? a.scheduledDate ?? a.ScheduledDate) -
                new Date(b.vaccinationDate ?? b.VaccinationDate ?? b.scheduledDate ?? b.ScheduledDate)
            )

        const nextRecord  = pendingRecords[0] ?? null
        const nextDueDate = nextRecord
            ? new Date(nextRecord.vaccinationDate ?? nextRecord.VaccinationDate ?? nextRecord.scheduledDate ?? nextRecord.ScheduledDate)
            : null
        const nextVaccineName = nextRecord
            ? (nextRecord.vaccineName ?? nextRecord.VaccineName ?? `Vaccine Dose ${nextRecord.doseNumber ?? nextRecord.DoseNumber}`)
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

        return { nextDueDate, nextVaccineName, vaccineStatus, completedCount }
    }

    const allChildren          = ref([])
    const allVaccinationRecords = ref([])
    const loadingChildren      = ref(false)
    const loadError            = ref('')
    const searchTerm           = ref('')
    const statusFilter         = ref('')

    // Fetches every VaccinationRecords row once so both the list and any
    // opened record can filter it client-side, instead of hitting
    // GET /api/VaccinationRecords/child/{id} separately (that endpoint's
    // DTO doesn't reliably include AdministeredByName/NurseObservation/
    // DoctorDiagnosis, unlike /all — confirmed working on the Doctor
    // side and in HealthcareVaccinationRecords.vue).
    async function fetchAllVaccinationRecords(children) {
        try {
            const recRes = await fetch(`${API_BASE}/VaccinationRecords/all`, { headers: authHeaders() })
            if (!recRes.ok) throw new Error(`VaccinationRecords/all failed (${recRes.status})`)
            return await recRes.json()
        } catch {
            const allRecords = []
            for (const c of children) {
                const childID = c.childID ?? c.ChildID
                try {
                    const r = await fetch(`${API_BASE}/VaccinationRecords/child/${childID}`, { headers: authHeaders() })
                    if (r.ok) {
                        const recs = await r.json()
                        allRecords.push(...recs.map(rec => ({ ...rec, childID })))
                    }
                } catch {}
            }
            return allRecords
        }
    }

    async function fetchChildren() {
    loadingChildren.value = true
    loadError.value = ''
    try {
        const res = await fetch(`${API_BASE}/Children/all`, { headers: authHeaders() })
        if (!res.ok) throw new Error(`Children request failed (${res.status})`)
        const children = await res.json()

        const allRecords = await fetchAllVaccinationRecords(children)
        allVaccinationRecords.value = allRecords

        const today = new Date()
        today.setHours(0, 0, 0, 0)

        allChildren.value = children.map(c => {
        const dob = c.birthDate ?? c.BirthDate ?? null
        const childID = c.childID ?? c.ChildID

        const childRecords = allRecords.filter(r =>
            (r.childID ?? r.ChildID)?.toString().toLowerCase() === childID?.toString().toLowerCase()
        )

        const { nextDueDate, nextVaccineName, vaccineStatus, completedCount } = computeVaccineSummary(childRecords, today)

        return {
    childID,
    name: `${c.firstName ?? c.FirstName ?? ''} ${c.lastName ?? c.LastName ?? ''}`.trim(),
    parentName: c.parentName ?? c.ParentName ?? '—',
    barangayNo: c.barangay ?? c.Barangay ?? null,
    sex: c.sex ?? c.Sex ?? null,
    allergies: c.allergies ?? c.Allergies ?? null,
    dateOfBirth: dob,
    ageLabel: dob ? ageLabelFromDOB(dob) : null,
    nextDueDate,
    nextVaccineName,
    vaccineStatus,
    completedCount,
}
        })
    } catch (e) {
        console.error('fetchChildren:', e)
        loadError.value = 'Could not load patients. Check that GET /api/Children/all is reachable.'
    } finally {
        loadingChildren.value = false
    }
    }

    function statusClass(status) {
    return {
        'Due Soon':    'bg-amber-100 text-amber-700',
        'Overdue':     'bg-red-100 text-red-600',
        'Up to Date':  'bg-blue-100 text-blue-700',
        'Completed':   'bg-emerald-100 text-emerald-700',
        'Not Started': 'bg-slate-100 text-slate-500',
    }[status] || 'bg-slate-100 text-slate-600'
    }

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

    const filteredChildren = computed(() => {
    const term = searchTerm.value.trim().toLowerCase()
    let list = allChildren.value
    if (term) list = list.filter(c => c.name.toLowerCase().includes(term))
    if (statusFilter.value) list = list.filter(c => c.vaccineStatus === statusFilter.value)
    return list
    })

    // ─────────────────────────────────────────────────────────────
    // PAGINATION (client-side) — "Show entries" size picker + a
    // numbered page list with ellipsis, matching StaffPatientRecords.vue
    // ─────────────────────────────────────────────────────────────
    const currentPage = ref(1)
    const pageSize = ref(10)
    const totalPages = computed(() => Math.max(1, Math.ceil(filteredChildren.value.length / pageSize.value)))
    const pageStart = computed(() => (currentPage.value - 1) * pageSize.value)
    const pagedChildren = computed(() => filteredChildren.value.slice(pageStart.value, pageStart.value + pageSize.value))

    // Compact page list: first, last, a window around the current page,
    // and '...' for any gap — e.g. 1 ... 4 5 6 ... 12
    const pageNumbers = computed(() => {
        const total = totalPages.value
        const current = currentPage.value
        const delta = 1
        const pages = []
        for (let i = 1; i <= total; i++) {
            if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
                pages.push(i)
            }
        }
        const withDots = []
        let last = 0
        for (const p of pages) {
            if (last && p - last > 1) withDots.push('…')
            withDots.push(p)
            last = p
        }
        return withDots
    })

    watch([searchTerm, statusFilter, pageSize], () => {
        currentPage.value = 1
    })

    watch(totalPages, (total) => {
        if (currentPage.value > total) currentPage.value = total
    })

    // ─────────────────────────────────────────────────────────────
    // SELECTED CHILD RECORD + VACCINATION HISTORY
    // History and the Next Vaccination summary are both derived from
    // GET /api/VaccinationRecords/all filtered to this child, the same
    // endpoint the patient list uses — /child/{id}'s DTO doesn't
    // reliably carry AdministeredByName/NurseObservation/DoctorDiagnosis,
    // which was showing those columns blank.
    // ─────────────────────────────────────────────────────────────
    const selectedChild  = ref(null)
    const history         = ref([])
    const loadingHistory  = ref(false)
    const historyError    = ref('')

async function openRecord(child) {
    selectedChild.value = { ...child }

    history.value = []
    historyError.value = ''

    loadingHistory.value = true

    // Load full child information
    try {
        const childRes = await fetch(
            `${API_BASE}/Children/${child.childID}`,
            { headers: authHeaders() }
        )

        if (childRes.ok) {
            const childData = await childRes.json()

            selectedChild.value = {
                ...selectedChild.value,
                allergies:
                    childData.allergies ??
                    childData.Allergies ??
                    null,
                existingConditions:
                    childData.existingConditions ??
                    childData.ExistingConditions ??
                    null,
                birthHeight:
                    childData.birthHeight ??
                    childData.BirthHeight ??
                    null,
                birthWeight:
                    childData.birthWeight ??
                    childData.BirthWeight ??
                    null,
                primaryContactNo:
                    childData.primaryContactNo ??
                    childData.PrimaryContactNo ??
                    childData.contactNo ??
                    childData.ContactNo ??
                    null
            }
        }
    } catch (e) {
        console.error('openRecord child details:', e)
    }

    // Vaccination history + Next Vaccination summary — both computed
    // from GET /api/VaccinationRecords/all filtered to this child (falls
    // back to /child/{id} only if /all is unreachable), so the detail
    // view always agrees with the list and reliably has
    // administeredByName/nurseObservation/doctorDiagnosis populated.
    try {
        let data
        try {
            const res = await fetch(`${API_BASE}/VaccinationRecords/all`, { headers: authHeaders() })
            if (!res.ok) throw new Error(`VaccinationRecords/all request failed (${res.status})`)
            const all = await res.json()
            data = all.filter(r =>
                (r.childID ?? r.ChildID)?.toString().toLowerCase() === child.childID?.toString().toLowerCase()
            )
        } catch {
            const res = await fetch(
                `${API_BASE}/VaccinationRecords/child/${child.childID}`,
                { headers: authHeaders() }
            )
            if (!res.ok) throw new Error(`VaccinationRecords request failed (${res.status})`)
            data = await res.json()
        }

        const today = new Date()
        today.setHours(0, 0, 0, 0)
        const { nextDueDate, nextVaccineName, vaccineStatus, completedCount } = computeVaccineSummary(data, today)
        selectedChild.value = {
            ...selectedChild.value,
            nextDueDate,
            nextVaccineName,
            vaccineStatus,
            completedCount,
        }

        history.value = data
            .map(r => ({
                vaccinationRecordID:
                    r.vaccinationRecordID ??
                    r.VaccinationRecordID,

                vaccineName:
                    r.vaccineName ??
                    r.VaccineName ??
                    r.vaccine?.vaccineName ??
                    r.Vaccine?.VaccineName ??
                    'Unknown',

                doseNumber:
                    r.doseNumber ??
                    r.DoseNumber,

                vaccinationDate:
                    r.vaccinationDate ??
                    r.VaccinationDate,

                nurseObservation:
                    r.nurseObservation ??
                    r.NurseObservation ??
                    '',

                doctorDiagnosis:
                    r.doctorDiagnosis ??
                    r.DoctorDiagnosis ??
                    '',

                administeredByName:
                    r.administeredByName ??
                    r.AdministeredByName ??
                    (r.administeredBy
                        ? `${r.administeredBy.firstName ?? r.administeredBy.FirstName ?? ''} ${r.administeredBy.lastName ?? r.administeredBy.LastName ?? ''}`.trim()
                        : (
                            r.AdministeredBy
                                ? `${r.AdministeredBy.FirstName ?? r.AdministeredBy.firstName ?? ''} ${r.AdministeredBy.LastName ?? r.AdministeredBy.lastName ?? ''}`.trim()
                                : (
                                    r.personnel
                                        ? `${r.personnel.firstName ?? r.personnel.FirstName ?? ''} ${r.personnel.lastName ?? r.personnel.LastName ?? ''}`.trim()
                                        : ''
                                )
                        )),
            }))
            .filter(r => r.vaccinationDate) // only actually-administered doses belong in History
            .sort(
                (a, b) =>
                    new Date(b.vaccinationDate) -
                    new Date(a.vaccinationDate)
            )

    } catch (e) {
        console.error('openRecord history:', e)
        historyError.value =
            'Could not load vaccination history for this child.'
    } finally {
        loadingHistory.value = false
    }
}

const groupedHistory = computed(() => {
    const groups = {}

    history.value.forEach(record => {
        const date = new Date(record.vaccinationDate)
        if (isNaN(date)) return // skip anything without a valid administered date

        // Group records that happened on the same date
        const dateKey = date.toISOString().split('T')[0]

        if (!groups[dateKey]) {
            groups[dateKey] = {
                date: record.vaccinationDate,
                records: []
            }
        }

        groups[dateKey].records.push(record)
    })

    // Convert object into an array and assign visit numbers
    return Object.values(groups)
        .sort((a, b) => new Date(a.date) - new Date(b.date))
        .map((visit, index) => ({
            visitNumber: index + 1,
            date: visit.date,
            records: visit.records
        }))
})

    function closeRecord() {
    selectedChild.value = null
    history.value = []
    }

    // ─────────────────────────────────────────────────────────────
    // SAFETY RULE: vaccination can only start from an assigned Queue
    // entry, never from a general patient search. This button is
    // deliberately inert — it explains why instead of doing anything.
    // ─────────────────────────────────────────────────────────────
    const notice = ref('')
    let noticeTimer = null
    function attemptVaccinateFromSearch() {
    notice.value = 'This child must be started from the Queue once assigned to you.'
    clearTimeout(noticeTimer)
    noticeTimer = setTimeout(() => (notice.value = ''), 3500)
    }

    // ─────────────────────────────────────────────────────────────
    // NEW PATIENT ASSIGNMENT POPUP — SCAFFOLDED, NOT LIVE
    // ─────────────────────────────────────────────────────────────
    // The Queue model (confirmed from QueueController.cs) has no
    // AssignedPersonnelID or any per-worker assignment field, so there
    // is currently no real data source that could trigger this popup.
    // Building it to fire automatically would mean faking the event.
    //
    // What's missing on the backend before this can go live:
    //   1. A field on Queue (or a new Assignment entity) linking a queue
    //      entry / child to a specific Personnel/User — e.g.
    //      AssignedPersonnelID on Queue, settable via a new endpoint like
    //      PUT /api/Queue/{id}/assign  { personnelId }
    //   2. A way for the frontend to detect NEW assignments — either a
    //      poll-able endpoint (GET /api/Queue/assigned/{personnelId}) or
    //      a push channel (SignalR/WebSocket). Polling GET /api/Queue and
    //      diffing client-side is NOT reliable for "new" detection once
    //      real assignment data exists.
    //
    // Once that exists, wire it up here:
    //
    //   import Swal from 'sweetalert2' // confirm this isn't already a
    //                                   // project dependency before adding it
    //
    //   async function presentAssignmentPopup(assignment) {
    //     const result = await Swal.fire({
    //       title: 'New Patient Assignment',
    //       html: `
    //         <div style="text-align:left">
    //           <p><strong>${assignment.childName}</strong></p>
    //           <p>Barangay: ${assignment.barangayNo ?? '—'}</p>
    //           <p>Queue #: ${assignment.queueNumber ?? '—'}</p>
    //           <p>Vaccine: ${assignment.vaccineName ?? '—'}</p>
    //           <p>Dose: ${assignment.doseNumber ?? '—'}</p>
    //           <p>Room: ${assignment.room ?? '—'}</p>
    //         </div>
    //       `,
    //       showCancelButton: true,
    //       confirmButtonText: 'Accept Patient',
    //       cancelButtonText: 'Decline',
    //       confirmButtonColor: '#059669',
    //     })
    //
    //     if (result.isConfirmed) {
    //       // PUT /api/Queue/{id}/assign/accept  (endpoint TBD)
    //       router.push(`/healthcare/vaccination/${assignment.queueID}?child=${assignment.childID}`)
    //     } else {
    //       // PUT /api/Queue/{id}/assign/decline  (endpoint TBD)
    //     }
    //   }

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
    function formatDate(dateStr) {
    if (!dateStr) return ''
    return new Date(dateStr).toLocaleDateString('en-PH', { month: 'short', day: 'numeric', year: 'numeric' })
    }
    </script>

    <style scoped>
    .toast-enter-active, .toast-leave-active { transition: all 0.3s ease; }
    .toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(12px); }
    </style>