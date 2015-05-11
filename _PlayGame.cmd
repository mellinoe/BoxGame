@echo off
setlocal

if "%1" == "" (
  echo No game specified.
  echo "Usage: PlayGame.cmd <game-name>"
  exit /b
)

set gamename=%1
set runtimedir=%~dp0%gamename%-Runtime

:FindGameLoc
if exist %runtimedir%\CoreRun.exe (
  if exist %runtimedir%\%gamename%.exe (
  goto :InvokeGame
  )
)

:NotFound
echo Game %gamename% not found. Building.
goto :BuildFirst

:BuildFirst
echo Game doesn't exist, building...
call .\build.cmd
goto :FindGameLoc

:InvokeGame
%runtimedir%\CoreRun.exe %runtimedir%\%gamename%.exe
