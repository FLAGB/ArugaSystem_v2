# ARUGA Frontend Analysis Report

**Project:** Aruga System - Pediatric Immunization Tracker  
**Backend URL:** `http://localhost:57147/api`  
**Date Analyzed:** 2026-07-15

---

## 1. FILES IN `src/services/` DIRECTORY

### Location: `src/services/api.js`
- **Single file:** `api.js`
- **Type:** API Service (Centralized HTTP client)
- **Purpose:** Handles all communication with .NET backend

---

## 2. API ENDPOINTS IN `api.js`

### Base URL
```
http://localhost:57147/api
```

### Parent Endpoints
| Method | Endpoint | Function | Status | Body/Params |
|--------|----------|----------|--------|-------------|
| GET | `/Parents/all` | `getAllParents()` | ✅ Implemented | N/A |
| GET | `/Parents/{id}` | `getParentById(parentId)` | ✅ Implemented | parentId (URL param) |
| POST | `/Parents` | `createParent(parentData)` | ✅ Implemented | firstName, lastName, email, contactNo, address, barangayNo, password |
| PUT | `/Parents/{id}` | `updateParent(parentId, parentData)` | ✅ Implemented | firstName, lastName, email, contactNo, address, barangayNo |

### Child Endpoints
| Method | Endpoint | Function | Status | Body/Params |
|--------|----------|----------|--------|-------------|
| GET | `/Children/all` | `getAllChildren()` | ✅ Implemented | N/A |
| GET | `/Children/parent/{id}` | `getChildrenByParent(parentId)` | ✅ Implemented | parentId (URL param) |
| POST | `/Children` | `createChild(childData)` | ✅ Implemented | firstName, lastName, middleName, birthDate, placeOfBirth, sex, barangay, address, parentId, motherId, fatherId, guardianId |
| PUT | `/Children/{id}` | `updateChild(childId, childData)` | ✅ Implemented | firstName, lastName, middleName, birthDate, placeOfBirth, sex, barangay, address |

### Relationship/Linking Endpoints
| Method | Endpoint | Function | Status | Body/Params |
|--------|----------|----------|--------|-------------|
| POST | `/Children/{id}/link-parent` | `linkParentToChild(parentId, childId, relationship)` | ✅ Implemented | parentId, relationship ("Mother" \| "Father" \| "Guardian") |
| DELETE | `/Children/{id}/unlink-parent/{parentId}` | `unlinkParentFromChild(childId, parentId)` | ✅ Implemented | childId, parentId (URL params) |

---

## 3. ROUTER CONFIGURATION

### Location: `src/router/index.js`

#### Parent/Login Routes
| Path | Component | Purpose |
|------|-----------|---------|
| `/` | Login.vue | Parent login page |
| `/StaffLogin` | StaffLogin.vue | Staff login page |
| `/admin-login` | Admin.vue | Admin login page |
| `/ParentHome` | ParentHomepage.vue | Parent dashboard |

#### Admin Routes
| Path | Component | Purpose |
|------|-----------|---------|
| `/AdminHome` | AdminHomepage.vue | Admin dashboard |
| `/admin/parents` | StaffParentRecord.vue | Parent registration/management |
| `/admin/children` | StaffChildRecord.vue | Child registration/management |
| `/admin/staff` | StaffDoctorNurse.vue | Staff management |
| `/admin/calendar` | StaffCalendar.vue | Calendar view |
| `/admin/reports` | StaffReport.vue | Reports |
| `/admin/vaccine-inventory` | StaffVaccineInventory.vue | Vaccine inventory |

#### Doctor/Nurse Routes
| Path | Component | Purpose |
|------|-----------|---------|
| `/doctor/home` | DoctorHomepage.vue | Doctor dashboard |
| `/doctor/patients` | DoctorPatients.vue | Patient list |
| `/doctor/calendar` | DoctorCalendar.vue | Calendar |
| `/doctor/records` | DoctorVaccinationRecords.vue | Vaccination records |
| `/doctor/reports` | DoctorReports.vue | Reports |
| `/doctor/account` | DoctorAccount.vue | Account settings |

