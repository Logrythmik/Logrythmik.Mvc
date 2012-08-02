using System;

namespace Logrythmik
{
    public interface IExceptionLogProxy
    {
        void LogException(Exception exc);
    }
}