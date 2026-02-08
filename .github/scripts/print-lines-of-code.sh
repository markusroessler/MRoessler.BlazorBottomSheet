#!/bin/bash
scriptDir=$(dirname "$0")
source "$scriptDir/lines-of-codes-lib.sh"

allSourceCodeFiles=$(printAllSourceCodeFiles)
appCount=$(printAppCount "$allSourceCodeFiles")
libCount=$(printLibCount "$allSourceCodeFiles")
testCount=$(printTestCount "$allSourceCodeFiles")
totalCount=$(echo "$allSourceCodeFiles" | xargs grep -v '^$' | wc -l)

echo "## Lines of Code"
echo "| Type | Count | "
echo "| :--- | :--- | "
echo "| Lib | $libCount | "
echo "| SampleApp | $appCount | "
echo "| Tests | $testCount | "
echo "| **Total** | **$totalCount** | "