Param(
    [string] $VersionSuffix = ""
)

$BuildDirectory = "$PSScriptRoot\..\build"
$PackageDirectory = "$BuildDirectory\package"

Write-Host -NoNewline ".NET SDK Version".PadRight(40, ' ')
dotnet --version

Write-Host -NoNewline "Script Path".PadRight(40, ' ')
Write-Host $PSScriptRoot

Write-Host ""

# Make sure our build directory and package directory exists.
if (-not (Test-Path $BuildDirectory)) {
    New-Item -Path $BuildDirectory -ItemType Directory | Out-Null
}
else {
    Remove-Item $BuildDirectory -Recurse
}

if (-not (Test-Path $PackageDirectory)) {
    New-Item -Path $PackageDirectory -ItemType Directory | Out-Null
}

# Drop any directories that we don't want to do anything with.
$projectDirs = Get-ChildItem $PSScriptRoot\..\src -Directory | Where-Object { -not ( $_.Name.StartsWith("_NCrunch")) }

# Projects that we just need to run test for.
$testProjects = $projectDirs | Where-Object { $_.Name.EndsWith(".Tests") }

# Projects that need to be built into packages.
$packageProjects = $projectDirs | Where-Object { -not ($_.Name.EndsWith(".Tests")) }

Write-Host @"
==================================================
  Restoring Packages...
==================================================
"@

foreach ($dir in $projectDirs) {
    dotnet restore $dir.FullName

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to successfully restore packages for `"$($dir.FullName)`""
        Write-Host  "##teamcity[ buildStatus status='FAILURE' ]"
        exit 1
    }
}

Write-Host @"
==================================================
  Running Tests...
==================================================
"@

foreach ($dir in $testProjects) {
    $csprojFile = "$($dir.FullName)/$($dir.Name).csproj"

    if (-not (Test-Path $csprojFile)) {
        Write-Host ".csproj file was not found at expected path: $csprojFile"
        continue
    }

    dotnet test $csprojFile -c Debug --filter "Category=Unit;Category=Integration"
}

Write-Host @"
==================================================
  Building Packages...
==================================================
"@

foreach ($dir in $packageProjects) {
    $csprojFile = "$($dir.FullName)/$($dir.Name).csproj"

    if (-not (Test-Path $csprojFile)) {
        Write-Host ".csproj file was not found at expected path: $csprojFile"
        continue
    }

    dotnet clean $csprojFile
    dotnet pack  $csprojFile -c Release -o $PackageDirectory
    if ($LASTEXITCODE -ne 0) {
        Write-Host "##teamcity[ buildStatus status='FAILURE' message text='Failed to build project at $csprojFile' ]"
        exit 1
    }
}