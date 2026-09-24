# API ENDPOINT DISCREPANCIES & MAPPING

## Problem Summary
Three different API endpoints are being used across the frontend for the same functionality:

1. **`src/services/api.js`** - Centralized API service (NEW, standard format)
2. **`StaffParentRecord.vue` + `ParentManager.vue`** - Using Vaccination controller
3. **`StaffChildRecord.vue`** - Using Vaccination controller

---

## Parent Registration - 3 Different Implementations

### Implementation #1: `api.js` (Recommended Standard)
```javascript
// src/services/api.js
POST http://localhost:57147/api/Parents
Body: {
  firstName: "",
  lastName: "",
  email: "",
  contactNo: "",
  address: "",
  barangayNo: "",
  password: ""
}
```

### Implementation #2: `StaffParentRecord.vue` (Admin Staff)
```javascript
// src/components/Admin/StaffParentRecord.vue:256
POST http://localhost:57147/api/Vaccination/SaveParent
Body: {
  Id: "00000000-0000-0000-0000-000000000000",
  GivenName: "",
  MiddleName: "",
  LastName: "",
  Email: "",
  ContactNo: "",
  BarangayNo: "",
  Address: "",
  Password: ""
}
```

### Implementation #3: `ParentManager.vue` (Test Bench)
```javascript
// src/components/Login/ParentManager.vue:79
POST http://localhost:57147/api/Vaccination/SaveParent
Body: {
  id: "00000000-0000-0000-0000-000000000000",
  givenName: "",
  middleName: "",
  lastName: "",
  email: "",
  contactNo: "",
  barangayNo: "",
  address: ""
  // NO PASSWORD FIELD
}
```

---

## Child Registration - 2 Different Implementations

### Implementation #1: `api.js` (Recommended Standard)
```javascript
// src/services/api.js
POST http://localhost:57147/api/Children
Body: {
  firstName: "",
  lastName: "",
  middleName: "",
  birthDate: "",
  placeOfBirth: "",
  sex: "",
  barangay: "",
  address: "",
  parentId: "",
  motherId: null,
  fatherId: null,
  guardianId: null
}
```

### Implementation #2: `StaffChildRecord.vue` (Admin Staff)
```javascript
// src/components/Admin/StaffChildRecord.vue:435
POST http://localhost:57147/api/Vaccination/SaveChild
Body: {
  FirstName: "",
  MiddleName: "",
  LastName: "",
  BirthDate: "",
  Sex: "Male",
  PlaceOfBirth: "",
  Address: "",
  MotherName: "",
  MotherEmail: "",
  MotherID: null,
  FatherName: "",
  FatherEmail: "",
  FatherID: null,
  GuardianName: "",
  GuardianEmail: "",
  GuardianID: null,
  BirthWeight: null,
  BirthHeight: null,
  HealthCenter: "Leveriza Health Center",
  Barangay: "",
  FamilyNo: ""
}
```

---

## GET Requests - Also Inconsistent

### Get Parents

**Standard (api.js):**
```javascript
GET http://localhost:57147/api/Parents/all
```

**Admin Staff (StaffParentRecord.vue:234):**
```javascript
GET http://localhost:57147/api/Vaccination/GetAllParents
```

**Test (ParentManager.vue):**
```javascript
GET http://localhost:57147/api/Vaccination/GetParents
```

### Get Children

**Standard (api.js):**
```javascript
GET http://localhost:57147/api/Children/all
GET http://localhost:57147/api/Children/parent/{parentId}
```

**Admin Staff (StaffChildRecord.vue:372):**
```javascript
GET http://localhost:57147/api/Vaccination/GetAllChildren
```

---

## DECISION NEEDED

The frontend has a **architectural decision conflict**:

### Option A: Use `api.js` Everywhere
- Migrate `StaffParentRecord.vue` → use `apiService.createParent()`
- Migrate `StaffChildRecord.vue` → use `apiService.createChild()`
- Delete/disable `ParentManager.vue` (test component)
- **Requires:** Backend to implement `/api/Parents/*` and `/api/Children/*` endpoints

### Option B: Update `api.js` to Match Vaccination Controller
- Update `api.js` to use `/api/Vaccination/SaveParent`, `/api/Vaccination/SaveChild`
- Keep existing components as-is
- Keep ParentManager for testing
- **Requires:** Less backend changes, but less standardized

### **RECOMMENDATION: Option A**
✅ Standardized API layer  
✅ Better separation of concerns  
✅ Easier to maintain  
✅ Follows Vue.js best practices  
❌ Requires some backend refactoring

---

## Field Name Mapping (If Using Option B)

If staying with Vaccination controller, map fields:

| Purpose | `api.js` Field | Vaccination Field | Type |
|---------|---|---|---|
| First Name | `firstName` | `GivenName` | string |
| Last Name | `lastName` | `LastName` | string |
| Email | `email` | `Email` | string |
| Contact Number | `contactNo` | `ContactNo` | string |
| Barangay | `barangayNo` | `BarangayNo` | string |
| Address | `address` | `Address` | string |
| Password | `password` | `Password` | string |
| Middle Name | `middleName` | `MiddleName` | string |
| Birth Date | `birthDate` | `BirthDate` | ISO date |
| Sex | `sex` | `Sex` | string |
| Place of Birth | `placeOfBirth` | `PlaceOfBirth` | string |

---

## CURRENT COMPONENT API USAGE

| Component | GET Parents | POST Parent | GET Children | POST Child |
|-----------|---|---|---|---|
| `StaffParentRecord.vue` | `/Vaccination/GetAllParents` | `/Vaccination/SaveParent` | - | - |
| `ParentManager.vue` | `/Vaccination/GetParents` | `/Vaccination/SaveParent` | - | - |
| `StaffChildRecord.vue` | `/Vaccination/GetAllParents` | - | `/Vaccination/GetAllChildren` | `/Vaccination/SaveChild` |
| `StaffPatientRecords.vue` | `apiService.getAllParents()` | `apiService.createParent()` | `apiService.getAllChildren()` | `apiService.createChild()` |
| `api.js` | `/Parents/all` | `/Parents` | `/Children/all` | `/Children` |

---

## ACTION ITEMS

### Immediate (Today)
- [ ] Decide: Option A (standardize) or Option B (match existing)?
- [ ] Document decision in code comments
- [ ] Create backend roadmap

### If Option A Selected
- [ ] Verify backend has `/api/Parents/*` endpoints
- [ ] Update `StaffParentRecord.vue` to use `apiService`
- [ ] Update `StaffChildRecord.vue` to use `apiService`
- [ ] Test all flows
- [ ] Consider retiring `ParentManager.vue`

### If Option B Selected
- [ ] Update `api.js` field names to match
- [ ] Add field mapping layer
- [ ] Document why using Vaccination controller
- [ ] Add comments in code

---

## Field Standardization Proposal

### Proposed Standard (api.js style - camelCase)
```javascript
// Parent
{
  firstName: "",
  middleName: "",
  lastName: "",
  email: "",
  contactNo: "",
  address: "",
  barangayNo: "",
  password: ""
}

// Child
{
  firstName: "",
  middleName: "",
  lastName: "",
  birthDate: "",
  placeOfBirth: "",
  sex: "",
  address: "",
  barangay: "",
  birthWeight: null,
  birthHeight: null,
  healthCenter: "",
  familyNo: "",
  motherId: null,
  fatherId: null,
  guardianId: null,
  parentId: null
}
```

**Benefits:**
- ✅ Consistent camelCase (JavaScript standard)
- ✅ Easier to remember
- ✅ No PascalCase confusion
- ✅ Matches JSON convention
