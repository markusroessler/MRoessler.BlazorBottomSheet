#!/bin/bash

# example rewrite:
# import { * } from "/_content/Foo/Bar.razor.js"
# to
# import { * } from "/MRoessler.BlazorBottomSheet/_content/Foo/Bar.razor.js"
find Sample.Wasm/bin/Release/net10.0/publish/wwwroot -name "*.js" -exec bash -c 'echo "Rewriting module import paths: $1"; sed -i "s/_content/MRoessler.BlazorBottomSheet\/_content/g" "$1"' _ {} \;
