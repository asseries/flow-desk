param([string]$Compiler, [string]$RuntimeDirectory = "$PSScriptRoot\dist", [string]$OutputDirectory = "$PSScriptRoot\dist\installer")
$ErrorActionPreference = 'Stop'
if (!$Compiler) {
    $Compiler = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
}
if (!(Test-Path -LiteralPath $Compiler)) { throw 'Install Inno Setup 6 or pass -Compiler with the path to ISCC.exe.' }
& "$PSScriptRoot\build.ps1" -OutputDirectory $RuntimeDirectory
& $Compiler "/DRuntimeDir=$RuntimeDirectory" "/DSetupOutputDir=$OutputDirectory" "$PSScriptRoot\installer\FlowDesk.iss"
if ($LASTEXITCODE -ne 0) { throw 'Installer build failed' }
$exe = Join-Path $OutputDirectory 'FlowDesk-Setup-win-x64.exe'
$hash = (Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash.ToLowerInvariant()
[IO.File]::WriteAllText("$exe.sha256", "$hash  FlowDesk-Setup-win-x64.exe`n")
Write-Host "Built $exe"
