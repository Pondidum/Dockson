import reducer from "./reducers";

it("should return the default state when input is null", () => {
  const state = reducer(undefined, {});
  expect(state).toEqual({ loading: false, groups: [] });
});

it("should return the current state when type is unknown", () => {
  const state = reducer({ test: true }, { type: "wat" });
  expect(state).toEqual({ test: true });
});

it("should set loading true when requesting all groups", () => {
  const state = reducer({}, { type: "LIST_GROUPS_REQUEST" });
  expect(state).toEqual({ loading: true, groups: [] });
});

it("should populate groups when receiving all groups", () => {
  const state = reducer(
    {},
    {
      type: "LIST_GROUPS_SUCCESS",
      groups: [1, 2, 3]
    }
  );

  expect(state).toEqual({
    loading: false,
    groups: [1, 2, 3]
  });
});
