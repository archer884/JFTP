using JFTP.Lib;
using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace JFTP.WebExtensions
{
    public static class JFileWebExtensions
    {
        private static readonly string BaseDirectory = ConfigurationManager.AppSettings["BaseDirectory"];

        private static string LinkPath(string path)
        {
            return "files/" + path.Replace(BaseDirectory, "").Replace("\\", "/").TrimStart('/');
        }

        public static IHtmlString DownloadLink(this JFile file)
        {
            var link = string.Format("<a href=\"{0}\">{1}</a>", LinkPath(file.Path), file.Name);
            return new NonEncodedHtmlString(link);
        }
    }
}