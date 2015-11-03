using iTextSharp.text;
using iTextSharp.tool.xml.html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class HTMLTagProcessor : AbstractTagProcessor
    {
        private Document document = new Document();

        public HTMLTagProcessor(Document doc)
        {
            this.document = doc;
        }

        public override IList<iTextSharp.text.IElement> Content(iTextSharp.tool.xml.IWorkerContext ctx, iTextSharp.tool.xml.Tag tag, string content)
        {



            switch (tag.Name)
            {
                case "html":
                    if (tag.Attributes.ContainsKey("lang") && !string.IsNullOrEmpty(tag.Attributes["lang"]))
                    {
                        this.document.AddLanguage(tag.Attributes["lang"]);
                    }
                    else
                    {
                        throw new Exception("document language is missing.");
                    }
                    break;
                case "title":
                    if (!string.IsNullOrEmpty(content))
                    {
                        this.document.AddTitle(content);
                    }
                    else
                    {
                        throw new Exception("document title is missing." + content);
                    }

                    break;
                default:
                    break;

            }

            //string language = tag.Attributes["lang"];

            //this.document.AddLanguage(language);
            return base.Content(ctx, tag, content);
        }
    }
}