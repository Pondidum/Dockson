import { argv } from "yargs";
import fetch from "node-fetch";

const owner = argv.owner;
const repo = argv.repo;

let pulls = [];

const appendPulls = dto => {
  pulls = pulls.concat(dto);
};

const sendToDockson = () => {
  const merged = pulls
    .filter(pr => pr.merged_at)
    .filter(pr => pr.base.ref === "master")
    .sort((a, b) => {
      if (a.merged_at > b.merged_at) return 1;
      if (a.merged_at < b.merged_at) return -1;
      return 0;
    });

  console.log(`${merged.length} items to send to Dockson`);

  // const branchSha = pr.head.sha;

  // const branchRequest = {
  //   timestamp: "$masterTimestamp",
  //   name: repo,
  //   branch: pr.head.ref,
  //   commit:  pr.head.sha
  // };
};

const parseLinks = linksHeader =>
  linksHeader.split(",").reduce((all, link) => {
    const match = link.match(/\<(.*)\>.*rel="(.*)"/);
    const url = match[1];
    const rel = match[2];

    all[rel] = url;
    return all;
  }, {});

const requestPage = url =>
  fetch(url, {
    headers: { Authorization: `token ${process.env.DOCKSON_GITHUB}` }
  }).then(res =>
    res
      .json()
      .then(appendPulls)
      .then(() => {
        const links = parseLinks(res.headers.get("Link"));

        if (links.next) {
          return requestPage(links.next);
        }
      })
  );

const run = () => {
  if (!owner || !repo) {
    console.error("You must specifiy --owner and --repo");
    return;
  }

  return requestPage(
    `https://api.github.com/repos/${owner}/${repo}/pulls?state=all`
  ).then(sendToDockson);
};

run();
