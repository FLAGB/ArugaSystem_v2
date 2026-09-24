import { ref } from 'vue'
import { useRouter } from 'vue-router'

const parentData = ref(null)

export function useParentSession() {
  const router = useRouter()

  function loadParent() {
    const savedUser = localStorage.getItem('parentUser')

    if (!savedUser) {
      router.push('/')
      return null
    }

    try {
      parentData.value = JSON.parse(savedUser)
      return parentData.value
    } catch (error) {
      console.error('Invalid parent session:', error)

      localStorage.removeItem('parentUser')
      router.push('/')

      return null
    }
  }

  function logout() {
    localStorage.removeItem('parentUser')
    localStorage.removeItem('selectedChild')

    parentData.value = null

    router.push('/')
  }

  return {
    parentData,
    loadParent,
    logout
  }
}