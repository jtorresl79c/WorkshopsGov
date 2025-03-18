<template>
  <HelloWorld/>
<!--  <img src="@/common/assets/vue-logo.svg" alt="" class="vue-logo"/>-->
  <img :src="logoSrc" alt="">
</template>

<script>
import HelloWorld from '@/pages/playground/components/HelloWorld'
import logoSrc from '@/common/assets/vue-logo.svg'
export default {
  name: "PlaygroundApp",
  components: { HelloWorld },
  data() {
    return {
      logoSrc
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