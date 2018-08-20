@echo Copy 协议体
del ..\..\..\client\Assets\Script\Propocol\*.cs /f
xcopy /y ..\Model\Data\enum\common\*.cs ..\..\..\client\Assets\Script\Propocol
xcopy /y ..\Model\Data\proto\common\*.cs ..\..\..\client\Assets\Script\Propocol
xcopy /y ..\Model\Data\struct\common\*.cs ..\..\..\client\Assets\Script\Propocol
xcopy /y ClientProtocol\client_protocol.xml ..\..\..\client\Assets\Config\Other_ab
xcopy /y ClientProtocol\ProtoEnum.cs ..\..\..\client\Assets\Script\Propocol
@pause