import React, { Component } from "react";
import { connect } from "react-redux";
import { listAllGroups } from "../Groups/actions";
import { NavLink } from "react-router-dom";

const mapStateToProps = state => {
  return state.groups;
};

const mapDispatchToProps = dispatch => {
  return {
    listAllGroups: () => dispatch(listAllGroups())
  };
};

const NavEntry = ({ link, text }) => (
  <li>
    <NavLink exact to={link} activeClassName="active">
      <span className="glyphicon glyphicon-home" /> {text}
    </NavLink>
  </li>
);

class Menu extends Component {
  componentDidMount() {
    this.props.listAllGroups();
  }

  render() {
    const groups = this.props.groups || [];
    return (
      <ul className="list-unstyled">
        <NavEntry link="/" text="Home" />
        {groups.map((group, index) => (
          <NavEntry key={index} link={`group/${group}`} text={group} />
        ))}
      </ul>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Menu);
