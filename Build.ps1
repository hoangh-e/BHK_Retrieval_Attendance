# ============================================================================
# BHK RETRIEVAL ATTENDANCE - AUTO BUILD & PUBLISH SCRIPT
# ============================================================================
# Developed by: BHK AI Development Team
# Lead Developer: Trịnh Việt Hoàng
# Version: 1.0.0
# ============================================================================

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [string]$Version = "1.0.0",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("win-x64", "win-x86", "win-arm64")]
    [string]$Runtime = "win-x64",
    
    [Parameter(Mandatory=$false)]
    [switch]$FrameworkDependent,
    
    [Parameter(Mandatory=$false)]
    [switch]$SingleFile,
    
    [Parameter(Mandatory=$false)]
    [switch]$CreateZip,
    
    [Parameter(Mandatory=$false)]
    [switch]$CreateInstaller,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [switch]$CleanOnly
)

# ============================================================================
# CONFIGURATION
# ============================================================================

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

$ProjectName = "BHK.Retrieval.Attendance"
$SolutionFile = "BHK_Retrieval_Attendance.Project.sln"
$MainProjectPath = "BHK.Retrieval.Attendance.WPF\BHK.Retrieval.Attendance.WPF.csproj"

$OutputRoot = ".\publish"
$BuildOutput = "$OutputRoot\build"
$PublishOutput = "$OutputRoot\app"
$ZipOutput = "$OutputRoot\zip"
$SetupOutput = "$OutputRoot\setup"

