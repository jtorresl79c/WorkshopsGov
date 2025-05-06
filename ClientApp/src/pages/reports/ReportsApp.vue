<template>
  <div class="container mt-4 p-2">
    <h4>Exportar Inspecciones</h4>
    <div class="row g-2 mb-3">
      <div class="col-md-3">
        <label>Desde:</label>
        <input type="date" v-model="inspection.from" class="form-control" />
      </div>
      <div class="col-md-3">
        <label>Hasta:</label>
        <input type="date" v-model="inspection.to" class="form-control" />
      </div>
      <div class="col-md-3">
        <label>Tipo de servicio:</label>
        <select v-model="inspection.serviceId" name="serviceId" class="form-select">
          <option value="">Todos</option>
          <option v-for="s in services" :key="s.id" :value="s.id">
            {{ s.name }}
          </option>
        </select>
      </div>
      <div class="col-md-3 d-flex align-items-end">
        <button @click="exportInspections" class="btn btn-success w-100">Exportar Excel</button>
      </div>
    </div>

    <hr />

    <h4>Exportar Vehículos</h4>
    <div class="row g-2">
      <div class="col-md-3">
        <label>Departamento:</label>
        <select v-model="vehicles.departmentId" class="form-select">
          <option value="">Todos</option>
          <option v-for="d in departments" :key="d.id" :value="d.id">
            {{ d.name }}
          </option>
        </select>
      </div>
      <div class="col-md-3 d-flex align-items-end">
        <button @click="exportVehicles" class="btn btn-success w-100">Exportar Excel</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ExportFilters",
  data() {
    return {
      inspection: {
        from: "",
        to: "",
        serviceId: "",
      },
      vehicles: {
        departmentId: "",
      },
      services: [],
      departments: [],
    };
  },
  mounted() {
    fetch("/api/catalogs/inspection-services")
        .then((res) => res.json())
        .then((data) => {
          this.services = data;
        });

    fetch("/api/catalogs/departments")
        .then((res) => res.json())
        .then((data) => {
          this.departments = data;
        });
  },
  methods: {
    exportInspections() {
      const params = new URLSearchParams();
      if (this.inspection.from) params.append("from", this.inspection.from);
      if (this.inspection.to) params.append("to", this.inspection.to);
      if (this.inspection.serviceId) params.append("serviceId", this.inspection.serviceId);

      window.open(`/api/spreadsheets/inspections?${params.toString()}`, "_blank");
    },
    exportVehicles() {
      const params = new URLSearchParams();
      if (this.vehicles.departmentId) params.append("departmentId", this.vehicles.departmentId);

      window.open(`/api/spreadsheets/vehicles?${params.toString()}`, "_blank");
    },
  },
};
</script>

<style scoped>
label {
  font-weight: 500;
}
</style>
