$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)
$EnvFile = Join-Path $ScriptDir ".env"

if (-not (Test-Path $EnvFile)) {
    throw "Missing $EnvFile. Copy .env.example to .env and configure it first."
}

Get-Content $EnvFile | ForEach-Object {
    if ($_ -match '^\s*#' -or $_ -match '^\s*$') {
        return
    }

    $parts = $_ -split '=', 2
    if ($parts.Length -eq 2) {
        [System.Environment]::SetEnvironmentVariable($parts[0], $parts[1])
    }
}

$AppDir = if ($env:APP_DIR) { $env:APP_DIR } else { Join-Path $RepoRoot "documentation\kcwweb" }
$PublicDir = $env:PUBLIC_DIR
$EnableBackup = if ($env:ENABLE_BACKUP) { $env:ENABLE_BACKUP } else { "true" }
$ReloadCommand = $env:RELOAD_COMMAND
$BackupRoot = if ($env:BACKUP_ROOT) { $env:BACKUP_ROOT } else { Join-Path $ScriptDir "backups" }
$BuildDir = Join-Path $AppDir "dist"
$StagingDir = Join-Path $ScriptDir "artifacts\current"
$Timestamp = Get-Date -Format "yyyyMMdd-HHmmss"

if (-not $PublicDir) {
    throw "PUBLIC_DIR is required."
}

Push-Location $AppDir
try {
    npm ci
    if ($LASTEXITCODE -ne 0) { throw "npm ci failed." }

    npm run build
    if ($LASTEXITCODE -ne 0) { throw "npm run build failed." }
}
finally {
    Pop-Location
}

if (-not (Test-Path $BuildDir)) {
    throw "Build output $BuildDir not found."
}

if (Test-Path $StagingDir) {
    Remove-Item -LiteralPath $StagingDir -Recurse -Force
}
New-Item -ItemType Directory -Path $StagingDir -Force | Out-Null
Copy-Item -Path (Join-Path $BuildDir "*") -Destination $StagingDir -Recurse -Force

if ($EnableBackup -eq "true" -and (Test-Path $PublicDir)) {
    $BackupPath = Join-Path $BackupRoot $Timestamp
    New-Item -ItemType Directory -Path $BackupPath -Force | Out-Null
    Copy-Item -Path (Join-Path $PublicDir "*") -Destination $BackupPath -Recurse -Force
}

New-Item -ItemType Directory -Path $PublicDir -Force | Out-Null
Get-ChildItem -LiteralPath $PublicDir -Force | Remove-Item -Recurse -Force
Copy-Item -Path (Join-Path $StagingDir "*") -Destination $PublicDir -Recurse -Force

if ($ReloadCommand) {
    Invoke-Expression $ReloadCommand
}

Write-Host "Deploy completed successfully."
