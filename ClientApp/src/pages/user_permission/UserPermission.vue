<template>
  <p>Permisos de usuario vue component</p>
</template>

<script>
export default {
  name: "UserPermission",
  mounted() {
    console.log("Component mounted.")
  },
  beforeUnmount() {
    console.log("Component is going to be destroyed");
  },
}
</script>

<style scoped>
.coco{
  color: red;
}

</style>