<template>
    <nav aria-label="breadcrumb" class="mb-3">
        <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item">
                <a href="/inspections">Inspecciones</a>
            </li>
            <li class="breadcrumb-item active" aria-current="page">Detalles de Inspeccion</li>
        </ol>
    </nav>


    <div v-if="isLoading" class="text-center p-5">
        <h2>Cargando detalles de inspección...</h2>
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando...</span>
        </div>
    </div>

    <!-- 🔹 Mostrar la información solo cuando los datos estén listos -->
    <div v-else>

        <div class="row">
            <div class="col-12 mb-4 col-lg-8 mb-lg-0">
                <div class="bg-white">
                    <div class="w-100 d-flex justify-content-between align-items-center border p-4 mb-4">
                        <div class="d-flex flex-column">
                            <h1 class="m-0">Detalles de Diagnostico - #{{inspection.id}} </h1>
                            <p class="m-0 fs-5 text-secondary">Operador: {{inspection.operatorName}}</p>
                            <p class="m-0 fs-5 text-secondary">Economico: {{inspection.vehicle.licensePlate}}</p>
                        </div>
                        <div>
                            <a :href="`/Inspections/edit/${inspection.id}`" class="btn btn-primary">
                                Editar
                            </a>
                        </div>
                        <div>
                        </div>
                    </div>
                </div>

                <div class="bg-white mb-4">
                    <div class="border p-4">
                        <div class="row">
                            <div class="col">
                                <h3 class="mb-3">Datos generales</h3>
                            </div>
                        </div>  <div class="row mb-lg-2">
                            <div class="col-12 col-lg-3 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">Memo</p>
                                <p class="m-0">{{inspection.memoNumber}}</p>
                            </div>
                            <div class="col-12 col-lg-5 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">Servicio</p>
                                <p class="m-0">{{inspection.inspectionService.name}}</p>
                            </div>
                            <div class="col-12 col-lg-4 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">Hora de Ingreso</p>
                                <p class="m-0">{{inspection.checkInTime}}</p>
                            </div>
                        </div>

                        <div class="row mb-lg-2">
                            <div class="col-12 col-lg-3 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">Fecha</p>
                                <p class="m-0">{{inspection.inspectionDate}}</p>
                            </div>


                            <div class="col-12 col-lg-4 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">MILLAS /KM</p>
                                {{inspection.distanceValue}} ({{inspection.distanceUnit}})
                            </div>
                        </div>
                        <div class="col-12 col-lg-12 mb-2 mb-lg-0">
                            <p class="fw-bold m-0">Reporte de Falla</p>
                            {{inspection.failureReport}}
                        </div>
                    </div>
                </div>
                <!--termina el col-->
            </div>



            <div class="col-12 mb-4 col-lg-4 mb-lg-0">



                <div class="card text-center bg-secondary bg-lighten-1">
                    <div class="card-content text-white">
                        <div class="card-body">
                            <h4 class="alert-heading d-flex justify-content-between">Estado</h4>
                            <div class="list-group">
                                {{inspection.inspectionStatus.name}}
                            </div>
                        </div>
                    </div>
                </div>

                <div class="alert alert-secondary">
                    <h4 class="alert-heading">Formatos</h4>
                    <p>N/A</p>
                </div>
                
                <div class="bg-white border p-4 mb-4">
                    <div class="d-flex flex-column align-items-end">
                        <i class="bi bi-file-earmark-pdf-fill text-danger" style="font-size: 5em"></i>
                        <!-- Enlace de descarga -->
                        <a href="ruta-del-archivo.pdf" class="mt-2 text-primary fw-bold" download>Descargar Formato</a>

                        <!-- Nombre del formato -->
                        <p class="mt-1 text-muted">Nombre del Formato</p>
                    </div>
                </div>


                <!-- 🔹 Panel del nivel de combustible -->
                <div class="card" id="nivelCombustible">
                    <div class="card-header">
                        <h4 class="card-title">Nivel de Combustible - {{ inspection.fuelLevel }}%</h4>
                    </div>
                    <div class="d-flex align-items-center justify-content-center border p-3">
                        <div class="svg-container mb-4">
                            <img class="gasolina-bg img-fluid" src="/img/gasolina_fondo.svg" alt="Fondo de Gasolina">
                            <svg class="aguja-gasolina" ref="aguja" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="50" height="50" fill="none" stroke="black" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                <line x1="12" y1="2" x2="12" y2="18" stroke="red" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></line>
                                <circle cx="12" cy="20" r="2" fill="red"></circle>
                            </svg>
                        </div>
                    </div>
                </div>

            </div>
        </div>
        <!--if del loader-->
    </div>
</template>

<script>
    import { reactive, ref, onMounted, watch } from "vue";
    import axios from "axios";
    import anime from "animejs";

    export default {
        name: "InspectionsDetails",
        setup() {
            // Estado reactivo para la inspección
            const inspection = reactive({
                id: 0,
                memoNumber: "",
                inspectionDate: "",
                checkInTime: "",
                operatorName: "",
                department: { id: 0, name: "" },
                externalWorkshopBranch: { id: 0, name: "" },
                inspectionService: { id: 0, name: "" },
                inspectionStatus: { id: 0, name: "" },
                vehicle: { id: 0, description: "", licensePlate: "" },
                towRequired: false,
                fuelLevel: 0 // Nuevo campo
            });

            // Estado de carga
            const isLoading = ref(true);
            const aguja = ref(null);

            // Función para animar la aguja
            const moverAguja = (valor) => {
                valor = Math.max(0, Math.min(100, valor));
                const nuevaRotacion = -60 + (120 * valor) / 100; // Rango de rotación entre -60 y 60 grados
                anime({
                    targets: aguja.value,
                    rotate: nuevaRotacion,
                    duration: 1000,
                    easing: "easeInOutQuad",
                });
            };

            // Función para obtener los datos
            const fetchInspection = async () => {
                try {
                    const inspectionId = window.location.pathname.split("/").pop();
                    const response = await axios.get(`/api/inspections/${inspectionId}`);

                    // Actualiza el modelo reactivo con los datos recibidos
                    Object.assign(inspection, response.data);
                } catch (error) {
                    console.error("Error cargando la inspección:", error);
                } finally {
                    // Desactiva el estado de carga cuando termine la petición
                    isLoading.value = false;
                }
            };

            // Vigilar cambios en fuelLevel y actualizar la aguja
            watch(() => inspection.fuelLevel, (newValue) => {
                if (aguja.value) {
                    moverAguja(newValue);
                }
            });

            // Llamar a la API cuando el componente se monte
            onMounted(async () => {
                await fetchInspection();
                moverAguja(inspection.fuelLevel); // Asegurar que se mueva al cargar
            });

            return { inspection, isLoading, aguja };
        }
    };
</script>
