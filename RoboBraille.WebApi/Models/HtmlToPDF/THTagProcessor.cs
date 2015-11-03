using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.tool.xml.html.table;
using iTextSharp.text;
using iTextSharp.tool.xml;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml.html.pdfelement;
using iTextSharp.tool.xml.html;

namespace RoboBraille.WebApi.Models
{
    public class THTagProcessor : TableData 
    {
        public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
        {
            if (tag.Name == "th") {
                IDictionary<string, string> attributes = tag.Attributes;
                var retval = base.End(ctx, tag, currentContent);
                PdfPCell cell = (PdfPCell)retval[0];
                cell.Role = PdfName.TH;

                if (attributes.ContainsKey("scope")) {

                    switch (attributes["scope"]) {
                        case "col":
                            cell.SetAccessibleAttribute(PdfName.SCOPE, PdfName.COLUMN);
                            break;
                        case "row":
                            cell.SetAccessibleAttribute(PdfName.SCOPE, PdfName.ROW);
                            break;
                        case "both":
                            cell.SetAccessibleAttribute(PdfName.SCOPE, PdfName.BOTH);
                            break;
                        default:
                            throw new Exception("Scope is missing or unsupported.");

                    }
                }

                return retval;
            }
            return new List<IElement>();    
        }
    }
}