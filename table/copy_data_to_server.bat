@echo
@echo Copy Config
xcopy /y Config\*.txt ..\server\Program\bin\Config
xcopy /y cs\*.cs ..\server\Program\Model\Data\config
@pause