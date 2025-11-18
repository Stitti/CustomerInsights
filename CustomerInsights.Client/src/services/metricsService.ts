import api from "./api";
import type {MetricsResponse} from "@/src/models/responses/metricsResponse.ts";

export async function getTenantMetrics(): Promise<MetricsResponse> {
    const response = await api.get<MetricsResponse>("/metrics");
    return response.data;
}

export async function getAccountMetrics(id: string): Promise<MetricsResponse> {
    if (!id)
        return null;

    const response = await api.get<MetricsResponse>(`/metrics/accounts/${id}`);
    return response.data;
}