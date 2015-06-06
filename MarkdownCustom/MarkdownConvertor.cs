using MarkdownDeep;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Text;
using System.Threading.Tasks;
using ConvertorExtender;
using System.ComponentModel.Composition;
using Sgml;
using System.IO;
using System.Xml.Linq;

namespace MarkdownCustom
{
    public class MarkdownConvertor
    {
        private Markdown markdown;

        [ImportMany(typeof(IMarkdownExtender))]
        private List<IMarkdownExtender> markdownExtenders { get; set; }

        [ImportMany(typeof(IHTMLExtender))]
        private List<IHTMLExtender> htmlExtenders { get; set; }

        [ImportMany(typeof(IHTMLExtenderWithMarkdown))]
        private List<IHTMLExtenderWithMarkdown> htmlExtendersWithMarkn { get; set; }

        DirectoryCatalog catalog;
        CompositionContainer container;


        public MarkdownConvertor()
        {
            markdown = new Markdown();
            markdown.ExtraMode = true;
            markdown.SafeMode = false;
            try {
                catalog = new DirectoryCatalog(Environment.CurrentDirectory + "\\plugin");
                container = new CompositionContainer(catalog);
                container.SatisfyImportsOnce(this);
            }
            catch { }
        }

        public async Task<string> Transform(string text)
        {
            return await Task.Run<string>(() =>
            {
                foreach (var markdownExtender in markdownExtenders)
                {
                    text = markdownExtender.ExtendMorkoown(text);
                }

                var data = markdown.Transform(text);
                data = data.Insert(0, "<div>\n");
                data = data.Insert(data.Length - 1, "\n</div>");
                var doc = ConvertHtmlToXml(data);
                foreach (var htmlExtenderWithMarkn in htmlExtendersWithMarkn)
                {
                    htmlExtenderWithMarkn.ExtendHTML(doc, text);
                }

                foreach (var htmlExtender in htmlExtenders)
                {
                    htmlExtender.ExtendHTML(doc);
                }

                return doc.ToString();
            });
        }

        public XDocument ConvertHtmlToXml(string html)
        {
            using (var memoryStream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(html)))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    SgmlReader reader = new SgmlReader();
                    reader.DocType = "HTML";
                    reader.CaseFolding = CaseFolding.ToLower;
                    reader.InputStream = streamReader;
                    return XDocument.Load(reader);
                }
            }
        }
    }
}
