<script setup>
import { ref, computed } from "vue";
import {
  Users, Syringe, ChevronLeft, ChevronRight, Search, Filter, ChevronDown,
  Download, Eye, UserCog, Printer, MoreHorizontal,
  List, CalendarDays, X, Clock, CheckCircle2, AlertTriangle, XCircle,
  ChevronUp, ArrowLeft, Check, Square, CheckSquare, User, Phone,
} from "lucide-vue-next";
import StaffSidebar from "./StaffSidebar.vue";
import StaffTopbar from "./StaffTopbar.vue";

/* ---------------------------------------------------------
   Aruga Pediatric System — Vaccine Schedule (Vue 3 + Tailwind)
   Same design language as the Staff Portal: sidebar, top bar,
   card system, table system, radii, shadows, green healthcare
   palette. Table now groups by appointment (one row per child
   visit, covering all vaccines due that day) instead of one
   row per vaccine.
--------------------------------------------------------- */



const summaryCards = [
  { label: "Vaccinations Due Today", value: "23", tint: "text-emerald-700", tintBg: "bg-emerald-50", icon: Syringe },
  { label: "Upcoming This Week", value: "87", tint: "text-sky-700", tintBg: "bg-sky-50", icon: CalendarDays },
  { label: "Overdue Vaccinations", value: "14", tint: "text-rose-700", tintBg: "bg-rose-50", icon: AlertTriangle },
  { label: "Completed Today", value: "16", tint: "text-emerald-700", tintBg: "bg-emerald-50", icon: CheckCircle2 },
  { label: "Tomorrow's Schedule", value: "19", tint: "text-sky-700", tintBg: "bg-sky-50", icon: Clock },
  { label: "Children Scheduled This Month", value: "412", tint: "text-emerald-700", tintBg: "bg-emerald-50", icon: Users },
];

/* View toggle */
const view = ref("list"); // "list" | "calendar"

/* Toolbar state */
const searchByOptions = ["Child Name", "Parent Name", "Patient ID"];
const searchBy = ref("Child Name");
const searchQuery = ref("");
const scheduleStatusFilter = ref("All");
const vaccineFilter = ref("All Vaccines");
const ageGroupFilter = ref("All Ages");

/* Appointment-level status (not per-vaccine) */
const statusStyle = {
  Waiting: "bg-amber-50 text-amber-700",
  "In Progress": "bg-sky-50 text-sky-700",
  Completed: "bg-emerald-50 text-emerald-700",
  Delayed: "bg-rose-50 text-rose-700",
  Cancelled: "bg-stone-100 text-stone-500",
};
const statusDot = {
  Waiting: "bg-amber-500",
  "In Progress": "bg-sky-500",
  Completed: "bg-emerald-600",
  Delayed: "bg-rose-600",
  Cancelled: "bg-stone-400",
};

/* Date helpers — the mock data below used to hardcode "Jul 12, 2026" as
   "today", which quietly went stale the moment the calendar was opened in
   any other month. Anchoring everything to the real current date instead
   means the calendar (and the mock schedule that feeds it) stays correct
   no matter when this page is actually opened, while keeping the same
   relative story: a few appointments today, one overdue ~12 days back,
   a few more later this week. */
const today = new Date();
today.setHours(0, 0, 0, 0);

function addDays(base, n) {
  const d = new Date(base);
  d.setDate(d.getDate() + n);
  return d;
}
function fmtDate(d) {
  return d.toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" });
}
// Precomputed once so each appointment's `due` (display string) and
// `dueDate` (real Date, used by the calendar) always agree.
const dueToday = addDays(today, 0);
const dueOverdue = addDays(today, -12);
const dueTomorrow = addDays(today, 1);
const dueInTwoDays = addDays(today, 2);
const todayLabel = fmtDate(today);

/* Each appointment = one child's visit, with all vaccines due that day */
const appointments = ref([
  {
    queueNo: "Q-014", id: "PT-10214", child: "Mika Santos", age: "1y 3m", parent: "Liza Santos", contact: "0917 555 0142",
    due: fmtDate(dueToday), dueDate: dueToday, worker: "Dr. Elena Cruz", room: "Room 1", status: "In Progress",
    vaccines: [
      { name: "MMR", dose: "Dose 2", done: true },
      { name: "OPV", dose: "Dose 3", done: false },
    ],
  },
  {
    queueNo: "Q-015", id: "PT-10215", child: "Julian Reyes", age: "2y 1m", parent: "Mark Reyes", contact: "0917 555 0198",
    due: fmtDate(dueToday), dueDate: dueToday, worker: "Nurse Bea Fernandez", room: "Room 2", status: "Completed",
    vaccines: [
      { name: "Pentavalent", dose: "Dose 3", done: true },
      { name: "OPV", dose: "Dose 3", done: true },
      { name: "IPV", dose: "—", done: true },
      { name: "PCV", dose: "Dose 3", done: true },
    ],
  },
  {
    queueNo: "Q-016", id: "PT-10216", child: "Ava Dizon", age: "8m", parent: "Carmen Dizon", contact: "0917 555 0231",
    due: fmtDate(dueOverdue), dueDate: dueOverdue, worker: "Nurse Kim Uy", room: "Room 3", status: "Delayed",
    vaccines: [
      { name: "Pentavalent", dose: "Dose 2", done: false },
      { name: "OPV", dose: "Dose 2", done: false },
    ],
  },
  {
    queueNo: "Q-017", id: "PT-10217", child: "Noah Bautista", age: "4y", parent: "Ramon Bautista", contact: "0917 555 0276",
    due: fmtDate(dueInTwoDays), dueDate: dueInTwoDays, worker: "Dr. Elena Cruz", room: "Room 1", status: "Waiting",
    vaccines: [
      { name: "DPT Booster", dose: "Booster", done: false },
    ],
  },
  {
    queueNo: "Q-018", id: "PT-10218", child: "Sophia Ramos", age: "6m", parent: "Ana Ramos", contact: "0917 555 0309",
    due: fmtDate(dueInTwoDays), dueDate: dueInTwoDays, worker: "—", room: "—", status: "Waiting",
    vaccines: [
      { name: "Pentavalent", dose: "Dose 1", done: false },
      { name: "OPV", dose: "Dose 1", done: false },
      { name: "IPV", dose: "—", done: false },
    ],
  },
  {
    queueNo: "Q-020", id: "PT-10220", child: "Zoe Manalo", age: "3m", parent: "Paolo Manalo", contact: "0917 555 0333",
    due: fmtDate(dueToday), dueDate: dueToday, worker: "Nurse Kim Uy", room: "Room 3", status: "Cancelled",
    vaccines: [
      { name: "BCG", dose: "Dose 1", done: false },
    ],
  },
  {
    queueNo: "Q-021", id: "PT-10222", child: "Ella Cruz", age: "5m", parent: "Vic Cruz", contact: "0917 555 0361",
    due: fmtDate(dueTomorrow), dueDate: dueTomorrow, worker: "Nurse Bea Fernandez", room: "Room 2", status: "Waiting",
    vaccines: [
      { name: "Hepatitis B", dose: "Dose 2", done: false },
      { name: "Pentavalent", dose: "Dose 1", done: false },
    ],
  },
]);

