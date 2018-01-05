const defaultState = {
  loading: false,
  groups: []
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case "LIST_GROUPS_REQUEST":
      return {
        loading: true,
        groups: []
      };

    case "LIST_GROUPS_SUCCESS":
      return {
        loading: false,
        groups: action.groups
      };

    default:
      return state;
  }
};
