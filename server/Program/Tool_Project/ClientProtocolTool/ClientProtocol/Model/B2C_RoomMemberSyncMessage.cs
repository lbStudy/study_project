using System.Collections.Generic;

namespace Model
{
	public class B2C_RoomMemberSyncMessage : Message
	{
        public RoomMemberInfo info;
        public List<byte> pais;
	}
}
