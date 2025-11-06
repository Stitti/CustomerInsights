import React from "react";
import ReactApexChart from "react-apexcharts";
import type {ApexOptions} from "apexcharts";

export const SentimentChart = () => {
    const [state, setState] = React.useState({

        series: [
            {
                data: [-33, -13, -45, 95, 12, 15, -34, -61, 66, 82, 1, -36]
            }
        ],

        options: {
            chart: {
                height: 350,
                type: 'line',
                zoom: {
                    enabled: false,
                },

            },
            grid: {
                show: false,
            },
            dataLabels: {
                enabled: false,
            },
            title: {
                text: 'Negative color for values less than 0',
                align: 'left',
            },
            xaxis: {
                categories: [
                    'Jan',
                    'Feb',
                    'Mar',
                    'Apr',
                    'May',
                    'Jun',
                    'Jul',
                    'Aug',
                    'Sep',
                    'Oct',
                    'Nov',
                    'Dec',
                ],
            },
            stroke: {
                width: 5,
                curve: "smooth"
            },
            plotOptions: {
                line: {
                    colors: {
                        threshold: 0,
                        colorAboveThreshold: '#0088ee',
                        colorBelowThreshold: '#ff0000',
                    },
                },
            }
        } as ApexOptions,


    });



    return (
        <div>
            <div id="chart">
                <ReactApexChart options={state.options}  series={state.series} type="line" height={350} />
            </div>
            <div id="html-dist"></div>
        </div>
    );
}