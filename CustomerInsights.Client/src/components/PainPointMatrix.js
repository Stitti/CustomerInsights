import { jsx as _jsx } from "react/jsx-runtime";
import Chart from "react-apexcharts";
// Tooltip-Theme aus Radix lesen
function getAppearance() {
    return document.documentElement.getAttribute("data-theme") || "light";
}
function cssVar(name, fallback) {
    const v = getComputedStyle(document.documentElement).getPropertyValue(name).trim();
    return v || fallback;
}
export default function PainPointMatrix({ data, title = "Pain Points (Frequenz × Schwere)" }) {
    const appearance = getAppearance();
    const trendColors = {
        up: cssVar("--red-9", "#ef4444"),
        flat: cssVar("--amber-9", "#f59e0b"),
        down: cssVar("--green-9", "#16a34a")
    };
    const trends = ["up", "flat", "down"];
    const series = trends.map((t) => ({
        name: t,
        data: data.filter((p) => p.trend === t).map((p) => ({ x: p.frequencyPct, y: p.severity, z: p.volume, meta: p.aspect }))
    }));
    const options = {
        chart: { type: "bubble", toolbar: { show: false }, background: "transparent", foreColor: "var(--gray-12)" },
        theme: { mode: appearance },
        colors: trends.map((t) => trendColors[t]),
        title: { text: title, style: { color: "var(--gray-12)" } },
        dataLabels: { enabled: false },
        grid: { borderColor: "var(--gray-6)", strokeDashArray: 3 },
        legend: {
            position: "top",
            labels: { colors: "var(--gray-12)" },
            markers: { fillColors: trends.map((t) => trendColors[t]) },
            formatter: (name) => (name === "up" ? "⟰ schlechter" : name === "down" ? "⟱ besser" : "⟷ stabil")
        },
        xaxis: {
            type: "numeric", max: 100, tickAmount: 5,
            title: { text: "Frequenz im Zeitraum (%)", style: { color: "var(--gray-12)" } },
            axisBorder: { color: "var(--gray-6)" }, axisTicks: { color: "var(--gray-6)" },
            labels: { style: { colors: "var(--gray-11)" } }
        },
        yaxis: {
            min: 0, max: 100,
            title: { text: "Schwere (neg. Emotion / 100 – SI)", style: { color: "var(--gray-12)" } },
            axisBorder: { color: "var(--gray-6)" }, axisTicks: { color: "var(--gray-6)" },
            labels: { style: { colors: "var(--gray-11)" } }
        },
        tooltip: {
            theme: appearance,
            custom: ({ seriesIndex, dataPointIndex, w }) => {
                const p = w.config.series[seriesIndex].data[dataPointIndex];
                const aspect = p.meta;
                return `<div style="padding:8px">
          <div><strong>${aspect}</strong></div>
          <div>Frequenz: ${p.x.toFixed(1)}%</div>
          <div>Schwere: ${p.y.toFixed(0)} / 100</div>
          <div>Volumen: ${p.z}</div>
          <div>Trend: ${w.config.series[seriesIndex].name}</div>
        </div>`;
            }
        },
        fill: { opacity: 0.75 },
        stroke: { width: 1, colors: ["transparent"] }
    };
    // Key wechselt mit Theme → Tooltip sicher neu gerendert
    return _jsx(Chart, { options: options, series: series, type: "bubble", height: 360 }, appearance);
}