#### System Admin Routes
| Path | Component | Purpose |
|------|-----------|---------|
| `/system-admin/home` | SystemAdmin.vue | System admin dashboard |
| `/system-admin/user-management` | SysAd-UserManagement.vue | User management |
| `/system-admin/patients` | SysAd-Patient.vue | Patient management |
| `/system-admin/vaccines` | SysAd-Vaccine.vue | Vaccine management |
| `/system-admin/inventory` | SysAd-Inventory.vue | Inventory management |
| `/system-admin/notifications` | SysAd-Notification.vue | Notifications |
| `/system-admin/reports` | SysAd-Reports.vue | Reports |
| `/system-admin/audit-logs` | SysAd-Auditlogs.vue | Audit logs |

#### Staff Routes
| Path | Component | Purpose |
|------|-----------|---------|
| `/staff/dashboard` | StaffDashboard.vue | Staff dashboard |
| `/staff/patient-records` | StaffPatientRecords.vue | Patient records management |
| `/staff/vaccine-schedule` | StaffVaccineSchedule.vue | Vaccine schedule |

---

## 4. COMPONENTS HANDLING REGISTRATION

### A. PARENT REGISTRATION COMPONENTS

#### 1. **StaffParentRecord.vue** ⭐ PRIMARY
- **Location:** `src/components/Admin/StaffParentRecord.vue`
- **Route:** `/admin/parents`
- **Role:** Admin/Staff parent management
- **API Base:** `http://localhost:57147/api/Vaccination` ⚠️ **DIFFERENT FROM api.js**
- **API Calls:**
  - GET `/GetAllParents` - Fetch all parents
  - POST `/SaveParent` - Create parent
  
**Form Fields:**
```javascript
{
  GivenName: '',
  MiddleName: '',
  LastName: '',
  Email: '',
  ContactNo: '',
  BarangayNo: '',
  Address: '',
  Password: ''
}
```

**Validation:**
- Password: 8-16 characters required
- Email uniqueness check (500 error = already registered)

**Issues:**
- ❌ Uses different API endpoint than `api.js` (Vaccination controller vs Parents controller)
- ❌ No error handling for duplicate emails
- ❌ Password not hashed before sending

---

#### 2. **ParentManager.vue** ⚠️ TEST COMPONENT
- **Location:** `src/components/Login/ParentManager.vue`
- **Type:** Test bench component
- **API Base:** `http://localhost:57147/api/Vaccination`
- **API Calls:**
  - POST `/SaveParent` - Register parent
  - GET `/GetParents` - Fetch all parents
  - DELETE - Delete parent

**Form Fields:**
```javascript
{
  id: '00000000-0000-0000-0000-000000000000',
  givenName: '',
  middleName: '',
  lastName: '',
  email: '',
  contactNo: '',
  barangayNo: '',
  address: ''
}
```

**Notes:** 
- Simple test UI with toggle between register/list view
- No password field (test-only)

---

#### 3. **StaffPatientRecords.vue** 🆕 NEW IMPLEMENTATION
- **Location:** `src/components/Staff/StaffPatientRecords.vue`
- **Route:** `/staff/patient-records`
- **Type:** Unified parent/child management
- **API Service:** Uses `apiService` from `src/services/api.js`
- **API Calls:**
  - `apiService.getAllParents()`
  - `apiService.createParent(parentData)`
  - `apiService.updateParent(parentId, parentData)`

**Form Fields:**
```javascript
FORM_FIELDS.parent = [
  { key: "name", label: "Full Name", full: true },
  { key: "contact", label: "Contact Number", full: true },
  { key: "email", label: "Email", full: true },
  { key: "username", label: "Username" },
  { key: "password", label: "Password", type: "password" },
  { key: "address", label: "Address", full: true },
  { key: "barangay", label: "Barangay", full: true },
]
```

---

### B. CHILD REGISTRATION COMPONENTS

