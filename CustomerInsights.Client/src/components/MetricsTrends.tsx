import React, { useEffect, useState } from "react";
import { Flex } from "@radix-ui/themes";
import { MetricCard } from "./MetricCard";
import { getAccountMetrics, getTenantMetrics } from "../services/metricsService";
import type { MetricsResponse } from "@/src/models/responses/metricsResponse";
import type {TimeInterval} from "@/src/types.ts";

type Props = {
    accountId?: string;
    timeInterval?: TimeInterval;
};

export function MetricsTrends({ accountId, timeInterval = "0" }: Props) {
    const [metrics, setMetrics] = useState<MetricsResponse | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let mounted = true;
        setLoading(true);

        const fetcher = accountId
            ? getAccountMetrics(accountId, timeInterval)
            : getTenantMetrics(timeInterval);

        fetcher
            .then((data) => {
                if (mounted) setMetrics(data ?? null);
            })
            .catch((err) => {
                console.error("Error loading metrics:", err);
                if (mounted) setMetrics(null);
            })
            .finally(() => {
                if (mounted) setLoading(false);
            });

        return () => {
            mounted = false;
        };
    }, [accountId, timeInterval]); // <–– Intervall als Dependency

    const satisfaction = metrics?.satisfactionIndex;
    const satisfactionTrend = metrics?.satisfactionIndexTrend ?? null;

    const interactions = metrics?.totalInteractions;
    const interactionsTrend = metrics?.totalInteractionsTrend ?? null;

    const confidence = metrics?.modelConfidence;
    const confidenceTrend = metrics?.modelConfidenceTrend ?? null;

    const highUrgency = metrics?.totalHighUrgency;
    const highUrgencyTrend = metrics?.totalHighUrgencyTrend ?? null;

    return (
        <Flex gap="4" direction="row" wrap="wrap">
            <MetricCard
                title="Satisfaction Index"
                value={satisfaction?.toString()}
                trend={typeof satisfactionTrend === "number" ? satisfactionTrend : null}
                loading={loading}
            />
            <MetricCard
                title="Interactions"
                value={interactions?.toString()}
                trend={typeof interactionsTrend === "number" ? interactionsTrend : null}
                loading={loading}
            />
            <MetricCard
                title="Model Confidence"
                value={typeof confidence === "number" ? `${confidence}%` : confidence ?? undefined}
                trend={typeof confidenceTrend === "number" ? confidenceTrend : null}
                loading={loading}
            />
            <MetricCard
                title="High Urgency Interactions"
                value={highUrgency?.toString()}
                trend={typeof highUrgencyTrend === "number" ? highUrgencyTrend : null}
                loading={loading}
            />
        </Flex>
    );
}