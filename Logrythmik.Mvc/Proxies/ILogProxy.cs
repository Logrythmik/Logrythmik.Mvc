namespace Logrythmik
{
    public interface ILogProxy : IExceptionLogProxy
    {
        bool IsTraceEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsWarnEnabled { get; }

        void Trace(string message);
        void Debug(string message);
        void Warn(string message);
    }
}