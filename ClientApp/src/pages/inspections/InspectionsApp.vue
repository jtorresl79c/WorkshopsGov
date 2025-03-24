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
            <div class="col-md-8">
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
                <div class="bg-white mb-4">
                    <div class="border p-4">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h5 class="mb-0">Asignar Taller Externo</h5>
                        </div>
                        <div class="card-body">
                            <div class="row align-items-end">
                                <div class="col-md-8">
                                    <label class="form-label">Seleccionar Sucursal</label>
                                    <select v-model="selectedBranchId" class="form-select" :disabled="inspection.externalWorkshopBranch.id !== 1">
                                        <option disabled value="">-- Selecciona una sucursal --</option>
                                        <option v-for="branch in inspection.availableBranches"
                                                :key="branch.id"
                                                :value="branch.id">
                                            {{ branch.name }} ({{ branch.workshopName }})
                                        </option>
                                    </select>
                                </div>
                                <div class="col-md-4">
                                    <button class="btn btn-success w-100"
                                            @click="assignBranch"
                                            :disabled="!selectedBranchId || inspection.externalWorkshopBranch.id !== 1">
                                        Asignar Taller
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!--termina el col-->
            </div>



            <div class="col-md-4">
                <div class="card text-center bg-secondary bg-lighten-1">
                    <div class="card-content text-white">
                        <div class="card-body">
                            <h4 class="alert-heading d-flex justify-content-between">Estado</h4>
                            <div class="list-group">
                                <h4 class="text-white">{{inspection.inspectionStatus.name}}</h4>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="bg-white border p-4 mb-4">
                    <div class="d-flex flex-column align-items-end">
                        <i class="bi bi-file-earmark-pdf-fill text-primary" style="font-size: 5em"></i>
                        <template v-if="inspection.fileMemoDigitalizado">
                            <p class="mt-1 text-muted">Archivo subido el {{ formatDate(inspection.fileMemoDigitalizado.uploadedAt) }}</p>
                            <a :href="getFileUrl(inspection.id, inspection.fileMemoDigitalizado.fileTypeId)"
                               target="_blank"
                               class="btn btn-primary">
                                Ver Memo
                            </a>
                            <button @click="deleteFile(inspection.fileMemoDigitalizado.id, inspection.fileMemoDigitalizado.fileTypeId)"
                                    class="btn btn-danger mt-2">
                                Eliminar Memo
                            </button>
                        </template>
                        <template v-else>
                            <form ref="memoDownloadForm"
                                  :action="`/Inspections/DownloadFileOrGenerateFile/${inspection.id}?fileTypeId=6`"
                                  method="POST"
                                  target="_blank">
                            </form>
                            <a href="#" @click.prevent="DownloadMemo" class="text-primary mt-2" style="cursor: pointer;">
                                {{ isGeneratingMemo ? "Generando..." : "Descargar Memo" }}
                            </a>
                            <p class="fs-4">Memo de Diagnóstico</p>
                            <form @submit.prevent="UploadMemo" enctype="multipart/form-data" class="d-flex flex-column align-items-end">
                                <input type="file" @change="handleMemoUpload" required class="form-control mb-2">
                                <button type="submit" class="btn btn-outline-success">
                                    {{ isUploadingMemo ? "Subiendo..." : "Subir Memo" }}
                                </button>
                            </form>
                        </template>
                    </div>
                </div>


                <div class="bg-white border p-4 mb-4">
                    <div class="d-flex flex-column align-items-end">
                        <i class="bi bi-file-earmark-pdf-fill text-danger" style="font-size: 5em"></i>
                        <template v-if="inspection.fileDigitalizado">
                            <p class="mt-1 text-muted">Archivo subido el Archivo subido el {{ formatDate(inspection.fileDigitalizado.uploadedAt) }}</p>
                            <a :href="getFileUrl(inspection.id, inspection.fileDigitalizado.fileTypeId)"
                               target="_blank"
                               class="btn btn-primary">
                                Ver Archivo
                            </a>
                            <button @click="deleteFile(inspection.fileDigitalizado.id, inspection.fileDigitalizado.fileTypeId)"
                                    class="btn btn-danger mt-2">
                                Eliminar Archivo
                            </button>
                        </template>
                        <template v-else>
                            <form ref="downloadForm"
                                  :action="`/Inspections/DownloadFileOrGenerateFile/${inspection.id}?fileTypeId=4`"
                                  method="POST"
                                  target="_blank">
                            </form>
                            <a href="#" @click.prevent="DownloadFileOrGenerateFile" class="text-primary mt-2" style="cursor: pointer;">
                                {{ isGenerating ? "Generando..." : "Descargar Formato" }}
                            </a>
                            <p class="fs-4">Entrega y Recepci&oacute;n</p>
                            <form @submit.prevent="UploadFile" enctype="multipart/form-data" class="d-flex flex-column align-items-end">
                                <input type="file" @change="handleFileUpload" required class="form-control mb-2">
                                <button type="submit" class="btn btn-outline-success">
                                    {{ isUploading ? "Subiendo..." : "Subir Archivo" }}
                                </button>
                            </form>
                        </template>
                    </div>
                </div>

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
                fuelLevel: 0,
                fileDigitalizado: null
            });

            // Estados de carga y PDF
            const isLoading = ref(true);
            const pdfUrl = ref(null);
            const aguja = ref(null);
            const isGenerating = ref(false);
            const downloadForm = ref(null);
            const selectedFile = ref(null);
            const isUploading = ref(false);
            const selectedBranchId = ref("");

            const memoDownloadForm = ref(null)
            const isGeneratingMemo = ref(false)
            const selectedMemoFile = ref(null)
            const isUploadingMemo = ref(false)

            const assignBranch = async () => {
                if (!selectedBranchId.value) return alert("Selecciona una sucursal primero");

                try {
                    const response = await axios.post(`/api/inspections/${inspection.id}/assign-workshop`, {
                        branchId: selectedBranchId.value,
                    });

                    alert("Sucursal asignada correctamente.");

                    await fetchInspection(); // refrescar los datos
                } catch (error) {
                    console.error("Error al asignar la sucursal:", error);
                    alert("No se pudo asignar la sucursal.");
                }
            };
            // 🔹 Función para formatear fechas
            const formatDate = (dateString) => {
                if (!dateString) return "Fecha no disponible"; // Evitar errores con fechas nulas
                return new Date(dateString).toLocaleString("es-ES", {
                    day: "2-digit",
                    month: "2-digit",
                    year: "numeric",
                    hour: "2-digit",
                    minute: "2-digit"
                });
            };
            // 🔹 Función para animar la aguja de gasolina
            const moverAguja = (valor) => {
                valor = Math.max(0, Math.min(100, valor));
                const nuevaRotacion = -60 + (120 * valor) / 100;
                anime({
                    targets: aguja.value,
                    rotate: nuevaRotacion,
                    duration: 1000,
                    easing: "easeInOutQuad",
                });
            };
            // 🔹 Función para obtener los datos de inspección
            const fetchInspection = async () => {
                try {
                    const inspectionId = window.location.pathname.split("/").pop();
                    const response = await axios.get(`/api/inspections/${inspectionId}`);

                    // Actualiza el modelo reactivo con los datos recibidos
                    Object.assign(inspection, response.data);

                    pdfUrl.value = `/Formats/Inspeccion_${inspectionId}.pdf`;
                } catch (error) {
                    console.error("Error cargando la inspección:", error);
                } finally {
                    isLoading.value = false;
                }
            };
            const getFileByType = (fileTypeId) => {
                return inspection.files.find(file => file.FileTypeId === fileTypeId) || null;
            };
            const getFileUrl = (inspectionId, fileTypeId) => {
                return `/Inspections/DownloadFile/${inspectionId}?fileTypeId=${fileTypeId}`;
            };
            // 🔹 Función para generar el PDF y abrirlo en una nueva pestaña
            const DownloadFileOrGenerateFile = () => {
                if (isGenerating.value) return;
                isGenerating.value = true;

                setTimeout(() => {
                    if (downloadForm.value) {
                        downloadForm.value.submit();
                    } else {
                        console.error("No se encontro el formulario para descargar el archivo.");
                    }
                    isGenerating.value = false;
                }, 100);
            };
            // 🔹 Función para manejar la selección del archivo
            const handleFileUpload = (event) => {
                selectedFile.value = event.target.files[0];
            };
            // 🔹 Función para subir el archivo digitalizado
            const UploadFile = async () => {
                if (!selectedFile.value) {
                    alert("Por favor, selecciona un archivo antes de subirlo.");
                    return;
                }

                isUploading.value = true;
                const formData = new FormData();
                formData.append("file", selectedFile.value);
                formData.append("id", inspection.id);
                formData.append("fileTypeId", 5); 

                try {
                    await axios.post("/Inspections/UploadFile", formData, {
                        headers: {
                            "Content-Type": "multipart/form-data",
                        },
                    });

                    alert("Archivo subido exitosamente.");
                    selectedFile.value = null; // Resetear el input
                    await fetchInspection(); // Recargar datos para obtener el archivo actualizado
                } catch (error) {
                    console.error("Error subiendo el archivo:", error);
                    alert("Error al subir el archivo.");
                } finally {
                    isUploading.value = false;
                }
            };
            const deleteFile = async (fileId, fileTypeId) => {
                if (!confirm("¿Estás seguro de que deseas eliminar este archivo?")) {
                    return;
                }

                try {
                    const response = await axios.delete(`/Inspections/DeleteFile/${fileId}/${fileTypeId}`);

                    if (response.status === 200) {
                        alert("Archivo eliminado exitosamente.");
                        inspection.fileDigitalizado = null; // Remover visualmente de la vista
                    } else {
                        alert("No se pudo eliminar el archivo.");
                    }
                } catch (error) {
                    console.error("Error eliminando el archivo:", error);
                    alert("Ocurrió un error al intentar eliminar el archivo.");
                }
            };
            // Observa cambios en fuelLevel para animar la aguja

            const DownloadMemo = () => {
                if (isGeneratingMemo.value) return;
                isGeneratingMemo.value = true;

                setTimeout(() => {
                    if (memoDownloadForm.value) {
                        memoDownloadForm.value.submit();
                    } else {
                        console.error("Formulario de memo no encontrado.");
                    }
                    isGeneratingMemo.value = false;
                }, 100);
            };

            const handleMemoUpload = (event) => {
                selectedMemoFile.value = event.target.files[0];
            };

            const UploadMemo = async () => {
                if (!selectedMemoFile.value) {
                    alert("Selecciona un archivo primero.");
                    return;
                }

                isUploadingMemo.value = true;

                const formData = new FormData();
                formData.append("file", selectedMemoFile.value);
                formData.append("id", inspection.id);
                formData.append("fileTypeId", 7); 

                try {
                    await axios.post("/Inspections/UploadFile", formData, {
                        headers: { "Content-Type": "multipart/form-data" }
                    });

                    alert("Memo subido correctamente.");
                    selectedMemoFile.value = null;
                    await fetchInspection(); // Recargar los datos
                } catch (err) {
                    console.error("Error al subir memo:", err);
                    alert("Hubo un problema al subir el memo.");
                } finally {
                    isUploadingMemo.value = false;
                }
            };

            watch(() => inspection.fuelLevel, (newValue) => {
                if (aguja.value) {
                    moverAguja(newValue);
                }
            });
            // Llamar a la API cuando el componente se monte
            onMounted(async () => {
                await fetchInspection();
                moverAguja(inspection.fuelLevel);
            });

            return {
                inspection, isLoading, pdfUrl,
                DownloadFileOrGenerateFile, aguja, downloadForm,
                handleFileUpload, UploadFile, isUploading, selectedFile, isGenerating,
                formatDate, getFileUrl, deleteFile, selectedBranchId, assignBranch, memoDownloadForm, isGeneratingMemo, selectedMemoFile, isUploadingMemo, DownloadMemo, UploadMemo, handleMemoUpload
            };
        }
    };
</script>

