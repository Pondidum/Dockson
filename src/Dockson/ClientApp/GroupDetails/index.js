import React, { Component } from "react";
import { connect } from "react-redux";
import { Row, Col, Panel } from "react-bootstrap";
import { fetchGroupDetails } from "../Groups/actions";
import { Line as Chart } from "react-chartjs-2";
import { buildGraph, buildAxes, buildDataset } from "./graphBuilder";

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

  compositeChart(group, title, cols, what) {
    return (
      <Col sm={12} md={cols}>
        <Panel>
          <div className="panel-heading">
            <h4 className="panel-title">{title}</h4>
          </div>
          <div className="panel-body">
            <Chart
              data={buildDataset(group, what)}
              options={{
                maintainAspectRatio: false,
                scales: {
                  yAxes: buildAxes(group, what)
                }
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
        {this.compositeChart(group, "Commit Lead Time", 6, [
          { name: "masterCommitLeadTime", keys: ["median", "deviation"] }
        ])}
        {this.compositeChart(group, "Commit Interval", 6, [
          { name: "masterCommitInterval", keys: ["median", "deviation"] }
        ])}
        {this.compositeChart(group, "Master Throughput", 12, [
          { name: "masterCommitLeadTime", keys: ["median" /*, "deviation"*/] },
          { name: "masterCommitInterval", keys: ["median" /*, "deviation"*/] }
        ])}
        {/* {this.chart(group, "buildLeadTime")}
        {this.chart(group, "buildInterval")}
        {this.chart(group, "buildRecoveryTime")}
        {this.chart(group, "buildFailureRate", ["rate"])}
        {this.chart(group, "deploymentLeadTime")}
        {this.chart(group, "deploymentInterval")} */}
      </Row>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(GroupDetails);
