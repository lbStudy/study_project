using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(90141)]
    public class B2C_DamageMessageHandler : AMHandler<B2C_DamageMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    B2C_DamageMessage msg = package.msg as B2C_DamageMessage;
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
