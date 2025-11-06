import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()],
    server: {
        host: "localhost",
        port: parseInt(process.env.PORT ?? '5174', 10),
        strictPort: true,
    },
});