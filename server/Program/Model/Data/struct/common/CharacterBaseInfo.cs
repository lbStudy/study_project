using System;


namespace Data
{
    [ProtoBuf.ProtoContract]
    public class CharacterBaseInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int configId { set; get; }    //配置ID
        [ProtoBuf.ProtoMember(3)]
        public S_Vector3 pos { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public float rotationY { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public CharacterState state { set; get; }
        [ProtoBuf.ProtoMember(6)]
        public float stateTime { set; get; }    //在该状态下度过的时间
        [ProtoBuf.ProtoMember(7)]
        public long group { set; get; }         //阵营
        [ProtoBuf.ProtoMember(8)]
        public float curSpeed { set; get; }     //当前移动速度
        [ProtoBuf.ProtoMember(9)]
        public int score { set; get; }          //积分
        [ProtoBuf.ProtoMember(10)]
        public S_Vector3 targetPos { set; get; }//目标点
        [ProtoBuf.ProtoMember(11)]
        public S_Vector3 otherMove { set; get; }//移动
        [ProtoBuf.ProtoMember(12)]
        public long param1 { set; get; }
        [ProtoBuf.ProtoMember(13)]
        public SkillInfo curSkill { set; get; }
        [ProtoBuf.ProtoMember(14)]
        public float radius { set; get; }
        [ProtoBuf.ProtoMember(15)]
        public int level { set; get; }
        [ProtoBuf.ProtoMember(16)]
        public int category { set; get; }
        [ProtoBuf.ProtoMember(17)]
        public int scId { set; get; }
        [ProtoBuf.ProtoMember(18)]
        public S_Vector3 bornPos { set; get; }//出生点
        [ProtoBuf.ProtoMember(19)]
        public long param2 { set; get; }
    }
}
