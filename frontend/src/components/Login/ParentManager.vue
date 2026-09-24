<template>
  <div class="min-h-screen bg-gray-100 p-8">
    <div class="max-w-4xl mx-auto">
      <h1 class="text-3xl font-bold text-blue-900 mb-6">Aruga System Test Bench</h1>

      <!-- Navigation Tabs -->
      <div class="flex space-x-4 mb-6">
        <button @click="view = 'form'" 
          :class="view === 'form' ? 'bg-blue-600 text-white' : 'bg-white text-blue-600'"
          class="px-4 py-2 rounded-lg shadow font-medium transition">Register Parent</button>
        <button @click="fetchParents" 
          :class="view === 'list' ? 'bg-blue-600 text-white' : 'bg-white text-blue-600'"
          class="px-4 py-2 rounded-lg shadow font-medium transition">View Parent List</button>
      </div>

      <!-- Registration Form -->
      <div v-if="view === 'form'" class="bg-white p-6 rounded-xl shadow-md">
        <h2 class="text-xl font-semibold mb-4 text-gray-700">Parent Registration</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <input v-model="form.givenName" type="text" placeholder="First Name" class="border p-2 rounded w-full">
          <input v-model="form.middleName" type="text" placeholder="Middle Name" class="border p-2 rounded w-full">
          <input v-model="form.lastName" type="text" placeholder="Last Name" class="border p-2 rounded w-full">
          <input v-model="form.email" type="email" placeholder="Email Address" class="border p-2 rounded w-full">
          <input v-model="form.contactNo" type="text" placeholder="Contact Number" class="border p-2 rounded w-full">
          <input v-model="form.barangayNo" type="text" placeholder="Barangay #" class="border p-2 rounded w-full">
          <textarea v-model="form.address" placeholder="Full Address" class="border p-2 rounded w-full md:col-span-2"></textarea>
        </div>
        <button @click="saveParent" class="mt-4 bg-green-600 text-white px-6 py-2 rounded font-bold hover:bg-green-700">
          Save Parent
        </button>
      </div>

      <!-- Parent List Table -->
      <div v-if="view === 'list'" class="bg-white rounded-xl shadow-md overflow-hidden">
        <table class="w-full text-left border-collapse">
          <thead class="bg-blue-50">
            <tr>
              <th class="p-4 border-b">Full Name</th>
              <th class="p-4 border-b">Email</th>
              <th class="p-4 border-b">Contact</th>
              <th class="p-4 border-b">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="parent in parents" :key="parent.id" class="hover:bg-gray-50">
              <td class="p-4 border-b">{{ parent.lastName }}, {{ parent.givenName }}</td>
              <td class="p-4 border-b">{{ parent.email }}</td>
              <td class="p-4 border-b">{{ parent.contactNo }}</td>
              <td class="p-4 border-b">
                <button @click="deleteParent(parent.id)" class="text-red-600 font-medium">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import axios from 'axios';

const API_URL = 'http://localhost:57147/api/Vaccination';
const view = ref('form');
const parents = ref([]);

const form = reactive({
  id: '00000000-0000-0000-0000-000000000000',
  givenName: '',
  middleName: '',
  lastName: '',
  email: '',
  contactNo: '',
  barangayNo: '',
  address: ''
});

const saveParent = async () => {
  try {
    const response = await axios.post(`${API_URL}/SaveParent`, form);
    alert(response.data.message);
    if (response.data.success) resetForm();
  } catch (error) {
    console.error(error);
    alert('Error saving data. Check console.');
  }
};

const fetchParents = async () => {
  view.value = 'list';
  try {
    const response = await axios.get(`${API_URL}/GetParents`);
    parents.value = response.data;
  } catch (error) {
    alert('Could not fetch data.');
  }
};

const deleteParent = async (id) => {
  if (confirm('Are you sure?')) {
    try {
      await axios.delete(`${API_URL}/DeleteParent/${id}`);
      fetchParents();
    } catch (error) {
      alert('Delete failed.');
    }
  }
};

const resetForm = () => {
  Object.assign(form, {
    id: '00000000-0000-0000-0000-000000000000',
    givenName: '', middleName: '', lastName: '',
    email: '', contactNo: '', barangayNo: '', address: ''
  });
};
</script>