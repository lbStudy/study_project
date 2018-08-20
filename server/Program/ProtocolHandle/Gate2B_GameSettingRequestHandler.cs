using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(35415)]
    public class Gate2B_GameSettingRequestHandler : AMRpcHandler<Gate2B_GameSettingRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            Gate2B_GameSettingRequest req = pacakage.msg as Gate2B_GameSettingRequest;
            B2Gate_GameSettingResponse response = pacakage.Response as B2Gate_GameSettingResponse;

		    try
		    {
                Room room =    RoomManagerComponent.Instance.Find(req.mRoomId);
                RoomMember member =    room.Find(req.id);
                switch (req.mInfo.mType)
                {
                    case GameSettingInfo.SETTING_TYPE.VOICE_STATUS:
                        member.info.voiceOn = System.BitConverter.ToBoolean(req.mInfo.mData, 0);
                        break;
                }

                response.errorCode = (int)ErrorCode.Success;
                response.mInfo = req.mInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
		    }
		    finally
		    {
			    pacakage.Reply();
		    }
        }
    }
}
