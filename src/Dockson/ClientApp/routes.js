import React from "react";
import { Route } from "react-router-dom";
import Layout from "./Layout";
import Dashboard from "./Dashboard";
import GroupView from "./GroupDetails";

const routes = (
  <Layout>
    <Route exact path="/" component={Dashboard} />
    <Route exact path="/groups/:group" component={GroupView} />
  </Layout>
);

module.exports = routes;
