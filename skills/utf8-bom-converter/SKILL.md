---
name: utf8-bom-converter
description: Convert text files to UTF-8 with BOM using a reusable PowerShell script. Use when a task requires normalizing file encoding to UTF-8 BOM for one file or many files, including recursive folder conversion and safe handling of existing BOMs.
---

# UTF-8 BOM Converter

Use `scripts/convert-to-utf8-bom.ps1` to convert files to UTF-8 with BOM.

## Primary Command

```powershell
pwsh -File 'skills/utf8-bom-converter/scripts/convert-to-utf8-bom.ps1' -Path '<file-or-folder>'
```

## Common Usage

Convert a single file:

```powershell
pwsh -File 'skills/utf8-bom-converter/scripts/convert-to-utf8-bom.ps1' -Path 'app/src/LibraryService.Application/Status/GetStatusResponseDto.cs'
```

Convert all files in a folder recursively:

```powershell
pwsh -File 'skills/utf8-bom-converter/scripts/convert-to-utf8-bom.ps1' -Path 'app/src' -Recurse
```

Convert only selected patterns:

```powershell
pwsh -File 'skills/utf8-bom-converter/scripts/convert-to-utf8-bom.ps1' -Path 'app/src' -Recurse -Include '*.cs','*.md','*.json'
```

Use a fallback source encoding when files have no BOM and are not valid UTF-8:

```powershell
pwsh -File 'skills/utf8-bom-converter/scripts/convert-to-utf8-bom.ps1' -Path 'legacy' -Recurse -AssumeEncoding 'windows-1251'
```

## Behavior Rules

- Skip files already encoded as UTF-8 with BOM unless `-Force` is provided.
- Auto-detect and convert UTF-16/UTF-32 files based on BOM.
- For files without BOM:
  - Treat as UTF-8 if byte sequence is valid UTF-8.
  - Require `-AssumeEncoding` if byte sequence is not valid UTF-8.
- Preserve file content and line endings.

## Safety

- Use `-WhatIf` to preview changes.
- Use `-Verbose` for per-file details.
- Start with narrow `-Include` patterns when converting large trees.
