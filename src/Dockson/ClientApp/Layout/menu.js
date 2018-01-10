import React, { Component } from "react";
import { connect } from "react-redux";
import { listAllGroups } from "../Groups/actions";
import MenuItem from "./menuItem";

const mapStateToProps = state => {
  return {
    names: state.groups.names
  };
};

const mapDispatchToProps = dispatch => {
  return {
    listAllGroups: () => dispatch(listAllGroups())
  };
};

class Menu extends Component {
  componentDidMount() {
    this.props.listAllGroups();
  }

  render() {
    const groupNames = this.props.names || [];

    if (groupNames.length === 0)
      return <div>No Services or Groups found :(</div>;

    return (
      <ul className="list-unstyled">
        {groupNames.map((group, index) => (
          <MenuItem key={index} link={`/groups/${group}`} text={group} />
        ))}
      </ul>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Menu);
