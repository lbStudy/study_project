
using System.Collections.Generic;
using System.Net;

namespace Base
{
    public interface IHttpHandle
    {
        void Run(HttpPackage httpPackage);
    }
}
