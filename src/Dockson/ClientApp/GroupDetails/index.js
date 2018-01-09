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
  }

  median(group) {
    const keys = Object.keys(group);

    return {
      label: "Median",
      data: keys.map(day => group[day].median),
      fill: false,
      borderColor: "rgba(255,99,132,1)"
    };
  }

  deviation(group) {
    const keys = Object.keys(group);

    return {
      label: "Standard Deviation",
      data: keys.map(day => group[day].deviation),
      fill: false,
      borderColor: "rgba(54, 162, 235, 1)"
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

  chart(group, property) {
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
                datasets: [this.median(graphData), this.deviation(graphData)]
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
        {this.chart(group, "buildFailureRate")}
        {this.chart(group, "deploymentLeadTime")}
        {this.chart(group, "deploymentInterval")}
      </Row>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(GroupDetails);
