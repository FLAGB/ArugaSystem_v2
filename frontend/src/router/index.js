import { createRouter, createWebHistory } from 'vue-router'
import { isLoggedIn, getRole, getUserType } from '@/utils/auth'

// ── Login / Change Password ─────────────────────────────────

import ChangePassword from '@/components/Login/ChangePassword.vue'
import Login from '@/components/Login/Login.vue'
import ForgotPassword from '@/components/Login/ForgotPassword.vue'

// ── Parent ───────────────────────────────────────────────────

import ParentOverview from '@/components/ParentViews/Views/Parentoverview.vue'
import ParentCheckin from '@/components/ParentViews/Views/Checkin.vue'
import ParentSchedule from '@/components/ParentViews/Views/Scheduled.vue'
import ParentRecords from '@/components/ParentViews/Views/Record.vue'

// ── Staff pages (formerly split across Admin/ and Staff/) ────

import StaffCalendar from '@/components/Staff/StaffCalendar.vue'
import StaffChildRecord from '@/components/Staff/StaffChildRecord.vue'
import StaffDoctorNurse from '@/components/Staff/StaffDoctorNurse.vue'
import StaffParentRecord from '@/components/Staff/StaffParentRecord.vue'
import StaffReport from '@/components/Staff/StaffReport.vue'
import StaffVaccineInventory from '@/components/Staff/StaffVaccineInventory.vue'
import StaffQueueManagement from '@/components/Staff/StaffQueueManagement.vue'

// ── Healthcare ──────────────────────────────────────────────

import DoctorHomepage from '@/components/Doctor/Nurse/DoctorHomepage.vue'
import DoctorPatients from '@/components/Doctor/Nurse/DoctorPatients.vue'
import DoctorCalendar from '@/components/Doctor/Nurse/DoctorCalendar.vue'
import DoctorVaccinationRecords from '@/components/Doctor/Nurse/DoctorVaccinationRecords.vue'
import DoctorReports from '@/components/Doctor/Nurse/DoctorReports.vue'
import DoctorAccount from '@/components/Doctor/Nurse/DoctorAccount.vue'
import DoctorQueue from '@/components/Doctor/Nurse/DoctorQueue.vue'

// ── System Admin ────────────────────────────────────────────

import SystemAdminHomepage from '@/components/SystemAdmin/SystemAdmin.vue'
import SystemAdminUserManagement from '@/components/SystemAdmin/SysAd-UserManagement.vue'
import SystemAdminPatient from '@/components/SystemAdmin/SysAd-Patient.vue'
import SystemAdminVaccine from '@/components/SystemAdmin/SysAd-Vaccine.vue'
import SystemAdminInventory from '@/components/SystemAdmin/SysAd-Inventory.vue'
import SystemAdminNotification from '@/components/SystemAdmin/SysAd-Notification.vue'
import SystemAdminReports from '@/components/SystemAdmin/SysAd-Reports.vue'
import SystemAuditlogs from '@/components/SystemAdmin/SysAd-Auditlogs.vue'
import TestAPI from '@/components/SystemAdmin/TestAPI.vue'
import VaccineSchedule from '@/components/SystemAdmin/VaccineSchedule.vue'
import SystemOperatingHours from '@/components/SystemAdmin/SysAd-Operating_hours.vue'

// ── Staff ────────────────────────────────────────────────────

import StaffDashboard from '@/components/Staff/StaffDashboard.vue'
import StaffAccountSettings from '@/components/Staff/StaffAccountSettings.vue'
import StaffPatientRecords from '@/components/Staff/StaffPatientRecords.vue'
import StaffVaccineSchedule from '@/components/Staff/StaffVaccineSchedule.vue'

// ── Healthcare ──────────────────────────────────────────────

import HealthCareHome from '@/components/Healthcare/HealthcareHome.vue'
import HealthCareQueue from '@/components/Healthcare/HealthcareQueue.vue'
import HealthCareCalendar from '@/components/Healthcare/HealthcareCalendar.vue'
import HealthcarePatient from '@/components/Healthcare/HealthcarePatient.vue'
import HealthcareReports from '@/components/Healthcare/HealthcareReports.vue'
import HealthcareAccounts from '@/components/Healthcare/HealthcareAccounts.vue'
import HealthcareVaccinationRecords from '@/components/Healthcare/HealthcareVaccinationRecords.vue'
import HealthcareVaccination from '@/components/Healthcare/HealthcareVaccination.vue'
import Registration from '@/components/Login/Registration.vue'


