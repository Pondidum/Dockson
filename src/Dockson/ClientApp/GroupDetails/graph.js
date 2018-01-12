import React from "react";
import { Line as Chart } from "react-chartjs-2";

const Graph = ({ labels, datasets, axes }) => (
  <Chart
    data={{
      labels: labels,
      datasets: datasets
    }}
    options={{
      maintainAspectRatio: false,
      scales: {
        yAxes: axes
      }
    }}
  />
);

export default Graph;
