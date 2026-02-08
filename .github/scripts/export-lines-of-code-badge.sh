#!/bin/bash
allSourceCodeFiles=$(git ls-files | grep -E '\.cs$|\.js$|\.razor$')
libCount=$(echo "$allSourceCodeFiles" | grep -E -v '.*Test/.*|Sample.*/' | xargs grep -v '^$' | wc -l)
badgeUri="https://img.shields.io/badge/lines_of_code-$libCount-blue"
curl $badgeUri --output badges/badge_lines_of_code.svg