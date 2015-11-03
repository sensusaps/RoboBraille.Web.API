using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml.html.table;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.IO;
using iTextSharp.tool.xml.html.pdfelement;

namespace RoboBraille.WebApi.Models
{
    public class TableTagProcessor : Table
    {
        public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
        {

            if (currentContent.Count > 0)
            {

                IDictionary<string, string> attributes = tag.Attributes;
                var retval = base.End(ctx, tag, currentContent);
                foreach (PdfPTable table in retval.OfType<PdfPTable>())
                {
                    if (attributes.ContainsKey("summary") && attributes.ContainsKey("caption"))
                    {
                        table.SetAccessibleAttribute(PdfName.SUMMARY, new PdfString(attributes["summary"].ToString()));
                        table.SetAccessibleAttribute(PdfName.CAPTION, new PdfString(attributes["caption"].ToString()));
                        table.KeepRowsTogether(0);
                    }
                    else
                    {
                        throw new Exception("Table is missing attributes. Summary and Caption must be available.");
                    }
                }

                return retval;
            }

            return new List<IElement>();
        }
    }
}