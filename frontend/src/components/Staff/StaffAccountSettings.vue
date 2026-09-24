<!--
  StaffAccountSettings.vue
  =========================
  New page, linked from the gear icon in StaffTopbar.vue.
  - Profile info: read-only, pulled straight from the session's stored
    account/user object (whatever field names Login.vue put there) —
    nothing to fetch, nothing to save on this half.
  - Change password: real call to PATCH /api/Users/{userId}/change-password
    (added to AuthController.cs during the auth-migration work), reusing the
    same complexity rule as everywhere else in the app (8+ chars, upper,
    lower, digit, special char).

  NOTE: this page is not yet wired into router/index.js — add a route for
  path "/staff/settings" pointing at this component, or the gear icon's
  router.push will 404.
-->
<template>
  <div class="flex min-h-screen w-full bg-stone-50 text-stone-900" style="font-family: 'Inter','Segoe UI',sans-serif;">
    <StaffSidebar />

    <main class="flex-1 min-w-0">
      <StaffTopbar title="Account Settings" breadcrumb="Aruga / Account Settings" />

      <div class="px-8 py-6 max-w-2xl space-y-6">
        <!-- Profile info (read-only) -->
        <div class="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <p class="text-[14px] font-semibold mb-4">Profile</p>
          <div class="flex items-center gap-4 mb-5">
            <div class="flex h-14 w-14 items-center justify-center rounded-full text-white text-base font-semibold bg-emerald-700 shrink-0">
              {{ staffInitials }}
            </div>
            <div class="min-w-0">
              <p class="text-[15px] font-semibold truncate">{{ staffName }}</p>
              <p class="text-[12.5px] text-stone-500 truncate">{{ staffRole }}</p>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <p class="text-[11px] font-semibold uppercase tracking-wide text-stone-400 mb-1">Username</p>
              <p class="text-[13px] text-stone-700">{{ staffUsername }}</p>
            </div>
            <div>
              <p class="text-[11px] font-semibold uppercase tracking-wide text-stone-400 mb-1">Role</p>
              <p class="text-[13px] text-stone-700">{{ staffRole }}</p>
            </div>
          </div>
          <p class="mt-4 text-[11.5px] text-stone-400">
            Profile fields aren't editable here yet — there's no update-own-profile
            endpoint wired up for Staff accounts. This section is display-only for now.
          </p>
        </div>

        <!-- Change password -->
        <div class="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <p class="text-[14px] font-semibold mb-4">Change Password</p>

          <form @submit.prevent="submitPasswordChange" class="space-y-4">
            <div>
              <label class="block text-[12px] font-medium text-stone-600 mb-1">Current Password</label>
              <input
                v-model="form.currentPassword"
                type="password"
                required
                class="w-full rounded-xl border border-stone-200 px-3 py-2.5 text-[13px] focus:outline-none focus:ring-2 focus:ring-emerald-600/30 focus:border-emerald-600"
              />
            </div>
            <div>
              <label class="block text-[12px] font-medium text-stone-600 mb-1">New Password</label>
              <input
                v-model="form.newPassword"
                type="password"
                required
                class="w-full rounded-xl border border-stone-200 px-3 py-2.5 text-[13px] focus:outline-none focus:ring-2 focus:ring-emerald-600/30 focus:border-emerald-600"
              />
            </div>
            <div>
              <label class="block text-[12px] font-medium text-stone-600 mb-1">Confirm New Password</label>
              <input
                v-model="form.confirmPassword"
                type="password"
                required
                class="w-full rounded-xl border border-stone-200 px-3 py-2.5 text-[13px] focus:outline-none focus:ring-2 focus:ring-emerald-600/30 focus:border-emerald-600"
              />
            </div>

            <!-- Live complexity checklist, same rule as ForceChangePassword.vue -->
            <ul class="grid grid-cols-2 gap-x-4 gap-y-1 text-[12px]">
              <li :class="checklist.length ? 'text-emerald-700' : 'text-stone-400'">✓ 8+ characters</li>
              <li :class="checklist.upper ? 'text-emerald-700' : 'text-stone-400'">✓ Uppercase letter</li>
              <li :class="checklist.lower ? 'text-emerald-700' : 'text-stone-400'">✓ Lowercase letter</li>
              <li :class="checklist.digit ? 'text-emerald-700' : 'text-stone-400'">✓ Number</li>
              <li :class="checklist.special ? 'text-emerald-700' : 'text-stone-400'">✓ Special character</li>
              <li :class="checklist.match ? 'text-emerald-700' : 'text-stone-400'">✓ Passwords match</li>
            </ul>

            <p v-if="errorMessage" class="text-[12.5px] text-rose-700">{{ errorMessage }}</p>
            <p v-if="successMessage" class="text-[12.5px] text-emerald-700">{{ successMessage }}</p>

            <button
              type="submit"
              :disabled="submitting || !allValid"
              class="rounded-xl bg-emerald-700 px-4 py-2.5 text-[13px] font-medium text-white hover:bg-emerald-800 disabled:opacity-40 disabled:cursor-not-allowed"
            >
              {{ submitting ? "Saving…" : "Update Password" }}
            </button>
          </form>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from "vue";
