const defaultState = {
  names: [],
  details: {}
};

const replaceGroup = (existing, name, value) =>
  Object.assign({}, existing, { [name]: value });

const buildDelta = (state, action) => {
  switch (action.type) {
    case "LIST_GROUPS_REQUEST":
      return {
        names: []
      };

    case "LIST_GROUPS_SUCCESS":
      return {
        names: action.names
      };

    case "FETCH_GROUP_REQUEST":
      return {
        details: replaceGroup(state.details, action.group, { loading: true })
      };

    case "FETCH_GROUP_SUCCESS":
      return {
        details: replaceGroup(state.details, action.group, action.view)
      };

    default:
      return state;
  }
};

export default (state = defaultState, action) =>
  Object.assign({}, state, buildDelta(state, action));
