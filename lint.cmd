@echo off
set "NO_CHANGE=false"
for %%A in (%*) do (
    if "%%A" == "--no-change" (
        set "NO_CHANGE=true"
    )
)

if "%NO_CHANGE%" == "true" (
    dotnet format Game.Runtime.csproj --verify-no-changes
) else (
    dotnet format Game.Runtime.csproj
)
pause
