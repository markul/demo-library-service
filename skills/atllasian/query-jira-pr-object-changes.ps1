[CmdletBinding(DefaultParameterSetName = "InlineObjects")]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern("^[A-Z][A-Z0-9]*-[0-9]+$")]
    [string]$IssueKey,

    [Parameter(Mandatory = $true, ParameterSetName = "InlineObjects")]
    [string[]]$Objects,

    [Parameter(Mandatory = $true, ParameterSetName = "ObjectsFile")]
    [string]$ObjectsFile,

    [string[]]$PullRequestUrl,

    [string]$PullRequestFieldName = "Ссылки на ПР-ы",

    [switch]$SearchDiffContent,

    [switch]$OutputJson
)

$ErrorActionPreference = "Stop"

$invokeScript = Join-Path $PSScriptRoot "invoke-atlassian-rest.ps1"
if (-not (Test-Path -LiteralPath $invokeScript)) {
    throw "Required script not found: $invokeScript"
}

$pullRequestUrlPattern = "(?i)https?://[^\s`"'<>\]]+/projects/(?<project>[^/\s]+)/repos/(?<repo>[^/\s]+)/pull-requests/(?<id>\d+)"

function Get-ObjectList {
    param(
        [string[]]$InlineValues,
        [string]$FilePath
    )

    $list = New-Object System.Collections.Generic.List[string]

    if (-not [string]::IsNullOrWhiteSpace($FilePath)) {
        if (-not (Test-Path -LiteralPath $FilePath)) {
            throw "Objects file not found: $FilePath"
        }

        Get-Content -LiteralPath $FilePath | ForEach-Object {
            $value = $_.Trim()
            if (-not [string]::IsNullOrWhiteSpace($value) -and -not $value.StartsWith("#")) {
                $list.Add($value)
            }
        }
    }
    else {
        foreach ($value in $InlineValues) {
            $trimmed = $value.Trim()
            if (-not [string]::IsNullOrWhiteSpace($trimmed)) {
                $list.Add($trimmed)
            }
        }
    }

    $seen = @{}
    $unique = New-Object System.Collections.Generic.List[string]

    foreach ($item in $list) {
        $key = $item.ToUpperInvariant()
        if (-not $seen.ContainsKey($key)) {
            $seen[$key] = $true
            $unique.Add($item)
        }
    }

    return ,$unique.ToArray()
}

function Invoke-AtlassianRaw {
    param(
        [string]$Product,
        [string]$Path,
        [string]$Method = "GET",
        [string]$BodyJson
    )

    $params = @{
        Product = $Product
        Path = $Path
        Method = $Method
        OutputJson = $true
    }

    if (-not [string]::IsNullOrWhiteSpace($BodyJson)) {
        $params.BodyJson = $BodyJson
    }

    return (& $invokeScript @params)
}

function Invoke-AtlassianJson {
    param(
        [string]$Product,
        [string]$Path,
        [string]$Method = "GET",
        [string]$BodyJson
    )

    $raw = Invoke-AtlassianRaw -Product $Product -Path $Path -Method $Method -BodyJson $BodyJson
    if ([string]::IsNullOrWhiteSpace($raw)) {
        return $null
    }

    return ($raw | ConvertFrom-Json)
}

function Get-JiraFieldByName {
    param([string]$FieldName)

    $fields = Invoke-AtlassianJson -Product jira -Path "rest/api/2/field"
    if ($null -eq $fields) {
        return $null
    }

    $exact = @($fields | Where-Object { $_.name -eq $FieldName })
    if ($exact.Count -gt 0) {
        return $exact[0]
    }

    return $null
}

function Convert-FieldValueToText {
    param([object]$Value)

    if ($null -eq $Value) {
        return $null
    }

    if ($Value -is [string]) {
        return [string]$Value
    }

    if ($Value -is [System.Collections.IEnumerable] -and -not ($Value -is [string])) {
        $parts = New-Object System.Collections.Generic.List[string]
        foreach ($item in $Value) {
            if ($null -eq $item) {
                continue
            }
            if ($item -is [string]) {
                $parts.Add([string]$item)
            }
            else {
                $parts.Add(($item | ConvertTo-Json -Compress -Depth 20))
            }
        }

        return ($parts -join "`n")
    }

    return ($Value | ConvertTo-Json -Compress -Depth 20)
}

