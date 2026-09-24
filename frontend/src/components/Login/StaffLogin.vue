<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// ─── Bound to the input fields ───────────────────────────────
const username = ref('')   // ← the email/username field
const password = ref('')   // ← the password field
const error    = ref('')   // ← shown in red below the form
const loading  = ref(false)

// ─── Called when "Sign In to Portal" is clicked ──────────────
async function handleLogin() {
  error.value   = ''
  loading.value = true

  try {
    const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/staff-login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: username.value,
        password: password.value,
      }),
    })

    if (!res.ok) {
      const data = await res.json()
      error.value = data.message || 'Invalid credentials.'
      return
    }

    const data = await res.json()

    // Save token + user so DoctorHomepage.vue can read them
    // DoctorHomepage reads: u.UserID, u.FirstName, u.LastName, u.UserType, u.PRCNo
    localStorage.setItem('aruga_token', data.token)
    localStorage.setItem('aruga_user', JSON.stringify({
      UserID:    data.user.userID,
      FirstName: data.user.firstName,
      LastName:  data.user.lastName,
      UserType:  data.user.userType,
      PRCNo:     data.user.prcNo,
    }))

    // Route based on role
    const role = data.user.userType
    if (role === 'Doctor' || role === 'Nurse')
      router.push('/doctor/home')
    else if (role === 'Admin' || role === 'Staff')
      router.push('/admin/home')
    else
      error.value = 'Unknown role. Contact administrator.'

  } catch (e) {
    error.value = 'Could not connect to server. Is the API running?'
  } finally {
    loading.value = false
  }
}

// ─── Navigation helpers for the footer links ─────────────────
function goParentLogin() { router.push('/') }
function goAdminLogin()  { router.push('/admin-login')  }
</script>

<template>
  <div class="min-h-screen flex">

    <!-- LEFT SIDE — hero panel -->
    <div
      class="w-1/2 relative bg-cover bg-center"
      style="background-image:url('https://images.unsplash.com/photo-1576091160550-2173dba999ef');"
    >
      <!-- Green Gradient -->
      <div class="absolute inset-0 bg-gradient-to-b from-green-900/70 via-green-800/70 to-green-700/80"></div>

      <!-- Text -->
      <div class="relative z-10 h-full flex flex-col justify-between p-12 text-white">

        <!-- Logo -->
        <div>
          <h1 class="text-3xl font-bold tracking-wide">ARUGA</h1>
          <p class="text-sm text-white/70 mt-1">Staff Portal</p>
        </div>

        <!-- Main -->
        <div class="max-w-xl">
          <h2 class="text-5xl font-bold leading-tight mb-6">
            Healthcare Worker Portal
          </h2>
          <p class="text-lg text-white/90 leading-relaxed">
            Manage child immunization records, monitor vaccination schedules,
            and ensure DOH EPI compliance for Filipino children.
          </p>
        </div>

        <!-- Bottom -->
        <div class="flex gap-6 text-sm text-white/80">
          <span>● Secure & Encrypted</span>
          <span>● HIPAA Compliant</span>
        </div>

      </div>
    </div>

    <!-- RIGHT SIDE — login form -->
    <div class="w-1/2 bg-[#fff8ec] flex items-center justify-center">

      <div class="w-[420px] bg-white rounded-3xl shadow-xl p-10">

        <!-- Header -->
        <h2 class="text-2xl font-bold text-[#2d3a26] mb-2">Healthcare Worker Login</h2>
        <p class="text-gray-500 mb-8">Access the Healthcare Worker Portal</p>

        <!-- ── Username field ─────────────────────────────────── -->
        <div class="mb-5">
          <label class="block mb-2 text-sm text-gray-700">Username</label>
          <input
            v-model="username"
            type="text"
            placeholder="Enter your Username"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none focus:border-[#546b41]"
          />
        </div>

        <!-- ── Password field ─────────────────────────────────── -->
        <div class="mb-5">
          <label class="block mb-2 text-sm text-gray-700">Password</label>
          <input
            v-model="password"
            type="password"
            placeholder="Enter your password"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none focus:border-[#546b41]"
          />
        </div>

        <!-- ── Error message (shown when login fails) ─────────── -->
        <p v-if="error" class="text-red-500 text-sm mb-4 -mt-2">{{ error }}</p>

        <!-- Remember / Forgot -->
        <div class="flex justify-between text-sm mb-6">
          <label class="flex gap-2 items-center cursor-pointer">
            <input type="checkbox" />
            Remember me
          </label>
          <a href="#" class="text-[#546b41]">Forgot password?</a>
        </div>

        <!-- ── Sign In button — calls handleLogin() ───────────── -->
        <button
          @click="handleLogin"
          :disabled="loading"
          class="w-full bg-[#546b41] text-white py-3 rounded-xl mb-4 font-medium
                 hover:bg-[#3f5230] transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
        >
          {{ loading ? 'Signing in…' : 'Sign In to Portal' }}
        </button>


        <!-- Footer Links -->
        <p class="text-center text-sm mt-6 text-gray-500">
          Parent or Guardian?
          <span @click="goParentLogin" class="text-[#546b41] font-bold cursor-pointer hover:underline ml-1">
            Login here
          </span>
        </p>
        <p class="text-center text-sm mt-3 text-gray-500">
          Administrator?
          <span @click="goAdminLogin" class="text-[#546b41] font-bold cursor-pointer hover:underline ml-1">
            Login here
          </span>
        </p>

      </div>
    </div>

  </div>
</template>