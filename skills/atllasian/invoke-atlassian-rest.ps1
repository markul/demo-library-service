[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("jira", "confluence", "bitbucket")]
    [string]$Product,

    [Parameter(Mandatory = $true)]
    [string]$Path,

    [ValidateSet("GET", "POST", "PUT", "PATCH", "DELETE")]
    [string]$Method = "GET",

    [string]$BodyJson,

    [switch]$OutputJson
)

$ErrorActionPreference = "Stop"

function Get-ProductConfig {
    param([string]$Target)

    if ($Target -eq "jira") {
        return @{
            Url = $env:JIRA_URL
            Username = $env:JIRA_USERNAME
            Token = $env:JIRA_PERSONAL_TOKEN
            SslVerify = $env:JIRA_SSL_VERIFY
        }
    }

    if ($Target -eq "bitbucket") {
        return @{
            Url = $env:BITBUCKET_URL
            Username = $env:BITBUCKET_USERNAME
            Token = $env:BITBUCKET_PERSONAL_TOKEN
            SslVerify = $env:BITBUCKET_SSL_VERIFY
        }
    }

    return @{
        Url = $env:CONFLUENCE_URL
        Username = $env:CONFLUENCE_USERNAME
        Token = $env:CONFLUENCE_PERSONAL_TOKEN
        SslVerify = $env:CONFLUENCE_SSL_VERIFY
    }
}

function Get-AuthHeaders {
    param(
        [string]$Username,
        [string]$Token
    )

    if ([string]::IsNullOrWhiteSpace($Token)) {
        throw "Personal token is not set for the selected product."
    }

    $authHeaders = New-Object System.Collections.Generic.List[string]
    $authHeaders.Add("Bearer $Token")

    if (-not [string]::IsNullOrWhiteSpace($Username)) {
        $pair = "{0}:{1}" -f $Username, $Token
        $encoded = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes($pair))
        $authHeaders.Add("Basic $encoded")
    }

    return ,$authHeaders.ToArray()
}

function Invoke-WithAuthFallback {
    param(
        [hashtable]$BaseParams,
        [hashtable]$Headers,
        [string[]]$AuthHeaders
    )

    for ($i = 0; $i -lt $AuthHeaders.Count; $i++) {
        $Headers.Authorization = $AuthHeaders[$i]

        try {
            return Invoke-RestMethod @BaseParams
        }
        catch {
            $statusCode = $null

            if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
                $statusCode = [int]$_.Exception.Response.StatusCode
            }

            $hasFallback = $i -lt ($AuthHeaders.Count - 1)
            if ($statusCode -eq 401 -and $hasFallback) {
                continue
            }

            throw
        }
    }

    throw "Authentication failed for all configured authorization methods."
}

function Assert-DnsResolution {
    param([string]$BaseUrl)

    $hostName = $null

    try {
        $uri = [Uri]$BaseUrl
        $hostName = $uri.Host
    }
    catch {
        throw "Base URL '$BaseUrl' is invalid."
    }

    if ([string]::IsNullOrWhiteSpace($hostName)) {
        throw "Base URL '$BaseUrl' does not contain a valid host name."
    }

    try {
        $addresses = [System.Net.Dns]::GetHostAddresses($hostName)
    }
    catch {
        throw "DNS resolution failed for '$hostName'. This usually means the company VPN is not working. Connect to VPN and retry."
    }

    if (-not $addresses -or $addresses.Count -eq 0) {
        throw "DNS resolution failed for '$hostName'. This usually means the company VPN is not working. Connect to VPN and retry."
    }
}

$config = Get-ProductConfig -Target $Product

if ([string]::IsNullOrWhiteSpace($config.Url)) {
    throw "Base URL is not set for the selected product."
}

$trimmedBase = $config.Url.TrimEnd("/")
Assert-DnsResolution -BaseUrl $trimmedBase
$trimmedPath = $Path.TrimStart("/")
$uri = "$trimmedBase/$trimmedPath"

$headers = @{
    Accept = "application/json"
}
$authHeaders = Get-AuthHeaders -Username $config.Username -Token $config.Token

$invokeParams = @{
    Uri = $uri
    Method = $Method
    Headers = $headers
}

if (-not [string]::IsNullOrWhiteSpace($BodyJson)) {
    $invokeParams.ContentType = "application/json"
    $invokeParams.Body = $BodyJson
}

if ($config.SslVerify -match "^(?i:false|0|no)$") {
    $invokeParams.SkipCertificateCheck = $true
}

$response = Invoke-WithAuthFallback -BaseParams $invokeParams -Headers $headers -AuthHeaders $authHeaders

if ($OutputJson) {
    $response | ConvertTo-Json -Depth 20 -Compress
}
else {
    $response
}