function Add-PullRequestRefsFromText {
    param(
        [hashtable]$Map,
        [string]$Text,
        [string]$SourceName
    )

    if ([string]::IsNullOrWhiteSpace($Text)) {
        return
    }

    $matches = [regex]::Matches($Text, $pullRequestUrlPattern)
    foreach ($match in $matches) {
        $project = $match.Groups["project"].Value
        $repo = $match.Groups["repo"].Value
        $idValue = $match.Groups["id"].Value
        if ([string]::IsNullOrWhiteSpace($project) -or [string]::IsNullOrWhiteSpace($repo) -or [string]::IsNullOrWhiteSpace($idValue)) {
            continue
        }

        $prId = [int]$idValue
        $url = $match.Value.TrimEnd([char[]]@('.', ',', ';', ')', ']'))
        $key = "{0}/{1}/{2}" -f $project.ToUpperInvariant(), $repo.ToLowerInvariant(), $prId

        if (-not $Map.ContainsKey($key)) {
            $Map[$key] = [PSCustomObject]@{
                ProjectKey = $project
                RepoSlug = $repo
                PullRequestId = $prId
                Url = $url
                Sources = New-Object System.Collections.Generic.List[string]
            }
        }

        $existing = $Map[$key]
        if ([string]::IsNullOrWhiteSpace($existing.Url)) {
            $existing.Url = $url
        }
        if (-not $existing.Sources.Contains($SourceName)) {
            $existing.Sources.Add($SourceName)
        }
    }
}

function Get-PullRequestChanges {
    param(
        [string]$ProjectKey,
        [string]$RepoSlug,
        [int]$PullRequestId
    )

    $results = New-Object System.Collections.Generic.List[object]
    $start = 0
    $limit = 1000

    while ($true) {
        $path = "rest/api/latest/projects/$ProjectKey/repos/$RepoSlug/pull-requests/$PullRequestId/changes?limit=$limit&start=$start"
        $page = Invoke-AtlassianJson -Product bitbucket -Path $path

        foreach ($item in @($page.values)) {
            $pathValue = ""
            if ($item.path -and $item.path.toString) {
                $pathValue = [string]$item.path.toString
            }
            elseif ($item.srcPath -and $item.srcPath.toString) {
                $pathValue = [string]$item.srcPath.toString
            }

            $results.Add([PSCustomObject]@{
                    Path = $pathValue
                    ChangeType = [string]$item.type
                })
        }

        if ($page.isLastPage -or $null -eq $page.nextPageStart) {
            break
        }

        $start = [int]$page.nextPageStart
    }

    return ,$results.ToArray()
}

