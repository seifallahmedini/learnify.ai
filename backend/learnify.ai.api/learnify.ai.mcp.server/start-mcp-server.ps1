# Learnify.ai MCP Server Launcher (PowerShell)
# Optimized for Claude Desktop MCP integration

param(
    [switch]$Verbose = $false
)

# Set environment variables for clean MCP operation
$env:SUPPRESS_STARTUP_MESSAGE = "true"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
$env:DOTNET_NOLOGO = "1"
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

try {
    # Verify .NET 8 is installed
    $dotnetVersion = & dotnet --version 2>$null
    if (-not $dotnetVersion) {
        Write-Error "Error: .NET 8.0 SDK is required but not installed"
        Write-Error "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download"
        exit 1
    }

    # Check if project file exists
    if (-not (Test-Path "learnify.ai.mcp.server.csproj")) {
        Write-Error "Error: learnify.ai.mcp.server.csproj not found in current directory"
        Write-Error "Current directory: $(Get-Location)"
        exit 1
    }

    # Restore packages
    if ($Verbose) {
        Write-Host "Restoring packages..." -ForegroundColor Yellow
    }
    & dotnet restore --verbosity quiet --nologo | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Error: Failed to restore NuGet packages"
        exit 1
    }

    # Build the project
    if ($Verbose) {
        Write-Host "Building project..." -ForegroundColor Yellow
    }
    & dotnet build --configuration Release --verbosity quiet --nologo | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Error: Build failed"
        exit 1
    }

    # Run the MCP server
    if ($Verbose) {
        Write-Host "Starting MCP server..." -ForegroundColor Green
    }
    & dotnet run --configuration Release --no-build --verbosity quiet --nologo
}
catch {
    Write-Error "Error: $($_.Exception.Message)"
    exit 1
}