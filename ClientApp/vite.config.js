import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { glob } from 'glob'
import { fileURLToPath, URL } from 'node:url'

// Busca automáticamente los archivos de entrada
const entries = {}
const entryFiles = glob.sync('./src/pages/**/index.js')

entryFiles.forEach(file => {
  const pageName = file.split('/').slice(-2)[0]
  entries[pageName] = fileURLToPath(new URL(file, import.meta.url))
})

export default defineConfig({
  plugins: [vue()],
  css: {
    preprocessorOptions: {
      scss: {}
    }
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
    extensions: ['.js', '.ts', '.vue', '.json']
  },

  // Configuración de múltiples entradas
  build: {
    manifest: true,
    rollupOptions: {
      input: entries,
      output: {
        entryFileNames: 'assets/[name]/[name].[hash].js',
        chunkFileNames: 'assets/[name]/chunks/[name].[hash].js',
        assetFileNames: 'assets/[name]/[name].[hash].[ext]'
      }
    },
    emptyOutDir: true,
    outDir: '../wwwroot/vue-apps',
    // assetsInlineLimit: 0 // Sin esta opcion cuando agrego un svg se pasa automaticamente a base64 
    // esta linea fuerza a que sea un archivo.
  },

  server: {
    port: 5173,
    strictPort: false,
    origin: 'http://localhost:5173'
  }
})