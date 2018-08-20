using System.Collections.Generic;
using System;
using Base;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2C_RoomMemberSyncMessage : IDisposable, IAwake
	{
        [ProtoBuf.ProtoMember(1)]
        public List<RoomMemberInfo> members { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public List<CharacterBaseInfo> characters { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public List<long> leaveCharacters { set; get; }
        public void Awake()
        {
            characters = new List<CharacterBaseInfo>();
            members = new List<RoomMemberInfo>();
            leaveCharacters = new List<long>();
        }

        public void Dispose()
        {
            characters.Clear();
            members.Clear();
            leaveCharacters.Clear();
        }
    }
}
