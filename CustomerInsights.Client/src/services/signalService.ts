import api from "./api";
import type {SignalResponse} from "@/src/models/responses/signalResponse.ts";

export async function getAllSignals(id: string): Promise<SignalResponse> {
    const response = await api.get<SignalResponse>('/signals');
    return response.data
}

export async function getSignalById(id: string): Promise<SignalResponse> {
    if (!id)
        return null;

    const response = await api.get<SignalResponse>(`/signals/${id}`);
    return response.data
}