const routes = [

  //Registration
{path: '/registration', component: Registration},
  // ── Login ──────────────────────────────────────────────────

  {
    path: '/',
    name: 'Login',
    component: Login
  },

  {
    path: '/forgot-password',
    name: 'ForgotPassword',
    component: ForgotPassword
  },


  // ── Parent ─────────────────────────────────────────────────

  {
    path: '/ParentOverview',
    component: ParentOverview,
    meta: {
      requiresAuth: true,
      role: 'Parent'
    }
  },

  {
    path: '/ParentCheckin',
    component: ParentCheckin,
    meta: {
      requiresAuth: true,
      role: 'Parent'
    }
  },

  {
    path: '/ParentRecords',
    component: ParentRecords,
    meta: {
      requiresAuth: true,
      role: 'Parent'
    }
  },

  {
    path: '/ParentSchedule',
    component: ParentSchedule,
    meta: {
      requiresAuth: true,
      role: 'Parent'
    }
  },


  // ── Healthcare ─────────────────────────────────────────────

  {
    path: '/healthcare/home',
    component: HealthCareHome,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/healthcare/queue',
    component: HealthCareQueue,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/healthcare/calendar',
    component: HealthCareCalendar,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/healthcare/patients',
    component: HealthcarePatient,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  // Read-only records list (sidebar item). Declared BEFORE the
  // dynamic /healthcare/vaccination/:queueId route below.
  {
    path: '/healthcare/vaccination-records',
    name: 'HealthcareVaccinationRecords',
    component: HealthcareVaccinationRecords,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  // Recording flow — reached from Queue → "Start Vaccinating",
  // which pushes /healthcare/vaccination/:queueId?child=<childID>
  {
    path: '/healthcare/vaccination/:queueId',
    name: 'HealthcareVaccination',
    component: HealthcareVaccination,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/healthcare/reports',
    component: HealthcareReports,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/healthcare/accounts',
    component: HealthcareAccounts,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },


  // ── Old Doctor routes ──────────────────────────────────────
  // Healthcare role only

  {
    path: '/doctor/home',
    name: 'DoctorHome',
    component: DoctorHomepage,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/doctor/patients',
    name: 'DoctorPatients',
    component: DoctorPatients,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/doctor/calendar',
    name: 'DoctorCalendar',
    component: DoctorCalendar,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/doctor/records',
    name: 'DoctorVaccinationRecords',
    component: DoctorVaccinationRecords,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/doctor/reports',
    name: 'DoctorReports',
    component: DoctorReports,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  {
    path: '/doctor/account',
    name: 'DoctorAccount',
    component: DoctorAccount,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

    {
    path: '/doctor/queue',
    name: 'DoctorQueue',
    component: DoctorQueue,
    meta: {
      requiresAuth: true,
      role: 'Healthcare'
    }
  },

  // ── Staff ──────────────────────────────────────────────────

  {
    path: '/staff/dashboard',
    component: StaffDashboard,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

 {
  path: '/staff/patient-records',
  component: StaffPatientRecords,
  meta: {
    requiresAuth: false
  }
},

  {
    path: '/staff/vaccine-schedule',
    component: StaffVaccineSchedule,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },


  // ── Formerly "Old Admin routes" ────────────────────────────
  // Renamed from /admin/* to /staff/* — these were always Staff*.vue
  // components gated by role: 'Staff' (not SystemAdmin), so the /admin/
  // prefix was just leftover naming from before AdminHomepage.vue was
  // retired (see note above), not an actual permission boundary.
  // /AdminHome removed: AdminHomepage.vue is retired — its real
  // functionality (GetRooms/AssignRoom/CompleteSession) now lives in
  // StaffDashboard.vue at /staff/dashboard.

  {
    path: '/staff/calendar',
    component: StaffCalendar,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

  {
    path: '/staff/children',
    component: StaffChildRecord,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

  {
    path: '/staff/doctor-staff',
    component: StaffDoctorNurse,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

  {
    path: '/staff/parents',
    component: StaffParentRecord,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

  {
    path: '/staff/reports',
    component: StaffReport,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

  {
    path: '/staff/vaccine-inventory',
    component: StaffVaccineInventory,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

  {
    path: '/staff/queue-management',
    component: StaffQueueManagement,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },

  {
      path: '/staff/settings',
    component: StaffAccountSettings,
    meta: {
      requiresAuth: true,
      role: 'Staff'
    }
  },


  // ── System Admin ───────────────────────────────────────────

  {
    path: '/system-admin/home',
    component: SystemAdminHomepage,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/user-management',
    component: SystemAdminUserManagement,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/patients',
    component: SystemAdminPatient,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/vaccines',
    component: SystemAdminVaccine,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/inventory',
    component: SystemAdminInventory,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/notifications',
    component: SystemAdminNotification,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/reports',
    component: SystemAdminReports,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/audit-logs',
    component: SystemAuditlogs,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/test-api',
    component: TestAPI,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/vaccine-schedule',
    component: VaccineSchedule,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },

  {
    path: '/system-admin/operating-hours',
    component: SystemOperatingHours,
    meta: {
      requiresAuth: true,
      role: 'SystemAdmin'
    }
  },


  // ── Change Password ────────────────────────────────────────
  // Any authenticated account can access this.

  {
    path: '/ChangePassword',
    component: ChangePassword,
    meta: {
      requiresAuth: true
    }
  }
]


const router = createRouter({
  history: createWebHistory(),
  routes
})

function homeFor(role) {
  switch (role) {
    case 'Parent':
      return '/ParentOverview'
    case 'Healthcare':
      return getUserType() === 'Doctor' ? '/doctor/home' : '/healthcare/home'
    case 'Staff':
      return '/staff/dashboard'
    case 'SystemAdmin':
      return '/system-admin/home'
    default:
      return '/'
  }
}


// ============================================================
// AUTHENTICATION / ROLE GUARD
// ============================================================

router.beforeEach((to) => {

  const loggedIn = isLoggedIn()
  const role = getRole()


  // ----------------------------------------------------------
  // 1. Protected page + NOT logged in
  // ----------------------------------------------------------

  if (to.meta.requiresAuth && !loggedIn) {
    return '/'
  }


  // ----------------------------------------------------------
  // 2. Already logged in + tries to open Login
  // ----------------------------------------------------------

  if (to.path === '/' && loggedIn) {
    return homeFor(role)
  }


  // ----------------------------------------------------------
  // 3. Logged in but wrong role
  // ----------------------------------------------------------

  if (to.meta.role && to.meta.role !== role) {
    return homeFor(role)
  }


  // ----------------------------------------------------------
  // 3b. Doctor and Healthcare share meta.role: 'Healthcare', so #3
  //     above can't tell them apart — split by UserType here.
  // ----------------------------------------------------------

  if (to.path.startsWith('/doctor/') && getUserType() !== 'Doctor') {
    return homeFor(role)
  }

  if (to.path.startsWith('/healthcare/') && getUserType() === 'Doctor') {
    return homeFor(role)
  }

  return true
})


export default router