import React, { Component } from "react";
import { connect } from "react-redux";
import { Line as Chart } from "react-chartjs-2";
import { fetchGroupDetails } from "../Groups/actions";

const mapStateToProps = (state, ownProps) => {
  const groupName = ownProps.match.params.group;
  return {
    groupName: groupName,
    group: state.groups.details[groupName]
  };
};

const mapDispatchToProps = dispatch => {
  return {
    fetchGroup: name => dispatch(fetchGroupDetails(name))
  };
};

class GroupDetails extends Component {
  componentDidMount() {
    this.props.fetchGroup(this.props.groupName);
  }

  render() {
    const group = this.props.group || { loading: true };

    if (group.loading) {
      return <div>Loading...</div>;
    }

    const days = Object.keys(group.masterCommitLeadTime);

    const data = {
      labels: days,
      datasets: [
        {
          label: "Median",
          data: days.map(day => group.masterCommitLeadTime[day].median),
          fill: false,
          borderColor: "rgba(255,99,132,1)"
        },
        {
          label: "Standard Deviation",
          data: days.map(day => group.masterCommitLeadTime[day].deviation),
          fill: false,
          borderColor: "rgba(54, 162, 235, 1)"
        }
      ]
    };
    return (
      <Chart
        data={data}
        options={{
          maintainAspectRatio: false
        }}
      />
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(GroupDetails);
