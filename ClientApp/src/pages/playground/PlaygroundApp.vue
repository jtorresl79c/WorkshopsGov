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
export default {
  name: "PlaygroundApp",
  components: { HelloWorld },
  data() {
    return {
      logoSrc,
      gundamColors
    }
  },
}
</script>

<!--<style scoped lang="scss">-->
<style scoped>
.vue-logo{
  max-width: 500px;
  display: block;
}
</style>