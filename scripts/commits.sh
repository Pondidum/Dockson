#!/bin/bash

apiUrl="http://localhost:5000/api/log/commit"
serviceName="lindorff-generator"

pushd $1 > /dev/null

git log --merges --pretty="%H %P" --reverse | while read line; do

  masterSha=$(echo $line | cut -f1 -d ' ')
  branchSha=$(echo $line | cut -f3 -d ' ')

  masterName="master"
  branchName=$(git log --format=%B $masterSha -n 1 | head -n1 | grep -oP "(?<=from ).*" | cut -f2 -d '/')

  masterTimestamp=$(git show -s --format="%ci" $masterSha)
  branchTimestamp=$(git show -s --format="%ci" $branchSha)

  masterJson="{\"timestamp\": \"$masterTimestamp\", \"name\": \"$serviceName\", \"version\": \"1.0.0\", \"branch\":\"$masterName\", \"commit\":\"$branchSha\"}"
  branchJson="{\"timestamp\": \"$branchTimestamp\", \"name\": \"$serviceName\", \"version\": \"1.0.0\", \"branch\":\"$branchName\", \"commit\":\"$branchSha\"}"

  curl "$apiUrl" -X POST -H "Content-Type: application/json" -d "$branchJson"
  curl "$apiUrl" -X POST -H "Content-Type: application/json" -d "$masterJson"
done

popd > /dev/null