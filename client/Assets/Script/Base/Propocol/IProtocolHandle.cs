
using Model;

namespace Base
{
    public interface IProtocolHandle
    {
        void Handle(Session session, object msg, uint rpcId = 0);
    }


}
