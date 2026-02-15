#!/bin/bash

printAllSourceCodeFiles() {
    allSourceCodeFiles=$(git ls-files | grep -E '\.cs$|\.js$|\.razor$')
    echo "$allSourceCodeFiles"
}

printCount() {
    xargs grep -v '^$' | wc -l
}

printLibCount() {
    allSourceCodeFiles=$1
    libCount=$(echo "$allSourceCodeFiles" | grep -E -v '.*Test/.*|Sample.*/' | printCount)
    echo $libCount
}

printAppCount() {
    allSourceCodeFiles=$1
    appCount=$(echo "$allSourceCodeFiles" | grep -E 'Sample.RazorComponents|Sample.Wasm|Sample.WebApp/' | printCount)
    echo $appCount
}

printTestCount() {
    allSourceCodeFiles=$1
    testCount=$(echo "$allSourceCodeFiles" | grep -E '.*Test/.*' | printCount)
    echo $testCount
}