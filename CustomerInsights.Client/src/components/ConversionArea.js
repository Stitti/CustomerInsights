import { jsx as _jsx } from "react/jsx-runtime";
import Chart from "react-apexcharts";
export default function ConversionArea({ data, title = "Conversion Rate" }) {
    const series = [{ name: "CVR", data: data.map(d => [new Date(d.date).getTime(), d.ratePct]) }];
    const options = {
        chart: { type: "area", toolbar: { show: false }, zoom: { enabled: false }, background: "transparent" },
        title: { text: title, style: { color: "var(--gray-12)" } },
        dataLabels: { enabled: false },
        stroke: { curve: "smooth", width: 2 },
        xaxis: { type: "datetime", labels: { style: { colors: "var(--gray-11)" } }, axisBorder: { color: "var(--gray-6)" }, axisTicks: { color: "var(--gray-6)" } },
        yaxis: { max: 100, labels: { formatter: (v) => `${v}%`, style: { colors: "var(--gray-11)" } }, title: { text: "Rate (%)", style: { color: "var(--gray-12)" } } },
        grid: { borderColor: "var(--gray-6)", strokeDashArray: 3 },
        tooltip: { y: { formatter: (v) => `${v.toFixed(1)}%` }, theme: document.documentElement.getAttribute("data-theme") || "light" }
    };
    return _jsx(Chart, { options: options, series: series, type: "area", height: 260 });
}
