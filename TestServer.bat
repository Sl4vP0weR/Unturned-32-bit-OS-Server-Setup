@echo off
echo n|>nul start /wait Unturned_32BitServerSetup.exe %~dp0 -q
RestartUnturned.bat TestServer