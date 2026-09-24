# QUICK START GUIDE - Connecting Vue Frontend to .NET Backend

## ✅ What I've Done for You

1. **Created API Service** (`src/services/api.js`)
   - All HTTP calls in one place
   - Ready to use with your backend
   - Handles errors automatically

2. **Updated StaffPatientRecords Component** 
   - Now loads data from API on mount
   - Register form creates real parent/child accounts
   - Edit form updates records in database
   - Automatic data transformation

3. **Prepared Backend Endpoints** (`API_ENDPOINTS_TO_ADD.cs`)
   - Copy-paste ready code
   - All DTOs included
   - Just add to your Controllers

---

## 🚀 NEXT STEPS (DO THIS NOW)

### Step 1: Start Your .NET Backend
```powershell
cd c:\Users\renzo\Desktop\ARUGA_Capstone\AndroidWebAPI\AndroidWebAPI
dotnet run
```
Should see: `Application started. Press Ctrl+C to shut down.`

### Step 2: Add Missing API Endpoints
1. Open `AndroidWebAPI/Controllers/ParentsController.cs`
2. Copy methods from `API_ENDPOINTS_TO_ADD.cs` (Parents section)
3. Open `AndroidWebAPI/Controllers/ChildrenController.cs`  
4. Copy methods from `API_ENDPOINTS_TO_ADD.cs` (Children section)
5. Save and restart backend (Ctrl+C, then `dotnet run`)

### Step 3: Verify in Browser
1. Go to `http://localhost:5174/staff/patient-records`
2. Should now show your real database data
3. Try registering a parent/child - should appear in database

---

## 📋 API Base URL
Frontend connects to: `http://localhost:57147/api`

Make sure your Program.cs has this:
```csharp
builder.WebHost.UseUrls(
    "http://localhost:57147",
    "http://localhost:57148"
);
```

---

## ⚠️ IMPORTANT: Password Security

The current code stores passwords as **plain text**. Before going to production:

1. Install BCrypt:
   ```
   dotnet add package BCrypt.Net-Next
   ```

2. Update CreateParent endpoint:
   ```csharp
   using BCrypt.Net;
   
   newParent.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
   ```

3. Update Login:
   ```csharp
   bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
   ```

---

## 🔗 Endpoint Summary

### Parents
- `GET /api/Parents/all` - Get all parents
- `GET /api/Parents/{id}` - Get single parent
- `POST /api/Parents` - Create parent ← YOU NEED TO ADD THIS
- `PUT /api/Parents/{id}` - Update parent ← YOU NEED TO ADD THIS

### Children
- `GET /api/Children/all` - Get all children
- `GET /api/Children/parent/{parentId}` - Get children by parent
- `POST /api/Children` - Create child ← YOU NEED TO ADD THIS
- `PUT /api/Children/{id}` - Update child ← YOU NEED TO ADD THIS
- `DELETE /api/Children/{childId}/unlink-parent/{parentId}` - Unlink parent ← YOU NEED TO ADD THIS

---

## 🧪 Testing the Integration

### Test 1: Load Data
1. Backend running? ✓
2. Navigate to `/staff/patient-records` 
3. Should see your database parents/children, not mock data

### Test 2: Create Parent
1. Click "Register Parent"
2. Fill form: name, email, contact, password
3. Click "Register Parent" button
4. Should show success modal
5. Check database - new record added!

### Test 3: Edit Parent
1. Click eye icon on any parent row
2. Click edit button
3. Change some fields
4. Click "Save Changes"
5. Check database - updated!

---

## 🐛 Troubleshooting

**"Failed to fetch" error?**
- Backend not running - start with `dotnet run`
- Wrong port - check Program.cs has `:57147`
- CORS issue - check Program.cs has `AllowAnyOrigin()`

**"Endpoint not found" error?**
- Endpoints not added to backend yet
- Check API_ENDPOINTS_TO_ADD.cs
- Make sure you added the methods

**Data not showing?**
- Check database connection string in appsettings.json
- Check if Parents/Children tables exist in SQL Server
- Check browser console for errors (F12)

---

## 📝 File Locations

- **Frontend**: `src/components/Staff/StaffPatientRecords.vue`
- **API Service**: `src/services/api.js`
- **Backend Controllers**: 
  - `AndroidWebAPI/Controllers/ParentsController.cs`
  - `AndroidWebAPI/Controllers/ChildrenController.cs`
- **Endpoint Template**: `API_ENDPOINTS_TO_ADD.cs` (reference only)

---

Good luck! Let me know if you hit any errors 🚀
