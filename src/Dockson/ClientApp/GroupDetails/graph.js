import React from "react";
import { Line as Chart } from "react-chartjs-2";

const schemes = {
  median: {
    label: "Median",
    color: "rgba(255,99,132,1)",
    axis: "left"
  },
  deviation: {
    label: "Standard Deviation",
    color: "rgba(54, 162, 235, 1)",
    axis: "right"
  },
  rate: {
    label: "Rate",
    color: "rgba(255,99,132,1)",
    axis: "left"
  }
};

const axis = key => {
  const scheme = schemes[key];

  return {
    id: key,
    type: "linear",
    position: scheme.axis
  };
};

const dataset = (group, key) => {
  const keys = Object.keys(group);
  const scheme = schemes[key];

  return {
    label: scheme.label,
    yAxisID: key,
    data: keys.map(day => group[day][key]),
    fill: false,
    borderColor: scheme.color
  };
};

const Graph = ({ dataSource, keys }) => {
  const labels = Object.keys(dataSource);
  const datasets = keys.map(key => dataset(dataSource, key));
  const axes = keys.map(key => axis(key));

  return (
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
};

export default Graph;
