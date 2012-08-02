﻿using System;
using System.Text.RegularExpressions;

namespace Logrythmik.Mvc.Utilities
{
    public class HtmlUtilities
    {
        private static readonly Regex Tags = new Regex("<[^>]*(>|$)",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static readonly Regex Whitelist = new Regex(@"
    ^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|
    ^<(b|h)r\s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex WhitelistA = new Regex(@"
    ^<a\s
    href=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""
    (\stitle=""[^""<>]+"")?\s?>$|
    ^</a>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex WhitelistImg = new Regex(@"
    ^<img\s
    src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
    (\swidth=""\d{1,3}"")?
    (\sheight=""\d{1,3}"")?
    (\salt=""[^""<>]*"")?
    (\stitle=""[^""<>]*"")?
    \s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);


        /// <summary>
        /// sanitize any potentially dangerous tags from the provided raw HTML input using 
        /// a whitelist based approach, leaving the "safe" HTML tags
        /// CODESNIPPET:4100A61A-1711-4366-B0B0-144D1179A937
        /// </summary>
        public static string Sanitize(string html)
        {
            if (String.IsNullOrEmpty(html)) return html;

            Match tag;

            // match every HTML tag in the input
            var tags = Tags.Matches(html);
            for (var i = tags.Count - 1; i > -1; i--)
            {
                tag = tags[i];
                var tagname = tag.Value.ToLowerInvariant();

                if (!(Whitelist.IsMatch(tagname) || WhitelistA.IsMatch(tagname) || WhitelistImg.IsMatch(tagname)))
                {
                    html = html.Remove(tag.Index, tag.Length);
                    System.Diagnostics.Debug.WriteLine("tag sanitized: " + tagname);
                }
            }

            return html;
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static readonly Regex HtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegex(string source)
        {
            return HtmlRegex.Replace(source, string.Empty);
        }

    }
}