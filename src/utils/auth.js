export function getAccount() {
  const savedAccount = localStorage.getItem('account')

  if (!savedAccount) {
    return null
  }

  try {
    return JSON.parse(savedAccount)
  } catch (error) {
    console.error('Invalid account session:', error)
    return null
  }
}

export function getToken() {
  return localStorage.getItem('authToken')
}

export function getRole() {
  return getAccount()?.role || null
}

export function getUser() {
  return getAccount()?.user || null
}

// UserType is the fine-grained type inside `user` — 'Doctor' | 'Nurse' |
// 'Midwife' | 'Admission' | 'Admin' | 'Staff'. `role` (above) is the coarse
// value the route guard checks against `meta.role` ('Staff' | 'Healthcare' |
// 'Parent' | 'SystemAdmin'); UserType is what tells Doctor apart from
// Nurse/Midwife within the shared 'Healthcare' role.
export function getUserType() {
  return getUser()?.UserType || null
}

export function isLoggedIn() {
  return !!getToken() && !!getAccount()
}

export function logout() {
  localStorage.removeItem('account')
  localStorage.removeItem('authToken')

  // Parent-specific session cleanup
  localStorage.removeItem('selectedParentChild')
  localStorage.removeItem('parentUser')
  localStorage.removeItem('selectedChild')

  // Legacy keys from the old Doctor/Healthcare auth path. Safe to clear
  // unconditionally now that Login.vue no longer writes them and every
  // page reads through this file instead of localStorage directly.
  localStorage.removeItem('aruga_token')
  localStorage.removeItem('aruga_user')
}