import StaffSidebar from "./StaffSidebar.vue";
import StaffTopbar from "./StaffTopbar.vue";
import { getAccount } from "@/utils/auth";

const API_BASE = "http://localhost:57147/api";

const account = getAccount() || {};
const user = account.user || {};

const staffName = computed(() =>
  user.name || user.Name || user.fullName || user.FullName ||
  [user.firstName || user.FirstName, user.lastName || user.LastName].filter(Boolean).join(" ") ||
  user.username || user.Username || "Staff"
);
const staffRole = computed(() => user.position || user.Position || user.jobTitle || account.role || "Staff");
const staffUsername = computed(() => user.username || user.Username || "—");
const staffInitials = computed(() =>
  staffName.value
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map(w => w[0]?.toUpperCase())
    .join("") || "S"
);
// Guessing the same way the rest of the codebase does, since the exact
// field name on `user` for the numeric/GUID ID hasn't been confirmed yet.
const staffUserId = user.userId || user.UserID || user.id || user.Id;

const form = reactive({
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
});

const checklist = computed(() => ({
  length: form.newPassword.length >= 8,
  upper: /[A-Z]/.test(form.newPassword),
  lower: /[a-z]/.test(form.newPassword),
  digit: /\d/.test(form.newPassword),
  special: /[^A-Za-z0-9]/.test(form.newPassword),
  match: form.newPassword.length > 0 && form.newPassword === form.confirmPassword,
}));

const allValid = computed(() => Object.values(checklist.value).every(Boolean) && form.currentPassword.length > 0);

const submitting = ref(false);
const errorMessage = ref("");
const successMessage = ref("");

const submitPasswordChange = async () => {
  errorMessage.value = "";
  successMessage.value = "";

  if (!staffUserId) {
    errorMessage.value = "Couldn't determine your account ID — please log out and back in, then try again.";
    return;
  }
  if (!allValid.value) {
    errorMessage.value = "Please meet all password requirements before submitting.";
    return;
  }

  submitting.value = true;
  try {
    const response = await fetch(`${API_BASE}/Users/${staffUserId}/change-password`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        currentPassword: form.currentPassword,
        newPassword: form.newPassword,
      }),
    });

    if (!response.ok) {
      const text = await response.text().catch(() => "");
      throw new Error(text || `Failed to update password. Status: ${response.status}`);
    }

    successMessage.value = "Password updated successfully.";
    form.currentPassword = "";
    form.newPassword = "";
    form.confirmPassword = "";
  } catch (error) {
    console.error("Change password error:", error);
    errorMessage.value = error.message || "Something went wrong. Please try again.";
  } finally {
    submitting.value = false;
  }
};
</script>