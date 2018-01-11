#!/bin/bash

pushd $1

git log --merges --pretty="%H %P" | while read line; do

  master=$(echo $line | cut -f1 -d ' ')
  branch=$(echo $line | cut -f3 -d ' ')

  echo "branch $branch -> master $master"
done

popd