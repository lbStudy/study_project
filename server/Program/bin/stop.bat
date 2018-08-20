tasklist /nh|find /i "dotnet.exe"
if ERRORLEVEL 1 (echo qqa.exe not exist) else (taskkill /f /im "dotnet.exe")
@pause