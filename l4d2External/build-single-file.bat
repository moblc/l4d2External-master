@echo off
chcp 65001 >nul
echo.
echo ¨X¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨[
echo ¨U           Left 4 Dead 2 Cheat - Single File Builder          ¨U
echo ¨^¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨a
echo.

echo [1/4] ?? Cleaning previous builds...
dotnet clean --configuration Release --verbosity quiet
if %errorlevel% neq 0 (
    echo ? Clean failed!
    pause
    exit /b 1
)

echo [2/4] ?? Restoring NuGet packages...
dotnet restore --verbosity quiet
if %errorlevel% neq 0 (
    echo ? Restore failed!
    pause
    exit /b 1
)

echo [3/4] ?? Building and publishing single file executable...
dotnet publish --configuration Release --runtime win-x86 --self-contained true --output "./publish" --verbosity minimal
if %errorlevel% neq 0 (
    echo ? Publish failed!
    pause
    exit /b 1
)

echo [4/4] ? Build completed!
echo.

if exist "./publish/l4d2External.exe" (
    echo ¨X¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨[
    echo ¨U                          SUCCESS! ??                         ¨U
    echo ¨^¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨a
    echo.
    echo ?? Location: %cd%\publish\l4d2External.exe
    echo.
    echo ?? File information:
    for %%A in ("./publish/l4d2External.exe") do (
        set size=%%~zA
        echo    ?? Size: !size! bytes
    )
    echo.
    echo ?? Benefits:
    echo    ? Single executable file - no DLL dependencies!
    echo    ? Ready to run on any Windows x86 system
    echo    ? No .NET installation required
    echo    ? Optimized and trimmed for smaller size
    echo.
    echo ?? Usage: Simply run l4d2External.exe
    echo.
    set /p openFolder="?? Open publish folder? (y/n): "
    if /i "!openFolder!"=="y" (
        start "" "%cd%\publish"
    )
) else (
    echo ¨X¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨[
    echo ¨U                         ERROR! ?                            ¨U
    echo ¨^¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨T¨a
    echo.
    echo Build failed or executable not found!
    echo Check the error messages above.
)

echo.
pause