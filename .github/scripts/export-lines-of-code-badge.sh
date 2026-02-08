#!/bin/bash
scriptDir=$(dirname "$0")
source "$scriptDir/lines-of-codes-lib.sh"

allSourceCodeFiles=$(printAllSourceCodeFiles)
libCount=$(printLibCount $allSourceCodeFiles)
badgeUri="https://img.shields.io/badge/Lines_of_Code-$libCount-blue"

curl $badgeUri --output badges/badge_lines_of_code.svg