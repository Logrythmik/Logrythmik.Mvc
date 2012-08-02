namespace Logrythmik
{
    public interface ILogProxyProvider
    {
        ILogProxy GetLogProxy(string name);
    }
}