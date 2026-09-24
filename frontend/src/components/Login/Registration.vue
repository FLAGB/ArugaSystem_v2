<script setup>
import { ref, computed } from "vue";
import {
  Baby,
  UserRound,
  Phone,
  Mail,
  MapPin,
  Check,
  ArrowRight,
  ArrowLeft,
  Plus,
  Trash2,
  ClipboardCheck,
  Info,
} from "lucide-vue-next";
import axios from "axios";

const API_BASE = "http://localhost:57147/api";

const api = {
  submitRegistration: (payload) =>
    axios.post(`${API_BASE}/RegistrationRequests`, payload).then((r) => r.data),
};

const currentStep = ref(1);
const submitting = ref(false);
const submitted = ref(false);
const error = ref("");

const registrationResult = ref({
  queueNumber: null,
  registrationId: null,
  status: "For Interview",
});

const parent = ref({
  firstName: "",
  middleName: "",
  lastName: "",
  contactNo: "",
  email: "",
  address: "",
});

function createChild() {
  return {
    firstName: "",
    middleName: "",
    lastName: "",
    birthDate: "",
    sex: "",
    placeOfBirth: "",
    hasExistingRecord: false,
  };
}

const children = ref([createChild()]);

function addChild() {
  children.value.push(createChild());
}

function removeChild(index) {
  if (children.value.length === 1) return;
  children.value.splice(index, 1);
}

const canContinueParent = computed(() => {
  return (
    parent.value.firstName.trim() &&
    parent.value.lastName.trim() &&
    parent.value.contactNo.trim()
  );
});

const canSubmitChildren = computed(() => {
  return children.value.every(
    (child) =>
      child.firstName.trim() &&
      child.lastName.trim() &&
      child.birthDate &&
      child.sex
  );
});

function nextStep() {
  error.value = "";

  if (currentStep.value === 1) {
    if (!canContinueParent.value) {
      error.value =
        "Please complete the required parent/guardian information.";
      return;
    }

    currentStep.value = 2;
    return;
  }

  if (currentStep.value === 2) {
    if (!canSubmitChildren.value) {
      error.value =
        "Please complete the required information for every child.";
      return;
    }

    currentStep.value = 3;
  }
}

function previousStep() {
  error.value = "";

  if (currentStep.value > 1) {
    currentStep.value--;
  }
}

async function submitRegistration() {
  error.value = "";

  if (!canSubmitChildren.value) {
    error.value =
      "Please complete the required information for every child.";
    return;
  }

  submitting.value = true;

  try {
    const payload = {
      parent: {
        firstName: parent.value.firstName.trim(),
        middleName: parent.value.middleName.trim() || null,
        lastName: parent.value.lastName.trim(),
        contactNo: parent.value.contactNo.trim(),
        email: parent.value.email.trim() || null,
        address: parent.value.address.trim() || null,
      },

      children: children.value.map((child) => ({
        firstName: child.firstName.trim(),
        middleName: child.middleName.trim() || null,
        lastName: child.lastName.trim(),
        birthDate: child.birthDate,
        sex: child.sex,
        placeOfBirth: child.placeOfBirth.trim() || null,

        // Staff will determine/link the actual patient record
        // during the interview.
        hasExistingRecord: child.hasExistingRecord,
      })),

      queueType: "Registration",
      queueStatus: "For Interview",
    };

    const result = await api.submitRegistration(payload);

    registrationResult.value = {
      queueNumber:
        result.queueNumber ||
        result.queue?.queueNumber ||
        result.QueueNumber ||
        null,

      registrationId:
        result.registrationId ||
        result.registrationRequestID ||
        result.RegistrationRequestID ||
        null,

      status:
        result.status ||
        result.queue?.status ||
        "For Interview",
    };

    submitted.value = true;
  } catch (e) {
    console.error("Registration submission failed:", e);

    error.value =
      e.response?.data?.message ||
      "Unable to submit your registration. Please try again.";
  } finally {
    submitting.value = false;
  }
}

function resetRegistration() {
  parent.value = {
    firstName: "",
    middleName: "",
    lastName: "",
    contactNo: "",
    email: "",
    address: "",
  };

  children.value = [createChild()];
  currentStep.value = 1;
  submitted.value = false;
  submitting.value = false;
  error.value = "";

  registrationResult.value = {
    queueNumber: null,
    registrationId: null,
    status: "For Interview",
  };
}
</script>

