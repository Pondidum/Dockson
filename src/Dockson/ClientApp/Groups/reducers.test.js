import reducer from "./reducers";
import freeze from "deep-freeze";

const emptyState = freeze({ names: [], details: {} });
describe("undefined action type", () => {
  it("should return the default state when input is null", () => {
    const state = reducer(undefined, {});

    expect(state).toEqual(emptyState);
  });

  it("should return the current state when type is unknown", () => {
    const state = reducer(freeze({ test: true }), {
      type: "wat"
    });

    expect(state).toEqual({ test: true });
  });
});

describe("LIST_GROUPS_REQUEST", () => {
  it("should return empty names when requesting all groups", () => {
    const state = reducer(undefined, {
      type: "LIST_GROUPS_REQUEST"
    });

    expect(state).toEqual(emptyState);
  });

  it("should replace names array, and keep other state", () => {
    const existing = freeze({
      names: ["one", "two"],
      details: {
        one: { loading: true },
        two: { loading: false }
      }
    });

    const state = reducer(existing, {
      type: "LIST_GROUPS_REQUEST"
    });

    expect(state).toEqual({
      names: [],
      details: {
        one: { loading: true },
        two: { loading: false }
      }
    });
  });
});

describe("LIST_GROUPS_SUCCESS", () => {
  it("should populate names when receiving all groups", () => {
    const existing = freeze({
      names: ["one", "two"],
      details: {
        one: { loading: true },
        two: { loading: false }
      }
    });

    const state = reducer(existing, {
      type: "LIST_GROUPS_SUCCESS",
      names: [1, 2, 3]
    });

    expect(state).toEqual({
      names: [1, 2, 3],
      details: {
        one: { loading: true },
        two: { loading: false }
      }
    });
  });
});

describe("FETCH_GROUP_REQUEST", () => {
  it("should mark the group as loading", () => {
    const state = reducer(undefined, {
      type: "FETCH_GROUP_REQUEST",
      group: "test"
    });

    expect(state).toEqual({
      names: [],
      details: { test: { loading: true } }
    });
  });

  it("should not remove old values", () => {
    const existing = freeze({
      names: [],
      details: {
        one: { loading: true },
        two: { loading: false }
      }
    });

    const state = reducer(existing, {
      type: "FETCH_GROUP_REQUEST",
      group: "test"
    });

    expect(state).toEqual({
      names: [],
      details: {
        one: { loading: true },
        two: { loading: false },
        test: { loading: true }
      }
    });
  });
});

describe("FETCH_GROUP_SUCCESS", () => {
  it("should not remove old values", () => {
    const existing = freeze({
      names: [],
      details: {
        one: { loading: true },
        two: { loading: false }
      }
    });

    const state = reducer(existing, {
      type: "FETCH_GROUP_SUCCESS",
      group: "test",
      view: {
        something: true,
        otherthing: []
      }
    });

    expect(state).toEqual({
      names: [],
      details: {
        one: { loading: true },
        two: { loading: false },
        test: {
          something: true,
          otherthing: []
        }
      }
    });
  });
});
