@echo
@echo Copy xml
xcopy /y xml\*.xml ..\server\Program\bin\Config\xml
xcopy /y xml\room\*.xml ..\server\Program\bin\Config\xml
xcopy /y xml\shop\*.xml ..\server\Program\bin\Config\xml
@pause