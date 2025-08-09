Param(
    # Optional: override the configuration (default = Release)
    [string]$Configuration = 'Release'
)

$SolutionRoot = Resolve-Path -LiteralPath "$PSScriptRoot\.." | Select-Object -ExpandProperty Path

Write-Host "Building NuGet packages in: $SolutionRoot" -ForegroundColor Cyan

$ProjectFiles = Get-ChildItem `
    -Path $SolutionRoot `
    -Recurse `
    -Filter *.csproj `
    | Where-Object { $_.FullName -match '\\src\\' }

if ($ProjectFiles.Count -eq 0) {
    Write-Warning "No .csproj files found under $SolutionRoot."
    exit 1
}

$PackageOutput = Join-Path -Path $SolutionRoot -ChildPath 'Packages'
if (-Not (Test-Path $PackageOutput)) {
    New-Item -ItemType Directory -Path $PackageOutput | Out-Null
}
# ------------------------------------------------------------
# 4️. Pack each project
# ------------------------------------------------------------
foreach ($proj in $ProjectFiles) {
    dotnet pack `
        --configuration $Configuration `
        --output $PackageOutput `
        --verbosity normal `
        "$($proj.FullName)"

    if ($LASTEXITCODE -ne 0) {
        Write-Error " Failed to pack project: $($proj.FullName)"
        exit $LASTEXITCODE
    }
}

Write-Host "`n All packages generated successfully!" -ForegroundColor Green