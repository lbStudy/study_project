using System;
using System.Collections.Generic;

namespace Base
{
    [Pool(100)]
    public class MsgPackage : Disposer
    {
        protected Session source;
        protected ProtocolInfo protoInfo;
        public Session Source { get { return source; } }
        public long Toid {
            get {
                if (toids.Count > 0)
                    return toids[0];
                else
                    return 0;
            }
        }
        public object msg;
        public List<long> toids = new List<long>();
        public virtual void Init(Session session, ProtocolInfo protoInfo, object msg, List<long> toids)
        {
            this.source = session;
            this.toids.AddRange(toids);
            this.msg = msg;
            this.protoInfo = protoInfo;
        }
        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            protoInfo.Back(msg);
            protoInfo = null;
            source = null;
            msg = null;
            toids.Clear();
        }
    }

    public class RpcPackage : MsgPackage
    {
        private uint rpcId;
        private object response;

        public object Response { get { return response; } }
        private bool isReply;
        private ProtocolInfo respProtoInfo;
        public void Init(Session session, ProtocolInfo protoInfo, ProtocolInfo respProtoInfo, object msg, uint rpcId, List<long> toids)
        {
            base.Init(session, protoInfo, msg, toids);
            this.rpcId = rpcId;
            this.respProtoInfo = respProtoInfo;
            this.response = respProtoInfo.Take();
            isReply = false;
        }
        public void Reply()
        {
            if (isReply)
                return;
            isReply = true;
            if(source.IsDisposed == false)
            {
                source.Reply(rpcId, response, Toid);
            }
            Dispose();
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();

            respProtoInfo.Back(response);
            response = null;
        }
    }
}
