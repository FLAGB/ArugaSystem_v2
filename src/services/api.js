/**
 * API Service - Handles all communication with the .NET backend
 * Backend runs on: http://localhost:57147
 */

const API_BASE_URL = "http://localhost:57147/api";

export const apiService = {
  // ========================================
  // PARENTS ENDPOINTS
  // ========================================

  // Get all parents (for staff to view)
  async getAllParents() {
    try {
      const response = await fetch(`${API_BASE_URL}/Parents/all`);
      if (!response.ok) throw new Error("Failed to fetch parents");
      return await response.json();
    } catch (error) {
      console.error("Error fetching parents:", error);
      throw error;
    }
  },

  // Get single parent by ID
  async getParentById(parentId) {
    try {
      const response = await fetch(`${API_BASE_URL}/Parents/${parentId}`);
      if (!response.ok) throw new Error("Failed to fetch parent");
      return await response.json();
    } catch (error) {
      console.error("Error fetching parent:", error);
      throw error;
    }
  },

  // Create new parent account
  async createParent(parentData) {
    try {
      const response = await fetch(`${API_BASE_URL}/Parents`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          firstName: parentData.firstName,
          lastName: parentData.lastName,
          email: parentData.email,
          contactNo: parentData.contact,
          address: parentData.address,
          barangayNo: parentData.barangay,
          password: parentData.password,
        }),
      });
      if (!response.ok) throw new Error("Failed to create parent");
      return await response.json();
    } catch (error) {
      console.error("Error creating parent:", error);
      throw error;
    }
  },

  // Update parent
  async updateParent(parentId, parentData) {
    try {
      const response = await fetch(`${API_BASE_URL}/Parents/${parentId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          firstName: parentData.firstName,
          lastName: parentData.lastName,
          email: parentData.email,
          contactNo: parentData.contact,
          address: parentData.address,
          barangayNo: parentData.barangay,
        }),
      });
      if (!response.ok) throw new Error("Failed to update parent");
      return await response.json();
    } catch (error) {
      console.error("Error updating parent:", error);
      throw error;
    }
  },

  // ========================================
  // CHILDREN ENDPOINTS
  // ========================================

  // Get all children (for staff view)
  async getAllChildren() {
    try {
      const response = await fetch(`${API_BASE_URL}/Children/all`);
      if (!response.ok) throw new Error("Failed to fetch children");
      return await response.json();
    } catch (error) {
      console.error("Error fetching children:", error);
      throw error;
    }
  },

  // Get children by parent ID
  async getChildrenByParent(parentId) {
    try {
      const response = await fetch(`${API_BASE_URL}/Children/parent/${parentId}`);
      if (!response.ok) throw new Error("Failed to fetch children");
      return await response.json();
    } catch (error) {
      console.error("Error fetching children:", error);
      throw error;
    }
  },

  // Create new child record.
  // NOTE: ChildrenController's real CreateChildDto takes a `Parents` ARRAY
  // (at least one required), not the singular parentId/motherId/fatherId/
  // guardianId this used to send — those fields don't exist on the DTO and
  // were silently ignored, which is why child creation never actually
  // linked a parent. `childData.parents` should be an array of
  // { parentID, relationshipType, isPrimaryContact, canReceiveNotifications }.
  async createChild(childData) {
    try {
      const response = await fetch(`${API_BASE_URL}/Children`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          firstName: childData.firstName,
          lastName: childData.lastName,
          middleName: childData.middleName || null,
          birthDate: childData.birthDate,
          placeOfBirth: childData.birthPlace,
          sex: childData.sex,
          barangay: childData.barangay,
          address: childData.address,
          healthCenter: childData.healthCenter || null,
          parents: childData.parents || [],
        }),
      });
      if (!response.ok) throw new Error("Failed to create child");
      return await response.json();
    } catch (error) {
      console.error("Error creating child:", error);
      throw error;
    }
  },

  // Update child record — scalar fields only. Linked parents are managed
  // separately via the relationship endpoints below (ChildParentRelationshipsController),
  // not through this call.
  async updateChild(childId, childData) {
    try {
      const response = await fetch(`${API_BASE_URL}/Children/${childId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          firstName: childData.firstName,
          lastName: childData.lastName,
          middleName: childData.middleName || null,
          birthDate: childData.birthDate,
          placeOfBirth: childData.birthPlace,
          sex: childData.sex,
          barangay: childData.barangay,
          address: childData.address,
          healthCenter: childData.healthCenter || null,
        }),
      });
      if (!response.ok) throw new Error("Failed to update child");
      return await response.json();
    } catch (error) {
      console.error("Error updating child:", error);
      throw error;
    }
  },

  // ========================================
  // RELATIONSHIP ENDPOINTS (for linking) — real routes are
  // ChildParentRelationshipsController (/api/ChildParentRelationships),
  // not /Children/{id}/link-parent, which doesn't exist on the backend.
  // ========================================

  // Link parent to child. relationship must be "Mother" | "Father" | "Guardian".
  async linkParentToChild(parentId, childId, relationship, isPrimaryContact = false) {
    try {
      const response = await fetch(`${API_BASE_URL}/ChildParentRelationships`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          childID: childId,
          parentID: parentId,
          relationshipType: relationship,
          isPrimaryContact,
          canReceiveNotifications: true,
        }),
      });
      if (!response.ok) throw new Error("Failed to link parent");
      return await response.json();
    } catch (error) {
      console.error("Error linking parent:", error);
      throw error;
    }
  },

  // Remove parent link from child. relationshipId is the RelationshipID
  // returned by linkParentToChild / getRelationshipsByChild, NOT a parentId.
  async unlinkParentFromChild(relationshipId) {
    try {
      const response = await fetch(`${API_BASE_URL}/ChildParentRelationships/${relationshipId}`, {
        method: "DELETE",
      });
      if (!response.ok) throw new Error("Failed to unlink parent");
      return await response.json();
    } catch (error) {
      console.error("Error unlinking parent:", error);
      throw error;
    }
  },

  // Get all relationships for a child / a parent
  async getRelationshipsByChild(childId) {
    const response = await fetch(`${API_BASE_URL}/ChildParentRelationships/child/${childId}`);
    if (!response.ok) throw new Error("Failed to fetch relationships");
    return await response.json();
  },

  async getRelationshipsByParent(parentId) {
    const response = await fetch(`${API_BASE_URL}/ChildParentRelationships/parent/${parentId}`);
    if (!response.ok) throw new Error("Failed to fetch relationships");
    return await response.json();
  },

  // ========================================