function Find-PathMatches {
    param(
        [string[]]$ObjectNames,
        [object[]]$Changes
    )

    $matches = New-Object System.Collections.Generic.List[object]

    foreach ($change in $Changes) {
        foreach ($name in $ObjectNames) {
            if ($change.Path.IndexOf($name, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                $matches.Add([PSCustomObject]@{
                        Object = $name
                        Path = $change.Path
                        ChangeType = $change.ChangeType
                    })
            }
        }
    }

    return ,$matches.ToArray()
}

function Find-DiffMatches {
    param(
        [string[]]$ObjectNames,
        [string]$ProjectKey,
        [string]$RepoSlug,
        [int]$PullRequestId
    )

    $matches = New-Object System.Collections.Generic.List[object]
    $diffPath = "rest/api/latest/projects/$ProjectKey/repos/$RepoSlug/pull-requests/$PullRequestId/diff?contextLines=1000"
    $diffData = Invoke-AtlassianJson -Product bitbucket -Path $diffPath

    foreach ($fileDiff in @($diffData.diffs)) {
        $pathValue = ""
        if ($fileDiff.destination -and $fileDiff.destination.toString) {
            $pathValue = [string]$fileDiff.destination.toString
        }
        elseif ($fileDiff.source -and $fileDiff.source.toString) {
            $pathValue = [string]$fileDiff.source.toString
        }

        foreach ($hunk in @($fileDiff.hunks)) {
            foreach ($segment in @($hunk.segments)) {
                foreach ($line in @($segment.lines)) {
                    $lineText = [string]$line.line
                    foreach ($name in $ObjectNames) {
                        if ($lineText.IndexOf($name, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                            $preview = $lineText
                            if ($preview.Length -gt 240) {
                                $preview = $preview.Substring(0, 240) + "..."
                            }

                            $matches.Add([PSCustomObject]@{
                                    Object = $name
                                    Path = $pathValue
                                    SegmentType = [string]$segment.type
                                    Line = $preview
                                })
                        }
                    }
                }
            }
        }
    }

    return [PSCustomObject]@{
        Truncated = [bool]$diffData.truncated
        Matches = @($matches.ToArray())
    }
}

$objectNames = Get-ObjectList -InlineValues $Objects -FilePath $ObjectsFile
if ($objectNames.Count -eq 0) {
    throw "No objects provided. Use -Objects or -ObjectsFile with at least one value."
}

$prField = Get-JiraFieldByName -FieldName $PullRequestFieldName
if ($null -eq $prField -or [string]::IsNullOrWhiteSpace([string]$prField.id)) {
    throw "Jira field '$PullRequestFieldName' was not found."
}

$prFieldId = [string]$prField.id

$issuePath = "rest/api/2/issue/${IssueKey}?fields=summary,$prFieldId"
$issueRaw = Invoke-AtlassianRaw -Product jira -Path $issuePath
$issue = $issueRaw | ConvertFrom-Json

if ([string]::IsNullOrWhiteSpace([string]$issue.id)) {
    throw "Could not resolve Jira issue id for '$IssueKey'."
}

$issueId = [string]$issue.id
$issueSummary = [string]$issue.fields.summary
$prMap = @{}

$prFieldProperty = $issue.fields.PSObject.Properties[$prFieldId]
$prFieldValue = $null
if ($null -ne $prFieldProperty) {
    $prFieldValue = $prFieldProperty.Value
}

$prFieldText = Convert-FieldValueToText -Value $prFieldValue
Add-PullRequestRefsFromText -Map $prMap -Text $prFieldText -SourceName "jira_field_$prFieldId"

foreach ($manualUrl in @($PullRequestUrl)) {
    Add-PullRequestRefsFromText -Map $prMap -Text $manualUrl -SourceName "manual_input"
}

$prRefs = @($prMap.Values | Sort-Object ProjectKey, RepoSlug, PullRequestId)
$prReports = New-Object System.Collections.Generic.List[object]

foreach ($pr in $prRefs) {
    $title = $null
    $state = $null
    $changes = @()
    $pathMatches = @()
    $diffMatches = @()
    $diffTruncated = $false
    $errors = New-Object System.Collections.Generic.List[string]

    try {
        $prMetaPath = "rest/api/latest/projects/$($pr.ProjectKey)/repos/$($pr.RepoSlug)/pull-requests/$($pr.PullRequestId)"
        $prMeta = Invoke-AtlassianJson -Product bitbucket -Path $prMetaPath
        $title = [string]$prMeta.title
        $state = [string]$prMeta.state
    }
    catch {
        $errors.Add("Failed to load PR metadata: $($_.Exception.Message)")
    }

    try {
        $changes = Get-PullRequestChanges -ProjectKey $pr.ProjectKey -RepoSlug $pr.RepoSlug -PullRequestId $pr.PullRequestId
        $pathMatches = Find-PathMatches -ObjectNames $objectNames -Changes $changes
    }
    catch {
        $errors.Add("Failed to load PR changes: $($_.Exception.Message)")
    }

    if ($SearchDiffContent) {
        try {
            $diffResult = Find-DiffMatches -ObjectNames $objectNames -ProjectKey $pr.ProjectKey -RepoSlug $pr.RepoSlug -PullRequestId $pr.PullRequestId
            $diffMatches = @($diffResult.Matches)
            $diffTruncated = [bool]$diffResult.Truncated
        }
        catch {
            $errors.Add("Failed to search PR diff content: $($_.Exception.Message)")
        }
    }

    $matchedObjects = @(
        @($pathMatches | ForEach-Object { $_.Object })
        @($diffMatches | ForEach-Object { $_.Object })
    ) | Sort-Object -Unique

    $prReports.Add([PSCustomObject]@{
            Url = $pr.Url
            ProjectKey = $pr.ProjectKey
            RepoSlug = $pr.RepoSlug
            PullRequestId = $pr.PullRequestId
            Title = $title
            State = $state
            Sources = @($pr.Sources)
            ChangedFiles = @($changes).Count
            PathMatches = @($pathMatches)
            DiffMatches = @($diffMatches)
            DiffTruncated = $diffTruncated
            MatchedObjects = @($matchedObjects)
            Errors = @($errors)
        })
}

[object[]]$prReportsArray = $prReports.ToArray()
$pullRequestsWithMatches = @($prReportsArray | Where-Object { @($_.MatchedObjects).Count -gt 0 }).Count
$pathMatchCountRaw = @($prReportsArray | ForEach-Object { @($_.PathMatches).Count } | Measure-Object -Sum).Sum
$diffMatchCountRaw = @($prReportsArray | ForEach-Object { @($_.DiffMatches).Count } | Measure-Object -Sum).Sum
$pathMatchCount = if ($null -eq $pathMatchCountRaw) { 0 } else { [int]$pathMatchCountRaw }
$diffMatchCount = if ($null -eq $diffMatchCountRaw) { 0 } else { [int]$diffMatchCountRaw }

$warningMessage = $null
if ($prReportsArray.Count -eq 0) {
    $warningMessage = "No pull requests were found in Jira field '$PullRequestFieldName'. If needed, pass PR URLs explicitly with -PullRequestUrl."
}

$result = [PSCustomObject]@{
    Issue = [PSCustomObject]@{
        Key = $IssueKey
        Id = $issueId
        Summary = $issueSummary
    }
    PullRequestField = [PSCustomObject]@{
        Name = $PullRequestFieldName
        Id = $prFieldId
        HasValue = -not [string]::IsNullOrWhiteSpace($prFieldText)
    }
    Objects = @($objectNames)
    PullRequestCount = $prReportsArray.Count
    PullRequests = $prReportsArray
    Totals = [PSCustomObject]@{
        PullRequestsWithMatches = $pullRequestsWithMatches
        PathMatchCount = $pathMatchCount
        DiffMatchCount = $diffMatchCount
    }
    Warning = $warningMessage
}

if ($OutputJson) {
    $result | ConvertTo-Json -Depth 20
}
else {
    $result
}
