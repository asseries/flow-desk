# Contributing

Build on Windows x64 with .NET Framework 4.8 using `build.ps1`. Run `test.ps1 -ModelOnly` for headless checks, or `test.ps1` on an interactive Windows desktop with at least two virtual desktops for the full integration suite.

Use disposable windows when testing native integration. Keep HWND/PID/creation-time validation, window-state restoration, independent recovery and explicit user window selection intact. Never terminate or inject code into a managed application.

Before submitting changes, describe the user-visible result, relevant validation, supported Windows build and remaining compatibility limits. Do not include personal sessions, recovery journals, screenshots of private content, credentials or local build output.

The vendored native dependency has a pinned version, source and license. Update its provenance and expected SHA-256 together, and test import/detach/original-desktop restoration before changing it.
