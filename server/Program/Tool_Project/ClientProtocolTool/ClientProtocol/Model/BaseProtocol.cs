
namespace Model
{
    public class Message
    {
        public long id;
    }
    public class Request : Message
    { 

    }
    public class Response : Message
    {
        public ErrorCode errorCode;
    }
}
