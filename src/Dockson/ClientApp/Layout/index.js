import React from "react";
import { Col, Row } from "react-bootstrap";
import Menu from "./menu";

export default ({ children }) => (
  <div className="container-fluid" id="root">
    <Row>
      <Col sm={3} md={2} className="sidebar">
        <h1>Dockson</h1>
        <Menu />
      </Col>
      <Col sm={9} md={10} smOffset={3} mdOffset={2} className="main">
        {children}
      </Col>
    </Row>
  </div>
);
