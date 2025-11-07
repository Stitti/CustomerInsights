import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import React from "react";
import ReactApexChart from "react-apexcharts";
export const ChannelChart = () => {
    const [state, setState] = React.useState({
        options: {
            chart: {
                type: 'bar',
            },
            plotOptions: {
                bar: {
                    horizontal: true,
                }
            },
            dataLabels: {
                enabled: true
            },
            grid: {
                show: false
            },
            yaxis: {
                labels: {
                    show: false
                }
            },
            xaxis: {
                labels: {
                    show: false
                }
            }
        },
    });
    return (_jsxs("div", { children: [_jsx("div", { id: "chart", children: _jsx(ReactApexChart, { options: state.options, type: "bar", series: [{ data: ([400, 430, 448, 470, 540, 580, 690].sort((n1, n2) => n2 - n1)) }] }) }), _jsx("div", { id: "html-dist" })] }));
};
