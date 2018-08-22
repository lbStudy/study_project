using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(86079)]
    public class M2Gate_GlobalNoticeMessageHandler : AMHandler<M2Gate_GlobalNoticeMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                M2Gate_GlobalNoticeMessage msg = pacakage.msg as M2Gate_GlobalNoticeMessage;
                Gate2C_GlobalNoticeMessage msgToC = new Gate2C_GlobalNoticeMessage();
                msgToC.id = IdGenerater.GenerateId();
                msgToC.content = msg.content;
                PlayerManagerComponent.Instance.SendMsgToAllPlayer(msgToC);
            }
            finally
            {
                pacakage.Clear();
            }

        }
    }
}