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

const toHours = value => value / 60;
const round = value => Math.round(value * 100) / 100;

const buildData = (series, key) =>
  Object.keys(series)
    .sort(byDate)
    .map(day => series[day][key])
    .map(toHours)
    .map(round);

const getColor = index => {
  if (index === 0) return "rgba(255,99,132,1)";
  if (index === 1) return "rgba(54, 162, 235, 1)";
  if (index === 2) return "rgba(255, 206, 86, 1)";
  return "rgba(75, 192, 192, 1)";
};

export const buildDataset = (dataSource, what) => {
  let current = -1;
  const datasets = what.map(set => {
    const series = dataSource[set.name];

    return set.keys.map(key => {
      current++;
      return {
        label: `${set.name}.${key}`,
        yAxisID: key,
        data: buildData(series, key),
        fill: false,
        borderColor: getColor(current)
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
