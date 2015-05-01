@echo off
setlocal

set runtimedir=%~dp0Runtime

if exist %runtimedir%\CoreRun.exe (
  if exist %runtimedir%\GameApplication.exe (
    goto :InvokeGame
  )
)

:BuildFirst
echo Game doesn't exist, building...
.\build.cmd

:InvokeGame
start cmd /c "pushd %runtimedir% && CoreRun.exe GameApplication.exe"
popd

