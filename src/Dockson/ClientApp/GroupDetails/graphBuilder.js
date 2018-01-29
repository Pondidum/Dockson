const reverseDate = date =>
  date
    .split("/")
    .reverse()
    .join();

const byDate = (a, b) => {
  const ra = reverseDate(a);
  const rb = reverseDate(b);

  return ra < rb ? -1 : ra > rb ? 1 : 0;
};

const buildData = (series, key) =>
  Object.keys(series)
    .sort(byDate)
    .map(day => series[day][key]);

export const buildDataset = (dataSource, what) => {
  const datasets = what.map(set => {
    const series = dataSource[set.name];

    return set.keys.map(key => {
      return {
        label: `${set.name}.${key}`,
        yAxisID: key,
        data: buildData(series, key),
        fill: false
      };
    });
  });

  return {
    labels: Object.keys(dataSource[what[0].name]).sort(byDate),
    datasets: [].concat.apply([], datasets)
  };
};

export const buildAxes = (dataSource, what) => {
  const datasets = what.map(set =>
    set.keys.map(key => ({
      id: key,
      type: "linear",
      position: key == "median" ? "left" : "right"
    }))
  );

  return [].concat.apply([], datasets);
};
