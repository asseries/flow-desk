param([switch]$ModelOnly)
$ErrorActionPreference='Stop'
$root=$PSScriptRoot
New-Item -ItemType Directory -Force -Path "$root\dist\native" | Out-Null
Copy-Item "$root\third_party\VirtualDesktopAccessor\bin\VirtualDesktopAccessor.dll" "$root\dist\native\VirtualDesktopAccessor.dll" -Force
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\FlowDesk.DesktopWorker.exe" "$root\src\DesktopWorker.cs"
if($LASTEXITCODE -ne 0){throw 'Desktop worker test build failed'}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\FlowDesk.Tests.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:Microsoft.CSharp.dll "$root\src\FlowDesk.cs" "$root\src\Launching.cs" "$root\src\DesktopBridge.cs"
if($LASTEXITCODE -ne 0){throw 'Test build failed'}
& "$root\dist\FlowDesk.Tests.exe" --self-test
if($LASTEXITCODE -ne 0){throw 'Tests failed'}
Get-Content "$root\dist\test-results.txt"
if($ModelOnly){exit 0}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\MenuLifecycle.Tests.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:"$root\dist\FlowDesk.Tests.exe" "$root\tests\MenuLifecycle.cs"
if($LASTEXITCODE -ne 0){throw 'Menu regression test build failed'}
& "$root\dist\MenuLifecycle.Tests.exe"
if($LASTEXITCODE -ne 0){throw 'Menu regression test failed'}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\CompactHeader.Tests.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:"$root\dist\FlowDesk.Tests.exe" "$root\tests\CompactHeader.cs"
if($LASTEXITCODE -ne 0){throw 'Header regression test build failed'}
& "$root\dist\CompactHeader.Tests.exe"
if($LASTEXITCODE -ne 0){throw 'Header regression test failed'}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\WindowSwitch.Tests.exe" /win32manifest:"$root\app.manifest" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:"$root\dist\FlowDesk.Tests.exe" "$root\tests\WindowSwitch.cs"
if($LASTEXITCODE -ne 0){throw 'Switching regression test build failed'}
& "$root\dist\WindowSwitch.Tests.exe"
if($LASTEXITCODE -ne 0){throw 'Switching regression test failed'}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\Startup.Tests.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:"$root\dist\FlowDesk.Tests.exe" "$root\tests\Startup.cs"
if($LASTEXITCODE -ne 0){throw 'Startup regression test build failed'}
& "$root\dist\Startup.Tests.exe"
if($LASTEXITCODE -ne 0){throw 'Startup regression test failed'}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\Launcher.Tests.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:Microsoft.CSharp.dll /reference:"$root\dist\FlowDesk.Tests.exe" "$root\tests\Launcher.cs"
if($LASTEXITCODE -ne 0){throw 'Launcher regression test build failed'}
& "$root\dist\Launcher.Tests.exe"
if($LASTEXITCODE -ne 0){throw 'Launcher regression test failed'}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe /platform:x64 /out:"$root\dist\DesktopBridge.Tests.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:"$root\dist\FlowDesk.Tests.exe" "$root\tests\DesktopBridge.cs"
if($LASTEXITCODE -ne 0){throw 'Desktop move integration test build failed'}
& "$root\dist\DesktopBridge.Tests.exe"
if($LASTEXITCODE -ne 0){throw 'Desktop move integration test failed'}
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /platform:x64 /out:"$root\dist\NativeFixture.exe" /win32manifest:"$root\app.manifest" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll "$root\tests\NativeFixture.cs"
if($LASTEXITCODE -ne 0){throw 'Fixture build failed'}
