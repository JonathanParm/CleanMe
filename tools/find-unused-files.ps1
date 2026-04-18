# Scans repo for class declarations and finds other references.
# Run from repo root: pwsh .\tools\find-unused-files.ps1

$excludes = @('bin','obj','.git')
Get-ChildItem -Recurse -Filter '*.cs' |
    Where-Object { $excludes -notcontains $_.FullName.Split([IO.Path]::DirectorySeparatorChar) | Select-Object -First 1 } |
    ForEach-Object {
        $file = $_.FullName
        $text = Get-Content $file -Raw
        foreach ($m in [regex]::Matches($text, '^\s*(public|internal|private|protected)?\s*(static\s+)?(partial\s+)?(class|record|interface|struct)\s+([A-Za-z0-9_]+)', 'Multiline')) {
            $typeName = $m.Groups[5].Value
            # search for type name elsewhere
            $refs = Select-String -Path (Get-ChildItem -Recurse -Filter '*.cs' | Where-Object { $_.FullName -ne $file } | ForEach-Object { $_.FullName }) -Pattern "\b$typeName\b" -SimpleMatch -Quiet
            if (-not $refs) {
                [PSCustomObject]@{ File = $file; Type = $typeName }
            }
        }
    } | Sort-Object File, Type | Format-Table -AutoSize