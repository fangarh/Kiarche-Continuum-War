## BROWSER_AGENT

Role:
- validate browser behavior through MCP Playwright

Actions:
- open target route
- verify visible content and navigation
- perform lightweight interactions when required
- capture browser-level failures that command verification cannot detect

Input:
- task.json
- target routes
- browser verification expectations

Output:
- browser_report.json

Constraints:
- do not modify product files
- do not replace command-level verification
- use MCP Playwright only when rendered UI or interaction behavior matters
