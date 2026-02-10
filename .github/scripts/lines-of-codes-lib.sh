#!/bin/bash

printAllSourceCodeFiles() {
    allSourceCodeFiles=$(git ls-files | grep -E '\.cs$|\.js$|\.razor$')
    echo "$allSourceCodeFiles"
}

printLibCount() {
    allSourceCodeFiles=$1
    libCount=$(echo "$allSourceCodeFiles" | grep -E -v '.*Test/.*|Sample.*/' | xargs grep -v '^$' | wc -l)
    echo $libCount
}

printAppCount() {
    allSourceCodeFiles=$1
    appCount=$(echo "$allSourceCodeFiles" | grep -E 'Sample.RazorComponents|Sample.Wasm|Sample.WebApp/' | xargs grep -v '^$' | wc -l)
    echo $appCount
}

printTestCount() {
    allSourceCodeFiles=$1
    testCount=$(echo "$allSourceCodeFiles" | grep -E '.*Test/.*' | xargs grep -v '^$' | wc -l)
    echo $testCount
}