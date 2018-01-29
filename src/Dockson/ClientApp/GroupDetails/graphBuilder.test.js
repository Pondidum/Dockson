import { buildDataset } from "./graphBuilder";

const testData = {
  masterCommitLeadTime: {
    "08/02/2017": { median: 11.88, deviation: 3.11 },
    "09/02/2017": { median: 14.08, deviation: 478.42 },
    "03/03/2017": { median: 22.55, deviation: 435.54 },
    "08/03/2017": { median: 22.65, deviation: 376.65 }
  },
  masterCommitInterval: {
    "08/02/2017": { median: 83.01, deviation: 0 },
    "09/02/2017": { median: 137.91, deviation: 743.62 },
    "03/03/2017": { median: 31486.73, deviation: 0 },
    "08/03/2017": { median: 3695.05, deviation: 5086.36 }
  },
  buildLeadTime: {},
  buildInterval: {},
  buildRecoveryTime: {},
  buildFailureRate: {},
  deploymentLeadTime: {},
  deploymentInterval: {}
};

it("should be ordered correctly", () => {
  const badOrder = {
    masterCommitLeadTime: {
      "03/03/2017": { median: 22.55, deviation: 435.54 },
      "08/02/2017": { median: 11.88, deviation: 3.11 },
      "09/02/2017": { median: 14.08, deviation: 478.42 },
      "08/03/2017": { median: 22.65, deviation: 376.65 }
    }
  };

  const result = buildDataset(badOrder, [
    { name: "masterCommitLeadTime", keys: ["median"] }
  ]);

  expect(result.datasets).toEqual([
    {
      label: "masterCommitLeadTime.median",
      data: [11.88, 14.08, 22.55, 22.65],
      fill: false,
      yAxisID: "median"
    }
  ]);

  expect(result.labels).toEqual([
    "08/02/2017",
    "09/02/2017",
    "03/03/2017",
    "08/03/2017"
  ]);
});

it("should return a single dataset", () => {
  const result = buildDataset(testData, [
    { name: "masterCommitLeadTime", keys: ["median"] }
  ]);

  expect(result.datasets).toEqual([
    {
      label: "masterCommitLeadTime.median",
      data: [11.88, 14.08, 22.55, 22.65],
      fill: false,
      yAxisID: "median"
    }
  ]);
});

it("should return two series in one dataset", () => {
  const result = buildDataset(testData, [
    { name: "masterCommitLeadTime", keys: ["median", "deviation"] }
  ]);

  expect(result.datasets).toEqual([
    {
      label: "masterCommitLeadTime.median",
      data: [11.88, 14.08, 22.55, 22.65],
      fill: false,
      yAxisID: "median"
    },
    {
      label: "masterCommitLeadTime.deviation",
      data: [3.11, 478.42, 435.54, 376.65],
      fill: false,
      yAxisID: "deviation"
    }
  ]);
});

it("should return two series in two datasets", () => {
  const result = buildDataset(testData, [
    { name: "masterCommitLeadTime", keys: ["median", "deviation"] },
    { name: "masterCommitInterval", keys: ["median", "deviation"] }
  ]);

  expect(result.datasets).toEqual([
    {
      label: "masterCommitLeadTime.median",
      data: [11.88, 14.08, 22.55, 22.65],
      fill: false,
      yAxisID: "median"
    },
    {
      label: "masterCommitLeadTime.deviation",
      data: [3.11, 478.42, 435.54, 376.65],
      fill: false,
      yAxisID: "deviation"
    },
    {
      label: "masterCommitInterval.median",
      data: [83.01, 137.91, 31486.73, 3695.05],
      fill: false,
      yAxisID: "median"
    },
    {
      label: "masterCommitInterval.deviation",
      data: [0, 743.62, 0, 5086.36],
      fill: false,
      yAxisID: "deviation"
    }
  ]);
});
