<template>
  <HelloWorld/>
  <!--
    NOTA: Si importas imagenes en dev si se van a ver pero en produccion no (cuando hagas el pnpm run build), 
    solo es posible importar svg por el momento, se corregira despues.
   -->
  <img src="@/common/assets/vue-logo.svg" alt="" class="vue-logo"/>
  <img src="@/common/assets/gundam-colors.png" alt="" class="vue-logo"/>
  <img :src="logoSrc" alt="">
  <img :src="gundamColors" alt="">
</template>

<script>
import HelloWorld from '@/pages/playground/components/HelloWorld'
import logoSrc from '@/common/assets/vue-logo.svg'
import gundamColors from '@/common/assets/gundam-colors.png'
import axios from 'axios'
export default {
  name: "PlaygroundApp",
  components: { HelloWorld },
  data() {
    return {
      logoSrc,
      gundamColors
    }
  },
  mounted() {
    axios.get('/api/Playground/Get')
        .then((response) => {
          console.log(response.data)
        })

    axios.post('/api/Playground/Create')
        .then((response) => {
          console.log(response.data)
        })
  },
}
</script>

<style scoped lang="scss">
.vue-logo{
  max-width: 500px;
  display: block;
}
</style>