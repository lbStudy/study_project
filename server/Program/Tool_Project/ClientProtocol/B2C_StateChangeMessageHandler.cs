using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(65232)]
    public class B2C_StateChangeMessageHandler : AMHandler<B2C_StateChangeMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    B2C_StateChangeMessage msg = package.msg as B2C_StateChangeMessage;
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
