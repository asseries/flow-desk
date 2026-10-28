# Roadmap

The current release is a functional Windows prototype. Priorities:

- Expand per-application input, clipboard, popup/modal and lifecycle testing.
- Verify multi-monitor mixed-DPI, sleep/resume, shell restart and hardware touchpad scenarios.
- Improve application-specific minimum-size handling and compatibility feedback.
- Add complete localization resources; current product UI is English.
- Persist workspace screen bounds and add optional per-app hosting preferences.
- Extend the layout model to nested split nodes and evaluate safe Embedded tab transfers.
- Exercise guardian/desktop restoration failures, including removed desktops and invalidated HWNDs.
- Maintain the vendored desktop adapter against Windows interface changes.

Native-child embedding remains experimental. Passing regression tests is not a promise that every external application can be embedded or recovered.
