# scripts\publish-nugets.ps1
param(
    # Optional: override the feed URL or name.
    [string]$FeedName   = "LocalNugets",
    [string]$PackagesDir = (Join-Path -Path $PSScriptRoot -ChildPath "..\packages")
)
# ------------------------------------------------------------------
# Helper – write a header to make output easier to read
function Write-Header {
    param([string]$Text)
    Write-Host "`n=== $Text ===`n"
}

Write-Header "NuGet Publish Script"

# Resolve the absolute path of the packages folder
$packagesPath = Resolve-Path -LiteralPath $PackagesDir

if (-not (Test-Path $packagesPath)) {
    Write-Error "Packages directory not found: $packagesPath"
    exit 1
}

# ------------------------------------------------------------------
# Get all .nupkg files (skip *.snupkg)
$nupkgs = Get-ChildItem -Path $packagesPath -Filter "*.nupkg" | Where-Object { $_.Name -notlike "*.snupkg*" }

if ($nupkgs.Count -eq 0) {
    Write-Warning "No .nupkg files found in $packagesPath"
    exit 0
}

# ------------------------------------------------------------------
foreach ($pkg in $nupkgs) {

    # Extract package ID and version for logging
    if ($pkg.Name -match '^(?<id>.+?)\.(?<ver>\d+\.\d+\.\d+(?:-[^\.]+)?)\.nupkg$') {
        $id   = $Matches['id']
    } else {
        # fallback – just use the file name
        $id = $pkg.BaseName
    }

    Write-Host "Pushing $($pkg.Name) Feed '$FeedName'"

    try {
        dotnet nuget push `
            "$($pkg.FullName)" `
            --source $FeedName `
            --api-key VSTS 
       
        Write-Host "Published: $id" -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to publish $($pkg.FullName): $_"
        exit 1   # stop on first failure
    }
}

Write-Header "All packages published successfully."
