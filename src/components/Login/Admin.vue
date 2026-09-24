<template>
  <div class="min-h-screen flex">

    <!-- LEFT SIDE -->
    <div
      class="w-1/2 relative bg-cover bg-center"
      style="background-image: url('https://images.unsplash.com/photo-1576091160550-2173dba999ef');"
    >
      <!-- Overlay -->
      <div class="absolute inset-0 bg-linear-to-b from-green-900/80 via-green-800/75 to-green-700/80"></div>

      <!-- Content -->
      <div class="relative z-10 h-full flex flex-col justify-between p-12 text-white">

        <!-- Logo -->
        <div>
          <h1 class="text-3xl font-bold tracking-wide">
            ARUGA
          </h1>
          <p class="text-white/70 text-sm mt-1">
            Admin Portal
          </p>
        </div>

        <!-- Main Text -->
        <div class="max-w-xl">
          <h2 class="text-5xl font-bold leading-tight mb-6">
            System Administrator Access
          </h2>

          <p class="text-lg text-white/90 leading-relaxed">
            Manage users, healthcare workers, schedules, reports, and
            secure platform operations for ARUGA.
          </p>
        </div>

        <!-- Bottom -->
        <div class="flex gap-6 text-sm text-white/80">
          <span>● Admin Access Only</span>
          <span>● Secure & Encrypted</span>
        </div>

      </div>
    </div>

    <!-- RIGHT SIDE -->
    <div class="w-1/2 bg-[#fff8ec] flex items-center justify-center">

      <!-- SAME SIZE AS LOGIN -->
      <div class="w-105 bg-white rounded-3xl shadow-xl p-10">

        <h2 class="text-3xl font-bold text-[#2d3a26] mb-2">
          Admin Login
        </h2>

        <p class="text-gray-500 mb-8">
          Access the administrator dashboard
        </p>

        <!-- Email -->
        <div class="mb-5">
          <label class="block mb-2 text-sm text-gray-700">
            Admin Email
          </label>

          <input
            type="email"
            placeholder="admin@aruga.com"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none"
          />
        </div>

        <!-- Password -->
        <div class="mb-5">
          <label class="block mb-2 text-sm text-gray-700">
            Password
          </label>

          <input
            type="password"
            placeholder="Enter password"
            class="w-full border-2 border-[#dcccac] rounded-xl px-4 py-3 focus:outline-none"
          />
        </div>

        <!-- Remember -->
        <div class="flex justify-between text-sm mb-6">
          <label class="flex gap-2">
            <input type="checkbox" />
            Remember me
          </label>

          <a href="#" class="text-[#546b41]">
            Forgot password?
          </a>
        </div>

        <!-- Button -->
        <button
          class="w-full bg-[#546b41] text-white py-3 rounded-xl mb-4 hover:bg-[#435534]"
        >
          Sign In
        </button>

        <!-- Navigation -->
        <p class="text-center text-sm mt-4 text-gray-500">
          Healthcare Worker?
          <span
            @click="$router.push('/StaffLogin')"
            class="text-[#546b41] font-bold cursor-pointer hover:underline"
          >
            Login here
          </span>
        </p>

        <p class="text-center text-sm mt-2 text-gray-500">
          Parent Portal?
          <span
            @click="$router.push('/')"
            class="text-[#546b41] font-bold cursor-pointer hover:underline"
          >
            Login here
          </span>
        </p>

      </div>

    </div>

  </div>
</template>

<script setup>
</script>