<template>
  <div
    class="min-h-screen bg-stone-50 text-stone-900"
    style="font-family: 'Inter', 'Segoe UI', sans-serif;"
  >
    <!-- Header -->
    <header class="border-b border-stone-200 bg-white">
      <div
        class="mx-auto flex max-w-4xl items-center gap-3 px-6 py-5"
      >
        <div
          class="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br from-emerald-600 to-emerald-800 text-sm font-bold text-white"
        >
          A
        </div>

        <div>
          <p class="text-[15px] font-semibold">
            Aruga Pediatric System
          </p>

          <p class="text-[11px] text-stone-500">
            Patient Registration
          </p>
        </div>
      </div>
    </header>

    <main class="mx-auto max-w-4xl px-6 py-10">
      <!-- ================= SUCCESS ================= -->
      <div v-if="submitted" class="mx-auto max-w-xl">
        <div
          class="rounded-3xl border border-stone-200 bg-white p-8 text-center shadow-sm"
        >
          <div
            class="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-emerald-50"
          >
            <Check :size="30" class="text-emerald-700" />
          </div>

          <h1 class="mt-5 text-2xl font-bold">
            Registration Submitted
          </h1>

          <p class="mt-2 text-[13px] leading-6 text-stone-500">
            Your registration has been received by the clinic.
            Please wait for Staff to verify your information and
            conduct the interview.
          </p>

          <div class="mt-6 rounded-2xl bg-stone-50 p-5">
            <p
              class="text-[11px] font-semibold uppercase tracking-wide text-stone-500"
            >
              Today's Clinic Queue
            </p>

            <p
              v-if="registrationResult.queueNumber"
              class="mt-2 text-4xl font-bold text-emerald-700"
            >
              #{{ registrationResult.queueNumber }}
            </p>

            <p
              v-else
              class="mt-2 text-lg font-semibold text-stone-700"
            >
              Registration Queue
            </p>

            <div
              class="mt-3 inline-flex items-center rounded-full bg-amber-50 px-3 py-1.5 text-xs font-semibold text-amber-700"
            >
              {{ registrationResult.status }}
            </div>
          </div>

          <div
            class="mt-5 rounded-2xl border border-sky-100 bg-sky-50 p-4 text-left"
          >
            <div class="flex items-start gap-3">
              <Info
                :size="18"
                class="mt-0.5 shrink-0 text-sky-700"
              />

              <div>
                <p class="text-[12.5px] font-semibold text-sky-900">
                  What happens next?
                </p>

                <p
                  class="mt-1 text-[12px] leading-5 text-sky-800"
                >
                  Please remain at the clinic and wait for Staff
                  to call you for your interview. During the
                  interview, Staff will confirm your information
                  and connect your account to the appropriate
                  child patient record.
                </p>
              </div>
            </div>
          </div>

          <div class="mt-6 border-t border-stone-200 pt-5">
            <p class="text-[12px] text-stone-500">
              Please bring the documents and information requested
              by the clinic.
            </p>
          </div>

          <button
            @click="resetRegistration"
            class="mt-5 rounded-xl border border-stone-200 px-5 py-2.5 text-[13px] font-medium text-stone-600 hover:bg-stone-50"
          >
            Start Another Registration
          </button>
        </div>
      </div>

      <!-- ================= REGISTRATION FORM ================= -->
      <div v-else>
        <div class="mb-8">
          <p
            class="text-[11px] font-semibold uppercase tracking-[0.16em] text-emerald-700"
          >
            Aruga Registration
          </p>

          <h1 class="mt-2 text-3xl font-bold tracking-tight">
            Register for Aruga
          </h1>

          <p class="mt-2 max-w-2xl text-[13px] leading-6 text-stone-500">
            Provide your basic information and your child's
            information. Clinic Staff will verify the details
            during the interview.
          </p>
        </div>

        <!-- Progress -->
        <div class="mb-8">
          <div class="flex items-center">
            <template v-for="step in 3" :key="step">
              <div class="flex items-center">
                <div
                  class="flex h-9 w-9 items-center justify-center rounded-full text-xs font-semibold"
                  :class="
                    step <= currentStep
                      ? 'bg-emerald-700 text-white'
                      : 'bg-stone-200 text-stone-500'
                  "
                >
                  {{ step }}
                </div>

                <div
                  v-if="step < 3"
                  class="h-px w-14 sm:w-24"
                  :class="
                    step < currentStep
                      ? 'bg-emerald-700'
                      : 'bg-stone-200'
                  "
                />
              </div>
            </template>
          </div>

          <div class="mt-2 grid grid-cols-3 max-w-[340px] text-[10.5px] text-stone-500">
            <span>Parent / Guardian</span>
            <span class="text-center">Child / Children</span>
            <span class="text-right">Review</span>
          </div>
        </div>

        <!-- Error -->
        <div
          v-if="error"
          class="mb-5 rounded-xl border border-rose-100 bg-rose-50 p-3.5 text-[12px] text-rose-700"
        >
          {{ error }}
        </div>

        <!-- ================= STEP 1 ================= -->
        <section
          v-if="currentStep === 1"
          class="rounded-2xl border border-stone-200 bg-white shadow-sm"
        >
          <div class="border-b border-stone-200 px-6 py-5">
            <div class="flex items-center gap-3">
              <div
                class="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-50"
              >
                <UserRound
                  :size="19"
                  class="text-emerald-700"
                />
              </div>

              <div>
                <h2 class="text-[16px] font-semibold">
                  Parent / Guardian Information
                </h2>

                <p class="text-[11.5px] text-stone-500">
                  Basic information for clinic verification.
                </p>
              </div>
            </div>
          </div>

          <div class="grid gap-4 p-6 sm:grid-cols-2">
            <div>
              <label class="text-[11.5px] font-medium text-stone-600">
                First Name *
              </label>

              <input
                v-model="parent.firstName"
                type="text"
                placeholder="First name"
                class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none transition focus:border-emerald-500"
              />
            </div>

            <div>
              <label class="text-[11.5px] font-medium text-stone-600">
                Middle Name
              </label>

              <input
                v-model="parent.middleName"
                type="text"
                placeholder="Middle name"
                class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none transition focus:border-emerald-500"
              />
            </div>

            <div>
              <label class="text-[11.5px] font-medium text-stone-600">
                Last Name *
              </label>

              <input
                v-model="parent.lastName"
                type="text"
                placeholder="Last name"
                class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none transition focus:border-emerald-500"
              />
            </div>

            <div>
              <label class="text-[11.5px] font-medium text-stone-600">
                Contact Number *
              </label>

              <div class="relative mt-1.5">
                <Phone
                  :size="15"
                  class="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400"
                />

                <input
                  v-model="parent.contactNo"
                  type="tel"
                  placeholder="09xxxxxxxxx"
                  class="w-full rounded-xl border border-stone-200 py-3 pl-10 pr-3.5 text-[13px] outline-none focus:border-emerald-500"
                />
              </div>
            </div>

            <div>
              <label class="text-[11.5px] font-medium text-stone-600">
                Email
              </label>

              <div class="relative mt-1.5">
                <Mail
                  :size="15"
                  class="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400"
                />

                <input
                  v-model="parent.email"
                  type="email"
                  placeholder="you@example.com"
                  class="w-full rounded-xl border border-stone-200 py-3 pl-10 pr-3.5 text-[13px] outline-none focus:border-emerald-500"
                />
              </div>
            </div>

            <div>
              <label class="text-[11.5px] font-medium text-stone-600">
                Address
              </label>

              <div class="relative mt-1.5">
                <MapPin
                  :size="15"
                  class="absolute left-3 top-3.5 text-stone-400"
                />

                <input
                  v-model="parent.address"
                  type="text"
                  placeholder="Address"
                  class="w-full rounded-xl border border-stone-200 py-3 pl-10 pr-3.5 text-[13px] outline-none focus:border-emerald-500"
                />
              </div>
            </div>
          </div>

          <div
            class="flex items-center justify-end border-t border-stone-200 px-6 py-4"
          >
            <button
              @click="nextStep"
              class="flex items-center gap-2 rounded-xl bg-emerald-700 px-5 py-2.5 text-[13px] font-semibold text-white shadow-sm hover:bg-emerald-800"
            >
              Continue
              <ArrowRight :size="15" />
            </button>
          </div>
        </section>

        <!-- ================= STEP 2 ================= -->
        <section
          v-else-if="currentStep === 2"
          class="rounded-2xl border border-stone-200 bg-white shadow-sm"
        >
          <div class="border-b border-stone-200 px-6 py-5">
            <div class="flex items-center gap-3">
              <div
                class="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-50"
              >
                <Baby
                  :size="19"
                  class="text-emerald-700"
                />
              </div>

              <div>
                <h2 class="text-[16px] font-semibold">
                  Child Information
                </h2>

                <p class="text-[11.5px] text-stone-500">
                  Add the child or children you are registering.
                </p>
              </div>
            </div>
          </div>

          <div class="space-y-4 p-6">
            <div
              v-for="(child, index) in children"
              :key="index"
              class="rounded-2xl border border-stone-200 p-5"
            >
              <div class="mb-4 flex items-center justify-between">
                <div>
                  <p class="text-[13px] font-semibold">
                    Child {{ index + 1 }}
                  </p>

                  <p class="text-[11px] text-stone-500">
                    Provide the basic information only.
                  </p>
                </div>

                <button
                  v-if="children.length > 1"
                  @click="removeChild(index)"
                  class="flex items-center gap-1.5 rounded-lg px-2.5 py-1.5 text-[11.5px] font-medium text-rose-700 hover:bg-rose-50"
                >
                  <Trash2 :size="13" />
                  Remove
                </button>
              </div>

              <div class="grid gap-4 sm:grid-cols-2">
                <div>
                  <label class="text-[11.5px] font-medium text-stone-600">
                    First Name *
                  </label>

                  <input
                    v-model="child.firstName"
                    type="text"
                    placeholder="First name"
                    class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none focus:border-emerald-500"
                  />
                </div>

                <div>
                  <label class="text-[11.5px] font-medium text-stone-600">
                    Middle Name
                  </label>

                  <input
                    v-model="child.middleName"
                    type="text"
                    placeholder="Middle name"
                    class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none focus:border-emerald-500"
                  />
                </div>

                <div>
                  <label class="text-[11.5px] font-medium text-stone-600">
                    Last Name *
                  </label>

                  <input
                    v-model="child.lastName"
                    type="text"
                    placeholder="Last name"
                    class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none focus:border-emerald-500"
                  />
                </div>

                <div>
                  <label class="text-[11.5px] font-medium text-stone-600">
                    Birth Date *
                  </label>

                  <input
                    v-model="child.birthDate"
                    type="date"
                    class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none focus:border-emerald-500"
                  />
                </div>

                <div>
                  <label class="text-[11.5px] font-medium text-stone-600">
                    Sex *
                  </label>

                  <select
                    v-model="child.sex"
                    class="mt-1.5 w-full rounded-xl border border-stone-200 bg-white px-3.5 py-3 text-[13px] outline-none focus:border-emerald-500"
                  >
                    <option value="" disabled>
                      Select sex
                    </option>

                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                  </select>
                </div>

                <div>
                  <label class="text-[11.5px] font-medium text-stone-600">
                    Place of Birth
                  </label>

                  <input
                    v-model="child.placeOfBirth"
                    type="text"
                    placeholder="Place of birth"
                    class="mt-1.5 w-full rounded-xl border border-stone-200 px-3.5 py-3 text-[13px] outline-none focus:border-emerald-500"
                  />
                </div>
              </div>

              <!-- Existing record -->
              <div
                class="mt-5 rounded-xl border border-sky-100 bg-sky-50 p-4"
              >
                <label class="flex cursor-pointer items-start gap-3">
                  <input
                    v-model="child.hasExistingRecord"
                    type="checkbox"
                    class="mt-0.5 rounded border-stone-300 text-emerald-600 focus:ring-emerald-500"
                  />

                  <span>
                    <span
                      class="block text-[12px] font-semibold text-sky-900"
                    >
                      My child may already have an Aruga patient record
                    </span>

                    <span
                      class="mt-1 block text-[11.5px] leading-5 text-sky-800"
                    >
                      If checked, you do not need to search for the
                      child. Staff will verify and link your account
                      to the existing child record during the
                      interview.
                    </span>
                  </span>
                </label>
              </div>
            </div>

            <button
              @click="addChild"
              class="flex w-full items-center justify-center gap-2 rounded-xl border border-dashed border-stone-300 px-4 py-3 text-[12.5px] font-medium text-emerald-700 hover:bg-emerald-50"
            >
              <Plus :size="15" />
              Add Another Child
            </button>
          </div>

          <div
            class="flex items-center justify-between border-t border-stone-200 px-6 py-4"
          >
            <button
              @click="previousStep"
              class="flex items-center gap-2 rounded-xl border border-stone-200 px-4 py-2.5 text-[13px] font-medium text-stone-600 hover:bg-stone-50"
            >
              <ArrowLeft :size="15" />
              Back
            </button>

            <button
              @click="nextStep"
              class="flex items-center gap-2 rounded-xl bg-emerald-700 px-5 py-2.5 text-[13px] font-semibold text-white shadow-sm hover:bg-emerald-800"
            >
              Review
              <ArrowRight :size="15" />
            </button>
          </div>
        </section>

        <!-- ================= STEP 3 ================= -->
        <section
          v-else
          class="rounded-2xl border border-stone-200 bg-white shadow-sm"
        >
          <div class="border-b border-stone-200 px-6 py-5">
            <div class="flex items-center gap-3">
              <div
                class="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-50"
              >
                <ClipboardCheck
                  :size="19"
                  class="text-emerald-700"
                />
              </div>

              <div>
                <h2 class="text-[16px] font-semibold">
                  Review Registration
                </h2>

                <p class="text-[11.5px] text-stone-500">
                  Check your information before submitting.
                </p>
              </div>
            </div>
          </div>

          <div class="space-y-6 p-6">
            <!-- Parent summary -->
            <div>
              <p class="mb-3 text-[12px] font-semibold text-stone-500">
                Parent / Guardian
              </p>

              <div
                class="rounded-xl bg-stone-50 p-4 text-[12.5px]"
              >
                <p class="font-semibold">
                  {{
                    [
                      parent.firstName,
                      parent.middleName,
                      parent.lastName,
                    ]
                      .filter(Boolean)
                      .join(" ")
                  }}
                </p>

                <p class="mt-1 text-stone-500">
                  {{ parent.contactNo }}
                  <span v-if="parent.email">
                    · {{ parent.email }}
                  </span>
                </p>

                <p
                  v-if="parent.address"
                  class="mt-1 text-stone-500"
                >
                  {{ parent.address }}
                </p>
              </div>
            </div>

            <!-- Children summary -->
            <div>
              <p class="mb-3 text-[12px] font-semibold text-stone-500">
                Children
              </p>

              <div class="space-y-2">
                <div
                  v-for="(child, index) in children"
                  :key="index"
                  class="rounded-xl border border-stone-200 p-4"
                >
                  <div class="flex items-start gap-3">
                    <div
                      class="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-emerald-50"
                    >
                      <Baby
                        :size="17"
                        class="text-emerald-700"
                      />
                    </div>

                    <div class="min-w-0 flex-1">
                      <p class="text-[12.5px] font-semibold">
                        {{
                          [
                            child.firstName,
                            child.middleName,
                            child.lastName,
                          ]
                            .filter(Boolean)
                            .join(" ")
                        }}
                      </p>

                      <p class="mt-1 text-[11.5px] text-stone-500">
                        {{ child.birthDate }}
                        ·
                        {{ child.sex }}
                      </p>

                      <span
                        v-if="child.hasExistingRecord"
                        class="mt-2 inline-flex rounded-full bg-sky-50 px-2.5 py-1 text-[10.5px] font-medium text-sky-700"
                      >
                        Existing patient record — Staff will link
                      </span>

                      <span
                        v-else
                        class="mt-2 inline-flex rounded-full bg-emerald-50 px-2.5 py-1 text-[10.5px] font-medium text-emerald-700"
                      >
                        New child record
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Information -->
            <div
              class="rounded-xl border border-amber-100 bg-amber-50 p-4"
            >
              <div class="flex items-start gap-3">
                <Info
                  :size="17"
                  class="mt-0.5 shrink-0 text-amber-700"
                />

                <p class="text-[11.5px] leading-5 text-amber-900">
                  After submission, you will be placed in today's
                  clinic queue under <strong>For Interview</strong>.
                  Staff will verify your information before
                  finalizing your registration.
                </p>
              </div>
            </div>
          </div>

          <div
            class="flex items-center justify-between border-t border-stone-200 px-6 py-4"
          >
            <button
              @click="previousStep"
              :disabled="submitting"
              class="flex items-center gap-2 rounded-xl border border-stone-200 px-4 py-2.5 text-[13px] font-medium text-stone-600 hover:bg-stone-50 disabled:opacity-50"
            >
              <ArrowLeft :size="15" />
              Back
            </button>

            <button
              @click="submitRegistration"
              :disabled="submitting"
              class="flex items-center gap-2 rounded-xl bg-emerald-700 px-5 py-2.5 text-[13px] font-semibold text-white shadow-sm hover:bg-emerald-800 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ submitting ? "Submitting..." : "Submit Registration" }}
              <Check :size="15" />
            </button>
          </div>
        </section>
      </div>
    </main>

    <!-- Footer -->
    <footer class="pb-8 text-center text-[11px] text-stone-400">
      Aruga Pediatric System
    </footer>
  </div>
</template>