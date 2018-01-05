import { fetch, addTask } from "domain-task";

export const listAllGroups = () => (dispatch, getState) => {
  let fetchTask = fetch(`/api/view/groups`)
    .then(response => response.json())
    .then(data =>
      dispatch({
        type: "LIST_GROUPS_SUCCESS",
        groups: data
      })
    );

  addTask(fetchTask);
  dispatch({ type: "LIST_GROUPS_REQUEST" });
};
