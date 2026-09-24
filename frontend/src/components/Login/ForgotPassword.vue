<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const API_BASE = 'http://localhost:57147/api/auth/forgot-password'

// step: 'request' -> 'verify' -> 'reset' -> 'done'
const step = ref('request')
const identifier = ref('')
const deliveryMethod = ref('email') // 'email' | 'sms'
const otp = ref('')
const newPassword = ref('')
const confirmPassword = ref('')

const loading = ref(false)
const errorMessage = ref('')
const infoMessage = ref('')

const passwordsMatch = computed(() => newPassword.value.length > 0 && newPassword.value === confirmPassword.value)

const requestCode = async () => {
  errorMessage.value = ''
  infoMessage.value = ''
  if (!identifier.value.trim()) {
    errorMessage.value = 'Enter your email or username first.'
    return
  }
  loading.value = true
  try {
    const res = await axios.post(`${API_BASE}/request`, {
      identifier: identifier.value.trim(),
      deliveryMethod: deliveryMethod.value,
    })
    infoMessage.value = res.data.message
    step.value = 'verify'
  } catch (err) {
    errorMessage.value = err?.response?.data?.message || 'Something went wrong. Please try again.'
  } finally {
    loading.value = false
  }
}

const verifyCode = async () => {
  errorMessage.value = ''
  if (!otp.value.trim()) {
    errorMessage.value = 'Enter the code we sent you.'
    return
  }
  loading.value = true
  try {
    await axios.post(`${API_BASE}/verify`, {
      identifier: identifier.value.trim(),
      otp: otp.value.trim(),
    })
    step.value = 'reset'
  } catch (err) {
    errorMessage.value = err?.response?.data?.message || 'Invalid or expired code.'
  } finally {
    loading.value = false
  }
}

const resetPassword = async () => {
  errorMessage.value = ''
  if (!passwordsMatch.value) {
    errorMessage.value = 'Passwords do not match.'
    return
  }
  loading.value = true
  try {
    await axios.post(`${API_BASE}/reset`, {
      identifier: identifier.value.trim(),
      otp: otp.value.trim(),
      newPassword: newPassword.value,
    })
    step.value = 'done'
  } catch (err) {
    errorMessage.value = err?.response?.data?.message || 'Could not reset password. Please try again.'
  } finally {
    loading.value = false
  }
}

const resendCode = () => {
  step.value = 'request'
  otp.value = ''
  errorMessage.value = ''
  infoMessage.value = ''
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-[#fff8ec]">
    <div class="w-[420px] bg-white rounded-3xl shadow-xl p-10">

      <h2 class="text-3xl font-bold text-[#2d3a26] mb-2">Reset Password</h2>
      <p class="text-gray-500 mb-8">
        <span v-if="step === 'request'">Enter your email or username to get a verification code.</span>
        <span v-else-if="step === 'verify'">Enter the code we sent you.</span>
        <span v-else-if="step === 'reset'">Choose a new password.</span>
        <span v-else>All set!</span>
      </p>

      <!-- STEP 1: request -->
      <template v-if="step === 'request'">
        <div class="mb-5">
          <label class="block mb-2 text-sm text-gray-700">Email or Username</label>
          <input
            v-model="identifier"
            type="text"
            placeholder="parent@example.com or your staff username"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none"
          />
        </div>

        <div class="mb-6">
          <label class="block mb-2 text-sm text-gray-700">Send code via</label>
          <div class="flex gap-4 text-sm">
            <label class="flex items-center gap-2">
              <input type="radio" value="email" v-model="deliveryMethod" /> Email
            </label>
            <label class="flex items-center gap-2">
              <input type="radio" value="sms" v-model="deliveryMethod" /> SMS
            </label>
          </div>
        </div>

        <button
          @click="requestCode"
          :disabled="loading"
          class="w-full bg-[#546b41] text-white py-3 rounded-xl mb-4 disabled:opacity-60"
        >
          {{ loading ? 'Sending...' : 'Send Code' }}
        </button>
      </template>

      <!-- STEP 2: verify -->
      <template v-else-if="step === 'verify'">
        <p v-if="infoMessage" class="text-[#546b41] text-sm mb-4">{{ infoMessage }}</p>

        <div class="mb-6">
          <label class="block mb-2 text-sm text-gray-700">Verification Code</label>
          <input
            v-model="otp"
            type="text"
            inputmode="numeric"
            maxlength="6"
            placeholder="6-digit code"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none tracking-widest text-center"
          />
        </div>

        <button
          @click="verifyCode"
          :disabled="loading"
          class="w-full bg-[#546b41] text-white py-3 rounded-xl mb-3 disabled:opacity-60"
        >
          {{ loading ? 'Verifying...' : 'Verify Code' }}
        </button>

        <button @click="resendCode" class="w-full text-sm text-[#546b41] py-2">
          Didn't get it? Send again
        </button>
      </template>

      <!-- STEP 3: reset -->
      <template v-else-if="step === 'reset'">
        <div class="mb-5">
          <label class="block mb-2 text-sm text-gray-700">New Password</label>
          <input
            v-model="newPassword"
            type="password"
            placeholder="At least 8 characters"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none"
          />
        </div>

        <div class="mb-6">
          <label class="block mb-2 text-sm text-gray-700">Confirm New Password</label>
          <input
            v-model="confirmPassword"
            type="password"
            placeholder="Re-enter password"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none"
          />
        </div>

        <button
          @click="resetPassword"
          :disabled="loading"
          class="w-full bg-[#546b41] text-white py-3 rounded-xl mb-4 disabled:opacity-60"
        >
          {{ loading ? 'Saving...' : 'Reset Password' }}
        </button>
      </template>

      <!-- STEP 4: done -->
      <template v-else>
        <p class="text-[#546b41] text-sm mb-6">Your password has been reset. You can now sign in.</p>
        <button
          @click="router.push('/')"
          class="w-full bg-[#546b41] text-white py-3 rounded-xl mb-4"
        >
          Back to Sign In
        </button>
      </template>

      <p v-if="errorMessage" class="text-red-500 text-xs mb-2 text-center">{{ errorMessage }}</p>

      <router-link v-if="step !== 'done'" to="/" class="block text-center text-xs text-gray-400 mt-4">
        Cancel and go back to Sign In
      </router-link>
    </div>
  </div>
</template>