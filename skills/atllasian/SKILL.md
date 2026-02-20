---
name: atlassian-rest-direct
description: Direct Jira, Confluence, and Bitbucket REST API access using environment-based authentication and reusable PowerShell tooling. Use when tasks require querying or updating Jira issues, searching Jira, reading or writing Confluence content, or reading and updating Bitbucket repositories and pull requests over REST with JIRA_*, CONFLUENCE_*, and BITBUCKET_* environment variables.
---

# Atlassian REST Direct

Use `invoke-atlassian-rest.ps1` for all Jira, Confluence, and Bitbucket REST calls.
Do not build raw auth logic in ad-hoc commands.

## Required Environment Variables

Set variables for the selected product only.

Jira:
- `JIRA_URL`
- `JIRA_USERNAME`
- `JIRA_PERSONAL_TOKEN`
- `JIRA_SSL_VERIFY`

Confluence:
- `CONFLUENCE_URL`
- `CONFLUENCE_USERNAME`
- `CONFLUENCE_PERSONAL_TOKEN`
- `CONFLUENCE_SSL_VERIFY`

Bitbucket:
- `BITBUCKET_URL`
- `BITBUCKET_USERNAME`
- `BITBUCKET_PERSONAL_TOKEN`
- `BITBUCKET_SSL_VERIFY`

Never print token values.

## Script Usage

```powershell
pwsh -File 'invoke-atlassian-rest.ps1' -Product <jira|confluence|bitbucket> -Path '<rest-path>' -Method GET -OutputJson
```

Use `-BodyJson` for `POST`, `PUT`, or `PATCH`.

## Common Examples

Jira issue lookup:

```powershell
pwsh -File 'invoke-atlassian-rest.ps1' -Product jira -Path 'rest/api/2/issue/UTK2-3108?fields=summary,status,issuetype,priority,assignee,reporter,created,updated,description' -OutputJson
```

Confluence page lookup by ID:

```powershell
pwsh -File 'invoke-atlassian-rest.ps1' -Product confluence -Path 'wiki/rest/api/content/123456?expand=body.storage,version,space' -OutputJson
```

Bitbucket Cloud repository lookup:

```powershell
pwsh -File 'invoke-atlassian-rest.ps1' -Product bitbucket -Path '2.0/repositories/my-workspace/my-repo' -OutputJson
```

Bitbucket pull requests list:

```powershell
pwsh -File 'invoke-atlassian-rest.ps1' -Product bitbucket -Path '2.0/repositories/my-workspace/my-repo/pullrequests?state=OPEN' -OutputJson
```

## Error Handling

- DNS/VPN issue: message contains `DNS resolution failed for '<host>'`. Tell user to connect to VPN and retry.
- `401`: credentials or token are invalid/expired for the selected product.
- `404`: resource path or identifier is invalid.

## Safety Rules

- Default to `GET`.
- Do not include unnecessary fields when reading resources.
- Do not log or echo credentials.

## Jira PR Object Search Script

Use `query-jira-pr-object-changes.ps1` to:
- read PR links from Jira custom field `Ссылки на ПР-ы`,
- fetch PR changed files,
- find objects from a list in PR paths and optional diff content.

Examples:

```powershell
# Inline object list
pwsh -File 'query-jira-pr-object-changes.ps1' -IssueKey 'DEMO-8952' -Objects 'DeleteScheduleTemplateHandler','GetScheduleTemplateHandler' -OutputJson

# Objects from file (one object name per line)
pwsh -File 'query-jira-pr-object-changes.ps1' -IssueKey 'DEMO-8952' -ObjectsFile '.\objects.txt' -OutputJson

# Fallback when Jira does not expose PR links
pwsh -File 'query-jira-pr-object-changes.ps1' -IssueKey 'DEMO-8952' -Objects 'DeleteScheduleTemplateHandler' -PullRequestUrl 'http://localhost:8080/projects/DEMO/repos/demo-service/pull-requests/1' -OutputJson

# Use a different Jira field name if needed
pwsh -File 'query-jira-pr-object-changes.ps1' -IssueKey 'DEMO-8952' -Objects 'DeleteScheduleTemplateHandler' -PullRequestFieldName 'PR links' -OutputJson

# Include diff line search (slower, may be truncated by server)
pwsh -File 'query-jira-pr-object-changes.ps1' -IssueKey 'DEMO-8952' -Objects 'DeleteScheduleTemplateHandler' -SearchDiffContent -OutputJson
```
