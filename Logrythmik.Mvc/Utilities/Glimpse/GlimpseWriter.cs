using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace Logrythmik.Mvc.Utilities.Glimpse
{
    public class GlimpseWriter : TextWriter
    {
        private bool _IsOpen;
        private readonly HttpContextBase _HttpContext;
        private static UnicodeEncoding _Encoding;

        public GlimpseWriter()
        {
            if (HttpContext.Current == null) return;

            _HttpContext = new HttpContextWrapper(HttpContext.Current);
            _IsOpen = true;
        }

        public GlimpseWriter(HttpContextBase httpContextBase)
        {
            _HttpContext = httpContextBase;
            _IsOpen = true;
        }


        protected override void Dispose(bool disposing)
        {
            _IsOpen = false;
            base.Dispose(disposing);
        }

        public override void Write(char value)
        {
            Write(value.ToString());
        }


        public override void Write(char[] buffer, int index, int count)
        {
            if (index < 0 || count < 0 || buffer.Length - index < count)
            {
                base.Write(buffer, index, count); // delegate throw exception to base class
            }
            this.Write(new string(buffer, index, count));

        }

        public override void Write(string value)
        {
            if (!_IsOpen) throw new ObjectDisposedException(null);

            if (value != null)
            {
                var list = _HttpContext.Items
                    .GetOrAdd(LinqToSqlPlugin.LING_TO_SQL_TRACE_MESSAGE_STORE_KEY,
                              () => new List<string> { "Events" });

                list.Add(value);
            }
        }

        public override Encoding Encoding
        {
            get { return _Encoding ?? (_Encoding = new UnicodeEncoding(false, false)); }
        }
    }
}