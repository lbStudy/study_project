using Base;

namespace Data
{
    public enum LoginState
    {
        None,
        Logining,
        Entering
    }

    [Pool]
    public class LoginInfo : Disposer
    {
        public bool IsInGame;
        public LoginState state;
        public long id;
        public int areaid;
        public string account;
        public string password;
        public string iconUrl;
        public string Name;
        public int sex;
        public Session session;

        public override void Dispose()
        {
            base.Dispose();
            session = null;
        }
    }
}


