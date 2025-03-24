import { createApp } from 'vue'
import UserPermissionApp from './UserPermission.vue'
import '../../common/styles/base.css'

let vueInstance = null; // Guardamos la instancia de Vue

window.mostrarModal = function (userId) {
    var modalElement = document.getElementById('exampleModal');
    var modal = new bootstrap.Modal(document.getElementById('exampleModal'));
    modal.show();

    setTimeout(() => {
        // Desmontar instancia anterior si existe
        if (vueInstance) {
            vueInstance.unmount();
        }

        // Crear una nueva instancia de Vue
        vueInstance = createApp(UserPermissionApp, { userId });
        vueInstance.mount('#user-permission-app');
    }, 100);

    // Evento cuando el modal se oculta
    modalElement.addEventListener('hidden.bs.modal', () => {
        console.log('Modal dismissed modal')
        if (vueInstance) {
            vueInstance.unmount();
            vueInstance = null;
            document.getElementById('user-permission-app').innerHTML = ''; // Limpiar
        }
    }, { once: true }); // `once: true` evita múltiples eventos innecesarios
}