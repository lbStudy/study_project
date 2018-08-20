
namespace Hotfix
{
    public struct EventPackage
    {
        public EventIdType eventType;
        public long param1;
        public long param2;
        public object param3;
        public object param4;

        public EventPackage(EventIdType eventType, long param1 = 0, long param2 = 0, object param3 = null, object param4 = null)
        {
            this.eventType = eventType;
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
        }
    }
}
