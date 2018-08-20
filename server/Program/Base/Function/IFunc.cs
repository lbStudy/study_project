
namespace Base
{
    public interface IFunc
    {
        void Run();
    }

    public interface IFunc<in A>
    {
        void Run(A uid);
    }

    public interface IFunc<in A, in B>
    {
        void Run(A a, B b);
    }

    public interface IFunc<in A, in B, in C>
    {
        void Run(A a, B b, C c);
    }

    public interface IFunc<in A, in B, in C, in D>
    {
        void Run(A a, B b, C c, D d);
    }

    public interface IFunc<in A, in B, in C, in D, in E>
    {
        void Run(A a, B b, C c, D d, E e);
    }

    public interface IFunc<in A, in B, in C, in D, in E, in F>
    {
        void Run(A a, B b, C c, D d, E e, F f);
    }

    public interface IFunc_R<A>
    {
        A Run();
    }

    public interface IFunc_R<A, in B>
    {
        A Run(B b);
    }

    public interface IFunc_R<A, in B, in C>
    {
        A Run(B b, C c);
    }

    public interface IFunc_R<A, in B, in C, in D>
    {
        A Run(B b, C c, D d);
    }

    public interface IFunc_R<A, in B, in C, in D, in E>
    {
        A Run(B b, C c, D d, E e);
    }

    public interface IFunc_R<A, in B, in C, in D, in E, in F>
    {
        A Run(B b, C c, D d, E e, F f);
    }
    public interface IFunc_R<A, in B, in C, in D, in E, in F, in G>
    {
        A Run(B b, C c, D d, E e, F f, G g);
    }
}
