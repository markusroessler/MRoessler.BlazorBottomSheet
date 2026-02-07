#!/bin/bash
allSourceCodeFiles=$(git ls-files | grep -E '\.cs$|\.js$|\.razor$')
appCount=$(echo "$allSourceCodeFiles" | grep -E '.*Sample/' | xargs grep -v '^$' | wc -l)
libCount=$(echo "$allSourceCodeFiles" | grep -E -v '.*Test/.*|Sample.*/' | xargs grep -v '^$' | wc -l)
testCount=$(echo "$allSourceCodeFiles" | grep -E '.*Test/.*' | xargs grep -v '^$' | wc -l)
totalCount=$(echo "$allSourceCodeFiles" | xargs grep -v '^$' | wc -l)

echo "## Lines of Code"
echo "| Type | Count | "
echo "| :--- | :--- | "
echo "| Lib | $libCount | "
echo "| SampleApp | $appCount | "
echo "| Tests | $testCount | "
echo "| **Total** | **$totalCount** | "