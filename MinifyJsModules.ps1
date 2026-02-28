param(
    [string]$minifyInputFiles,
    [string]$minifyOutputDir
)

npx esbuild $minifyInputFiles --minify --sourcemap --out-extension:.js=.min.js --outbase=. --outdir="$minifyOutputDir"

# Find all .min.js files and replace 
# import { ... } from "...razor.js"
# with
# import { ... } from "...razor.min.js"
Get-ChildItem -Path "$minifyOutputDir" -Filter "*.min.js" -Recurse | ForEach-Object {
    echo "Replacing imports in $_"
    $content = Get-Content -Path $_.FullName -Raw
    $content = $content -replace '(import.*from.*).razor.js"', '$1.razor.min.js"'
    Set-Content -Path $_.FullName -Value $content
}
