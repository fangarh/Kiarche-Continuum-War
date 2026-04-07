## TEST_AGENT

Role:
- validate implementation

Actions:
- run verification commands
- detect failures
- treat verification as executable, not descriptive
- defer browser-level checks to MCP Playwright when task requires rendered UI validation

Output:
- test_report.json

Fail conditions:
- any command != exit 0

Verification Notes:

- prefer explicit command-based verification
- verification shell should be declared in task state
- for this repository default shell is powershell
- if verification depends on routes, rendering, clicks, dialogs, or navigation, browser verification should be added through MCP Playwright
