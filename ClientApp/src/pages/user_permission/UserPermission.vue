<template>
  <div>
    <h5>Gestionar Roles</h5>
    <div class="mb-3">
      <label class="form-label">Roles Asignados</label>
      <ul class="list-group">
        <li v-for="role in userRoles" :key="role" class="list-group-item d-flex justify-content-between align-items-center">
          {{ translatedRoles[role.normalizedName] }}
          <button class="btn btn-danger btn-sm" :disabled="loading" @click="removeRole(role)">Quitar</button>
        </li>
      </ul>
    </div>
    <div class="mb-3">
      <label class="form-label">Asignar Nuevo Rol</label>
      <div class="input-group">
        <select v-model="selectedRole" class="form-select">
          <option v-for="role in filteredAvailableRoles" :key="role" :value="role">{{ translatedRoles[role.normalizedName] || role.name }}</option>
        </select>
        <button class="btn btn-primary" @click="assignRole" :disabled="loading">Asignar</button>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  props: {
    userId: String,
  },
  data() {
    return {
      userRoles: [],
      availableRoles: [],
      selectedRole: '',
      translatedRoles: {
        'VERIFIER': 'Verificador',
        'MUNICIPAL_WORKSHOP': 'Taller municipal',
        'EXTERNAL_WORKSHOP': 'Taller externo',
        'ADMINISTRATOR': 'Administrador'
      },
      loading: false
    };
  },
  methods: {
    async fetchRoles() {
      try {
        const [userRolesRes, availableRolesRes] = await Promise.all([
          axios.get(`/api/RolesApi/GetUserRoles/${this.userId}`),
          axios.get('/api/RolesApi/GetAvailableRoles')
        ]);
        this.userRoles = userRolesRes.data;
        this.availableRoles = availableRolesRes.data.filter(role => !this.userRoles.includes(role));
      } catch (error) {
        console.error("Error al obtener los roles", error);
      }
    },
    async assignRole() {
      if (!this.selectedRole) return;
      try {
        this.loading = true
        await axios.post('/api/RolesApi/AssignRoleToUser', { userId: this.userId, roleId: this.selectedRole.id });
        this.userRoles.push(this.selectedRole);
        this.selectedRole = '';
      } catch (error) {
        console.error("Error al asignar rol", error);
      }
      this.loading = false
    },
    async removeRole(role) {
      try {
        this.loading = true
        await axios.post('/api/RolesApi/RemoveRoleFromUser', { userId: this.userId, roleId: role.id });
        this.userRoles = this.userRoles.filter(userRole => userRole.id !== role.id);
        this.loading = false
      } catch (error) {
        console.error("Error al quitar rol", error);
        this.loading = false
      }
    }
  },
  computed: {
    filteredAvailableRoles() {
      return this.availableRoles.filter(role => !this.userRoles.some(userRole => userRole.id === role.id));
    }
  },
  mounted() {
    this.fetchRoles();
  }
};
</script>