function progressOf(appt) {
  const done = appt.vaccines.filter((v) => v.done).length;
  return { done, total: appt.vaccines.length };
}

/* Keep an appointment's status in sync with its checklist */
function recalcStatus(appt) {
  if (appt.status === "Cancelled") return;
  const { done, total } = progressOf(appt);
  if (done === 0) appt.status = appt.status === "Delayed" ? "Delayed" : "Waiting";
  else if (done === total) appt.status = "Completed";
  else appt.status = "In Progress";
}

const filteredAppointments = computed(() => {
  return appointments.value.filter((a) => {
    const q = searchQuery.value.trim().toLowerCase();
    if (q) {
      if (searchBy.value === "Patient ID" && !a.id.toLowerCase().includes(q)) return false;
      if (searchBy.value === "Parent Name" && !a.parent.toLowerCase().includes(q)) return false;
      if (searchBy.value === "Child Name" && !a.child.toLowerCase().includes(q)) return false;
    }
    if (scheduleStatusFilter.value === "Overdue" && a.status !== "Delayed") return false;
    if (scheduleStatusFilter.value === "Completed" && a.status !== "Completed") return false;
    if (vaccineFilter.value !== "All Vaccines" && !a.vaccines.some((v) => v.name === vaccineFilter.value)) return false;
    return true;
  });
});

/* Export Schedule — downloads whatever's currently visible in the list
   (respecting the active search/status/vaccine filters) as a CSV. Exports
   `filteredAppointments`, not the full mock `appointments` array, so what
   the person sees on screen is what they get in the file. Once this page
   is wired to a real schedule endpoint instead of the mock array above,
   this keeps working unchanged — it just reflects live data at that point. */
