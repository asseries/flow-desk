# Security

Flow Desk is an early native Windows integration prototype. It runs with the current user's privileges and controls only explicitly selected windows. Elevated/protected applications may reject management.

The experimental virtual-desktop adapter uses undocumented Windows interfaces in an isolated helper process. It validates the bundled DLL hash and window/process identity. Windows updates can break compatibility. Recovery cannot guarantee restoration after power loss, removal of a desktop or destruction of an embedded HWND.

Please report vulnerabilities privately through the repository's Security tab when available. Do not post credentials, private window titles, personal screenshots or document contents in a public issue. Avoid testing experimental embedding with valuable unsaved work.
