using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(40416)]
    public class B2C_StartBattleMessageHandler : AMHandler<B2C_StartBattleMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    B2C_StartBattleMessage msg = package.msg as B2C_StartBattleMessage;
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
