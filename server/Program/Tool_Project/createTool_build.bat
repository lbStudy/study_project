cd ClientProtocolTool//CreateTool/bin/Debug
CreateTool.exe
cd ../../../ClientProtocol
@pause
call vsvars32.bat
devenv ClientProtocol.csproj /build "Debug"
devenv ClientProtocol.csproj /build "Release"

xcopy /y bin\Debug\ClientProtocol.dll ..\..\ClientProtocol
xcopy /y bin\Debug\ClientProtocol.pdb ..\..\ClientProtocol
@pause