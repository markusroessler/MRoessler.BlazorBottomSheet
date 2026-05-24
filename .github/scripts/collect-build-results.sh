#!/bin/bash
dotnet export-dotnet-results-for-github --export-step-summary true --github-server-url $1 --github-repo $2 --github-ref-name $3 --culture "de-DE"
