const buildDataset = (dataSource, what) => {
  const datasets = what.map(set => {
    const series = dataSource[set.name];

    return set.keys.map(key => {
      return {
        label: `${set.name}.${key}`,
        data: Object.keys(series).map(day => series[day][key]),
        fill: false
      };
    });
  });

  return [].concat.apply([], datasets);
};

export default buildDataset;
