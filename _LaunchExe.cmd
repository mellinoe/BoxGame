@echo off
setlocal

set exename=%1
shift

if "%1" == "" goto EndArgLoop
set remainder=%1
shift
:ArgLoop
if "%1" == "" goto EndArgLoop
set remainder=%remainder% %1
shift
goto ArgLoop

:EndArgLoop

set runtimedir=%~dp0%exename%-Runtime

:FindExeLoc
if exist %runtimedir%\CoreRun.exe (
  if exist %runtimedir%\%exename%.exe (
  goto :InvokeExe
  )
  echo Error, executable not found: %runtimedir%\%exename%.exe
  exit /b
)

:InvokeExe
%runtimedir%\CoreRun.exe %runtimedir%\%exename%.exe %remainder%
