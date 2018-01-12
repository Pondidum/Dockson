import React, { Component } from "react";
import { connect } from "react-redux";
import { Row, Col, Panel } from "react-bootstrap";
import { fetchGroupDetails } from "../Groups/actions";
import Graph from "./graph";

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

  renderTitle(name) {
    return (
      name.charAt(0).toUpperCase() +
      name
        .slice(1)
        .replace(/([A-Z])/g, " $1")
        .trim()
    );
  }

  chart(group, property, keys = ["median", "deviation"]) {
    const graphData = group[property];

    return (
      <Col sm={12} md={6}>
        <Panel>
          <div className="panel-heading">
            <h4 className="panel-title">{this.renderTitle(property)}</h4>
          </div>
          <div className="panel-body">
            <Graph dataSource={graphData} keys={keys} />
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
