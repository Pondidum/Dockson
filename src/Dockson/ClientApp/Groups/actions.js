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

  const success = data => ({
    type: "FETCH_GROUP_SUCCESS",
    group: group,
    view: data
  });

  const failure = res => ({
    type: "FETCH_GROUP_FAILURE",
    group: group,
    status: res.status,
    message: res.statusText
  });

  const fetchTask = fetch(`/api/view/groups/${group}`).then(res => {
    if (res.ok) {
      return res.json().then(json => dispatch(success(json)));
    } else {
      dispatch(failure(res));
    }
  });

  addTask(fetchTask);
  dispatch({ type: "FETCH_GROUP_REQUEST", group: group });
};