# ============================================================================
# FUNCTIONS
# ============================================================================

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "============================================================================" -ForegroundColor Cyan
    Write-Host " $Message" -ForegroundColor Yellow
    Write-Host "============================================================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "→ $Message" -ForegroundColor Blue
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Test-Command {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

function Clean-Directory {
    param([string]$Path)
    
    if (Test-Path $Path) {
        Write-Info "Cleaning directory: $Path"
        Remove-Item -Path $Path -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    New-Item -ItemType Directory -Path $Path -Force | Out-Null
    Write-Success "Directory cleaned: $Path"
}

function Clean-Solution {
    Write-Header "CLEANING SOLUTION"
    
    Write-Info "Cleaning solution..."
    dotnet clean $SolutionFile --configuration $Configuration --verbosity quiet
    
    Write-Info "Removing bin and obj folders..."
    Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    
    Write-Success "Solution cleaned successfully"
}

function Restore-Packages {
    Write-Header "RESTORING PACKAGES"
    
    Write-Info "Restoring NuGet packages..."
    dotnet restore $SolutionFile --verbosity quiet
    
    Write-Success "Packages restored successfully"
}

function Build-Solution {
    Write-Header "BUILDING SOLUTION"
    
    Write-Info "Building configuration: $Configuration"
    Write-Info "Target framework: net8.0-windows"
    
    dotnet build $SolutionFile `
        --configuration $Configuration `
        --no-restore `
        --verbosity minimal
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed!"
        exit 1
    }
    
    Write-Success "Build completed successfully"
}

function Publish-Application {
    Write-Header "PUBLISHING APPLICATION"
    
    # Determine publish settings
    $SelfContained = -not $FrameworkDependent
    $PublishSingleFile = $SingleFile.IsPresent
    
    Write-Info "Configuration: $Configuration"
    Write-Info "Runtime: $Runtime"
    Write-Info "Self-Contained: $SelfContained"
    Write-Info "Single File: $PublishSingleFile"
    
    # Clean output directory
    Clean-Directory -Path $PublishOutput
    
    # Build publish command
    $publishArgs = @(
        "publish",
        $MainProjectPath,
        "--configuration", $Configuration,
        "--output", $PublishOutput,
        "--runtime", $Runtime,
        "--self-contained", $SelfContained.ToString().ToLower(),
        "--property:PublishReadyToRun=true",
        "--property:DebugType=None",
        "--property:DebugSymbols=false",
        "--verbosity", "minimal"
    )
    
    if ($PublishSingleFile) {
        $publishArgs += "--property:PublishSingleFile=true"
        $publishArgs += "--property:IncludeNativeLibrariesForSelfExtract=true"
    }
    
    Write-Info "Publishing application..."
    & dotnet $publishArgs
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Publish failed!"
        exit 1
    }
    
    Write-Success "Application published successfully to: $PublishOutput"
}

function Copy-AdditionalFiles {
    Write-Header "COPYING ADDITIONAL FILES"
    
    # Copy README
    if (Test-Path "README.txt") {
        Copy-Item "README.txt" -Destination $PublishOutput
        Write-Success "README.txt copied"
    }
    
    # Copy LICENSE
    if (Test-Path "LICENSE.txt") {
        Copy-Item "LICENSE.txt" -Destination $PublishOutput
        Write-Success "LICENSE.txt copied"
    }
    
    # Copy CHANGELOG
    if (Test-Path "CHANGELOG.txt") {
        Copy-Item "CHANGELOG.txt" -Destination $PublishOutput
        Write-Success "CHANGELOG.txt copied"
    }
    
    # Copy additional dependencies
    $DependenciesPath = "packages\Riss.Devices.dll"
    if (Test-Path $DependenciesPath) {
        Copy-Item $DependenciesPath -Destination $PublishOutput
        Write-Success "Riss.Devices.dll copied"
    }
    
    Write-Success "Additional files copied"
}

function Create-ZipPackage {
    Write-Header "CREATING ZIP PACKAGE"
    
    if (-not $CreateZip) {
        Write-Info "Skipping ZIP creation (use -CreateZip to enable)"
        return
    }
    
    # Check if 7-Zip is available
    $use7Zip = Test-Command "7z"
    
    Clean-Directory -Path $ZipOutput
    
    $ZipFileName = "$ProjectName`_v$Version`_$Runtime.zip"
    $ZipFilePath = Join-Path $ZipOutput $ZipFileName
    
    Write-Info "Creating ZIP package: $ZipFileName"
    
    if ($use7Zip) {
        # Use 7-Zip for better compression
        & 7z a -tzip -mx=9 $ZipFilePath "$PublishOutput\*" | Out-Null
    }
    else {
        # Use PowerShell's Compress-Archive
        Compress-Archive -Path "$PublishOutput\*" -DestinationPath $ZipFilePath -CompressionLevel Optimal -Force
    }
    
    $ZipSize = [math]::Round((Get-Item $ZipFilePath).Length / 1MB, 2)
    Write-Success "ZIP package created: $ZipFileName ($ZipSize MB)"
}

function Create-Installer {
    Write-Header "CREATING INSTALLER"
    
    if (-not $CreateInstaller) {
        Write-Info "Skipping Installer creation (use -CreateInstaller to enable)"
        return
    }
    
    # Check if Inno Setup is available
    $InnoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    if (-not (Test-Path $InnoSetupPath)) {
        Write-Warning "Inno Setup not found at: $InnoSetupPath"
        Write-Warning "Please install Inno Setup from: https://jrsoftware.org/isdl.php"
        return
    }
    
    $SetupScriptPath = "BHK_Attendance_Setup.iss"
    if (-not (Test-Path $SetupScriptPath)) {
        Write-Warning "Setup script not found: $SetupScriptPath"
        return
    }
    
    Write-Info "Compiling installer with Inno Setup..."
    & $InnoSetupPath $SetupScriptPath
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Installer creation failed!"
        return
    }
    
    Write-Success "Installer created successfully"
}

function Show-Summary {
    Write-Header "BUILD SUMMARY"
    
    Write-Host "Configuration:      " -NoNewline; Write-Host $Configuration -ForegroundColor Green
    Write-Host "Version:            " -NoNewline; Write-Host $Version -ForegroundColor Green
    Write-Host "Runtime:            " -NoNewline; Write-Host $Runtime -ForegroundColor Green
    Write-Host "Self-Contained:     " -NoNewline; Write-Host $(-not $FrameworkDependent) -ForegroundColor Green
    Write-Host "Single File:        " -NoNewline; Write-Host $SingleFile.IsPresent -ForegroundColor Green
    
    Write-Host ""
    Write-Host "Output Locations:" -ForegroundColor Yellow
    Write-Host "  Published App:    " -NoNewline; Write-Host $PublishOutput -ForegroundColor Cyan
    
    if ($CreateZip) {
        Write-Host "  ZIP Package:      " -NoNewline; Write-Host $ZipOutput -ForegroundColor Cyan
    }
    
    if ($CreateInstaller) {
        Write-Host "  Installer:        " -NoNewline; Write-Host $SetupOutput -ForegroundColor Cyan
    }
    
    Write-Host ""
    
    # Calculate total size
    if (Test-Path $PublishOutput) {
        $TotalSize = (Get-ChildItem -Path $PublishOutput -Recurse | Measure-Object -Property Length -Sum).Sum
        $TotalSizeMB = [math]::Round($TotalSize / 1MB, 2)
        Write-Host "Total Size:         " -NoNewline; Write-Host "$TotalSizeMB MB" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Success "Build process completed successfully!"
    Write-Host ""
}

function Open-OutputFolder {
    Write-Info "Opening output folder..."
    Start-Process explorer.exe -ArgumentList $OutputRoot
}

# ============================================================================
# MAIN EXECUTION
# ============================================================================

function Main {
    Clear-Host
    
    Write-Header "BHK RETRIEVAL ATTENDANCE - BUILD SCRIPT"
    Write-Host "Developed by: BHK AI Development Team" -ForegroundColor Gray
    Write-Host "Lead Developer: Trịnh Việt Hoàng" -ForegroundColor Gray
    Write-Host ""
    
    # Check prerequisites
    Write-Info "Checking prerequisites..."
    
    if (-not (Test-Command "dotnet")) {
        Write-Error ".NET SDK not found! Please install .NET 8.0 SDK"
        exit 1
    }
    
    $dotnetVersion = (dotnet --version)
    Write-Success ".NET SDK version: $dotnetVersion"
    
    # Check if solution file exists
    if (-not (Test-Path $SolutionFile)) {
        Write-Error "Solution file not found: $SolutionFile"
        exit 1
    }
    
    # Create output directories
    if (-not (Test-Path $OutputRoot)) {
        New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null
    }
    
    # Execute build pipeline
    try {
        if ($CleanOnly) {
            Clean-Solution
            Write-Success "Clean completed. Exiting."
            return
        }
        
        # Step 1: Clean
        Clean-Solution
        
        # Step 2: Restore packages
        Restore-Packages
        
        # Step 3: Build solution
        if (-not $SkipBuild) {
            Build-Solution
        }
        
        # Step 4: Publish application
        Publish-Application
        
        # Step 5: Copy additional files
        Copy-AdditionalFiles
        
        # Step 6: Create ZIP package
        Create-ZipPackage
        
        # Step 7: Create installer
        Create-Installer
        
        # Step 8: Show summary
        Show-Summary
        
        # Step 9: Open output folder
        $OpenFolder = Read-Host "Do you want to open the output folder? (Y/N)"
        if ($OpenFolder -eq "Y" -or $OpenFolder -eq "y") {
            Open-OutputFolder
        }
    }
    catch {
        Write-Error "Build process failed with error: $_"
        exit 1
    }
}

# ============================================================================
# SCRIPT ENTRY POINT
# ============================================================================

Main

# ============================================================================
# USAGE EXAMPLES
# ============================================================================

<#
.SYNOPSIS
    Build and publish BHK Retrieval Attendance application

.DESCRIPTION
    This script automates the build and publish process for the BHK Attendance application.
    It supports various configurations and output formats.

.PARAMETER Configuration
    Build configuration: Debug or Release (default: Release)

.PARAMETER Version
    Application version (default: 1.0.0)

.PARAMETER Runtime
    Target runtime: win-x64, win-x86, or win-arm64 (default: win-x64)

.PARAMETER FrameworkDependent
    Create framework-dependent deployment (requires .NET Runtime on target machine)

.PARAMETER SingleFile
    Publish as single executable file

.PARAMETER CreateZip
    Create ZIP package of published application

.PARAMETER CreateInstaller
    Create installer using Inno Setup

.PARAMETER SkipBuild
    Skip build step (useful for testing publish only)

.PARAMETER CleanOnly
    Only clean the solution and exit

.EXAMPLE
    .\Build.ps1
    Build and publish with default settings (Release, win-x64, self-contained)

.EXAMPLE
    .\Build.ps1 -Configuration Release -SingleFile -CreateZip
    Build, publish as single file, and create ZIP package

.EXAMPLE
    .\Build.ps1 -Version "1.0.1" -CreateZip -CreateInstaller
    Build version 1.0.1 with ZIP and Installer

.EXAMPLE
    .\Build.ps1 -FrameworkDependent
    Build framework-dependent version (smaller size, requires .NET Runtime)

.EXAMPLE
    .\Build.ps1 -CleanOnly
    Only clean the solution without building

.NOTES
    Developed by: BHK AI Development Team
    Lead Developer: Trịnh Việt Hoàng
    Version: 1.0.0
#>
