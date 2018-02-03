import { argv } from "yargs";
import fetch from "node-fetch";

const owner = argv.owner;
const repo = argv.repo;

const parseLinks = linksHeader =>
  linksHeader.split(",").reduce((all, link) => {
    const match = link.match(/\<(.*)\>.*rel="(.*)"/);
    const url = match[1];
    const rel = match[2];

    all[rel] = url;
    return all;
  }, {});

const appendPulls = dto => {
  console.log(`adding ${dto.length} items`);
  return Promise.resolve();
};

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
  );
};

run();
