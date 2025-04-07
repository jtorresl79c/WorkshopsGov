<template>
    <div>
        <h2>Detalles de Inspección</h2>
        <p><strong>Memo:</strong> {{ inspection.memoNumber }}</p>
        <p><strong>Fecha:</strong> {{ inspection.inspectionDate }}</p>
        <p><strong>Inspector:</strong> {{ inspection.operatorName }}</p>
    </div>
</template>

<script>
    import axios from "axios";

    export default {
        props: ["id"],
        data() {
            return {
                inspection: {},
            };
        },
        mounted() {
            axios.get(`/api/inspections/${this.id}`)
                .then(response => {
                    this.inspection = response.data;
                })
                .catch(error => {
                    console.error("Error al cargar la inspección:", error);
                });
        }
    };
</script>
