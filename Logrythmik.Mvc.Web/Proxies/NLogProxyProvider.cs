namespace Logrythmik.Mvc
{
    public class NLogProxyProvider : ILogProxyProvider
    {
        public ILogProxy GetLogProxy(string typeName)
        {
            return new NLogLogProxy(typeName);
        }
    }
}
