<template>
    <div class="container-fluid px-4">
        <nav aria-label="breadcrumb" class="mb-3">
            <ol class="breadcrumb mb-0">
                <li class="breadcrumb-item">
                    <a href="/ReviewCenter">Centro de Revisión</a>
                </li>
                <li class="breadcrumb-item active" aria-current="page">Revisión</li>
            </ol>
        </nav>

        <h2 class="h4 fw-semibold mb-3">Centro de Revisión</h2>

        <div class="kanban-scroll-wrapper overflow-auto mb-3">
            <div class="d-flex flex-nowrap" style="gap: 12px;">
                <!-- Diagnosticos terminados -->
                <div class="bg-info bg-opacity-25 rounded p-3 shadow-sm text-center"
                     @click="filterStatus(6)">
                    <h6 class="text-white fw-semibold mb-0">Diagnósticos Finalizados ({{ countByStatus(6) }})</h6>
                </div>
                <!-- Asignadas -->
                <div class="bg-light rounded p-3 shadow-sm text-center" style="min-width: 250px; cursor: pointer;"
                     @click="filterStatus(2)">
                    <h6 class="text-secondary fw-semibold mb-0">Asignadas ({{ countByStatus(2) }})</h6>
                </div>

                <!-- Por aprobar cotización -->
                <div class="bg-warning bg-opacity-25 rounded p-3 shadow-sm text-center" style="min-width: 250px; cursor: pointer;"
                     @click="filterStatus(4)">
                    <h6 class="text-warning fw-semibold mb-0">Por aprobar cotización ({{ countByStatus(4) }})</h6>
                </div>

                <!-- En reparación -->
                <div class="bg-primary rounded p-3 shadow-sm text-center" style="min-width: 250px;">
                    <h6 class="text-white fw-semibold mb-0">En reparación</h6>
                </div>

                <!-- Verificadas -->
                <div class="bg-success bg-opacity-25 rounded p-3 shadow-sm text-center" style="min-width: 250px;">
                    <h6 class="text-success fw-semibold mb-0">Verificadas</h6>
                </div>
            </div>
        </div>

        <!-- Tabla general -->
        <div class="card shadow-sm">
            <div class="card-body p-3">
              <div class="d-flex justify-content-between align-content-center py-3">
                <h5 class="card-title mb-3">Listado general de seguimiento de inspecciones</h5>
              </div>
              
                <div class="table-responsive">
                    <table class="table table-bordered table-hover table-striped text-nowrap align-middle mb-0">
                        <thead class="table table-dark">
                            <tr>
                                <th>Memo</th>
                                <th>Unidad</th>
                                <th>Estado</th>
                                <th>Sucursal</th>
                                <th>Fecha de Inspección</th>
                                <th>Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-if="isLoading">
                                <td colspan="5" class="text-center">Cargando...</td>
                            </tr>
                            <tr v-else-if="filteredInspections.length === 0">
                                <td colspan="5" class="text-center">No hay inspecciones asignadas.</td>
                            </tr>
                            <tr v-else v-for="insp in filteredInspections" :key="insp.id">
                                <td>{{ insp.memoNumber }}</td>
                                <td>{{ insp.vehiculo }}</td>
                                <td>
                                    <span class="badge bg-secondary">{{ insp.estado }}</span>
                                    <br v-if="insp.estatusId === 2" />
                                    <small v-if="insp.estatusId === 2" class="text-muted fst-italic">En espera de cotización</small>
                                </td>
                                <td>{{ insp.taller }}</td>
                                <td>{{ new Date(insp.inspectionDate).toLocaleDateString('es-MX') }}</td>
                                <td>
                                    <a :href="`/Inspections/Details/${insp.id}`" class="btn btn-sm btn-outline-primary">
                                        Detalles
                                    </a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { onMounted, ref } from 'vue'
    import axios from 'axios'

    const assignedInspections = ref([])
    const filteredInspections = ref([])
    const selectedStatus = ref(2)
    const isLoading = ref(true)

    const countByStatus = (statusId) => {
        return assignedInspections.value.filter(i => i.estatusId === statusId).length
    }

    const filterStatus = (statusId) => {
        selectedStatus.value = statusId
        filteredInspections.value = assignedInspections.value.filter(i => i.estatusId === statusId)
    }

    onMounted(async () => {
        try {
            const { data } = await axios.get('/api/reviewcenter/assigned')
            assignedInspections.value = data
            filteredInspections.value = data.filter(i => i.estatusId === selectedStatus.value)
        } catch (error) {
            console.error('Error al cargar inspecciones asignadas:', error)
        } finally {
            isLoading.value = false
        }
    })
</script>


<style scoped>
    .kanban-scroll-wrapper {
        padding-bottom: 0.25rem;
    }
</style>
