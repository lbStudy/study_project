cd ../Model
@echo ±‡“ÎModel
devenv Model.csproj /build "Debug"
@pause
cd ..
xcopy /y bin\Debug\netcoreapp2.0\Model.dll Tool_Project\CreatAttributeScripte\CreatAttributeScripte\bin\Debug
xcopy /y bin\Debug\netcoreapp2.0\Model.pdb Tool_Project\CreatAttributeScripte\CreatAttributeScripte\bin\Debug
cd Tool_Project/CreatAttributeScripte/CreatAttributeScripte/bin/Debug/
CreatAttributeScripte.exe
@pause