#### 1. **StaffChildRecord.vue** ⭐ PRIMARY
- **Location:** `src/components/Admin/StaffChildRecord.vue`
- **Route:** `/admin/children`
- **Role:** Admin/Staff child management
- **API Base:** `http://localhost:57147/api/Vaccination` ⚠️ **DIFFERENT FROM api.js**
- **API Calls:**
  - GET `/GetAllChildren` - Fetch all children
  - POST `/SaveChild` - Create child
  - PUT `/UpdateChild/{id}` - Update child
  - GET `/GetAllParents` - For parent linking

**Form Fields:**
```javascript
{
  FirstName: '',
  MiddleName: '',
  LastName: '',
  BirthDate: '',
  Sex: 'Male',
  PlaceOfBirth: '',
  Address: '',
  MotherName: '',
  MotherEmail: '',
  MotherID: null,
  FatherName: '',
  FatherEmail: '',
  FatherID: null,
  GuardianName: '',
  GuardianEmail: '',
  GuardianID: null,
  BirthWeight: null,
  BirthHeight: null,
  HealthCenter: 'Leveriza Health Center',
  Barangay: '',
  FamilyNo: ''
}
```

**Features:**
- ✅ Parent/Guardian linking (search & select)
- ✅ Manual entry option for parents
- ✅ Mother, Father, Guardian support
- ✅ Profile view/edit modal
- ✅ Refresh functionality

**Issues:**
- ⚠️ Uses different API endpoint than `api.js` (Vaccination controller vs Children controller)
- ⚠️ Different response format from ParentManager.vue

---

#### 2. **StaffPatientRecords.vue** 🆕 NEW IMPLEMENTATION
- **Location:** `src/components/Staff/StaffPatientRecords.vue`
- **Route:** `/staff/patient-records`
- **API Service:** Uses `apiService` from `src/services/api.js`
- **API Calls:**
  - `apiService.getAllChildren()`
  - `apiService.createChild(childData)`
  - `apiService.updateChild(childId, childData)`
  - `apiService.linkParentToChild(parentId, childId, relationship)`

**Form Fields:**
```javascript
FORM_FIELDS.child = [
  { key: "name", label: "Child Name", full: true },
  { key: "birthDate", label: "Birth Date", type: "date" },
  { key: "birthPlace", label: "Birth Place" },
  { key: "sex", type: "select", placeholder: "Sex", options: ["Male", "Female"] },
  { key: "vaccStatus", type: "select", placeholder: "Vaccination Status" },
  { key: "height", label: "Height (cm)" },
  { key: "weight", label: "Weight (kg)" },
  { key: "address", label: "Address", full: true },
  { key: "barangay", label: "Barangay", full: true },
]
```

**Features:**
- ✅ Generic picker modal for linking parents
- ✅ Search functionality for finding parents
- ✅ Linked accounts display
- ✅ Profile drawer for viewing details

---

### C. PARENT LOGIN COMPONENT

#### **Login.vue**
- **Location:** `src/components/Login/Login.vue`
- **Route:** `/`
- **Purpose:** Parent login page
- **API Call:**
  - POST `http://localhost:57148/api/Parents/login` ⚠️ **WRONG PORT (57148 instead of 57147)**

**Form Fields:**
```javascript
{
  email: '',
  password: ''
}
```

**Issues:**
- ❌ **CRITICAL:** API port is wrong (57148 instead of 57147)
- ⚠️ Login response stored in localStorage as `parentUser`

---

### D. PARENT DASHBOARD COMPONENT

#### **ParentHomepage.vue**
- **Location:** `src/components/ParentViews/ParentHomepage.vue`
- **Route:** `/ParentHome`
- **Purpose:** Parent dashboard - view child vaccination records
- **API Calls:**
  - Uses `apiService.getAllChildren()` and `apiService.getChildrenByParent()`
  - Displays child information, vaccination status, and upcoming doses

**Features:**
- ✅ Child search by name or ID
- ✅ Display vaccination status
- ✅ Queue management view
- ✅ Notification system
- ✅ Profile management

---

## 5. REQUEST/RESPONSE FORMATS

