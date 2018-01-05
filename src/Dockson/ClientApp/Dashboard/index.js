import React, { Component } from "react";
import { connect } from "react-redux";
import { listAllToggles } from "./actions";

const mapStateToProps = state => {
  return state.toggles;
};

const mapDispatchToProps = dispatch => {
  return {
    listAllToggles: () => dispatch(listAllToggles())
  };
};

class Dashboard extends Component {
  componentDidMount() {
    //this.props.listAllToggles();
  }

  render() {
    const toggles = this.props.toggles || [];
    return (
      <ul className="list-unstyled row">
        {toggles.map((toggle, index) => <h4>Test</h4>)}
      </ul>
    );
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Dashboard);
