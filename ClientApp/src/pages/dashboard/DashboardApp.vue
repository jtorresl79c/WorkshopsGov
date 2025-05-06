<template>
  <!--<p>{{userRole}}</p>-->
  <template v-if="loading">
    
  </template>
  
  <template v-else>

      <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center p-4">
              <div class="header-title">
                  <h4 class="card-title">Panel Administrador</h4>
              </div>
          </div>

          <div class="row g-3 justify-content-around">
              <div class="col-6 col-sm-4 col-md-3 col-lg-2"
                   v-for="card in cards"
                   :key="card.title">
                  <div class="card custom-card text-dark border shadow-sm">
                      <div class="card-body d-flex flex-column justify-content-between align-items-center text-center p-3">
                          <div>
                              <h6 class="card-title mb-2">{{ card.title }}</h6>
                          </div>
                          <div>
                              <p class="fs-4 fw-bold mb-0">{{ cardsData ? cardsData[card.property] ?? 0 : '...' }}</p>
                          </div>
                      </div>
                  </div>
              </div>
          </div>

          <div class="row">
              <div class="col">
                  <div class="w-50">
                      <BarChart :chartData="barChartData" :chartOptions="barChartOptions" />
                  </div>
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
      userRole: '',
      loading: true,
        cards: [
            { title: "Usuarios", bgClass: "bg-primary", property: 'ApplicationUsersCount' },
            { title: "Inspecciones", bgClass: "bg-success", property: 'InspectionsCount' },
            { title: "Talleres", bgClass: "bg-warning", property: 'ExternalWorkshopsCount' },
            { title: "Sucursales", bgClass: "bg-danger", property: 'ExternalWorkshopBranchesCount' },
            { title: "Unidades", bgClass: "bg-info", property: 'VehiclesCount' },
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
    this.userRole = window.userRole
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
          console.log(results[0].data)

      this.loading = false

     this.cardsData = {};
          results[0].data.forEach(item => {
              this.cardsData[item.name] = item.count;
          });
      console.log(results[0].data)
      console.log(results[1].data)
    }
  },
}
</script>

<style scoped>

</style>