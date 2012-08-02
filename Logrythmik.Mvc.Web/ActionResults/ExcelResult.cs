using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Logrythmik.Mvc
{
    public class ExcelResult : ActionResult
    {
        private readonly string _FileName;
        private readonly TableItemStyle _HeaderStyle;
        private readonly TableItemStyle _ItemStyle;
        private readonly IQueryable _Rows;
        private readonly TableStyle _TableStyle;
        private string[] _Headers;


        public ExcelResult(IQueryable rows, string fileName)
            : this(rows, fileName, null, null, null, null)
        {
        }

        public ExcelResult(string fileName, IQueryable rows, string[] headers)
            : this(rows, fileName, headers, null, null, null)
        {
        }

        public ExcelResult( 
            IQueryable rows, string fileName, 
            string[] headers,
            TableStyle tableStyle, TableItemStyle headerStyle, TableItemStyle itemStyle)
        {
            //_Mapping = mapping;
            _Rows = rows;
            _FileName = fileName;
            _Headers = headers;
            _TableStyle = tableStyle;
            _HeaderStyle = headerStyle;
            _ItemStyle = itemStyle;

            // provide defaults
            if (_TableStyle == null)
                _TableStyle = new TableStyle();

            if (_HeaderStyle == null)
                _HeaderStyle = new TableItemStyle
                {
                    BackColor = Color.LightGray,
                };

            if (_ItemStyle == null)
                _ItemStyle = new TableItemStyle
                {
                    BorderStyle = BorderStyle.Solid,
                    BorderWidth = new Unit("1px"),
                    BorderColor = Color.LightGray
                };
        }

        public string FileName
        {
            get { return _FileName; }
        }

        public IQueryable Rows
        {
            get { return _Rows; }
        }

        public override void ExecuteResult(ControllerContext context = null)
        {
            // Create HtmlTextWriter
            var sw = new StringWriter();
            var tw = new HtmlTextWriter(sw);

            // Build HTML Table from Items
            if (_TableStyle != null)
                _TableStyle.AddAttributesToRender(tw);
            tw.RenderBeginTag(HtmlTextWriterTag.Table);

            // Generate headers from table
            if (_Headers == null)
            {
                _Headers = _Rows.ElementType.GetProperties()
                    .Select(p => p.Name).ToArray();
            }
            
            // Create Header Row
            tw.RenderBeginTag(HtmlTextWriterTag.Thead);
            foreach (var header in _Headers)
            {
                if (_HeaderStyle != null)
                    _HeaderStyle.AddAttributesToRender(tw);
                tw.RenderBeginTag(HtmlTextWriterTag.Th);
                tw.Write(header.SplitCamelCase());
                tw.RenderEndTag();
            }
            tw.RenderEndTag();


            // Create Data Rows
            tw.RenderBeginTag(HtmlTextWriterTag.Tbody);
            foreach (var row in _Rows)
            {
                tw.RenderBeginTag(HtmlTextWriterTag.Tr);
                foreach (var header in _Headers)
                {
                    object propertyValue = null;
                    Type type = null;

                    if(row.GetType().IsA<IDynamicMetaObjectProvider>())
                    {
                        var dict = row as IDictionary<string, object>;
                        if (dict != null)
                        {
                            propertyValue = dict[header];
                            type = propertyValue.GetType();
                        }
                    }
                    else
                    {
                        var property = row.GetType().GetProperty(header);
                        type = property.ReflectedType;
                        propertyValue = property.GetValue(row, null);
                    }
                    
                    string strValue;
                    if (type == typeof(DateTime) && propertyValue != null)
                        strValue = ((DateTime)propertyValue).ToShortDateString();
                    else
                        strValue = ReplaceSpecialCharacters(propertyValue.AsSafeString());

                    _ItemStyle.AddAttributesToRender(tw);

                    tw.RenderBeginTag(HtmlTextWriterTag.Td);
                    tw.Write(HttpUtility.HtmlEncode(strValue));
                    tw.RenderEndTag();
                }
                tw.RenderEndTag();
            }
            tw.RenderEndTag(); // tbody

            tw.RenderEndTag(); // table
            
            WriteFileAction(_FileName, "application/ms-excel", sw.ToString());
        }


        private static string ReplaceSpecialCharacters(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            value = value.Replace("’", "'");
            value = value.Replace("“", "\"");
            value = value.Replace("”", "\"");
            value = value.Replace("–", "-");
            value = value.Replace("…", "...");
            return value;
        }

        public Action<string, string, string> WriteFileAction = WriteFile;

        private static void WriteFile(string fileName, string contentType, string content)
        {
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            context.Response.Charset = "";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = contentType;
            context.Response.Write(content);
            context.Response.End();
        }
    }
}