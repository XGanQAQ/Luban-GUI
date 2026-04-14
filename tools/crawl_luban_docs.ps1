param(
    [string]$OutDir = "docs/lubanDocs_crawled",
    [string]$Domain = "www.datable.cn",
    [string]$ScopePrefix = "/docs",
    [int]$MaxPages = 300,
    [switch]$Overwrite
)

$ErrorActionPreference = "Stop"

$scriptPath = Join-Path $PSScriptRoot "crawl_luban_docs.py"
if (-not (Test-Path $scriptPath)) {
    throw "未找到脚本: $scriptPath"
}

$seedArgs = @(
    "--seed", "https://www.datable.cn/docs/basic",
    "--seed", "https://www.datable.cn/docs/beginner",
    "--seed", "https://www.datable.cn/docs/manual"
)

$sourceArgs = @(
    "--from-existing-sources", "docs/lubanDocs"
)

$args = @(
    $scriptPath,
    "--out-dir", $OutDir,
    "--domain", $Domain,
    "--scope-prefix", $ScopePrefix,
    "--max-pages", "$MaxPages"
) + $seedArgs + $sourceArgs

if ($Overwrite) {
    $args += "--overwrite"
}

Write-Host "执行: python $($args -join ' ')" -ForegroundColor Cyan
python @args
