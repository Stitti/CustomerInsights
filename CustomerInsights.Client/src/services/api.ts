import axios from "axios";

const base = import.meta.env.VITE_CUSTOMER_VOICE_API;
console.log(base);
export const api = axios.create({
    baseURL: `${base}/api/v1`,
    withCredentials: false,
});
export default api;