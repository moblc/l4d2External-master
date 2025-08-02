# Left 4 Dead 2 Cheat - Single File Build Instructions

## ?? Overview
This project has been configured for **Single File Deployment**, which means all DLL dependencies will be compiled into a single executable file.

## ?? Quick Start

### Method 1: Using Batch Script (Recommended)
```batch
# Double-click or run in command prompt:
build-single-file.bat
```

### Method 2: Using PowerShell Script
```powershell
# Run in PowerShell:
.\build-single-file.ps1
```

### Method 3: Manual Command
```bash
# Clean, restore, and publish:
dotnet clean --configuration Release
dotnet restore
dotnet publish --configuration Release --runtime win-x86 --self-contained true --output "./publish"
```

## ?? Before vs After

### Before (Multiple Files):
```
?? bin/Release/net8.0/
©À©¤©¤ ?? l4d2External.exe          (~50 KB)
©À©¤©¤ ?? ClickableTransparentOverlay.dll
©À©¤©¤ ?? ImGui.NET.dll
©À©¤©¤ ?? SixLabors.ImageSharp.dll
©À©¤©¤ ?? swed32.dll
©À©¤©¤ ?? Veldrid.ImGui.dll
©À©¤©¤ ?? Vortice.Mathematics.dll
©¸©¤©¤ ?? ... many more DLL files
```

### After (Single File):
```
?? publish/
©¸©¤©¤ ?? l4d2External.exe          (~15-25 MB, standalone)
```

## ? Benefits

| Feature | Description |
|---------|-------------|
| ?? **No Dependencies** | Single .exe file with all libraries included |
| ?? **Portable** | Run on any Windows x86 system without installation |
| ?? **Self-Contained** | No .NET runtime installation required |
| ?? **Optimized** | Code trimming reduces file size |
| ??? **Antivirus Friendly** | Fewer files = less false positives |

## ?? Technical Details

### Project Configuration
- **Target Framework**: .NET 8.0
- **Platform**: x86 (32-bit)
- **Runtime**: win-x86
- **Publish Mode**: Single File + Self-Contained
- **Trimming**: Enabled (Partial mode)
- **Compression**: Enabled

### Key Settings in .csproj
```xml
<PublishSingleFile>true</PublishSingleFile>
<SelfContained>true</SelfContained>
<RuntimeIdentifier>win-x86</RuntimeIdentifier>
<PublishTrimmed>true</PublishTrimmed>
<IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
<EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
```

## ?? Usage

1. **Build**: Run one of the build scripts
2. **Deploy**: Copy `publish/l4d2External.exe` to any Windows PC
3. **Run**: Double-click the executable (no installation needed)

## ?? Troubleshooting

### Common Issues:
- **Build Fails**: Ensure .NET 8 SDK is installed
- **Large File Size**: Normal for single file deployment (~15-25 MB)
- **Antivirus Warning**: Add to exclusions if needed
- **Won't Run**: Verify target system is Windows x86/x64

### File Size Optimization:
The executable may be larger than before, but this is normal for single file deployment. The convenience of having no DLL dependencies usually outweighs the size increase.

## ?? Notes

- Original project structure remains unchanged
- Development builds still use regular DLL references
- Only publish builds create single file executables
- Compatible with all existing project features