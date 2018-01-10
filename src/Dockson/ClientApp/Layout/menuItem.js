import React from "react";
import { NavLink } from "react-router-dom";

const MenuItem = ({ link, text }) => (
  <li>
    <NavLink exact to={link} activeClassName="active">
      <span className="glyphicon glyphicon-stats" /> {text}
    </NavLink>
  </li>
);

export default MenuItem;
