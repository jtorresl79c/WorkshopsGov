import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { glob } from 'glob'
import { fileURLToPath, URL } from 'node:url'
import path from 'path';

// Busca automáticamente los archivos de entrada
const entries = {}
const entryFiles = glob.sync('./src/pages/**/index.js')

entryFiles.forEach(file => {
    // Normaliza la ruta para que funcione en Windows y Linux/Mac
    const normalizedPath = file.split(path.sep).join('/');
    const pageName = normalizedPath.split('/').slice(-2)[0];

    entries[pageName] = fileURLToPath(new URL(normalizedPath, import.meta.url));
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
          format: "es",
          entryFileNames: 'assets/pages/[name]/[name].[hash].js',
          chunkFileNames: 'assets/chunks/[name].[hash].js',
          assetFileNames: 'assets/pages/[name].[hash].[ext]'
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
      origin: 'http://localhost:5173',
      'Access-Control-Allow-Origin': '*'
  }
})