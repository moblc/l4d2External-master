# Left 4 Dead 2 Cheat - Single File Publisher
# PowerShell Script for Single File Deployment

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x86",
    [string]$OutputPath = "./publish"
)

# 设置控制台编码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host ""
Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║           Left 4 Dead 2 Cheat - Single File Builder          ║" -ForegroundColor Cyan  
Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

try {
    # Step 1: Clean
    Write-Host "[1/4] ?? Cleaning previous builds..." -ForegroundColor Yellow
    $cleanResult = dotnet clean --configuration $Configuration --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        throw "Clean operation failed"
    }
    
    # Step 2: Restore
    Write-Host "[2/4] ?? Restoring NuGet packages..." -ForegroundColor Yellow
    $restoreResult = dotnet restore --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        throw "Package restore failed"
    }
    
    # Step 3: Publish
    Write-Host "[3/4] ?? Building and publishing single file executable..." -ForegroundColor Yellow
    $publishArgs = @(
        "publish"
        "--configuration", $Configuration
        "--runtime", $Runtime
        "--self-contained", "true"
        "--output", $OutputPath
        "--verbosity", "minimal"
    )
    
    $publishResult = & dotnet $publishArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Publish operation failed"
    }
    
    # Step 4: Verify and report
    Write-Host "[4/4] ? Build completed!" -ForegroundColor Yellow
    Write-Host ""
    
    $exePath = Join-Path $OutputPath "l4d2External.exe"
    if (Test-Path $exePath) {
        Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Green
        Write-Host "║                          SUCCESS! ??                         ║" -ForegroundColor Green
        Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Green
        Write-Host ""
        
        $fullPath = Resolve-Path $exePath
        Write-Host "?? Location: $fullPath" -ForegroundColor Cyan
        Write-Host ""
        
        # Get file information
        $fileInfo = Get-Item $exePath
        $fileSizeBytes = $fileInfo.Length
        $fileSizeMB = [math]::Round($fileSizeBytes / 1MB, 2)
        $fileSizeKB = [math]::Round($fileSizeBytes / 1KB, 0)
        
        Write-Host "?? File information:" -ForegroundColor Cyan
        Write-Host "   ?? Size: $fileSizeBytes bytes ($fileSizeMB MB / $fileSizeKB KB)" -ForegroundColor White
        Write-Host "   ?? Created: $($fileInfo.CreationTime)" -ForegroundColor White
        Write-Host ""
        
        Write-Host "?? Benefits:" -ForegroundColor Green
        Write-Host "   ? Single executable file - no DLL dependencies!" -ForegroundColor White
        Write-Host "   ? Ready to run on any Windows x86 system" -ForegroundColor White
        Write-Host "   ? No .NET installation required" -ForegroundColor White
        Write-Host "   ? Optimized and trimmed for smaller size" -ForegroundColor White
        Write-Host ""
        
        Write-Host "?? Usage: Simply run l4d2External.exe" -ForegroundColor Yellow
        Write-Host ""
        
        # 询问是否打开文件夹
        $openFolder = Read-Host "?? Open publish folder? (y/n)"
        if ($openFolder -eq "y" -or $openFolder -eq "Y") {
            Start-Process "explorer.exe" -ArgumentList "/select,`"$fullPath`""
        }
        
        # 询问是否立即运行
        $runNow = Read-Host "?? Run the executable now? (y/n)"
        if ($runNow -eq "y" -or $runNow -eq "Y") {
            Write-Host "?? Starting Left 4 Dead 2 Cheat..." -ForegroundColor Green
            Start-Process $exePath
        }
    }
    else {
        Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Red
        Write-Host "║                         ERROR! ?                            ║" -ForegroundColor Red
        Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Red
        Write-Host ""
        Write-Host "Build completed but executable not found at: $exePath" -ForegroundColor Red
    }
}
catch {
    Write-Host ""
    Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Red
    Write-Host "║                         ERROR! ?                            ║" -ForegroundColor Red
    Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Red
    Write-Host ""
    Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "   1. Make sure .NET 8 SDK is installed" -ForegroundColor White
    Write-Host "   2. Check that all NuGet packages can be restored" -ForegroundColor White
    Write-Host "   3. Verify no antivirus is blocking the build" -ForegroundColor White
    Write-Host "   4. Try running as administrator" -ForegroundColor White
}

Write-Host ""
Read-Host "Press Enter to exit"