#!/bin/bash

pushd $1

git log --merges --pretty="%H %P" | awk '{ $2 = ""; print }'

popd