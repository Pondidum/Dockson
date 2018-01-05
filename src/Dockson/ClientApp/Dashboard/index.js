import React, { Component } from "react";
import { connect } from "react-redux";
import { listAllGroups } from "../Groups/actions";

const mapStateToProps = state => {
  return state.groups;
};

const mapDispatchToProps = dispatch => {
  return {
    listAllGroups: () => dispatch(listAllGroups())
  };
};

class Dashboard extends Component {
  componentDidMount() {
    this.props.listAllGroups();
  }

  render() {
    const groups = this.props.groups || [];
    return (
      <ul className="list-unstyled row">
        {groups.map((group, index) => <h4 key={index}>{group}</h4>)}
      </ul>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Dashboard);
