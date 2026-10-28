# Verification

Recorded on 2026-10-28. The local compatibility environment was Windows 11 x64 26H2 build 26300.9711, with .NET Framework 4.8. These results describe that environment, not a universal certification.

## Application coverage

| Application | Coverage | Remaining limitations |
|---|---|---|
| XAMPP Control Panel 3.2.4 | Live Aligned attachment, panel resize and Aligned crash restoration exercised. | Embedded attempt rejected for DPI mismatch; controlled integrity-level matrix not completed. |
| 1C 8.3 Configurator | Live second-panel attachment, reconnect matching, normal detach and Aligned crash restoration exercised. | No database/configuration edits; full input/dialog coverage incomplete. |
| Telegram Desktop | User exercised Aligned attachment alongside another app and normal detach. | No messages/calls/authentication automated; minimum-width behavior may constrain panels. |
| Android Studio | User exercised project-window tabs, split panels and full-screen layout; screenshots show actual use. | Java/Swing popup, mixed-DPI and extensive shortcut behavior not comprehensively certified. |
| File Explorer | User supplied a live split-panel screenshot. | Broader shell-window lifecycle coverage incomplete. |
| pgAdmin | Installation/discovery checked during development. | Live attachment not systematically exercised. |
| ChatGPT desktop | Window discovery available. | No automated application-UI compatibility test performed. |
| Disposable native fixtures | Automated native-window, shortcut-launch and cross-process desktop-move scenarios. | Do not substitute for application-specific testing. |

## Automated suite

`test.ps1 -ModelOnly` runs headless layout/session checks. Full `test.ps1` additionally requires an interactive Windows session and at least two existing virtual desktops.

The suite passed locally:

- Ratio bounds, invalid ratio fallback, tab metadata/layout XML round-trip, atomic file replacement and exclusion of runtime HWND from persistent layout identity.
- Menu click/close/reopen lifecycle and owner cleanup.
- Compact 24 px header geometry, runtime metric changes, exclusion of hidden native-header hit regions, dark empty-state rendering, drag mouse routing, full-screen geometry and normal-bounds restoration.
- Native hide/show at prepared bounds and stable minimum-size reconciliation.
- Single saved-workspace startup, duplicate suppression, temporary-layout exclusion and Windows minimize/restore/maximize/close commands.
- Uncloak event handling that repairs introduced stale frame styles without a tab click.
- Start menu/packaged-app catalog enumeration, conservative matching and actual disposable shortcut process/window discovery.
- Local native DLL probing, separate-process virtual-desktop import, attachment, detach and original-desktop GUID restoration without switching the current desktop.

## Manual scenarios still needed

- Exhaustive keyboard, clipboard, scrolling, menus, file dialogs, nested/unowned modal dialogs and application restart behavior for each target app.
- Multiple physical monitors with mixed scaling, touchpad task-switch/virtual-desktop gestures and repeated sleep/resume/shell restart cycles.
- Differential integrity/elevation and protected application behavior.
- Positive native-child embedding for the named third-party applications and embedding-specific crash failure cases.

## Practical limits

Aligned windows retain native title bars and application-specific minimum dimensions. They are not perfectly clipped child windows. Embedded mode is experimental and may lose its HWND when the host fails; the guardian cannot recover a destroyed native window or unsaved document. Full-screen edge correction depends on valid DWM visible-frame bounds. Workspace shortcuts require Flow Desk focus. Native desktop compatibility may change after Windows updates.

README screenshots were supplied by the project owner and approved for public use. Automated tests use disposable content; private sessions and journals are not part of the repository.
