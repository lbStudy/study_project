using Data;


public partial class PlayerDataHandle
{
	private PlayerDataHandle()
	{
		attributeHandleFuncs[(int)D_AttributeType.roomcard] = new D_roomcard();
	}
}