// VACCINE INVENTORY
// ========================================

async getInventory() {
  try {
    const response = await fetch(`${API_BASE_URL}/VaccineInventory`);

    if (!response.ok)
      throw new Error("Failed to fetch inventory");

    return await response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
},

async getInventoryById(id) {
  const response = await fetch(`${API_BASE_URL}/VaccineInventory/${id}`);

  if (!response.ok)
    throw new Error("Failed to fetch inventory");

  return await response.json();
},

async createInventory(data) {
  const response = await fetch(`${API_BASE_URL}/VaccineInventory`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok)
    throw new Error("Failed to create inventory");

  return await response.json();
},

async updateInventory(id, data) {
  const response = await fetch(`${API_BASE_URL}/VaccineInventory/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok)
    throw new Error("Failed to update inventory");

  return await response.json();
},

async deleteInventory(id) {
  const response = await fetch(`${API_BASE_URL}/VaccineInventory/${id}`, {
    method: "DELETE",
  });

  if (!response.ok)
    throw new Error("Failed to delete inventory");

  return true;
},

// ========================================
// VACCINE CATALOG
// ========================================

async getVaccinesWithDoses() {
  const response = await fetch(`${API_BASE_URL}/Vaccines/with-doses`);

  if (!response.ok)
    throw new Error("Failed to fetch vaccine catalog");

  return await response.json();
},
};