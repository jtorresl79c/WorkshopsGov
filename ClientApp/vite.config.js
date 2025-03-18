// import { defineConfig } from 'vite'
// import vue from '@vitejs/plugin-vue'
// import path from 'path'
//
// export default defineConfig({
//   plugins: [vue()],
//   resolve: {
//     alias: {
//       '@': path.resolve(__dirname, 'src')
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
import { glob } from 'glob'

// Busca automáticamente los archivos de entrada
const entries = {}
const entryFiles = glob.sync('./src/pages/**/*/App.{js,vue}')

entryFiles.forEach(file => {
  // Extrae el nombre de la página del path
  const pageName = file.split('/').slice(-2)[0]
  entries[pageName] = path.resolve(__dirname, file)
})

export default defineConfig({
  plugins: [vue()],

  // Configuración de múltiples entradas
  build: {
    rollupOptions: {
      input: entries,
      output: {
        entryFileNames: 'assets/[name]/[name].[hash].js',
        chunkFileNames: 'assets/[name]/chunks/[name].[hash].js',
        assetFileNames: 'assets/[name]/[name].[hash].[ext]'
      }
    },
    emptyOutDir: true,
    outDir: '../wwwroot/vue-apps'
  },

  server: {
    port: 5173,
    strictPort: true
  }
})