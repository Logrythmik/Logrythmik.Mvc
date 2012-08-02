using System.Collections.Generic;
using System.Web;
using Glimpse.Core.Extensibility;

namespace Logrythmik.Mvc.Utilities.Glimpse
{
    [GlimpsePlugin]
    public class LinqToSqlPlugin : IGlimpsePlugin
    {
        public const string LING_TO_SQL_TRACE_MESSAGE_STORE_KEY = "Glimpse.LinqToSql.Trace.Messages";

        public object GetData(HttpContextBase context)
        {
            return context.Items[LING_TO_SQL_TRACE_MESSAGE_STORE_KEY] as IList<string>;
        }

        public void SetupInit()
        {

        }

        public string Name
        {
            get { return "LinqToSql"; }
        }

    }
}
