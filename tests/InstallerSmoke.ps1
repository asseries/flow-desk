param([string]$Setup = "$PSScriptRoot\..\dist\installer\FlowDesk-Setup-win-x64.exe")
$ErrorActionPreference = 'Stop'
# Run only on a disposable CI account: installs real per-user shortcuts and ARP entry.
if (!$env:GITHUB_ACTIONS) { throw 'This smoke test requires a disposable GitHub Actions runner.' }
$install = Join-Path $env:LOCALAPPDATA 'Programs\Flow Desk'
$startup = Join-Path ([Environment]::GetFolderPath('Startup')) 'Flow Desk.lnk'
$menu = Join-Path ([Environment]::GetFolderPath('Programs')) 'Flow Desk.lnk'
$p = Start-Process -FilePath $Setup -ArgumentList '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /TASKS="startup"' -Wait -PassThru
if ($p.ExitCode -ne 0) { throw "Install failed: $($p.ExitCode)" }
foreach ($file in @("$install\FlowDesk.exe", "$install\FlowDesk.DesktopWorker.exe", "$install\native\VirtualDesktopAccessor.dll", $startup, $menu)) {
    if (!(Test-Path -LiteralPath $file)) { throw "Missing installed file: $file" }
}
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($startup)
if ($shortcut.TargetPath -ne "$install\FlowDesk.exe" -or $shortcut.Arguments -ne '--background') { throw 'Wrong startup shortcut' }
$arp = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\{D61E8430-DFE1-48EC-B310-B359B96D9182}_is1'
if ((Get-ItemProperty $arp).DisplayName -ne 'Flow Desk') { throw 'Missing Installed apps registration' }
$p = Start-Process -FilePath "$install\FlowDesk.exe" -ArgumentList '--self-test' -Wait -PassThru
if ($p.ExitCode -ne 0) { throw 'Installed runtime self-test failed' }
$p = Start-Process -FilePath "$install\unins000.exe" -ArgumentList '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART' -Wait -PassThru
if ($p.ExitCode -ne 0) { throw 'Uninstall failed' }
foreach ($path in @($startup, $menu, $arp, "$install\FlowDesk.exe")) {
    if (Test-Path -LiteralPath $path) { throw "Uninstall left behind: $path" }
}
Write-Host 'Installer, startup shortcut, Installed apps, runtime and uninstall checks passed.'
