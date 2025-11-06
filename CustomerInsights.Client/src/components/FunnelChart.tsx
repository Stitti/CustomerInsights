import React from "react";
import Chart from "react-apexcharts";
import type { FunnelStep } from "../mock/mockAnalytics";

export default function FunnelChart({ steps }: { steps: FunnelStep[] }) {
    const series = [{ data: steps.map((s) => s.count) }];

    const options: ApexCharts.ApexOptions = {
        chart: { type: "bar", toolbar: { show: false }, background: "transparent" },
        plotOptions: { bar: { horizontal: true, barHeight: "60%", distributed: true } },
        dataLabels: {
            enabled: true,
            formatter: (value, opt) => {
                const max = Math.max(...steps.map((s) => s.count));
                const pct = max ? Math.round((Number(value) / max) * 100) : 0;
                return `${value} (${pct}%)`;
            },
            style: { colors: ["var(--gray-12)"] }
        },
        xaxis: {
            categories: steps.map((s) => s.name),
            labels: { style: { colors: "var(--gray-11)" } },
            axisBorder: { color: "var(--gray-6)" },
            axisTicks: { color: "var(--gray-6)" }
        },
        yaxis: { labels: { style: { colors: "var(--gray-12)" } } },
        grid: { borderColor: "var(--gray-6)", strokeDashArray: 3 }
    };

    return <Chart options={options} series={series} type="bar" height={320} />;
}
