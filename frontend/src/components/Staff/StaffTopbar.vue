<!--
  StaffTopbar.vue
  ================
  Shared top bar for every Staff*.vue page.

  Bell    -> real late-patient alerts, pulled straight from GET /api/Queue/today
             (same endpoint the dashboard's queue table uses) and filtered to
             status === 'Late'. This is intentionally self-contained rather than
             fed via props, so the bell works correctly no matter which
             Staff*.vue page is currently mounted, not just the dashboard.
  Gear    -> routes to /staff/settings (StaffAccountSettings.vue). That route
             still needs to be added to router/index.js — it isn't in the
             files this component has visibility into.
  Avatar  -> identity-card dropdown only (name, role, on-duty status). No
             actions in here on purpose: logout already lives in
             StaffSidebar.vue, so a second logout in this dropdown would just
             be a duplicate (per earlier decision).

  Usage:
    <StaffTopbar title="Children Records" breadcrumb="Aruga / Children Record">
      <button @click="openAddModal" class="...">+ Register Child</button>
    </StaffTopbar>

  Anything passed in the default slot renders to the left of the bell/gear/
  avatar cluster, so each page can still have its own action button(s).
-->
<template>
  <header class="relative flex items-center justify-between px-8 py-5 border-b border-stone-200 bg-white">
    <div>
      <h1 class="text-[22px] font-bold">{{ title }}</h1>
      <p v-if="breadcrumb" class="text-[12.5px] mt-0.5 text-stone-500">{{ breadcrumb }}</p>
    </div>
    <div class="flex items-center gap-3">
      <slot />

      <!-- Bell: late-patient alerts -->
      <div class="relative">
        <button
          @click.stop="toggle('bell')"
          class="topbar-trigger relative flex h-9 w-9 items-center justify-center rounded-full bg-stone-50 hover:bg-stone-100"
        >
          <Bell :size="17" class="text-stone-500" />
          <span v-if="lateQueue.length > 0" class="absolute top-1.5 right-2 h-1.5 w-1.5 rounded-full bg-rose-600" />
        </button>

        <div
          v-if="open === 'bell'"
          class="topbar-panel absolute right-0 top-11 z-50 w-80 rounded-xl border border-stone-200 bg-white shadow-lg overflow-hidden"
        >
          <div class="flex items-center justify-between px-4 py-3 border-b border-stone-100">
            <p class="text-[13px] font-semibold">Late Patients</p>
            <span class="text-[11px] text-stone-400">{{ lateQueue.length }} today</span>
          </div>
          <div class="max-h-72 overflow-y-auto">
            <button
              v-for="q in lateQueue"
              :key="q.queueID"
              @click="goToLate(q)"
              class="flex w-full items-start gap-3 px-4 py-3 text-left hover:bg-stone-50 border-b border-stone-50 last:border-0"
            >
              <span class="mt-1 h-1.5 w-1.5 rounded-full bg-rose-600 shrink-0" />
              <div class="min-w-0">
                <p class="text-[12.5px] font-medium truncate">{{ q.child }}</p>
                <p class="text-[11px] text-stone-500 truncate">Queue {{ q.no }} · Parent: {{ q.parent }}</p>
              </div>
            </button>
            <p v-if="!loadingLate && lateQueue.length === 0" class="px-4 py-6 text-center text-[12px] text-stone-400">
              No late patients right now.
            </p>
            <p v-if="loadingLate && lateQueue.length === 0" class="px-4 py-6 text-center text-[12px] text-stone-400">
              Loading…
            </p>
          </div>
        </div>
      </div>

      <!-- Gear: account settings -->
      <button
        @click="router.push('/staff/settings')"
        class="flex h-9 w-9 items-center justify-center rounded-full bg-stone-50 hover:bg-stone-100"
        title="Account Settings"
      >
        <Settings :size="17" class="text-stone-500" />
      </button>

      <!-- Avatar: identity card, view-only -->
      <div class="relative">
        <button
          @click.stop="toggle('avatar')"
          class="topbar-trigger flex h-9 w-9 items-center justify-center rounded-full text-white text-xs font-semibold bg-emerald-700 hover:bg-emerald-800"
        >
          {{ staffInitials }}
        </button>

        <div
          v-if="open === 'avatar'"
          class="topbar-panel absolute right-0 top-11 z-50 w-64 rounded-xl border border-stone-200 bg-white shadow-lg p-4"
        >
          <div class="flex items-center gap-3">
            <div class="flex h-11 w-11 items-center justify-center rounded-full text-white text-sm font-semibold bg-emerald-700 shrink-0">
              {{ staffInitials }}
            </div>
            <div class="min-w-0">
              <p class="text-[13.5px] font-semibold truncate">{{ staffName }}</p>
              <p class="text-[11.5px] text-stone-500 truncate">{{ staffRole }}</p>
            </div>
          </div>
          <!-- No shift-schedule endpoint exists yet, so this is a static
               "logged in = on duty" placeholder, not a real per-staff shift
               status. Swap in real data once that endpoint exists. -->
          <div class="mt-3 pt-3 border-t border-stone-100 flex items-center justify-between">
            <span class="text-[11.5px] text-stone-500">Status</span>
            <span class="inline-flex items-center gap-1.5 rounded-full bg-emerald-50 px-2.5 py-1 text-[11px] font-medium text-emerald-700">
              <span class="h-1.5 w-1.5 rounded-full bg-emerald-600" />
              On Duty
            </span>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from "vue";
import { useRouter } from "vue-router";
import { Bell, Settings } from "lucide-vue-next";
import { getAccount } from "@/utils/auth";

defineProps({
  title: { type: String, required: true },
  breadcrumb: { type: String, default: "" },
});

const router = useRouter();
const API_BASE = "http://localhost:57147/api";

const account = getAccount() || {};
const user = account.user || {};
const staffName = computed(() =>
  user.name || user.Name || user.fullName || user.FullName ||
  [user.firstName || user.FirstName, user.lastName || user.LastName].filter(Boolean).join(" ") ||
  user.username || user.Username || "Staff"
);
const staffRole = computed(() => user.position || user.Position || user.jobTitle || account.role || "Staff");
const staffInitials = computed(() =>
  staffName.value
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map(w => w[0]?.toUpperCase())
    .join("") || "S"
);

// Which popover is open: 'bell' | 'avatar' | null
const open = ref(null);
const toggle = (key) => {
  open.value = open.value === key ? null : key;
};
const closeAll = (event) => {
  if (event.target.closest(".topbar-trigger") || event.target.closest(".topbar-panel")) return;
  open.value = null;
};

// Late-patient alerts. Self-fetched from the same Queue/today endpoint the
// dashboard uses, so the bell is accurate on every Staff*.vue page, not
// just the dashboard.
const lateQueue = ref([]);
const loadingLate = ref(false);
let lateRefreshInterval = null;

const loadLateQueue = async () => {
  loadingLate.value = true;
  try {
    const response = await fetch(`${API_BASE}/Queue/today`);
    if (!response.ok) throw new Error(`Failed to load queue. Status: ${response.status}`);
    const data = await response.json();
    lateQueue.value = data
      .filter((q) => (q.status || "Waiting") === "Late")
      .map((q) => ({
        queueID: q.queueID,
        no: `Q-${String(q.queueNumber).padStart(3, "0")}`,
        child: q.children?.map((c) => c.name).join(", ") || "—",
        parent: q.requestBy || "—",
      }));
  } catch (error) {
    console.error("Late queue loading error:", error);
    lateQueue.value = [];
  } finally {
    loadingLate.value = false;
  }
};

const goToLate = (q) => {
  open.value = null;
  router.push("/staff/dashboard");
};

onMounted(() => {
  loadLateQueue();
  document.addEventListener("click", closeAll);
  lateRefreshInterval = setInterval(loadLateQueue, 15000);
});

onUnmounted(() => {
  if (lateRefreshInterval) clearInterval(lateRefreshInterval);
  document.removeEventListener("click", closeAll);
});
</script>