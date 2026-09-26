// Which vaccination station the signed-in health worker is at.
//
// The Admission Staff puts each Doctor/Nurse at a station and sends
// checked-in patients to a station. A health worker can only vaccinate
// the patient at their own station (the backend enforces the same rule).
import { API_ORIGIN } from '@/utils/apiBase'
import axios from 'axios'
import { getUser } from '@/utils/auth'

const API = API_ORIGIN

export const sameId = (a, b) => !!a && !!b && String(a).toLowerCase() === String(b).toLowerCase()

export function myUserId() {
  return getUser()?.UserID || null
}

// { roomId, roomName, isOccupied, currentQueueId, ... } or null
export async function fetchMyStation() {
  const me = myUserId()
  const res = await axios.get(`${API}/api/Vaccination/GetRooms`)
  return res.data.find(r => sameId(r.workerId, me)) || null
}

// True when this visit (a /api/Queue/today row) is at my station right now.
export function isAtMyStation(visit) {
  const status = (visit?.status || '').toLowerCase().replace(/\s+/g, '')
  return status === 'inprogress' && sameId(visit.assignedWorkerID, myUserId())
}
