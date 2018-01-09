import React, { Component } from "react";
import { connect } from "react-redux";
import { Row, Col, Panel } from "react-bootstrap";
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

    this.schemes = {
      median: { label: "Median", color: "rgba(255,99,132,1)" },
      deviation: {
        label: "Standard Deviation",
        color: "rgba(54, 162, 235, 1)"
      },
      rate: { label: "Rate", color: "rgba(255,99,132,1)" }
    };
  }

  dataset(group, key) {
    const keys = Object.keys(group);
    const scheme = this.schemes[key];

    return {
      label: scheme.label,
      data: keys.map(day => group[day][key]),
      fill: false,
      borderColor: scheme.color
    };
  }

  renderTitle(name) {
    return (
      name.charAt(0).toUpperCase() +
      name
        .slice(1)
        .replace(/([A-Z])/g, " $1")
        .trim()
    );
  }

  chart(group, property, datasets = ["median", "deviation"]) {
    const graphData = group[property];
    const days = Object.keys(graphData);

    return (
      <Col sm={6}>
        <Panel>
          <div className="panel-heading">
            <h4 className="panel-title">{this.renderTitle(property)}</h4>
          </div>
          <div className="panel-body">
            <Chart
              data={{
                labels: days,
                datasets: datasets.map(key => this.dataset(graphData, key))
              }}
              options={{
                maintainAspectRatio: false
              }}
            />
          </div>
        </Panel>
      </Col>
    );
  }

  render() {
    const group = this.props.group || { loading: true };

    if (group.loading) {
      return <div>Loading...</div>;
    }

    return (
      <Row>
        {this.chart(group, "masterCommitLeadTime")}
        {this.chart(group, "masterCommitInterval")}
        {this.chart(group, "buildLeadTime")}
        {this.chart(group, "buildInterval")}
        {this.chart(group, "buildRecoveryTime")}
        {this.chart(group, "buildFailureRate", ["rate"])}
        {this.chart(group, "deploymentLeadTime")}
        {this.chart(group, "deploymentInterval")}
      </Row>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(GroupDetails);
