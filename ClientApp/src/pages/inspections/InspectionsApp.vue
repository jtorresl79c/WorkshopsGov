<template>
    <nav aria-label="breadcrumb" class="mb-3">
        <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item">
                <a :href="breadcrumbBaseUrl">{{ breadcrumbBaseLabel }}</a>
            </li>
            <li class="breadcrumb-item active" aria-current="page">
                Detalles de Inspección #{{ inspection.id }}
            </li>
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
                        <div v-if="!(currentUserRole === 'External_Workshop')">
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
                            <h5 class="mb-0">
                                {{ currentUserRole === 'External_Workshop' ? 'Sucursal Asignada' : 'Asignar Taller Externo' }}
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="row align-items-end">
                                <div class="col-md-8">
                                    <label class="form-label">Sucursal</label>
                                    <select v-model="selectedBranchId"
                                            class="form-select"
                                            :disabled="currentUserRole === 'External_Workshop' || inspection.externalWorkshopBranch.id !== 1">
                                        <option disabled value="">-- Selecciona una sucursal --</option>
                                        <option v-for="branch in inspection.availableBranches"
                                                :key="branch.id"
                                                :value="branch.id">
                                            {{ branch.name }} ({{ branch.workshopName }})
                                        </option>
                                    </select>
                                </div>

                                <!-- 🔹 Mostrar botón solo si NO es taller externo -->
                                <div v-if="currentUserRole !== 'External_Workshop'" class="col-md-4">
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


                <div class="bg-white border p-4 mb-4 mt-3">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h4 class="mb-0">Cotizaciones</h4>

                        <!-- Solo los talleres externos pueden generar cotización -->
                        <button v-if="currentUserRole === 'External_Workshop' && inspection.inspectionStatus.id == 2"
                                class="btn btn-outline-primary"
                                @click="goToQuoteForm">
                            Generar Cotización
                        </button>
                    </div>

                    <div v-if="isLoadingQuotes" class="text-center py-3">
                        <span class="spinner-border spinner-border-sm text-primary me-2"></span> Cargando cotizaciones...
                    </div>

                    <div v-else>
                        <div v-if="quotes.length > 0">
                            <div class="mb-2 text-primary fw-bold">
                                Selecciona las cotizaciones que deseas enviar a revisión
                            </div>
                            <table class="table table-bordered table-striped table-hover">
                                <thead class="table-dark">
                                    <tr>
                                        <th v-if="!(currentUserRole === 'External_Workshop' && inspection.inspectionStatus.id != 2)">
                                            <input type="checkbox" @change="toggleSelectAllQuotes" v-model="selectAllQuotes">
                                        </th>
                                        <th>#</th>
                                        <th>Número</th>
                                        <th>Fecha</th>
                                        <th>Costo Total</th>
                                        <th>Estado</th>
                                        <th>Acciones</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="quote in quotes" :key="quote.id">
                                        <td v-if="!(currentUserRole === 'External_Workshop' && inspection.inspectionStatus.id !=2 )">
                                            <input type="checkbox" v-model="selectedCheckQuotes" :value="quote.id" />
                                        </td>
                                        <td>
                                            <span v-if="quote.hasFile">
                                                <a :href="getQuoteFileUrl(quote.id)" target="_blank">
                                                    {{ quote.id }}
                                                </a>
                                            </span>
                                            <span v-else>
                                                {{ quote.id }}
                                            </span>
                                        </td>
                                        <td>{{ quote.quoteNumber }}</td>
                                        <td>{{ formatDate(quote.quoteDate) }}</td>
                                        <td>$ {{ quote.totalCost.toFixed(2) }}</td>
                                        <td>{{ quote.quoteStatus }}</td>
                                        <td>
                                            <button @click="loadQuoteDetails(quote.id)" class="btn btn-sm btn-outline-secondary">
                                                <i class="bi bi-eye-fill"></i>
                                            </button>
                                            <a v-if="currentUserRole === 'External_Workshop' && inspection.inspectionStatus.id == 2"
                                               :href="`/WorkshopQuotes/Edit/${quote.id}?inspectionId=${inspection.id}`"
                                               class="btn btn-sm btn-outline-success mx-1">
                                                <i class="bi bi-pencil-square"></i>
                                            </a>
                                            <button v-if="currentUserRole === 'External_Workshop' && inspection.inspectionStatus.id == 2"
                                                    @click="deleteQuote(quote.id)"
                                                    class="btn btn-sm btn-outline-danger">
                                                <i class="bi bi-trash3-fill"></i>
                                            </button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <div v-if="currentUserRole === 'External_Workshop' && inspection.inspectionStatus.id == 2" class="d-flex gap-2 mt-3">
                                <button class="btn btn-success mt-3" :disabled="selectedCheckQuotes.length === 0" @click="sendQuotesToReview">
                                    Enviar a Revisión
                                </button>
                            </div>


                            <div v-if="currentUserRole === 'Administrator' && quotes.some(q => q.quoteStatus === 'En revision')" class="d-flex gap-2 mt-3">
                                <button class="btn btn-danger"
                                        :disabled="selectedCheckQuotes.length === 0"
                                        @click="updateQuoteStatus(3)">
                                    Rechazar Cotización
                                </button>
                                <button class="btn btn-success"
                                        :disabled="selectedCheckQuotes.length === 0"
                                        @click="updateQuoteStatus(4)">
                                    Aprobar Cotización
                                </button>
                            </div>

                            <div v-if="currentUserRole === 'Administrator' && inspection.inspectionStatus.id==4  && !quotes.some(q=> q.quoteStatus === 'En Revision')" class="d-flex gap-2 mt-3">
                                <button class="btn btn-warning"
                                        @click="updateInspectionStatus(2, '¿Estás seguro de regresar la inspección al taller externo?', 'Inspección regresada a Taller Externo.')">
                                    Regresar a Taller Externo
                                </button>
                                <button class="btn btn-primary"
                                        @click="updateInspectionStatus(5, '¿Estás seguro de aprobar la inspección a reparación?', 'Inspección aprobada y enviada a reparación.')">
                                    Aprobar a Reparación
                                </button>
                            </div>

                        </div>
                        <div v-else>
                            <p class="text-muted text-center">No hay cotizaciones registradas para esta inspección.</p>
                        </div>
                    </div>
                </div>
                <!-- Modal -->
                <div v-if="showModal">
                    <!-- Backdrop -->
                    <div class="modal-backdrop fade show"></div>
                    <!-- Modal content -->
                    <div class="modal fade show d-block" tabindex="-1" @click.self="closeModal">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Detalles de Cotización</h5>
                                    <button type="button" class="btn-close" @click="closeModal"></button>
                                </div>
                                <div class="modal-body" v-if="selectedQuote">
                                    <p><strong>Número:</strong> {{ selectedQuote.quoteNumber }}</p>
                                    <p><strong>Fecha:</strong> {{ formatDate(selectedQuote.quoteDate) }}</p>
                                    <p><strong>Estado:</strong> {{ selectedQuote.quoteStatus }}</p>
                                    <p><strong>Taller:</strong> {{ selectedQuote.workshopName }}</p>
                                    <p><strong>Sucursal:</strong> {{ selectedQuote.workshopBranch }}</p>
                                    <p><strong>Costo Total:</strong> ${{ selectedQuote.totalCost.toFixed(2) }}</p>
                                    <p><strong>Fecha Estimada:</strong> {{ formatDate(selectedQuote.estimatedCompletionDate) }}</p>
                                    <p><strong>Detalles:</strong> {{ selectedQuote.quoteDetails }}</p>
                                    <hr />
                                    <div>
                                        <strong>Documento:</strong>
                                        <span v-if="selectedQuote.hasFile">
                                            <a :href="getQuoteFileUrl(selectedQuote.id)" target="_blank" class="btn btn-sm btn-outline-primary ms-2">
                                                <i class="bi bi-file-earmark-pdf"></i> Ver archivo
                                            </a>
                                        </span>
                                        <span v-else class="text-muted ms-2">
                                            Sin documento adjunto
                                        </span>
                                    </div>

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
                            <div v-if="currentUserRole === 'Administrator'" class="d-flex gap-2 mt-3">
                                <button @click="deleteFile(inspection.fileMemoDigitalizado.id, inspection.fileMemoDigitalizado.fileTypeId,'fileMemoDigitalizado')"
                                        class="btn btn-danger mt-2">
                                    Eliminar Memo
                                </button>
                            </div>
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
                            <div v-if="currentUserRole === 'Administrator'" class="d-flex gap-2 mt-3">
                                <button @click="deleteFile(inspection.fileDigitalizado.id, inspection.fileDigitalizado.fileTypeId, 'fileDigitalizado')"
                                        class="btn btn-danger mt-2">
                                    Eliminar Archivo
                                </button>
                            </div>
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


            //titulo
            const breadcrumbBaseUrl = ref("/inspections");
            const breadcrumbBaseLabel = ref("Inspecciones");

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
            const currentUserRole = ref('');

            //cotizaciones
            const quotes = ref([]);
            const isLoadingQuotes = ref(false);
            //seleccion
            const selectedCheckQuotes = ref([]);
            const selectAllQuotes = ref(false);

            //modal
            const selectedQuote = ref(null);
            const showModal = ref(false);

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

                    currentUserRole.value = response.data.currentUserRole;

                    if (inspection.externalWorkshopBranch?.id) {
                        selectedBranchId.value = inspection.externalWorkshopBranch.id;
                    }

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
            const deleteFile = async (fileId, fileTypeId, propName) => {
                if (!confirm("¿Estás seguro de que deseas eliminar este archivo?")) {
                    return;
                }

                try {
                    const response = await axios.delete(`/Inspections/DeleteFile/${fileId}/${fileTypeId}`);

                    if (response.status === 200) {
                        alert("Archivo eliminado exitosamente.");
                        //inspection.fileDigitalizado = null; // Remover visualmente de la vista
                        inspection[propName] = null;
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

            const fetchQuotes = async () => {
                isLoadingQuotes.value = true;
                try {
                    const { data } = await axios.get(`/api/WorkshopQuotesApi/by-inspection/${inspection.id}`);
                    quotes.value = data;
                } catch (err) {
                    console.error("Error cargando cotizaciones:", err);
                } finally {
                    isLoadingQuotes.value = false;
                }
            };
            const goToQuoteForm = () => {
                window.location.href = `/WorkshopQuotes/Create?inspectionId=${inspection.id}`;
            };
            const deleteQuote = async (quoteId) => {
                if (!confirm("¿Estás seguro de que deseas eliminar esta cotización?")) {
                    return;
                }

                try {
                    const response = await axios.delete(`/api/WorkshopQuotesApi/${quoteId}`);

                    if (response.status === 200) {
                        alert("Cotización eliminada correctamente.");

                        // Quitar la cotización del array sin recargar
                        quotes.value = quotes.value.filter(q => q.id !== quoteId);
                    } else {
                        alert("No se pudo eliminar la cotización.");
                    }
                } catch (error) {
                    console.error("Error eliminando cotización:", error);
                    alert("Ocurrió un error al intentar eliminar la cotización.");
                }
            };
            const loadQuoteDetails = async (id) => {
                try {
                    const response = await axios.get(`/api/WorkshopQuotesApi/${id}`);
                    selectedQuote.value = response.data;
                    showModal.value = true;
                } catch (error) {
                    console.error("Error al cargar cotización", error);
                    alert("No se pudo cargar la cotización.");
                }
            };
            const getQuoteFileUrl = (quoteId) => {
                return `/api/WorkshopQuotesApi/DownloadQuoteFile/${quoteId}`;
            };
            const closeModal = () => {
                showModal.value = false;
                selectedQuote.value = null;
            }
            const toggleSelectAllQuotes = () => {
                if (selectAllQuotes.value) {
                    selectedCheckQuotes.value = quotes.value.map(q => q.id);
                } else {
                    selectedCheckQuotes.value = [];
                }
            };
            const sendQuotesToReview = async () => {
                if (selectedCheckQuotes.value.length === 0) {
                    alert("Selecciona al menos una cotización.");
                    return;
                }

                if (!confirm("¿Deseas enviar las cotizaciones seleccionadas a revisión?")) return;

                try {
                    const response = await axios.post(`/api/WorkshopQuotesApi/send-to-review`, {
                        quoteIds: selectedCheckQuotes.value,
                        inspectionId: inspection.id
                    });

                    alert("Cotizaciones enviadas a revisión correctamente.");
                    await fetchQuotes();
                    await fetchInspection();
                    selectedCheckQuotes.value = [];
                    selectAllQuotes.value = false;
                } catch (err) {
                    console.error("Error al enviar a revisión:", err);
                    alert("Error al procesar la solicitud.");
                }
            };
            const updateQuoteStatus = async (status) => {
                if (selectedCheckQuotes.value.length === 0) {
                    alert("Selecciona al menos una cotización.");
                    return;
                }

                const confirmMessage = status === 3
                    ? "¿Deseas rechazar las cotizaciones seleccionadas?"
                    : "¿Deseas aprobar las cotizaciones seleccionadas?";

                if (!confirm(confirmMessage)) return;

                try {
                    await axios.post(`/api/WorkshopQuotesApi/update-status`, {
                        quoteIds: selectedCheckQuotes.value,
                        newStatus: status,
                        inspectionId: inspection.id
                    });

                    alert(status === 3 ? "Cotizaciones rechazadas." : "Cotizaciones aprobadas.");

                    // Refrescar todo
                    await fetchQuotes();
                    await fetchInspection();
                    selectedCheckQuotes.value = [];
                    selectAllQuotes.value = false;
                } catch (err) {
                    console.error("Error al actualizar estatus:", err);
                    alert("Error al procesar la solicitud.");
                }
            };


            const updateInspectionStatus = async (statusId, confirmationMessage, successMessage) => {
                if (!confirm(confirmationMessage)) return;

                try {
                    const response = await axios.post(`/api/WorkshopInspections/update-status`, {
                        inspectionId: inspection.id,
                        statusId
                    });

                    alert(successMessage);
                    await fetchInspection(); // Refrescar inspección
                } catch (error) {
                    console.error("Error al actualizar el estado de la inspección:", error);
                    alert("Error al procesar la solicitud.");
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
                await fetchQuotes();
                moverAguja(inspection.fuelLevel);
                if (inspection.currentUserRole === "External_Workshop") {
                    breadcrumbBaseUrl.value = "/ReviewCenter";
                    breadcrumbBaseLabel.value = "Centro de Revisión";
                }
            });

            return {
                inspection, isLoading, pdfUrl,
                DownloadFileOrGenerateFile, aguja, downloadForm,
                handleFileUpload, UploadFile, isUploading, selectedFile, isGenerating,
                formatDate, getFileUrl, deleteFile, selectedBranchId, assignBranch, memoDownloadForm,
                isGeneratingMemo, selectedMemoFile, isUploadingMemo, DownloadMemo, UploadMemo, handleMemoUpload,
                currentUserRole, quotes, isLoadingQuotes, goToQuoteForm, breadcrumbBaseUrl, breadcrumbBaseLabel,
                deleteQuote, loadQuoteDetails, showModal, selectedQuote, closeModal, getQuoteFileUrl, selectedCheckQuotes,
                selectAllQuotes, toggleSelectAllQuotes, sendQuotesToReview, updateQuoteStatus, updateInspectionStatus
            };
        }
    };
</script>

