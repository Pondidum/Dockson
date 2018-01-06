import React from "react";

const View = ({ match }) => {
  const group = match.params.group;

  return <div>Group {group || "WAT?!"}</div>;
};

export default View;
