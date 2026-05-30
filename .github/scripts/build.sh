#!/bin/bash
dotnet build -c:$1 -p:ErrorLog="compiler-diagnostics.sarif%2Cversion=2.1" --no-restore