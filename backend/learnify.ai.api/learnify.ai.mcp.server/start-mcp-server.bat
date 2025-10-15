@echo off
setlocal enabledelayedexpansion

REM Learnify.ai MCP Server Launcher for Claude Desktop
REM Optimized for MCP protocol compliance and reliable execution

REM Set environment variables for clean MCP operation
set SUPPRESS_STARTUP_MESSAGE=true
set DOTNET_CLI_TELEMETRY_OPTOUT=1
set DOTNET_NOLOGO=1
set ASPNETCORE_ENVIRONMENT=Development

REM Navigate to the MCP server directory
cd /d "%~dp0" 2>nul
if errorlevel 1 (
    echo Error: Failed to navigate to MCP server directory >&2
    exit /b 1
)

REM Verify .NET 8 is installed
where dotnet >nul 2>&1
if errorlevel 1 (
    echo Error: .NET 8.0 SDK is required but not installed >&2
    echo Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download >&2
    exit /b 1
)

REM Check if project file exists
if not exist "learnify.ai.mcp.server.csproj" (
    echo Error: learnify.ai.mcp.server.csproj not found in current directory >&2
    echo Current directory: %cd% >&2
    exit /b 1
)

REM Restore packages (suppress output)
dotnet restore --verbosity quiet --nologo >nul 2>&1
if errorlevel 1 (
    echo Error: Failed to restore NuGet packages >&2
    exit /b 1
)

REM Build the project (suppress output)
dotnet build --configuration Release --verbosity quiet --nologo >nul 2>&1
if errorlevel 1 (
    echo Error: Build failed >&2
    exit /b 1
)

REM Run the MCP server with clean output for Claude Desktop
dotnet run --configuration Release --no-build --verbosity quiet --nologo 2>nul