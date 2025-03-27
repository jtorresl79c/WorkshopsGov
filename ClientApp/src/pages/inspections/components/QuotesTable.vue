<!-- components/QuotesTable.vue -->
<template>
    <div class="bg-white border p-4 mb-4 mt-3">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h4 class="mb-0">Cotizaciones</h4>

            <!-- Solo los talleres externos pueden generar cotización -->
            <button v-if="currentUserRole === 'External_Workshop'"
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
                            <th>
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
                            <td>
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
                                <a v-if="currentUserRole === 'External_Workshop'" :href="`/WorkshopQuotes/Edit/${quote.id}?inspectionId=${inspection.id}`" class="btn btn-sm btn-outline-success mx-1">
                                    <i class="bi bi-pencil-square"></i>
                                </a>
                                <button v-if="currentUserRole === 'External_Workshop'"
                                        @click="deleteQuote(quote.id)"
                                        class="btn btn-sm btn-outline-danger">
                                    <i class="bi bi-trash3-fill"></i>
                                </button>
                            </td>
                        </tr>
                    </tbody>
                </table>

                <!-- Botones según el rol -->
                <div v-if="currentUserRole === 'External_Workshop'" class="d-flex gap-2 mt-3">
                    <button class="btn btn-success mt-3" :disabled="selectedCheckQuotes.length === 0" @click="sendQuotesToReview">
                        Enviar a Revisión
                    </button>
                </div>
                <div v-if="currentUserRole === 'Administrator'" class="d-flex gap-2 mt-3">
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

            </div>
            <div v-else>
                <p class="text-muted text-center">No hay cotizaciones registradas para esta inspección.</p>
            </div>
        </div>

        <!-- Modal para Detalles de Cotización -->
        <div v-if="showModal">
            <div class="modal-backdrop fade show"></div>
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
    </div>
</template>

<script>
export default {
  name: 'QuotesTable',
  props: {
    quotes: Array,
    selectedCheckQuotes: Array,
    currentUserRole: String,
    selectAllQuotes: Boolean,
    showModal: Boolean,
    selectedQuote: Object,
    inspection: Object,
    isLoadingQuotes: Boolean
  },
  emits: ['toggleSelectAllQuotes', 'sendQuotesToReview', 'updateQuoteStatus', 'loadQuoteDetails', 'deleteQuote', 'closeModal', 'goToQuoteForm'],
  methods: {
    getQuoteFileUrl(quoteId) {
      return `/api/WorkshopQuotesApi/DownloadQuoteFile/${quoteId}`;
    },
    formatDate(dateString) {
      return new Date(dateString).toLocaleString("es-ES", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit"
      });
    }
  }
};
</script>