### Parent Registration Request
```json
// Using Vaccination API (StaffParentRecord.vue)
{
  "Id": "00000000-0000-0000-0000-000000000000",
  "GivenName": "John",
  "MiddleName": "R",
  "LastName": "Doe",
  "Email": "john@example.com",
  "ContactNo": "09171234567",
  "BarangayNo": "001",
  "Address": "123 Main St",
  "Password": "password123"
}

// Using Parents API (StaffPatientRecords.vue)
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "contactNo": "09171234567",
  "address": "123 Main St",
  "barangayNo": "001",
  "password": "password123"
}
```

### Child Registration Request
```json
// Using Vaccination API (StaffChildRecord.vue)
{
  "FirstName": "Jane",
  "MiddleName": "M",
  "LastName": "Doe",
  "BirthDate": "2024-01-15",
  "Sex": "Female",
  "PlaceOfBirth": "Manila",
  "Address": "123 Main St",
  "MotherName": "Maria Doe",
  "MotherEmail": "maria@example.com",
  "MotherID": "mom-uuid-here",
  "FatherName": "",
  "FatherEmail": "",
  "FatherID": null,
  "GuardianName": "",
  "GuardianEmail": "",
  "GuardianID": null,
  "BirthWeight": 3.5,
  "BirthHeight": 50,
  "HealthCenter": "Leveriza Health Center",
  "Barangay": "001",
  "FamilyNo": "FAM-001"
}

// Using Children API (StaffPatientRecords.vue)
{
  "firstName": "Jane",
  "lastName": "Doe",
  "middleName": "M",
  "birthDate": "2024-01-15",
  "placeOfBirth": "Manila",
  "sex": "Female",
  "barangay": "001",
  "address": "123 Main St",
  "parentId": "parent-uuid",
  "motherId": "mom-uuid",
  "fatherId": "dad-uuid",
  "guardianId": null
}
```

---

## 6. CRITICAL ISSUES & DISCREPANCIES

### 🔴 CRITICAL ISSUES

1. **DUPLICATE API ENDPOINTS**
   - `src/services/api.js` uses: `http://localhost:57147/api/Parents` and `http://localhost:57147/api/Children`
   - `StaffParentRecord.vue` uses: `http://localhost:57147/api/Vaccination/SaveParent`
   - `StaffChildRecord.vue` uses: `http://localhost:57147/api/Vaccination/SaveChild`
   - **Impact:** Two different backend controllers expected - confusion about which is correct

2. **WRONG LOGIN PORT** (Login.vue)
   - Uses port **57148** instead of **57147**
   - **Impact:** Parent login will fail

3. **MISSING FIELD NAME CONSISTENCY**
   - `api.js` expects: `firstName`, `lastName`, `contactNo`, `barangayNo`
   - `StaffParentRecord.vue` sends: `GivenName`, `MiddleName`, `LastName`, `BarangayNo`, `ContactNo`
   - `StaffChildRecord.vue` sends: `FirstName`, `MiddleName`, `LastName`, `Barangay`, `FamilyNo`
   - **Impact:** API calls will fail if backend expects exact field names

### ⚠️ MISSING VALIDATIONS

- No email format validation (regex)
- No phone number format validation
- No password strength requirements (except length 8-16)
- No duplicate parent/child name validation
- No address validation
- No birth date future validation (reject if > today)

### ⚠️ MISSING ERROR HANDLING

- No network error handling in most components
- No timeout handling
- No retry logic
- Generic "Error saving record" messages (not user-friendly)
- No validation error feedback

### ⚠️ MISSING SECURITY

- Passwords sent in plain text over HTTP (not HTTPS)
- No password hashing on frontend (should be backend-only)
- No CSRF token handling
- No session timeout
- No permission checks

---

## 7. MISSING API ENDPOINTS (Should Exist)

Based on `api.js` implementation, these endpoints should exist on backend:

