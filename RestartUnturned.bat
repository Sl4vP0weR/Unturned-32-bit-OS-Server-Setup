@echo off
SET SERVERNAME=%1

echo Batch author: Greenorine
:unturned
echo [%time%] Unturned started.
echo n|>nul start /wait Unturned.exe -nographics -batchmode +secureserver/%SERVERNAME%
echo [%time%] WARNING: Unturned closed or crashed, restarting.
goto :unturned