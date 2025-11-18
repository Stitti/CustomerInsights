import path from "path"
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const api = process.env.VITE_CUSTOMER_VOICE_API;
if (!api) {
  throw new Error(`VITE_EVENT_MANAGER_API is missing.`);
}

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), ],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./"),
    },
  },
})
