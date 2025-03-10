Get-ChildItem -Recurse -File | Where-Object { $_.DirectoryName -notmatch "\\(.git|node_modules|bin|obj|build|dist)\\?" } | ForEach-Object {
    try {
        $content = [System.IO.File]::ReadAllText($_.FullName)
        $content = $content -replace "`r`n", "`n"
        [System.IO.File]::WriteAllText($_.FullName, $content, [System.Text.UTF8Encoding]::new($false))
        Write-Host "Processed: $($_.FullName)"
    } catch {
        Write-Host "Skipped (likely binary): $($_.FullName)"
    }
}