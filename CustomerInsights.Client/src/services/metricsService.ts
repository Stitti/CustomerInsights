import api from "./api";
import type { MetricsResponse } from "@/src/models/responses/metricsResponse";
import type { TimeInterval } from "@/src/types";

export async function getTenantMetrics(interval?: TimeInterval): Promise<MetricsResponse> {
    const response = await api.get<MetricsResponse>("/metrics", {
        params: {
            interval: interval ?? null
        }
    });

    return response.data;
}

export async function getAccountMetrics(id: string, interval?: TimeInterval): Promise<MetricsResponse | null> {
    if (!id) return null;

    const response = await api.get<MetricsResponse>(`/metrics/accounts/${id}`, {
        params: {
            interval: interval ?? null
        }
    });

    return response.data;
}