function csvField(value) {
  const str = String(value ?? "");
  return /[",\n]/.test(str) ? `"${str.replace(/"/g, '""')}"` : str;
}

function exportScheduleCSV() {
  const header = [
    "Queue No.", "Patient ID", "Child Name", "Age", "Parent", "Contact",
    "Due Date", "Vaccines", "Progress", "Healthworker", "Room", "Status",
  ];

  const rows = filteredAppointments.value.map((a) => {
    const { done, total } = progressOf(a);
    return [
      a.queueNo,
      a.id,
      a.child,
      a.age,
      a.parent,
      a.contact,
      a.due,
      a.vaccines.map((v) => `${v.name} (${v.dose})`).join("; "),
      `${done}/${total}`,
      a.worker,
      a.room,
      a.status,
    ];
  });

  const csv = [header, ...rows].map((row) => row.map(csvField).join(",")).join("\r\n");
  // Leading BOM so Excel opens the UTF-8 file without mangling special characters.
  const blob = new Blob(["\ufeff" + csv], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);

  const link = document.createElement("a");
  link.href = url;
  link.download = `vaccine-schedule-${new Date().toISOString().slice(0, 10)}.csv`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
}

/* Expand / collapse rows */
const expandedRows = ref(new Set());
function toggleExpand(queueNo) {
  const next = new Set(expandedRows.value);
  if (next.has(queueNo)) next.delete(queueNo);
  else next.add(queueNo);
  expandedRows.value = next;
}

const actionsMenuOpenFor = ref(null);
function toggleActionsMenu(id) {
  actionsMenuOpenFor.value = actionsMenuOpenFor.value === id ? null : id;
}

/* Right sidebar data */
const todaysSummary = [
  { label: "Appointments", value: 24, icon: CalendarDays, tint: "text-sky-700" },
  { label: "Completed", value: 16, icon: CheckCircle2, tint: "text-emerald-700" },
  { label: "Waiting", value: 6, icon: Clock, tint: "text-amber-700" },
  { label: "Late", value: 2, icon: AlertTriangle, tint: "text-rose-700" },
  { label: "Cancelled", value: 1, icon: XCircle, tint: "text-stone-500" },
];

const upcomingVaccines = [
  { vaccine: "Pentavalent", count: 12, window: "Next 7 days" },
  { vaccine: "OPV", count: 9, window: "Next 7 days" },
  { vaccine: "MMR", count: 7, window: "Next 7 days" },
  { vaccine: "DPT Booster", count: 5, window: "Next 7 days" },
];

const overdueList = computed(() =>
  appointments.value.filter((a) => a.status === "Delayed").slice(0, 5)
);

// ── PAGINATION ────────────────────────────────────────────────────
const schedPage     = ref(1)
const schedPageSize = ref(10)
const schedTotalPages = computed(() =>
  Math.max(1, Math.ceil(filteredAppointments.value.length / schedPageSize.value))
)
const paginatedAppointments = computed(() => {
  const start = (schedPage.value - 1) * schedPageSize.value
  return filteredAppointments.value.slice(start, start + schedPageSize.value)
})
const schedPaginationPages = computed(() => {
  const total = schedTotalPages.value
  const cur   = schedPage.value
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1)
  const pages = []
  pages.push(1)
  if (cur > 3) pages.push('...')
  for (let p = Math.max(2, cur - 1); p <= Math.min(total - 1, cur + 1); p++) pages.push(p)
  if (cur < total - 2) pages.push('...')
  pages.push(total)
  return pages
})

/* Vaccine distribution (simple bar chart, no chart library) */
const distribution = [
  { name: "BCG", value: 58, color: "bg-emerald-600" },
  { name: "Hepatitis B", value: 74, color: "bg-sky-600" },
  { name: "Pentavalent", value: 96, color: "bg-emerald-600" },
  { name: "OPV", value: 88, color: "bg-sky-600" },
  { name: "IPV", value: 41, color: "bg-emerald-600" },
  { name: "MMR", value: 63, color: "bg-sky-600" },
  { name: "Booster", value: 29, color: "bg-emerald-600" },
];
const distMax = Math.max(...distribution.map((d) => d.value));

/* ---------------- Calendar view ---------------- */
// Starts on the real current month instead of a hardcoded one, and
// prev/next actually change it now — this used to be frozen on "July
// 2026" no matter what today's date was.
const calendarViewDate = ref(new Date(today.getFullYear(), today.getMonth(), 1));

const calendarMonthLabel = computed(() =>
  calendarViewDate.value.toLocaleDateString("en-US", { month: "long", year: "numeric" })
);

function goToPrevMonth() {
  const d = new Date(calendarViewDate.value);
  d.setMonth(d.getMonth() - 1);
  calendarViewDate.value = d;
  selectedDay.value = null;
}
function goToNextMonth() {
  const d = new Date(calendarViewDate.value);
  d.setMonth(d.getMonth() + 1);
  calendarViewDate.value = d;
  selectedDay.value = null;
}

function isToday(day) {
  return (
    day === today.getDate() &&
    calendarViewDate.value.getFullYear() === today.getFullYear() &&
    calendarViewDate.value.getMonth() === today.getMonth()
  );
}

const dueCountByDay = computed(() => {
  const map = {};
  const viewYear = calendarViewDate.value.getFullYear();
  const viewMonth = calendarViewDate.value.getMonth();
  appointments.value.forEach((a) => {
    if (a.dueDate.getFullYear() === viewYear && a.dueDate.getMonth() === viewMonth) {
      const day = a.dueDate.getDate();
      map[day] = (map[day] || 0) + 1;
    }
  });
  return map;
});

const calendarCells = computed(() => {
  const year = calendarViewDate.value.getFullYear();
  const month = calendarViewDate.value.getMonth();
  const firstWeekday = new Date(year, month, 1).getDay();
  const daysInMonth = new Date(year, month + 1, 0).getDate();
  const cells = [];
  for (let i = 0; i < firstWeekday; i++) cells.push(null);
  for (let d = 1; d <= daysInMonth; d++) cells.push(d);
  return cells;
});

const selectedDay = ref(null);
function selectDay(day) {
  if (!day) return;
  selectedDay.value = day;
}
const selectedDayLabel = computed(() => {
  if (!selectedDay.value) return "";
  const d = new Date(calendarViewDate.value.getFullYear(), calendarViewDate.value.getMonth(), selectedDay.value);
  return fmtDate(d);
});
const selectedDaySchedule = computed(() => {
  if (!selectedDay.value) return [];
  const year = calendarViewDate.value.getFullYear();
  const month = calendarViewDate.value.getMonth();
  return appointments.value.filter(
    (a) =>
      a.dueDate.getFullYear() === year &&
      a.dueDate.getMonth() === month &&
      a.dueDate.getDate() === selectedDay.value
  );
});

/* ---------------- Vaccination Session (full page) ---------------- */
const sessionAppt = ref(null);
function openSession(appt) {
  sessionAppt.value = appt;
}
function closeSession() {
  sessionAppt.value = null;
}
function toggleVaccineDone(v) {
  v.done = !v.done;
  recalcStatus(sessionAppt.value);
}
function completeSession() {
  sessionAppt.value.vaccines.forEach((v) => (v.done = true));
  recalcStatus(sessionAppt.value);
  closeSession();
}
</script>

<template>
  <div class="flex min-h-screen w-full bg-stone-50 text-stone-900" style="font-family: 'Inter','Segoe UI',sans-serif;">
    <!-- ---------------- Sidebar ---------------- -->
    <StaffSidebar />

    <!-- ---------------- Main ---------------- -->
    <main class="flex-1 min-w-0">
      <!-- ===================================================
           VACCINATION SESSION (full page, replaces content)
      ==================================================== -->
      <template v-if="sessionAppt">
        <header class="flex items-center justify-between px-8 py-5 border-b border-stone-200 bg-white">
          <div class="flex items-center gap-3">
            <button @click="closeSession" class="flex h-9 w-9 items-center justify-center rounded-full bg-stone-50">
              <ArrowLeft :size="17" class="text-stone-500" />
            </button>
            <div>
              <h1 class="text-[22px] font-bold">Vaccination Session</h1>
              <p class="text-[12.5px] mt-0.5 text-stone-500">Dashboard &gt; Vaccine Schedule &gt; {{ sessionAppt.child }}</p>
            </div>
          </div>
          <span class="inline-flex items-center gap-1.5 rounded-full px-3 py-1.5 text-xs font-medium" :class="statusStyle[sessionAppt.status]">
            <span class="h-1.5 w-1.5 rounded-full" :class="statusDot[sessionAppt.status]" />
            {{ sessionAppt.status }}
          </span>
        </header>

        <div class="px-8 py-6 max-w-4xl space-y-6">
          <!-- Patient summary -->
          <div class="rounded-2xl border border-stone-200 bg-white shadow-sm p-5">
            <div class="flex items-center gap-4">
              <div class="flex h-14 w-14 items-center justify-center rounded-2xl bg-sky-50 shrink-0">
                <User :size="24" class="text-sky-700" />
              </div>
              <div class="flex-1">
                <p class="text-[16px] font-bold">{{ sessionAppt.child }}</p>
                <p class="text-[12px] text-stone-500">{{ sessionAppt.id }} · {{ sessionAppt.age }} · Queue {{ sessionAppt.queueNo }}</p>
              </div>
              <div class="text-right">
                <p class="text-[12px] text-stone-500">Assigned Healthworker</p>
                <p class="text-[13px] font-medium">{{ sessionAppt.worker }}</p>
              </div>
            </div>
            <div class="grid grid-cols-3 gap-3 mt-4">
              <div class="rounded-xl bg-stone-50 p-3">
                <p class="text-[10.5px] uppercase tracking-wide text-stone-500">Parent / Guardian</p>
                <p class="text-[13px] font-medium mt-0.5">{{ sessionAppt.parent }}</p>
              </div>
              <div class="rounded-xl bg-stone-50 p-3 flex items-center gap-2">
                <Phone :size="14" class="text-stone-400" />
                <div>
                  <p class="text-[10.5px] uppercase tracking-wide text-stone-500">Contact</p>
                  <p class="text-[13px] font-medium">{{ sessionAppt.contact }}</p>
                </div>
              </div>
              <div class="rounded-xl bg-stone-50 p-3">
                <p class="text-[10.5px] uppercase tracking-wide text-stone-500">Room</p>
                <p class="text-[13px] font-medium mt-0.5">{{ sessionAppt.room }}</p>
              </div>
            </div>
          </div>

          <!-- Vaccine checklist -->
          <div class="rounded-2xl border border-stone-200 bg-white shadow-sm p-5">
            <div class="flex items-center justify-between mb-1">
              <p class="text-[14px] font-semibold">Vaccines for This Visit</p>
              <span class="text-[12.5px] font-semibold text-emerald-700">
                {{ progressOf(sessionAppt).done }} / {{ progressOf(sessionAppt).total }} Completed
              </span>
            </div>
            <div class="h-2 w-full rounded-full bg-stone-200 overflow-hidden mb-4">
              <div
                class="h-full rounded-full bg-emerald-600 transition-all"
                :style="{ width: (progressOf(sessionAppt).done / progressOf(sessionAppt).total) * 100 + '%' }"
              ></div>
            </div>

            <div class="space-y-2">
              <button
                v-for="(v, i) in sessionAppt.vaccines"
                :key="i"
                @click="toggleVaccineDone(v)"
                class="flex w-full items-center gap-3 rounded-xl border px-4 py-3 text-left transition-colors"
                :class="v.done ? 'border-emerald-200 bg-emerald-50' : 'border-stone-200 hover:bg-stone-50'"
              >
                <CheckSquare v-if="v.done" :size="19" class="text-emerald-700 shrink-0" />
                <Square v-else :size="19" class="text-stone-400 shrink-0" />
                <div class="flex-1">
                  <p class="text-[13.5px] font-medium" :class="v.done ? 'text-emerald-800 line-through decoration-emerald-400' : 'text-stone-800'">
                    {{ v.name }}
                  </p>
                  <p class="text-[11.5px] text-stone-500">{{ v.dose }}</p>
                </div>
                <span v-if="v.done" class="text-[11px] font-medium text-emerald-700">Administered</span>
              </button>
            </div>
          </div>

          <!-- Session actions -->
          <div class="flex items-center justify-end gap-3">
            <button @click="closeSession" class="rounded-xl px-4 py-2.5 text-[13px] font-medium text-stone-600 hover:bg-stone-100">
              Save &amp; Exit
            </button>
            <button
              @click="completeSession"
              class="flex items-center gap-2 rounded-xl px-5 py-2.5 text-[13px] font-semibold text-white bg-emerald-700 hover:bg-emerald-800 shadow-sm"
            >
              <Check :size="16" /> Complete Session
            </button>
          </div>
        </div>
      </template>

      <!-- ===================================================
           VACCINE SCHEDULE (default view)
      ==================================================== -->
      <template v-else>
        <!-- Top bar -->
        <StaffTopbar title="Vaccine Schedule" breadcrumb="Aruga / Vaccine Schedule" />

        <div class="px-8 py-6 space-y-6">
          <!-- Summary cards -->
          <div class="grid grid-cols-6 gap-4">
            <div
              v-for="c in summaryCards"
              :key="c.label"
              class="rounded-2xl border border-stone-200 bg-white p-4 shadow-sm"
            >
              <div class="flex items-center justify-between">
                <p class="text-[10.5px] font-semibold uppercase tracking-wide text-stone-500 leading-tight">{{ c.label }}</p>
                <div class="flex h-7 w-7 items-center justify-center rounded-lg shrink-0" :class="c.tintBg">
                  <component :is="c.icon" :size="14" :class="c.tint" />
                </div>
              </div>
              <p class="text-[24px] font-bold mt-2">{{ c.value }}</p>
            </div>
          </div>

          <!-- Toolbar -->
          <div class="rounded-2xl border border-stone-200 bg-white p-4 shadow-sm">
            <div class="flex flex-wrap items-center gap-3">
              <div class="flex items-center gap-2 flex-1 min-w-[240px] rounded-xl border border-stone-200 px-3 py-2.5">
                <Search :size="16" class="text-stone-400 shrink-0" />
                <input
                  v-model="searchQuery"
                  type="text"
                  :placeholder="`Search by ${searchBy.toLowerCase()}...`"
                  class="flex-1 text-[13px] outline-none placeholder:text-stone-400"
                />
                <div class="relative shrink-0">
                  <select v-model="searchBy" class="appearance-none bg-stone-50 rounded-lg pl-3 pr-7 py-1.5 text-[12px] font-medium text-stone-600 outline-none cursor-pointer">
                    <option v-for="opt in searchByOptions" :key="opt" :value="opt">{{ opt }}</option>
                  </select>
                  <ChevronDown :size="13" class="pointer-events-none absolute right-2 top-1/2 -translate-y-1/2 text-stone-400" />
                </div>
              </div>

              <div class="relative">
                <select v-model="scheduleStatusFilter" class="appearance-none rounded-xl border border-stone-200 pl-9 pr-8 py-2.5 text-[13px] font-medium text-stone-600 outline-none cursor-pointer">
                  <option>All</option>
                  <option>Due Today</option>
                  <option>Upcoming</option>
                  <option>Overdue</option>
                  <option>Completed</option>
                </select>
                <Filter :size="14" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-stone-400" />
                <ChevronDown :size="13" class="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-stone-400" />
              </div>

              <div class="relative">
                <select v-model="vaccineFilter" class="appearance-none rounded-xl border border-stone-200 pl-9 pr-8 py-2.5 text-[13px] font-medium text-stone-600 outline-none cursor-pointer">
                  <option>All Vaccines</option>
                  <option>BCG</option>
                  <option>Hepatitis B</option>
                  <option>Pentavalent</option>
                  <option>OPV</option>
                  <option>IPV</option>
                  <option>MMR</option>
                  <option>PCV</option>
                  <option>DPT Booster</option>
                </select>
                <Syringe :size="14" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-stone-400" />
                <ChevronDown :size="13" class="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-stone-400" />
              </div>

              <div class="relative">
                <select v-model="ageGroupFilter" class="appearance-none rounded-xl border border-stone-200 pl-3 pr-8 py-2.5 text-[13px] font-medium text-stone-600 outline-none cursor-pointer">
                  <option>All Ages</option>
                  <option>Birth</option>
                  <option>0–6 Months</option>
                  <option>7–12 Months</option>
                  <option>1–2 Years</option>
                </select>
                <ChevronDown :size="13" class="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-stone-400" />
              </div>

              <div class="flex-1"></div>

              <div class="flex items-center rounded-xl border border-stone-200 p-1">
                <button
                  @click="view = 'list'"
                  class="flex items-center gap-1.5 rounded-lg px-3 py-1.5 text-[12.5px] font-medium"
                  :class="view === 'list' ? 'bg-emerald-700 text-white' : 'text-stone-500'"
                >
                  <List :size="14" /> List
                </button>
                <button
                  @click="view = 'calendar'"
                  class="flex items-center gap-1.5 rounded-lg px-3 py-1.5 text-[12.5px] font-medium"
                  :class="view === 'calendar' ? 'bg-emerald-700 text-white' : 'text-stone-500'"
                >
                  <CalendarDays :size="14" /> Calendar
                </button>
              </div>

              <button
                @click="exportScheduleCSV"
                class="flex items-center gap-2 rounded-xl px-4 py-2.5 text-[13px] font-medium border border-stone-200 hover:bg-stone-50"
              >
                <Download :size="16" class="text-stone-500" />
                Export Schedule
              </button>
            </div>
          </div>

          <!-- Content grid: main + right sidebar -->
          <div class="grid grid-cols-12 gap-6">
            <!-- Main column -->
            <div class="col-span-8 space-y-6">
              <!-- LIST VIEW -->
              <div v-if="view === 'list'" class="rounded-2xl border border-stone-200 bg-white shadow-sm overflow-hidden">
                <div class="flex items-center justify-between px-5 py-4 border-b border-stone-200">
                  <p class="text-[14px] font-semibold">Today's Schedule</p>
                  <span class="text-[12px] text-stone-500">{{ filteredAppointments.length }} appointments</span>
                </div>
                <div class="overflow-x-auto">
                  <table class="w-full text-[13px] table-fixed">
                    <colgroup>
                      <col class="w-9" />
                      <col class="w-[76px]" />
                      <col class="w-[76px]" />
                      <col class="w-[100px]" />
                      <col class="w-[175px]" />
                      <col class="w-[90px]" />
                      <col class="w-[125px]" />
                      <col class="w-[150px]" />
                      <col class="w-[150px]" />
                    </colgroup>
                    <thead>
                      <tr class="bg-stone-50">
                        <th
                          v-for="(h, i) in ['', 'Queue No.', 'Patient ID', 'Child Name', &quot;Today's Vaccines&quot;, 'Progress', 'Healthworker', 'Status', '']"
                          :key="i"
                          class="text-left font-semibold px-4 py-3 whitespace-nowrap text-[11px] uppercase tracking-wide text-stone-500"
                          :class="[i === 7 ? 'text-center' : '', i === 8 ? 'sticky right-0 z-10 bg-stone-50 shadow-[-8px_0_8px_-8px_rgba(0,0,0,0.15)]' : '']"
                        >
                          {{ h }}
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      <template v-for="appt in paginatedAppointments" :key="appt.queueNo">
                        <tr class="border-t border-stone-200 cursor-pointer hover:bg-stone-50/60" @click="toggleExpand(appt.queueNo)">
                          <td class="px-3 py-3 text-center">
                            <ChevronUp v-if="expandedRows.has(appt.queueNo)" :size="15" class="text-stone-400 mx-auto" />
                            <ChevronDown v-else :size="15" class="text-stone-400 mx-auto" />
                          </td>
                          <td class="px-4 py-3 font-medium whitespace-nowrap truncate">{{ appt.queueNo }}</td>
                          <td class="px-4 py-3 whitespace-nowrap text-stone-500 truncate">{{ appt.id }}</td>
                          <td class="px-4 py-3">
                            <p class="font-medium truncate">{{ appt.child }}</p>
                            <p class="text-[11px] text-stone-500 truncate">{{ appt.age }}</p>
                          </td>
                          <td class="px-4 py-3">
                            <div class="flex flex-wrap items-center gap-1.5">
                              <span class="inline-flex items-center gap-1 rounded-full bg-emerald-50 text-emerald-700 text-[11px] font-medium px-2 py-1 whitespace-nowrap">
                                💉 {{ appt.vaccines.length }} {{ appt.vaccines.length === 1 ? 'Vaccine' : 'Vaccines' }}
                              </span>
                              <span
                                v-for="(v, i) in appt.vaccines.slice(0, 2)"
                                :key="i"
                                class="inline-flex rounded-full bg-sky-50 text-sky-700 text-[11px] font-medium px-2 py-1 whitespace-nowrap"
                              >
                                {{ v.name }}
                              </span>
                              <span
                                v-if="appt.vaccines.length > 2"
                                class="inline-flex rounded-full bg-stone-100 text-stone-600 text-[11px] font-medium px-2 py-1 whitespace-nowrap"
                              >
                                +{{ appt.vaccines.length - 2 }} More
                              </span>
                            </div>
                          </td>
                          <td class="px-4 py-3 whitespace-nowrap">
                            <div class="flex items-center gap-1.5">
                              <div class="h-1.5 w-10 shrink-0 rounded-full bg-stone-200 overflow-hidden">
                                <div
                                  class="h-full rounded-full bg-emerald-600"
                                  :style="{ width: (progressOf(appt).done / progressOf(appt).total) * 100 + '%' }"
                                ></div>
                              </div>
                              <span class="text-[11px] font-semibold text-stone-600 shrink-0">
                                {{ progressOf(appt).done }}/{{ progressOf(appt).total }}
                              </span>
                            </div>
                          </td>
                          <td class="px-4 py-3 truncate" :title="appt.worker">{{ appt.worker }}</td>
                          <td class="px-4 py-3 whitespace-nowrap text-center">
                            <span class="inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-medium" :class="statusStyle[appt.status]">
                              <span class="h-1.5 w-1.5 rounded-full shrink-0" :class="statusDot[appt.status]" />
                              {{ appt.status }}
                            </span>
                          </td>
                          <td
                            class="sticky right-0 bg-white px-4 py-3 whitespace-nowrap shadow-[-8px_0_8px_-8px_rgba(0,0,0,0.15)]"
                            :class="actionsMenuOpenFor === appt.queueNo ? 'z-30' : 'z-10'"
                            @click.stop
                          >
                            <div class="flex items-center gap-1 relative">
                              <button
                                @click="openSession(appt)"
                                class="flex items-center gap-1.5 rounded-lg px-2.5 py-1.5 text-[12px] font-semibold text-white bg-emerald-700 hover:bg-emerald-800"
                                title="Open Vaccination Session"
                              >
                                <Syringe :size="14" /> Session
                              </button>
                              <button @click="toggleActionsMenu(appt.queueNo)" class="p-1.5 rounded-lg hover:bg-stone-100" title="More actions">
                                <MoreHorizontal :size="15" class="text-stone-500" />
                              </button>
                              <div
                                v-if="actionsMenuOpenFor === appt.queueNo"
                                class="absolute right-0 top-9 z-40 w-52 rounded-xl border border-stone-200 bg-white shadow-lg py-1.5"
                              >
                                <button class="flex w-full items-center gap-2.5 px-3.5 py-2 text-[12.5px] text-stone-700 hover:bg-stone-50">
                                  <Eye :size="15" class="text-stone-500" /> View Patient
                                </button>
                                <button class="flex w-full items-center gap-2.5 px-3.5 py-2 text-[12.5px] text-stone-700 hover:bg-stone-50">
                                  <UserCog :size="15" class="text-stone-500" /> Assign Healthworker
                                </button>
                                <button class="flex w-full items-center gap-2.5 px-3.5 py-2 text-[12.5px] text-stone-700 hover:bg-stone-50">
                                  <Printer :size="15" class="text-stone-500" /> Print Schedule
                                </button>
                              </div>
                            </div>
                          </td>
                        </tr>

                        <!-- Expanded checklist row -->
                        <tr v-if="expandedRows.has(appt.queueNo)" class="border-t border-stone-100 bg-stone-50/60">
                          <td colspan="9" class="px-6 py-4">
                            <div class="flex items-center justify-between mb-2">
                              <p class="text-[12.5px] font-semibold text-stone-700">Vaccines Scheduled for This Appointment</p>
                              <span class="text-[12px] font-semibold text-emerald-700">
                                {{ progressOf(appt).done }} / {{ progressOf(appt).total }} Completed
                              </span>
                            </div>
                            <div class="grid grid-cols-2 gap-2">
                              <div
                                v-for="(v, i) in appt.vaccines"
                                :key="i"
                                class="flex items-center gap-2.5 rounded-lg bg-white border border-stone-200 px-3 py-2"
                              >
                                <CheckSquare v-if="v.done" :size="16" class="text-emerald-700 shrink-0" />
                                <Square v-else :size="16" class="text-stone-400 shrink-0" />
                                <span class="text-[12.5px]" :class="v.done ? 'text-stone-500 line-through decoration-stone-300' : 'text-stone-700'">
                                  {{ v.name }} {{ v.dose !== '—' ? v.dose : '' }}
                                </span>
                              </div>
                            </div>
                          </td>
                        </tr>
                      </template>
                    </tbody>
                  </table>
                </div>
                <!-- PAGINATION -->
                <div v-if="filteredAppointments.length > 0" class="flex items-center justify-between px-5 py-3.5 border-t border-stone-200 flex-wrap gap-3">
                  <div class="flex items-center gap-3">
                    <div class="flex items-center gap-2 text-[12px] text-stone-500">
                      <span>Show</span>
                      <select v-model="schedPageSize" @change="schedPage = 1"
                        class="border border-stone-200 rounded-lg px-2 py-1 text-[12px] text-stone-600 outline-none bg-white">
                        <option :value="10">10</option>
                        <option :value="20">20</option>
                        <option :value="50">50</option>
                        <option :value="100">100</option>
                      </select>
                      <span>entries</span>
                    </div>
                    <p class="text-[12px] text-stone-500">
                      Showing {{ Math.min((schedPage - 1) * schedPageSize + 1, filteredAppointments.length) }}–{{ Math.min(schedPage * schedPageSize, filteredAppointments.length) }}
                      of {{ filteredAppointments.length }} appointments
                    </p>
                  </div>
                  <div class="flex items-center gap-1">
                    <button @click="schedPage--" :disabled="schedPage === 1"
                      class="p-1.5 rounded-lg text-stone-400 hover:text-stone-600 hover:bg-stone-100 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
                      <ChevronLeft :size="15" />
                    </button>
                    <template v-for="p in schedPaginationPages" :key="p">
                      <span v-if="p === '...'" class="px-1 text-[12px] text-stone-400">…</span>
                      <button v-else @click="schedPage = p"
                        class="min-w-[28px] h-7 rounded-lg text-[12px] font-medium transition-colors"
                        :class="p === schedPage ? 'bg-emerald-700 text-white' : 'text-stone-600 hover:bg-stone-100'">
                        {{ p }}
                      </button>
                    </template>
                    <button @click="schedPage++" :disabled="schedPage >= schedTotalPages"
                      class="p-1.5 rounded-lg text-stone-400 hover:text-stone-600 hover:bg-stone-100 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
                      <ChevronRight :size="15" />
                    </button>
                  </div>
                </div>
              </div>

              <!-- CALENDAR VIEW -->
              <div v-else class="rounded-2xl border border-stone-200 bg-white shadow-sm p-5">
                <div class="flex items-center justify-between mb-4">
                  <p class="text-[14px] font-semibold">{{ calendarMonthLabel }}</p>
                  <div class="flex items-center gap-1">
                    <button @click="goToPrevMonth" class="flex h-8 w-8 items-center justify-center rounded-lg border border-stone-200 text-stone-500 hover:bg-stone-50">
                      <ChevronLeft :size="15" />
                    </button>
                    <button @click="goToNextMonth" class="flex h-8 w-8 items-center justify-center rounded-lg border border-stone-200 text-stone-500 hover:bg-stone-50">
                      <ChevronRight :size="15" />
                    </button>
                  </div>
                </div>
                <div class="grid grid-cols-7 gap-2 mb-2">
                  <div v-for="d in ['Sun','Mon','Tue','Wed','Thu','Fri','Sat']" :key="d" class="text-center text-[11px] font-semibold text-stone-500 uppercase">
                    {{ d }}
                  </div>
                </div>
                <div class="grid grid-cols-7 gap-2">
                  <button
                    v-for="(day, i) in calendarCells"
                    :key="i"
                    @click="selectDay(day)"
                    :disabled="!day"
                    class="aspect-square rounded-xl border p-1.5 flex flex-col items-start justify-between text-left transition-colors"
                    :class="[
                      !day ? 'border-transparent' : 'border-stone-200 hover:border-emerald-400',
                      day === selectedDay ? 'ring-2 ring-emerald-500 border-emerald-400' : '',
                      isToday(day) ? 'bg-emerald-50' : 'bg-white',
                    ]"
                  >
                    <span v-if="day" class="text-[12px] font-medium" :class="isToday(day) ? 'text-emerald-800' : 'text-stone-700'">{{ day }}</span>
                    <span
                      v-if="day && dueCountByDay[day]"
                      class="text-[10px] font-semibold px-1.5 py-0.5 rounded-full bg-emerald-600 text-white self-end"
                    >
                      {{ dueCountByDay[day] }}
                    </span>
                  </button>
                </div>

                <!-- Selected day detail -->
                <div v-if="selectedDay" class="mt-5 rounded-xl bg-stone-50 p-4">
                  <div class="flex items-center justify-between mb-2">
                    <p class="text-[13px] font-semibold">{{ selectedDayLabel }} — Scheduled Appointments</p>
                    <button @click="selectedDay = null" class="p-1 rounded-lg hover:bg-stone-200">
                      <X :size="14" class="text-stone-500" />
                    </button>
                  </div>
                  <div v-if="selectedDaySchedule.length === 0" class="text-[12.5px] text-stone-500 py-2">
                    No appointments scheduled on this date.
                  </div>
                  <div v-else class="space-y-2">
                    <div
                      v-for="appt in selectedDaySchedule"
                      :key="appt.queueNo"
                      class="flex items-center justify-between rounded-lg bg-white px-3 py-2 border border-stone-200"
                    >
                      <div>
                        <p class="text-[12.5px] font-medium">{{ appt.child }}</p>
                        <p class="text-[11px] text-stone-500">{{ appt.vaccines.length }} vaccines · {{ appt.worker }}</p>
                      </div>
                      <span class="inline-flex items-center gap-1.5 rounded-full px-2 py-0.5 text-[11px] font-medium" :class="statusStyle[appt.status]">
                        {{ appt.status }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Vaccine distribution -->
              <div class="rounded-2xl border border-stone-200 bg-white shadow-sm p-5">
                <p class="text-[14px] font-semibold mb-4">Vaccine Distribution</p>
                <div class="space-y-3">
                  <div v-for="d in distribution" :key="d.name" class="flex items-center gap-3">
                    <span class="w-28 text-[12px] text-stone-500 shrink-0">{{ d.name }}</span>
                    <div class="flex-1 h-2.5 rounded-full bg-stone-100 overflow-hidden">
                      <div class="h-full rounded-full" :class="d.color" :style="{ width: (d.value / distMax) * 100 + '%' }"></div>
                    </div>
                    <span class="w-8 text-right text-[12px] font-semibold">{{ d.value }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Right sidebar -->
            <div class="col-span-4 space-y-6">
              <!-- Today's summary -->
              <div class="rounded-2xl border border-stone-200 bg-white shadow-sm p-5">
                <div class="flex items-center justify-between mb-3">
                  <p class="text-[14px] font-semibold">Today's Summary</p>
                  <span class="text-[11.5px] text-stone-500">{{ todayLabel }}</span>
                </div>
                <div class="space-y-2">
                  <div
                    v-for="t in todaysSummary"
                    :key="t.label"
                    class="flex items-center justify-between rounded-xl bg-stone-50 px-3 py-2.5"
                  >
                    <div class="flex items-center gap-2">
                      <component :is="t.icon" :size="15" :class="t.tint" />
                      <span class="text-[12.5px] text-stone-600">{{ t.label }}</span>
                    </div>
                    <span class="text-[13px] font-semibold">{{ t.value }}</span>
                  </div>
                </div>
              </div>

              <!-- Upcoming vaccines -->
              <div class="rounded-2xl border border-stone-200 bg-white shadow-sm p-5">
                <p class="text-[14px] font-semibold mb-3">Upcoming Vaccines</p>
                <div class="space-y-2">
                  <div
                    v-for="u in upcomingVaccines"
                    :key="u.vaccine"
                    class="flex items-center justify-between rounded-xl bg-stone-50 px-3 py-2.5"
                  >
                    <div>
                      <p class="text-[12.5px] font-medium">{{ u.vaccine }}</p>
                      <p class="text-[11px] text-stone-500">{{ u.window }}</p>
                    </div>
                    <span class="text-[13px] font-semibold text-sky-700">{{ u.count }}</span>
                  </div>
                </div>
              </div>

              <!-- Overdue list -->
              <div class="rounded-2xl border border-stone-200 bg-white shadow-sm p-5">
                <div class="flex items-center justify-between mb-3">
                  <p class="text-[14px] font-semibold">Overdue List</p>
                  <span class="text-[11.5px] text-rose-700 font-medium">{{ overdueList.length }} shown</span>
                </div>
                <div class="space-y-2">
                  <div
                    v-for="appt in overdueList"
                    :key="appt.queueNo"
                    class="flex items-center justify-between rounded-xl bg-rose-50 px-3 py-2.5"
                  >
                    <div class="min-w-0">
                      <p class="text-[12.5px] font-medium truncate">{{ appt.child }}</p>
                      <p class="text-[11px] text-stone-500 truncate">{{ appt.vaccines.length }} vaccines · Due {{ appt.due }}</p>
                    </div>
                    <span class="text-[10.5px] font-semibold text-rose-700 shrink-0">Delayed</span>
                  </div>
                  <p v-if="overdueList.length === 0" class="text-[12.5px] text-stone-500 py-2">No delayed appointments.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
    </main>
  </div>
</template>