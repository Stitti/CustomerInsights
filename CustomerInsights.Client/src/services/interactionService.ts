import api from "./api";
import type {InteractionResponse} from "@/src/models/responses/interactionResponse.ts";

export async function getAllInteractions(): Promise<InteractionResponse[]> {
    const response = await api.get<InteractionResponse[]>("/interactions");
    return response.data;
}

export async function getInteractionById(id: string): Promise<InteractionResponse> {
    if (!id)
        return null;

    const response = await api.get<InteractionResponse>(`/interactions/${id}`);
    return response.data;
}

export async function deleteInteractionById(id: string): Promise<void> {
    if (!id)
        return;

    await api.delete(`/interactions/${id}`);
}