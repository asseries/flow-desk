#ifndef RuntimeDir
  #define RuntimeDir "..\dist"
#endif
#ifndef SetupOutputDir
  #define SetupOutputDir "..\dist\installer"
#endif
[Setup]
AppId={{D61E8430-DFE1-48EC-B310-B359B96D9182}
AppName=Flow Desk
AppVersion=0.2.0
AppPublisher=asseries
AppPublisherURL=https://github.com/asseries/flow-desk
AppSupportURL=https://github.com/asseries/flow-desk/issues
DefaultDirName={localappdata}\Programs\Flow Desk
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
UninstallDisplayIcon={app}\FlowDesk.exe
SetupIconFile={#RuntimeDir}\FlowDesk.ico
OutputDir={#SetupOutputDir}
OutputBaseFilename=FlowDesk-Setup-win-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no

[Tasks]
Name: desktopicon; Description: "Create a desktop shortcut"; Flags: unchecked
Name: startup; Description: "Start Flow Desk when I sign in to Windows"; Flags: unchecked

[Files]
Source: "{#RuntimeDir}\FlowDesk.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#RuntimeDir}\FlowDesk.DesktopWorker.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#RuntimeDir}\FlowDesk.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#RuntimeDir}\native\*"; DestDir: "{app}\native"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"
Source: "..\THIRD_PARTY_NOTICES.md"; DestDir: "{app}"

[Icons]
Name: "{userprograms}\Flow Desk"; Filename: "{app}\FlowDesk.exe"; WorkingDir: "{app}"
Name: "{userdesktop}\Flow Desk"; Filename: "{app}\FlowDesk.exe"; Tasks: desktopicon
Name: "{userstartup}\Flow Desk"; Filename: "{app}\FlowDesk.exe"; Parameters: "--background"; WorkingDir: "{app}"; Tasks: startup

[Run]
Filename: "{app}\FlowDesk.exe"; Description: "Launch Flow Desk"; Flags: nowait postinstall skipifsilent
