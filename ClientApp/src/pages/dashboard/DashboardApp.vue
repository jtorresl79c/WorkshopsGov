<template>
  <template v-if="loading">
    
  </template>
  
  <template v-else>
    <div class="row g-3 justify-content-around">
      <div
          class="col-md-6 col-lg-4 col-xl-2"
          v-for="card in cards"
          :key="card.title"
      >
        <div
            class="card text-white h-100"
            :class="card.bgClass"
        >
          <div class="card-body d-flex flex-column justify-content-center align-items-center">
            <h5 class="card-title text-white">{{ card.title }}</h5>
            <p class="display-6 mb-0 text-white fw-bold">{{ cardsData[card.property] }}</p>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col">
        <div class="w-100">
          <BarChart :chartData="barChartData" :chartOptions="barChartOptions" />
        </div>
      </div>
    </div>
  </template>
</template>
<script>
import BarChart from './components/BarChart.vue'
import axios from 'axios'
export default {
  name: "DashboardApp",
  components: { BarChart },
  data() {
    return {
      loading: true,
      cards: [
        {title: "Usuarios", count: 12, bgClass: "bg-primary", property: 'applicationUsersCount'},
        {title: "Inspecciones", count: 25, bgClass: "bg-success", property: 'inspectionsCount'},
        {title: "Talleres", count: 5, bgClass: "bg-warning", property: 'externalWorkshopsCount'},
        {title: "Sucursales", count: 3, bgClass: "bg-danger", property: 'externalWorkshopBranchesCount'},
        {title: "Unidades", count: 18, bgClass: "bg-info", property: 'vehiclesCount'},
      ],
      cardsData: null,
      barChartData: {
        labels: [],
        datasets: [
          {
            label: 'Inspecciones',
            data: [],
            backgroundColor: 'rgba(54, 162, 235, 0.6)',
          },
        ],
      },
      barChartOptions: {
        responsive: true,
        plugins: {
          legend: {
            display: false,
            position: 'top',
          },
          title: {
            display: true,
            text: 'Vehículos con mas reparaciones',
          },
        },
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    }
  },
  mounted() {
    this.fetchData()
  },
  methods: {
    async fetchData() {
      const response1 = axios.get('/api/Dashboard')
      const response2 = axios.get('/api/Vehicles/top-vehicles')
      const results = await Promise.all([response1, response2])
      this.cardsData = results[0].data
      
      this.barChartData.labels = results[1].data.map(r => r.oficialia)
      this.barChartData.datasets[0].data = results[1].data.map(r => r.inspectionsCount)
      
      this.loading = false
      
      console.log(results[0].data)
      console.log(results[1].data)
    }
  },
}
</script>

<style scoped>

</style>