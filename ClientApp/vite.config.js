// import { defineConfig } from 'vite'
// import vue from '@vitejs/plugin-vue'
//
// export default defineConfig({
//   root: 'ClientApp', // Indica que el frontend está en esta carpeta
//   plugins: [vue()]
// })

// import { defineConfig } from 'vite'
// import vue from '@vitejs/plugin-vue'
// import path from 'path'
//
// export default defineConfig({
//   root: 'ClientApp', // Indica que Vite debe ejecutarse dentro de esta carpeta
//   plugins: [vue()],
//   resolve: {
//     alias: {
//       '@': path.resolve(__dirname, 'ClientApp/src')
//     }
//   },
//   server: {
//     port: 5173,
//     strictPort: true
//   }
// })

// import { defineConfig } from 'vite'
// import vue from '@vitejs/plugin-vue'
// import path from 'path'
//
// export default defineConfig({
//   root: path.resolve(__dirname, 'ClientApp'), // Asegura que Vite use esta carpeta
//   plugins: [vue()],
//   resolve: {
//     alias: {
//       '@': path.resolve(__dirname, 'ClientApp/src')
//     }
//   },
//   server: {
//     port: 5173,
//     strictPort: true
//   }
// })

// import { defineConfig } from 'vite'
// import vue from '@vitejs/plugin-vue'
// import path from 'path'
//
// export default defineConfig({
//   root: path.resolve(__dirname, 'ClientApp'), // 🔹 Usa ClientApp como raíz
//   plugins: [vue()],
//   resolve: {
//     alias: {
//       '@': path.resolve(__dirname, 'ClientApp/src')
//     }
//   },
//   server: {
//     port: 5173,
//     strictPort: true
//   }
// })

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src')
    }
  },
  server: {
    port: 5173,
    strictPort: true
  }
})