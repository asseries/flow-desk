param([string]$OutputDirectory = "$PSScriptRoot\dist")
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
New-Item -ItemType Directory -Force -Path "$OutputDirectory\native" | Out-Null
Copy-Item -LiteralPath "$root\third_party\VirtualDesktopAccessor\bin\VirtualDesktopAccessor.dll" -Destination "$OutputDirectory\native\VirtualDesktopAccessor.dll" -Force
Copy-Item -LiteralPath "$root\third_party\VirtualDesktopAccessor\LICENSE.txt" -Destination "$OutputDirectory\native\VirtualDesktopAccessor.LICENSE.txt" -Force
Copy-Item -LiteralPath "$root\third_party\VirtualDesktopAccessor\VENDOR.json" -Destination "$OutputDirectory\native\VirtualDesktopAccessor.VENDOR.json" -Force
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$OutputDirectory\FlowDesk.DesktopWorker.exe" "$root\src\DesktopWorker.cs"
if ($LASTEXITCODE -ne 0) { throw 'Desktop helper build failed' }
Add-Type -AssemblyName System.Drawing
$bitmap = New-Object System.Drawing.Bitmap 64,64
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::FromArgb(35,43,65))
$brush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(93,151,255))
$graphics.FillRectangle($brush,10,12,18,40)
$graphics.FillRectangle($brush,33,12,21,18)
$graphics.FillRectangle($brush,33,35,21,17)
$icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
$iconFile = [System.IO.File]::Create("$OutputDirectory\FlowDesk.ico")
$icon.Save($iconFile)
$iconFile.Dispose(); $icon.Dispose(); $brush.Dispose(); $graphics.Dispose(); $bitmap.Dispose()
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /platform:x64 /out:"$OutputDirectory\FlowDesk.exe" /win32icon:"$OutputDirectory\FlowDesk.ico" /win32manifest:"$root\app.manifest" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Runtime.Serialization.dll /reference:Microsoft.CSharp.dll "$root\src\FlowDesk.cs" "$root\src\Launching.cs" "$root\src\DesktopBridge.cs"
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
Write-Host "Built $OutputDirectory\FlowDesk.exe"
