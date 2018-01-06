import { fetch, addTask } from "domain-task";

export const listAllGroups = () => (dispatch, getState) => {
  let fetchTask = fetch(`/api/view/groups`)
    .then(response => response.json())
    .then(data =>
      dispatch({
        type: "LIST_GROUPS_SUCCESS",
        names: data
      })
    );

  addTask(fetchTask);
  dispatch({ type: "LIST_GROUPS_REQUEST" });
};

export const fetchGroupDetails = group => (dispatch, getState) => {
  if (!group) {
    return;
  }

  const fetchTask = fetch(`/api/view/groups/${group}`)
    .then(res => res.json())
    .then(data =>
      dispatch(
        Object.assign({
          type: "FETCH_GROUP_SUCCESS",
          group: group,
          view: data
        })
      )
    );

  addTask(fetchTask);
  dispatch({ type: "FETCH_GROUP_REQUEST", group: group });
};
