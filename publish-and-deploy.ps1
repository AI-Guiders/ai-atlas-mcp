# Publish Release (win-x64, self-contained) and mirror to a fixed path for Cursor MCP.
# Run from repo:  cd ...\agent-notes-mcp  ;  .\publish-and-deploy.ps1
# Optional: -Target "D:\agent-notes-mcp"
[CmdletBinding()]
param(
    [string] $Target = "D:\agent-notes-mcp",
    [switch] $KillRunning
)

$ErrorActionPreference = "Stop"
$here = $PSScriptRoot
$csproj = Join-Path $here "AgentNotesMcp.csproj"
if (-not (Test-Path -LiteralPath $csproj)) {
    Write-Error "AgentNotesMcp.csproj not found. Run this script from the agent-notes-mcp directory (PSScriptRoot=$here)."
    exit 1
}

Push-Location $here
try {
    # Keep docs/manifests in sync with ToolCatalog.
    & dotnet run --project (Join-Path $here "tools\\ExportMcpManifest\\ExportMcpManifest.csproj") -- --write | Out-Null
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    # Prefer local tool (repo-pinned), but global install works too.
    if (Test-Path -LiteralPath (Join-Path $here ".config\\dotnet-tools.json")) {
        & dotnet tool restore | Out-Null
    }
    & dotnet aid-publish -Project $csproj -Target $Target -Runtime "win-x64" -Configuration "Release" -SelfContained -KillRunning:$KillRunning
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $configSrc = Join-Path $here "config\agent-notes-mcp.toml"
    if (-not (Test-Path -LiteralPath $configSrc)) {
        Write-Error "Missing config template: $configSrc"
        exit 1
    }
    $configDst = Join-Path $Target "agent-notes-mcp.toml"
    # Target TOML is machine-local (paths, roots may be edited in place). Never overwrite
    # a live config with the repo template — 2026-10-25 incident: deploy clobbered a
    # fixed scope_map/aliases path back to the stale -v1 names and broke scope resolution.
    if (Test-Path -LiteralPath $configDst) {
        Write-Host "Config exists in target: kept local copy (template not forced). $configDst"
        $templateCopy = Join-Path $Target "agent-notes-mcp.template.toml"
        Copy-Item -LiteralPath $configSrc -Destination $templateCopy -Force
        Write-Host "Template refreshed next to it: $templateCopy (diff & merge by hand)"
    }
    else {
        Copy-Item -LiteralPath $configSrc -Destination $configDst
    }

    $exe = Join-Path $Target "AgentNotesMcp.exe"
    $exeJson = $exe.Replace('\', '\\')
    $configJson = $configDst.Replace('\', '/')
    Write-Host ""
    Write-Host "Config: $configDst"
    Write-Host ""
    Write-Host "Cursor MCP: paste into mcp.json ->"
    Write-Host @"
  "agent-notes": {
    "command": "$exeJson",
    "args": ["--config", "$configJson"],
    "env": {}
  }
"@
    Write-Host ""
} finally {
    Pop-Location
}

