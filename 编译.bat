@echo off
setlocal
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Release
set PROJECT_DIR=%~dp0
set CSPROJ=JiuGeKeyClick.csproj
set OUT_DIR=%PROJECT_DIR%bin
echo ================================
echo JiuGe Build Script
echo Config: %CONFIG%
echo ================================
echo.
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET SDK not found
    goto :end
)
cd /d "%PROJECT_DIR%"
echo [0/4] Stop running process...
taskkill /f /im "??????.exe" >nul 2>&1
echo Done
echo [1/4] Clean old files...
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
echo Done
echo [2/4] Restore dependencies...
dotnet restore "%CSPROJ%"
echo [3/4] Build single file...
dotnet publish "%CSPROJ%" -c "%CONFIG%" -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --output "%OUT_DIR%"
if errorlevel 1 (
    echo.
    echo [FAILED] Build error
    goto :end
)
echo [4/4] Clean residual files...
if exist "%OUT_DIR%\Release" rmdir /s /q "%OUT_DIR%\Release"
if exist "%OUT_DIR%\net10.0-windows" rmdir /s /q "%OUT_DIR%\net10.0-windows"
if exist "obj" rmdir /s /q "obj"
echo Done
echo.
echo ================================
echo Build SUCCESS
echo ================================
if exist "%OUT_DIR%\??????.exe" (
    echo Output: %OUT_DIR%\??????.exe
) else (
    echo Not found: %OUT_DIR%\??????.exe
)
:end
echo.
pause
endlocal
