using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using System.Text;

namespace Logrythmik
{
    public class DebuggerWriter : TextWriter
    {
        private bool _IsOpen;
        private static UnicodeEncoding _Encoding;
        private readonly int _Level;
        private readonly string _Category;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerWriter"/> class.
        /// </summary>
        public DebuggerWriter()
            : this(0, Debugger.DefaultCategory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerWriter"/> class with the specified level and category.
        /// </summary>
        /// <param name="level">A description of the importance of the messages.</param>
        /// <param name="category">The category of the messages.</param>
        public DebuggerWriter(int level, string category)
            : this(level, category, CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerWriter"/> class with the specified level, category and format provider.
        /// </summary>
        /// <param name="level">A description of the importance of the messages.</param>
        /// <param name="category">The category of the messages.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
        public DebuggerWriter(int level, string category, IFormatProvider formatProvider)
            : base(formatProvider)
        {
            this._Level = level;
            this._Category = category;
            this._IsOpen = true;
        }

        protected override void Dispose(bool disposing)
        {
            _IsOpen = false;
            base.Dispose(disposing);
        }

        public override void Write(char value)
        {
            if (!_IsOpen)
            {
                throw new ObjectDisposedException(null);
            }
            Debugger.Log(_Level, _Category, value.ToString());
        }

        public override void Write(string value)
        {
            if (!_IsOpen)
            {
                throw new ObjectDisposedException(null);
            }
            if (value != null)
            {
                Debugger.Log(_Level, _Category, value);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (!_IsOpen)
            {
                throw new ObjectDisposedException(null);
            }
            if (index < 0 || count < 0 || buffer.Length - index < count)
            {
                base.Write(buffer, index, count); // delegate throw exception to base class
            }
            Debugger.Log(_Level, _Category, new string(buffer, index, count));
        }

        public override Encoding Encoding
        {
            get { return _Encoding ?? (_Encoding = new UnicodeEncoding(false, false)); }
        }

        public int Level
        {
            get { return _Level; }
        }

        public string Category
        {
            get { return _Category; }
        }
    }
}
