@echo off
setlocal EnableDelayedExpansion

:: ============================================================
::  九歌键鼠助手 - 一键编译
::  输出: bin\九歌键鼠助手.exe (单文件)
:: ============================================================

set CONFIG=%~1
if "%CONFIG%"=="" set CONFIG=Release
set PROJECT_DIR=%~dp0
set CSPROJ=JiuGeKeyClick.csproj
set OUT_DIR=%PROJECT_DIR%bin

echo ================================
echo  九歌键鼠助手 编译脚本
echo  配置: %CONFIG%
echo ================================
echo.

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [错误] 未安装 .NET SDK
    goto :end
)

set TEMP=%USERPROFILE%\AppData\Local\Temp
set TMP=%TEMP%
if not exist "%TEMP%" mkdir "%TEMP%" 2>nul

cd /d "%PROJECT_DIR%"

:: 编译前清理
echo [1/4] 清理旧文件...
if exist "bin" rmdir /s /q "bin" 2>nul && echo   bin\ 已删除
if exist "obj" rmdir /s /q "obj" 2>nul && echo   obj\ 已删除

:: Restore
echo [2/4] 还原依赖...
dotnet restore "%CSPROJ%" 2>nul

:: Publish 单文件 -> bin\
echo [3/4] 编译单文件...
dotnet publish "%CSPROJ%" ^
    -c %CONFIG% ^
    -r win-x64 ^
    --self-contained false ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    --output "%OUT_DIR%"

if errorlevel 1 (
    echo.
    echo [失败] 编译出错
    goto :end
)

:: 编译后清理: 删除多余文件夹
echo [4/4] 清理多余文件夹...
if exist "%OUT_DIR%\Release" (
    rmdir /s /q "%OUT_DIR%\Release" 2>nul
    echo   bin\Release\ 已删除
)
if exist "%OUT_DIR%\net10.0-windows" (
    rmdir /s /q "%OUT_DIR%\net10.0-windows" 2>nul
    echo   bin\net10.0-windows\ 已删除
)
if exist "obj" (
    rmdir /s /q "obj" 2>nul
    echo   obj\ 已删除
)

echo.
echo ================================
echo  编译成功！
echo ================================
if exist "%OUT_DIR%\九歌键鼠助手.exe" (
    echo 输出: %OUT_DIR%\九歌键鼠助手.exe
    for %%i in ("%OUT_DIR%\九歌键鼠助手.exe") do echo 大小: %%~zi 字节
) else (
    echo 未找到: %OUT_DIR%\九歌键鼠助手.exe
    echo 请检查输出目录:
    dir "%OUT_DIR%" /b 2>nul || echo   (目录为空或不存在)
)

:end
echo.
pause
endlocal
