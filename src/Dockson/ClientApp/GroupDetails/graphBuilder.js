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

const buildDataset = (dataSource, what) => {
  const datasets = what.map(set => {
    const series = dataSource[set.name];

    return set.keys.map(key => {
      return {
        label: `${set.name}.${key}`,
        data: buildData(series, key),
        fill: false
      };
    });
  });

  return [].concat.apply([], datasets);
};

export default buildDataset;
