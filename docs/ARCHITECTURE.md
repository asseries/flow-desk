# Architecture and technical decisions

`src/FlowDesk.cs` contains the native adapter, attachment lifecycle, XML session models, window picker, panels, and multi-window application context. `WorkspaceModel → PanelModel → TabModel` is independent of HWNDs. For nested splits, replace the fixed two-element panel array with a split/leaf tree; the ManagedWindow adapter need not change. Nested layout is not implemented.

## Two distinct hosting strategies

**Aligned (default):** keeps WS_CHILD unset, retains application window styles, temporarily sets its owner to the workspace, and moves/resizes the top-level window to panel screen coordinates. Inactive tabs are hidden. Window ownership provides normal minimize/restore stacking behavior. Coordinates are refreshed on move, resize, DPI messages, tab selection and a 500 ms reconciliation timer. The timer uses non-activating show/position calls; focus changes are explicit on tab selection. This preserves menus and modal relationships substantially better than arbitrary reparenting, but a top-level window is not clipped to the panel. Applications may enforce minimum sizes, overlap a neighbor, fight positioning, or create independent windows. It is not equivalent to a native child.

**Embedded (experimental):** capture original state, verify matching window DPI contexts, clear popup/caption/thick frame styles, set WS_CHILD, SetParent to the actual tab-page HWND, verify parent, and synchronize UI state. Native input is delivered to the real external window. Cross-process activation uses foreground requests without joining input queues. Some embedded controls may require a direct click to focus their editor. Matching DPI contexts avoid the documented cross-process DPI-awareness reset risk. DPI mismatch is rejected with an actionable reason, not forced. Matching DPI does not prove that an application supports reparenting: Chromium, Java/Swing, Qt, app-specific menus, accelerators and modal ownership still require individual validation. Moving embedded tabs across panels is intentionally rejected; detach/re-add instead.

References: [SetParent](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setparent), [SetWindowLongPtr](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptrw), [ShowWindow](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow), [SetWindowPos](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos).

## Attachment transaction and restoration

Capture parent, owner, style, extended style, WINDOWPLACEMENT, HWND, PID and process creation time. Claim the HWND using a named window property so two workspaces or Flow Desk instances cannot manage it concurrently. Write an atomic journal, start an independent guardian process, and require its ready acknowledgement **before** mutating the external window. On failure, roll back. On detach/normal workspace close, restore state and remove the claim. A normal application-close request detaches first and posts WM_CLOSE; it never terminates the foreign process.

Before every operation, validate IsWindow, PID, creation time and claim. This prevents most stale/reused HWND operations. A process restart requires explicit re-selection. No Windows API sequence can make a check/use race completely atomic across unrelated processes.

The guardian waits for the host process to exit and restores a still-valid claimed window. The tested Aligned crash case retained both 1C and XAMPP windows. This is best effort, **not a crash-safety guarantee for Embedded**: Windows or the foreign application may destroy a child HWND before the guardian can recover it. Power loss, logoff, watchdog termination, process-wide job termination, invalidated HWNDs or integrity restrictions can prevent recovery. There is no startup replay against historical HWNDs. Recovery does not restart applications or restore unsaved documents.

## Privileges, DPI, dialogs

Runs asInvoker. UIPI may prevent setting properties, ownership, styles or focus for elevated apps. Failed claims/ownership produce errors; always running Flow Desk as administrator is not the solution. Installed XAMPP reported elevated execution during the initial test, so results on this desktop do not certify behavior at every integrity level.

The executable declares PerMonitorV2. Aligned uses physical screen coordinates from the native host controls. Multi-monitor mixed-scaling behavior still needs hardware verification. No claim of tested mixed-DPI compatibility is made.

Explicit focus prefers a visible direct owned popup of the selected window. Flow Desk-owned modal dialogs temporarily hide managed content so its picker/rename UI remains accessible. Deep ownership chains, app-created unowned dialogs, menus, modal loops and application-defined minimum sizes are not comprehensively solved. No global keyboard hook, injected DLL, forced process termination, application content extraction or login automation is used.

Session writes use same-directory temp + atomic replacement. Matching requires exact path and title and exactly one candidate. Title changes or multiple matching windows require user re-selection. Window titles and executable paths are metadata that may still contain identifying information; no credentials are deliberately collected. Unsaved workspaces are temporary; only explicit saves are exported. The local desktop helper and its vendored source/provenance are documented in `third_party/VirtualDesktopAccessor/README.flowdesk.md`.
