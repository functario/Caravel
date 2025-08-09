$targetPath = "\bin\Debug\net9.0"
$scriptsPath = "\bin\Debug\net9.0\Scripts"
$currentPath = Get-Location

if ($currentPath.Path.EndsWith($scriptsPath)) {
    $playwrightPath = "..\playwright.ps1"
} elseif ($currentPath.Path.EndsWith($targetPath)) {
    $playwrightPath = ".\playwright.ps1"
} else {
    $playwrightPath = Join-Path -Path $PSScriptRoot -ChildPath "..\$targetPath\playwright.ps1"
}

& $playwrightPath codegen https://app-salc-qa-01.azurewebsites.net/
