using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(81438)]
    public class B2C_PickupItemMessageHandler : AMHandler<B2C_PickupItemMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    B2C_PickupItemMessage msg = package.msg as B2C_PickupItemMessage;
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
