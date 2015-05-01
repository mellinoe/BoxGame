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
call .\build.cmd

:InvokeGame
%runtimedir%\CoreRun.exe %runtimedir%\GameApplication.exe
popd

