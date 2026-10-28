# Vendored VirtualDesktopAccessor

All runtime files are local. Flow Desk performs no download or online loading.

- `bin/VirtualDesktopAccessor.dll`: upstream Windows x64 release `2024-12-16-windows11`.
- `release-source/VirtualDesktopAccessor-aac8fc6e33178ae74eaaa55c0c5d88f73c7dea8b/`: source matching that binary release.
- `source/VirtualDesktopAccessor-ceb648849dcc5760a7f4ed223308ffe3de0b51c6/`: pinned current source retained for future maintenance.
- `LICENSE.txt`: upstream MIT license. Copyright notice retained.
- `VENDOR.json`: exact commits, binary hash and tested Windows build.

The worker verifies the DLL SHA-256 before loading it by absolute path with restricted dependency search. It exposes only probe and move operations. It does not expose desktop creation/deletion, desktop switching, pinning or application termination.

Runtime layout: `FlowDesk.exe`, `FlowDesk.DesktopWorker.exe`, `native/VirtualDesktopAccessor.dll`, plus license/provenance in `native/`.

The worker runs separately because this is an undocumented Windows interface. It checks desktop count, process session, HWND/PID/process creation time and public-API desktop GUID consistency before moving. Detach restoration resolves the original desktop GUID instead of trusting an old desktop index after reorder. Removing the original desktop makes restoration fail explicitly; the app window remains usable and a local recovery log records that result.

On Windows 26H2 build 26300.9711 the upstream DLL can remain alive during COM shutdown after returning a completed response. The client therefore waits briefly and then terminates only its own helper process. It never terminates Explorer or a managed application. A missing/incompatible DLL or helper timeout does not load native code into the Flow Desk UI process.

Verified locally: four existing desktops, actual separate-process test window moved to another desktop, imported through the public-API fallback, attached, detached and restored to its original desktop GUID. No desktop switching was requested. This does not guarantee compatibility after a Windows update or with every protected/elevated app.
