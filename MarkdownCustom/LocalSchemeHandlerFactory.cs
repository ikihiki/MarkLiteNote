using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownCustom
{
    public class LocalSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public ISchemeHandler Create()
        {
            return new LocalSchemeHandler();
        }

        public static string SchemeName { get { return "local"; } }
    }
}
