using Model;
namespace Base
{
    public class NetOuterComponent : NetworkComponent, IAwake, IAwake<string, int>
    {
        public static NetOuterComponent Instance;

        public void Awake()
        {
            Instance = this;
            this.Awake(NetworkProtocol.TCP);
        }

        public void Awake(string host, int port)
        {
            Instance = this;
            this.Awake(NetworkProtocol.TCP, host, port);
        }
        public override void Dispose()
        {
            if (IsDisposer)
            {
                return;
            }
            base.Dispose();
            Instance = null;
        }
        private new void Update()
        {
            base.Update();
        }
        public override void Remove(Session session)
        {
            base.Remove(session);
        }
    }
}