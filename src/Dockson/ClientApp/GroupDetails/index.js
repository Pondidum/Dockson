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

  chart(group) {
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
      <Col sm={6}>
        <Panel>
          <div className="panel-heading">
            <h4 className="panel-title">Title</h4>
          </div>
          <div className="panel-body">
            <Chart
              data={data}
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
        {this.chart(group)}
        {this.chart(group)}
      </Row>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(GroupDetails);
