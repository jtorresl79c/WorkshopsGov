<template>
    <nav aria-label="breadcrumb" class="mb-3">
        <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item">
                <a :href="breadcrumbBaseUrl">{{ breadcrumbBaseLabel }}</a>
            </li>
            <li class="breadcrumb-item active" aria-current="page">
                Detalles de Solicitud #{{ solicitud?.id || "..." }}
            </li>
        </ol>
    </nav>


    <div v-if="isLoading" class="text-center mt-5">
        <h2>Cargando solicitud de servicio...</h2>
    </div>

    <div v-else-if="solicitud">



        <div class="row">
            <div class="col-md-8">
                <div class="bg-white">
                    <div class="w-100 d-flex justify-content-between align-items-center border p-4 mb-4">
                        <div class="d-flex flex-column">
                            <h1 class="m-0">Detalles de Solicitud de Servicio - #{{ solicitud.id }} </h1>
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
                        </div>
                        <div class="row mb-lg-2">
                            <div class="col-12 col-lg-3 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">Vehiculo (Oficialia)</p>
                                <p class="m-0">{{ solicitud.vehicle.oficialia }}</p>
                            </div>
                            <div class="col-12 col-lg-3 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">No.Economico</p>
                                <p class="m-0">{{ solicitud.vehicle.LicensePlate }} - </p>
                            </div>
                            <div class="col-12 col-lg-2 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">Fecha</p>
                                <p class="m-0">{{ formatDate(solicitud.receptionDate) }}</p>
                            </div>
                            <div class="col-12 col-lg-4 mb-2 mb-lg-0">
                                <p class="fw-bold m-0">Departamento Solicitante</p>
                                <p class="m-0">{{ solicitud.department }}</p>
                            </div>
                        </div>

                        <div class="col-12 col-lg-12 mb-2 mb-lg-0">
                            <p class="fw-bold m-0">Descripción</p>
                            {{ solicitud.description }}
                        </div>
                    </div>
                </div>


                <div class="bg-white mb-4">
                    <div class="border p-4">
                        <h5 class="mb-3">Acciones</h5>
                        <button class="btn btn-outline-primary"
                                @click="irACrearDiagnostico(solicitud.id)">
                            Generar Diagnóstico
                        </button>
                    </div>
                </div>

                <!--cierra col md 8-->
            </div>
            <div class="col-md-4">
                <div class="card text-center bg-dark bg-lighten-1">
                    <div class="card-content text-white">
                        <div class="card-body">
                            <div class="list-group">
                                <h4 class="text-white">Pendiente de Diagnostico</h4>
                            </div>
                        </div>
                    </div>
                </div>



                <div class="bg-white border p-4 mb-4">
                    <div class="d-flex flex-column align-items-end">
                        <i class="bi bi-file-earmark-pdf-fill text-primary" style="font-size: 5em"></i>

                        <template v-if="solicitud.fileDigitalizado">
                            <p class="mt-1 text-muted">Archivo subido el {{ formatDate(solicitud.fileDigitalizado.uploadedAt) }}</p>
                            <a :href="getFileUrl(solicitud.id)"
                               target="_blank"
                               class="btn btn-primary">
                                Ver Solicitud Digitalizada
                            </a>
                            <div class="d-flex gap-2 mt-3">
                                <button @click="deleteFile(solicitud.fileDigitalizado.id)"
                                        class="btn btn-danger mt-2">
                                    Eliminar Archivo
                                </button>
                            </div>
                        </template>

                        <template v-else>
                            <p class="fs-4">Subir Solicitud Digitalizada</p>
                            <form @submit.prevent="uploadFile" enctype="multipart/form-data" class="d-flex flex-column align-items-end">
                                <input type="file" @change="handleFileUpload" required class="form-control mb-2">
                                <button type="submit" class="btn btn-outline-success">
                                    Subir Archivo
                                </button>
                            </form>
                        </template>
                    </div>
                </div>

            </div>
            <!--cierra el col md 4-->
        </div>
        </div>

            <div v-else class="text-danger text-center mt-4">
                No se encontró la solicitud.
            </div>
</template>

<script>
    import axios from "axios";

    export default {
        name: "RequestServiceDetail",
        data() {
            return {
                solicitud: null,
                isLoading: true,
                breadcrumbBaseUrl: "/requestservices/PorAtenderTaller",
                breadcrumbBaseLabel: "Solicitudes de Servicio",
                selectedFile: null
            };
        },
        methods: {
            irACrearDiagnostico(solicitudId) {
                window.location.href = `/Inspections/Create?requestServiceId=${solicitudId}`;
            },
            getFileUrl(requestId) {
                return `/api/RequestServicesApi/DownloadSolicitudFile/${requestId}`;
            },
            handleFileUpload(event) {
                this.selectedFile = event.target.files[0];
            },
            async uploadFile() {
                if (!this.selectedFile) {
                    alert("Selecciona un archivo antes de subir.");
                    return;
                }

                const formData = new FormData();
                formData.append("file", this.selectedFile);
                formData.append("id", this.solicitud.id);
                formData.append("fileTypeId", 9); // <- Solicitud digitalizada

                try {
                    await axios.post("/RequestServices/UploadFile", formData, {
                        headers: {
                            "Content-Type": "multipart/form-data"
                        }
                    });

                    alert("Archivo subido exitosamente.");
                    this.selectedFile = null;
                    await this.fetchData(); // Refrescar la vista
                } catch (error) {
                    console.error("Error subiendo archivo:", error);
                    alert("Error al subir el archivo.");
                }
            },
       
            async deleteFile(fileId) {
                if (!confirm("¿Deseas eliminar este archivo?")) return;

                try {
                    await axios.delete(`/api/RequestServicesApi/${fileId}`);
                    alert("Archivo eliminado correctamente.");
                    await this.fetchData();
                } catch (err) {
                    console.error("Error al eliminar el archivo:", err);
                    alert("No se pudo eliminar el archivo.");
                }
            },
            async fetchData() {
                const id = window.location.pathname.split("/").pop();
                try {
                    const response = await axios.get(`/api/RequestServicesApi/${id}`);
                    this.solicitud = response.data;
                } catch (error) {
                    console.error("Error al cargar la solicitud:", error);
                } finally {
                    this.isLoading = false;
                }
            },
            formatDate(dateStr) {
                return new Date(dateStr).toLocaleDateString("es-MX", {
                    day: "2-digit",
                    month: "2-digit",
                    year: "numeric"
                });
            }
        },
        mounted() {
            this.fetchData();
        }
    };
</script>

