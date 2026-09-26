import './assets/main.css'

import { createApp } from 'vue'
import axios from 'axios'
import App from './App.vue'
import router from '@/router'
import { getToken, logout } from '@/utils/auth'

const API_ORIGIN = (import.meta.env.VITE_API_URL || 'http://localhost:57147').replace(/\/$/, '')

// A sign-in lasts 8 hours. After that the API answers 401 to every call, so
// instead of pages silently showing nothing, go back to the login page and
// say why (then come back to this page after signing in).
function sessionExpired() {
  logout()
  const here = router.currentRoute.value
  if (here.path !== '/') {
    router.push({ path: '/', query: { expired: '1', redirect: here.fullPath } })
  }
}

// Send the signed-in user's token with every API call made through axios.
// The backend requires it (everything except login / Forgot Password), and
// uses it for role checks and for the audit log.
// Calls that already set their own Authorization header are left alone.
axios.interceptors.request.use(config => {
  const token = getToken()
  if (token && !config.headers?.Authorization) {
    config.headers = config.headers || {}
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// (A wrong password on the login page is also a 401, but no token is sent
// there, so it isn't treated as an expired session.)
axios.interceptors.response.use(
  response => response,
  error => {
    const sentToken = !!error?.config?.headers?.Authorization
    if (error?.response?.status === 401 && sentToken && getToken()) sessionExpired()
    return Promise.reject(error)
  }
)

// Some pages call the API with fetch() instead of axios; give those the same
// token and the same expired-session handling.
const nativeFetch = window.fetch.bind(window)
window.fetch = async (input, init = {}) => {
  const url = typeof input === 'string' ? input : input?.url || ''
  const token = getToken()
  const toApi = url.startsWith(API_ORIGIN)

  if (token && toApi) {
    const headers = new Headers(init.headers || (typeof input !== 'string' ? input.headers : undefined) || {})
    if (!headers.has('Authorization')) headers.set('Authorization', `Bearer ${token}`)
    init = { ...init, headers }
  }

  const response = await nativeFetch(input, init)
  if (response.status === 401 && token && toApi) sessionExpired()
  return response
}

const app = createApp(App)

app.use(router)
app.mount('#app')
