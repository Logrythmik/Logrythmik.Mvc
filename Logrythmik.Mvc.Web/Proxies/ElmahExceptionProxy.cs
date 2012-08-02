using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elmah;

namespace Logrythmik.Mvc
{
    public class ElmahExceptionProxy : IExceptionLogProxy
    {
        #region IExceptionLogProxy Members

        public void LogException( Exception exc )
        {
            ErrorSignal.FromCurrentContext().Raise( exc );
        }

        #endregion
    }
}
