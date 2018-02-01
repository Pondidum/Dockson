import { argv } from "yargs";
import fetch from "node-fetch";

const owner = argv.owner;
const repo = argv.repo;

const run = () => {
  if (!owner || !repo) {
    console.error("You must specifiy --owner and --repo");
    return;
  }

  return fetch(
    `https://api.github.com/repos/${owner}/${repo}/pulls?state=all`,
    {
      headers: {
        Authorization: `token: ${process.env.DOCKSON_GITHUB}`
      }
    }
  );
};

run();
