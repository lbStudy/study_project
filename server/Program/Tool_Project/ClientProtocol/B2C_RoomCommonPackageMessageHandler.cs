using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(79355)]
    public class B2C_RoomCommonPackageMessageHandler : AMHandler<B2C_RoomCommonPackageMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    B2C_RoomCommonPackageMessage msg = package.msg as B2C_RoomCommonPackageMessage;
	    try
	    {

	    }
	    catch(Exception e)
	    {
		Console.WriteLine(e.ToString());
	    }
	    finally
	    {
		package.Clear();
	    }
        }
    }
}
