import React from "react";
import ReactApexChart from "react-apexcharts";
import type {ApexOptions} from "apexcharts";

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
        } as ApexOptions,


    });



    return (
        <div>
            <div id="chart">
                <ReactApexChart options={state.options} type="bar" series={[{data: ([400, 430, 448, 470, 540, 580, 690].sort((n1,n2) => n2 - n1))}]}/>
            </div>
            <div id="html-dist"></div>
        </div>
    );
}