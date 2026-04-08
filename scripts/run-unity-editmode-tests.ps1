param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\6000.4.0f1\Editor\Unity.exe",
    [string]$ProjectPath = "",
    [ValidateSet("EditMode", "PlayMode")]
    [string]$TestPlatform = "EditMode",
    [string]$ResultsPath = "",
    [string]$LogPath = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $repoRoot "KiarcheContinuumWar"
}

$artifactsRoot = Join-Path $repoRoot "test-results\\unity"
if ([string]::IsNullOrWhiteSpace($ResultsPath)) {
    $ResultsPath = Join-Path $artifactsRoot "$($TestPlatform.ToLower())-results.xml"
}

if ([string]::IsNullOrWhiteSpace($LogPath)) {
    $LogPath = Join-Path $artifactsRoot "$($TestPlatform.ToLower()).log"
}

$resultsDirectory = Split-Path -Parent $ResultsPath
$logDirectory = Split-Path -Parent $LogPath

if (!(Test-Path $UnityPath)) {
    throw "Unity executable not found: $UnityPath"
}

if (!(Test-Path $ProjectPath)) {
    throw "Unity project not found: $ProjectPath"
}

if (!(Test-Path $resultsDirectory)) {
    New-Item -ItemType Directory -Path $resultsDirectory -Force | Out-Null
}

if (!(Test-Path $logDirectory)) {
    New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null
}

if (Test-Path $LogPath) {
    Remove-Item -Path $LogPath -Force
}

$arguments = @(
    "-batchMode",
    "-nographics",
    "-quit",
    "-projectPath", $ProjectPath,
    "-runTests",
    "-testPlatform", $TestPlatform,
    "-testResults", $ResultsPath,
    "-logFile", $LogPath
)

$process = Start-Process -FilePath $UnityPath -ArgumentList $arguments -Wait -PassThru
$exitCode = $process.ExitCode
if ($exitCode -ne 0) {
    if ((Test-Path $LogPath) -and (Select-String -Path $LogPath -Pattern "No valid Unity Editor license found" -Quiet)) {
        throw "Unity batch mode could not run tests because no valid Unity Editor license is active. Activate Unity and rerun. See $LogPath"
    }

    throw "Unity $TestPlatform tests failed with exit code $exitCode. See $LogPath"
}

Write-Host "Unity $TestPlatform tests passed."
Write-Host "Results: $ResultsPath"
Write-Host "Log: $LogPath"
