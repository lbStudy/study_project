namespace Base
{
    public interface IAwake
    {
        void Awake();
    }

    public interface IAwake<in A>
    {
        void Awake(A uid);
    }

    public interface IAwake<in A, in B>
    {
        void Awake(A a, B b);
    }

    public interface IAwake<in A, in B, in C>
    {
        void Awake(A a, B b, C c);
    }

    public interface IAwake<in A, in B, in C, in D>
    {
        void Awake(A a, B b, C c, D d);
    }
}