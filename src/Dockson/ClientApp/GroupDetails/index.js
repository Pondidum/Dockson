import React, { Component } from "react";
import { connect } from "react-redux";
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
    return <pre>Group {JSON.stringify(group, null, 2)}</pre>;
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(GroupDetails);
