const defaultState = {
  loading: false,
  names: []
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case "LIST_GROUPS_REQUEST":
      return {
        loading: true,
        names: []
      };

    case "LIST_GROUPS_SUCCESS":
      return {
        loading: false,
        names: action.names
      };

    default:
      return state;
  }
};
