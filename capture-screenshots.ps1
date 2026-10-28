$ErrorActionPreference='Stop'
& "$PSScriptRoot\build.ps1" -OutputDirectory "$PSScriptRoot\dist\screenshot-build"
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$PSScriptRoot\dist\screenshot-build\CaptureScreenshots.exe" /win32manifest:"$PSScriptRoot\app.manifest" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:"$PSScriptRoot\dist\screenshot-build\FlowDesk.exe" "$PSScriptRoot\tests\CaptureScreenshots.cs"
if($LASTEXITCODE -ne 0){throw 'Screenshot fixture build failed'}
& "$PSScriptRoot\dist\screenshot-build\CaptureScreenshots.exe" --output-root "$PSScriptRoot"
if($LASTEXITCODE -ne 0){throw 'Screenshot capture failed'}
