[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string[]]$Path,

    [Parameter()]
    [string[]]$Include = @("*"),

    [Parameter()]
    [switch]$Recurse,

    [Parameter()]
    [switch]$Force,

    [Parameter()]
    [string]$AssumeEncoding
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-BomEncoding {
    param(
        [byte[]]$Bytes
    )

    if ($Bytes.Length -ge 3 -and $Bytes[0] -eq 0xEF -and $Bytes[1] -eq 0xBB -and $Bytes[2] -eq 0xBF) { return "utf8bom" }
    if ($Bytes.Length -ge 4 -and $Bytes[0] -eq 0xFF -and $Bytes[1] -eq 0xFE -and $Bytes[2] -eq 0x00 -and $Bytes[3] -eq 0x00) { return "utf32le" }
    if ($Bytes.Length -ge 4 -and $Bytes[0] -eq 0x00 -and $Bytes[1] -eq 0x00 -and $Bytes[2] -eq 0xFE -and $Bytes[3] -eq 0xFF) { return "utf32be" }
    if ($Bytes.Length -ge 2 -and $Bytes[0] -eq 0xFF -and $Bytes[1] -eq 0xFE) { return "utf16le" }
    if ($Bytes.Length -ge 2 -and $Bytes[0] -eq 0xFE -and $Bytes[1] -eq 0xFF) { return "utf16be" }
    return $null
}

function Get-StrictUtf8StringOrNull {
    param(
        [byte[]]$Bytes
    )

    $strictUtf8 = [System.Text.UTF8Encoding]::new($false, $true)
    try {
        return $strictUtf8.GetString($Bytes)
    }
    catch {
        return $null
    }
}

function Convert-BytesToUtf8Bom {
    param(
        [byte[]]$Bytes,
        [string]$AssumeEncodingName
    )

    $utf8Bom = [System.Text.UTF8Encoding]::new($true)
    function New-Utf8BomBytesFromString {
        param([string]$Text)
        [byte[]]($utf8Bom.GetPreamble() + $utf8Bom.GetBytes($Text))
    }
    $bomKind = Get-BomEncoding -Bytes $Bytes

    switch ($bomKind) {
        "utf8bom" {
            return [PSCustomObject]@{
                AlreadyUtf8Bom = $true
                Utf8Bytes       = $Bytes
                SourceEncoding  = "utf8bom"
            }
        }
        "utf16le" {
            $content = [System.Text.Encoding]::Unicode.GetString($Bytes, 2, $Bytes.Length - 2)
            return [PSCustomObject]@{
                AlreadyUtf8Bom = $false
                Utf8Bytes       = New-Utf8BomBytesFromString -Text $content
                SourceEncoding  = "utf16le"
            }
        }
        "utf16be" {
            $enc = [System.Text.Encoding]::BigEndianUnicode
            $content = $enc.GetString($Bytes, 2, $Bytes.Length - 2)
            return [PSCustomObject]@{
                AlreadyUtf8Bom = $false
                Utf8Bytes       = New-Utf8BomBytesFromString -Text $content
                SourceEncoding  = "utf16be"
            }
        }
        "utf32le" {
            $content = [System.Text.Encoding]::UTF32.GetString($Bytes, 4, $Bytes.Length - 4)
            return [PSCustomObject]@{
                AlreadyUtf8Bom = $false
                Utf8Bytes       = New-Utf8BomBytesFromString -Text $content
                SourceEncoding  = "utf32le"
            }
        }
        "utf32be" {
            $enc = [System.Text.Encoding]::GetEncoding(
                "utf-32BE",
                [System.Text.EncoderExceptionFallback]::new(),
                [System.Text.DecoderExceptionFallback]::new()
            )
            $content = $enc.GetString($Bytes, 4, $Bytes.Length - 4)
            return [PSCustomObject]@{
                AlreadyUtf8Bom = $false
                Utf8Bytes       = New-Utf8BomBytesFromString -Text $content
                SourceEncoding  = "utf32be"
            }
        }
        default {
            $utf8Content = Get-StrictUtf8StringOrNull -Bytes $Bytes
            if ($null -ne $utf8Content) {
                return [PSCustomObject]@{
                    AlreadyUtf8Bom = $false
                    Utf8Bytes       = New-Utf8BomBytesFromString -Text $utf8Content
                    SourceEncoding  = "utf8"
                }
            }

            if ([string]::IsNullOrWhiteSpace($AssumeEncodingName)) {
                throw "File has no BOM and is not valid UTF-8. Specify -AssumeEncoding (example: windows-1251)."
            }

            $fallbackEncoding = [System.Text.Encoding]::GetEncoding(
                $AssumeEncodingName,
                [System.Text.EncoderExceptionFallback]::new(),
                [System.Text.DecoderExceptionFallback]::new()
            )
            $content = $fallbackEncoding.GetString($Bytes)
            return [PSCustomObject]@{
                AlreadyUtf8Bom = $false
                Utf8Bytes       = New-Utf8BomBytesFromString -Text $content
                SourceEncoding  = $AssumeEncodingName
            }
        }
    }
}

function Resolve-TargetFiles {
    param(
        [string[]]$InputPath,
        [string[]]$FilePatterns,
        [switch]$Recursive
    )

    $result = New-Object System.Collections.Generic.List[string]
    foreach ($entry in $InputPath) {
        if (-not (Test-Path -LiteralPath $entry)) {
            throw "Path not found: $entry"
        }

        $item = Get-Item -LiteralPath $entry
        if ($item.PSIsContainer) {
            $files = Get-ChildItem -LiteralPath $item.FullName -File -Recurse:$Recursive
            foreach ($file in $files) {
                foreach ($pattern in $FilePatterns) {
                    if ($file.Name -like $pattern) {
                        $result.Add($file.FullName)
                        break
                    }
                }
            }
        }
        else {
            $result.Add($item.FullName)
        }
    }

    return $result | Sort-Object -Unique
}

$filesToProcess = Resolve-TargetFiles -InputPath $Path -FilePatterns $Include -Recursive:$Recurse

$summary = [PSCustomObject]@{
    Processed = 0
    Converted = 0
    Skipped   = 0
    Failed    = 0
}

foreach ($filePath in $filesToProcess) {
    $summary.Processed++
    try {
        $bytes = [System.IO.File]::ReadAllBytes($filePath)
        $conversion = Convert-BytesToUtf8Bom -Bytes $bytes -AssumeEncodingName $AssumeEncoding

        if ($conversion.AlreadyUtf8Bom -and -not $Force) {
            $summary.Skipped++
            Write-Verbose "Skip (already UTF-8 BOM): $filePath"
            continue
        }

        if ($PSCmdlet.ShouldProcess($filePath, "Convert to UTF-8 BOM (source: $($conversion.SourceEncoding))")) {
            [System.IO.File]::WriteAllBytes($filePath, $conversion.Utf8Bytes)
            $summary.Converted++
            Write-Verbose "Converted ($($conversion.SourceEncoding) -> utf8bom): $filePath"
        }
    }
    catch {
        $summary.Failed++
        Write-Error "Failed: $filePath. $($_.Exception.Message)"
    }
}

$summary