| Endpoint | Method | Purpose | Status |
|----------|--------|---------|--------|
| `/api/Parents` | POST | Create parent | ❓ Unknown |
| `/api/Parents/{id}` | PUT | Update parent | ❓ Unknown |
| `/api/Parents/all` | GET | Get all parents | ❓ Unknown |
| `/api/Parents/{id}` | GET | Get single parent | ❓ Unknown |
| `/api/Children` | POST | Create child | ❓ Unknown |
| `/api/Children/{id}` | PUT | Update child | ❓ Unknown |
| `/api/Children/all` | GET | Get all children | ❓ Unknown |
| `/api/Children/parent/{id}` | GET | Get children by parent | ✅ Likely exists |
| `/api/Children/{id}/link-parent` | POST | Link parent to child | ❓ Unknown |
| `/api/Children/{id}/unlink-parent/{parentId}` | DELETE | Unlink parent | ❓ Unknown |

---

## 8. RECOMMENDATIONS

### Priority 1 - FIX CRITICAL ISSUES
- [ ] Fix Login.vue API port (57148 → 57147)
- [ ] Standardize API endpoints (decide: use Vaccination or Parents/Children controller)
- [ ] Standardize request field names across all components
- [ ] Update all components to use `api.js` service consistently

### Priority 2 - VALIDATION & ERROR HANDLING
- [ ] Add email validation (regex pattern)
- [ ] Add phone number validation
- [ ] Add birth date validation (no future dates)
- [ ] Add password strength validation
- [ ] Implement proper error messages for each failure type
- [ ] Add try-catch to all async operations

### Priority 3 - SECURITY
- [ ] Force HTTPS in production
- [ ] Implement secure session management
- [ ] Add CSRF protection
- [ ] Hash passwords on backend (not frontend)
- [ ] Add authentication middleware to protected routes
- [ ] Implement logout functionality

### Priority 4 - USER EXPERIENCE
- [ ] Add loading indicators during API calls
- [ ] Add success notifications
- [ ] Add form auto-save for long forms
- [ ] Implement real-time validation feedback
- [ ] Add "unsaved changes" warning

### Priority 5 - CODE QUALITY
- [ ] Create single API service class in `api.js`
- [ ] Remove duplicate axios calls from components
- [ ] Add API request logging for debugging
- [ ] Add unit tests for validation logic
- [ ] Document API contract in OpenAPI/Swagger

---

## 9. SUMMARY TABLE

| Aspect | Status | Notes |
|--------|--------|-------|
| **Parent Registration** | ⚠️ Partial | Two implementations, different APIs |
| **Child Registration** | ⚠️ Partial | Two implementations, different APIs |
| **API Service** | ✅ Complete | `src/services/api.js` fully implemented |
| **Router Configuration** | ✅ Complete | All routes defined |
| **Parent Login** | ❌ Broken | Wrong port (57148) |
| **Parent Dashboard** | ✅ Working | Can view children & vaccines |
| **Error Handling** | ⚠️ Minimal | Generic messages only |
| **Validation** | ⚠️ Minimal | Only password length checked |
| **Security** | ❌ Weak | Passwords in plain text, no HTTPS |
| **API Consistency** | ❌ Poor | Three different endpoints for same functions |

---

## 10. FILE LOCATIONS QUICK REFERENCE

```
src/
├── services/
│   └── api.js                          ← Centralized API service
├── router/
│   └── index.js                        ← Route definitions
└── components/
    ├── Login/
    │   ├── Login.vue                   ← Parent login ❌ Wrong port
    │   ├── StaffLogin.vue
    │   ├── Admin.vue
    │   └── ParentManager.vue           ← Test parent registration
    ├── ParentViews/
    │   └── ParentHomepage.vue          ← Parent dashboard
    ├── Admin/
    │   ├── StaffParentRecord.vue       ← Staff parent management
    │   ├── StaffChildRecord.vue        ← Staff child management
    │   ├── AdminHomepage.vue
    │   └── ...
    ├── Doctor/Nurse/
    │   ├── DoctorHomepage.vue
    │   ├── DoctorPatients.vue
    │   └── ...
    ├── Staff/
    │   ├── StaffPatientRecords.vue     ← NEW: Unified parent/child management
    │   ├── StaffDashboard.vue
    │   └── StaffVaccineSchedule.vue
    └── SystemAdmin/
        ├── SystemAdmin.vue
        └── SysAd-*.vue
```

---

## END OF ANALYSIS
