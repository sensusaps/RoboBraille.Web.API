using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;
using Microsoft.Office.Interop.Word;

namespace RoboBraille.WebApi.Models
{
    public class OfficeWordProcessor
    {
        //static values of paths to be moved in the web config
        private static readonly string FileDirectory = ConfigurationManager.AppSettings.Get("FileDirectory");
        private static readonly string officeMathMLSchemaFilePath = FileDirectory + @"Util\OMML2MML.XSL";
        private static readonly string TempImageFolder = FileDirectory + @"Images\";

        //result lists based on types mapped according to the PlaceholderID
        private static Dictionary<string, string> PlainTextList = new Dictionary<string, string>();
        private static Dictionary<string, string> MathList = new Dictionary<string, string>();
        private static Dictionary<string, WordTable> TableList = new Dictionary<string, WordTable>();
        private static Dictionary<string, string> ImageList = new Dictionary<string, string>();

        //sorted list of the placeholders for the actual content according to the real document structure
        private List<String> PlaceholderIDList = new List<String>();

        //indexes for used for creating the placeholder ids
        private static int tableIndex, mathIndex, textIndex, imageIndex;

        //A string builder for creating the text parts when iterating through the document
        private static StringBuilder textBuilder = new StringBuilder();

        public string ProcessDocument(string sourceFilePath)
        {
            ProcessWordDocument(sourceFilePath);
            Dictionary<string, Object> result = GetProcessedDocument();
            string text = null;
            foreach (KeyValuePair<string, Object> val in result)
            {
                switch (val.Key.Substring(0, 4))
                {
                    case "MATH":
                    case "IMAG":
                    case "TABL":
                        break;
                    default:
                        text = text + val.Value + Environment.NewLine;
                        break;
                }
            }
            return text;
        }

        public Dictionary<string, Object> GetProcessedDocument()
        {
            Dictionary<string, Object> resultList = new Dictionary<string, Object>();
            foreach (string placeholderID in PlaceholderIDList)
            {
                if (placeholderID.StartsWith("%Math-&"))
                {
                    resultList.Add(placeholderID.Replace("%", "").Replace("&", "").ToUpperInvariant(), MathList[placeholderID]);
                }
                else
                    if (placeholderID.StartsWith("%Table-&"))
                    {
                        resultList.Add(placeholderID.Replace("%", "").Replace("&", "").ToUpperInvariant(), TableList[placeholderID]);
                    }
                    else
                        if (placeholderID.StartsWith("%Image-&"))
                        {
                            resultList.Add(placeholderID.Replace("%", "").Replace("&", "").ToUpperInvariant(), ImageList[placeholderID]);
                        }
                        else
                            resultList.Add(placeholderID.Replace("%", "").Replace("&", "").ToUpperInvariant(), PlainTextList[placeholderID]);
            }
            return resultList;
        }

        public void ProcessWordDocument(string docFilePath)
        {
            tableIndex = 1;
            mathIndex = 1;
            imageIndex = 1;
            textIndex = 1;
            using (WordprocessingDocument doc = WordprocessingDocument.Open(docFilePath, false))
            {
                foreach (var table in doc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>())
                {
                    int trows = table.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>().Count();
                    int tcols = table.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>().First().Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>().Count();
                    WordTable wordTable = new WordTable(trows, tcols);
                    //create a table class and add the text from the rows and cells
                    int row = 0, cell = 0;
                    foreach (var tableRow in table.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>())
                    {
                        foreach (var tableCell in tableRow.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>())
                        {
                            string text = tableCell.InnerText;
                            wordTable.AddText(row, cell, text);
                            cell++;
                            //save the cell into a table class for later processing with row info
                        }
                        cell = 0;
                        row++;
                    }
                    DocumentFormat.OpenXml.Wordprocessing.Paragraph para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
                    Run run = para.AppendChild(new Run());
                    string IDplaceholder = "%Table-&" + tableIndex;
                    run.AppendChild(new Text(IDplaceholder));
                    table.Parent.ReplaceChild(para, table);
                    //table.Remove();
                    tableIndex++;
                    //store the table
                    TableList.Add(IDplaceholder, wordTable);
                }
                foreach (var formula in doc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Math.OfficeMath>())
                {
                    string wordDocXml = formula.OuterXml;
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    xslTransform.Load(officeMathMLSchemaFilePath);
                    string mmlFormula = null;

                    using (TextReader tr = new StringReader(wordDocXml))
                    {
                        // Load the xml of your main document part.
                        using (XmlReader reader = XmlReader.Create(tr))
                        {
                            XmlWriterSettings settings = xslTransform.OutputSettings.Clone();

                            // Configure xml writer to omit xml declaration.
                            settings.ConformanceLevel = ConformanceLevel.Fragment;
                            settings.OmitXmlDeclaration = true;

                            using (MemoryStream ms = new MemoryStream())
                            {
                                XmlWriter xw = XmlWriter.Create(ms, settings);

                                // Transform our OfficeMathML to MathML.
                                xslTransform.Transform(reader, xw);
                                ms.Seek(0, SeekOrigin.Begin);
                                using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                                {
                                    mmlFormula = sr.ReadToEnd();
                                }
                            }
                        }
                        DocumentFormat.OpenXml.Wordprocessing.Paragraph para = formula.Parent.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                        Run run = para.AppendChild(new Run());
                        string IDplaceholder = "%Math-&" + mathIndex;
                        run.AppendChild(new Text(IDplaceholder));
                        mathIndex++;
                        formula.Remove();
                        if (mmlFormula != null)
                        {
                            MathList.Add(IDplaceholder, mmlFormula);
                        }

                    }
                }
                foreach (var graphic in doc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Drawing.Graphic>())
                {
                    DocumentFormat.OpenXml.Drawing.Blip blip = graphic.FirstChild.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().First();
                    string imageId = blip.Embed.Value;
                    ImagePart imagePart = (ImagePart)doc.MainDocumentPart.GetPartById(imageId);
                    var uri = imagePart.Uri;
                    var filename = uri.ToString().Split('/').Last();
                    var stream = doc.Package.GetPart(uri).GetStream();
                    Bitmap b = new Bitmap(stream);
                    string imagePath = TempImageFolder + filename;
                    b.Save(imagePath);
                    DocumentFormat.OpenXml.Wordprocessing.Paragraph para = graphic.Parent.Parent.Parent.Parent.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                    Run run = para.AppendChild(new Run());
                    string IDplaceholder = "%Image-&" + imageIndex;
                    run.AppendChild(new Text(IDplaceholder));
                    imageIndex++;
                    ImageList.Add(IDplaceholder, imagePath);
                }
                try
                {
                    foreach (var video in doc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Drawing.VideoFromFile>())
                    {
                        string localName = video.LocalName;
                        string innerXml = video.InnerXml;
                    }
                    foreach (var video in doc.MainDocumentPart.EmbeddedObjectParts)
                    {
                        string vct = video.ContentType;
                    }
                } catch
                {

                }
                foreach (var element in doc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                {
                    try
                    {
                        var psID = element.ParagraphProperties.ParagraphStyleId;
                        string type = null;
                        switch (psID.Val.ToString().ToLowerInvariant())
                        {
                            //for each case save the inner text of the paragraph and remove it
                            case "heading1": { type = "h1-"; break; }
                            case "heading2": { type = "h2-"; break; }
                            case "heading3": { type = "h3-"; break; }
                            case "heading4": { type = "h4-"; break; }
                            case "heading5": { type = "h5-"; break; }
                            case "title": { type = "title-"; break; }
                            case "subtitle": { type = "subtitle-"; break; }
                            default: break;
                        }
                        if (type != null)
                        {
                            string id = "%" + type + "&" + textIndex;
                            PlainTextList.Add(id, element.InnerText);
                            textIndex++;
                            element.RemoveAllChildren();
                            DocumentFormat.OpenXml.Wordprocessing.Paragraph para = element.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                            Run run = para.AppendChild(new Run());
                            run.AppendChild(new Text(id));
                        }
                    }
                    catch
                    { //do nothing 
                    }
                }

                PlaceholderIDList = ExtractTextAndCreatePlaceholderList(doc);
                if (textBuilder.Length > 0)
                {
                    string s2 = AddTextToTextList();
                    if (s2 != null)
                        PlaceholderIDList.Add(s2);
                }
            }
        }

        //used by the method above
        private static List<String> ExtractTextAndCreatePlaceholderList(WordprocessingDocument package)
        {
            OpenXmlElement docBody = package.MainDocumentPart.Document.Body;
            if (docBody == null)
            {
                return null;
            }
            return ExtractTextFromWordDocument(docBody);
        }

        //used by the method above
        private static List<String> ExtractTextFromWordDocument(OpenXmlElement element)
        {
            List<String> DocumentStructureList = new List<String>();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    // Text 
                    case "t":
                        if (section.InnerText.StartsWith("%Math-&") || section.InnerText.StartsWith("%Image-&") || section.InnerText.StartsWith("%Table-&") ||
                            section.InnerText.StartsWith("%h1-&") ||
                            section.InnerText.StartsWith("%h2-&") ||
                            section.InnerText.StartsWith("%h3-&") ||
                            section.InnerText.StartsWith("%h4-&") ||
                            section.InnerText.StartsWith("%h5-&") ||
                            section.InnerText.StartsWith("%title-&") ||
                            section.InnerText.StartsWith("%subtitle-&"))
                        {
                            string s1 = AddTextToTextList();
                            if (s1 != null)
                                DocumentStructureList.Add(s1);
                            DocumentStructureList.Add(section.InnerText);
                        }
                        else
                        {
                            textBuilder.Append(section.InnerText);
                        }
                        break;
                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        string s4 = AddTextToTextList();
                        if (s4 != null)
                            DocumentStructureList.Add(s4);
                        break;
                    // Tab 
                    case "tab":
                        textBuilder.Append("\t");
                        break;
                    // Paragraph 
                    case "p":
                        string s3 = AddTextToTextList();
                        if (s3 != null)
                            DocumentStructureList.Add(s3);
                        DocumentStructureList.AddRange(ExtractTextFromWordDocument(section));
                        break;
                    case "pict":
                    case "drawing":
                        break;
                    case "pgNum": break;
                    case "sym": break;
                    default:
                        DocumentStructureList.AddRange(ExtractTextFromWordDocument(section));
                        break;
                }
            }
            return DocumentStructureList;
        }

        /// <summary>
        /// Gets the text from the stringbuilder and initializes it with a new string builder, used in the method ExtractTextFromWordDocument
        /// </summary>
        /// <returns></returns>
        private static string AddTextToTextList()
        {
            string ret = null;
            string src = textBuilder.ToString();
            //if it matches visible character range
            if (Regex.IsMatch(src, "[\x20-\x7e]"))
            {
                ret = "%Text-&" + textIndex;
                PlainTextList.Add(ret, src);
                textIndex++;
            }
            textBuilder = new StringBuilder();
            return ret;
        }

        public static string ConvertWordToRtf(string source,string guid)
        {
            string result = null;
            try
            {
                Application app = new Application();
                var word = app.Documents.Open(source);
                word.SaveAs(FileDirectory + @"Temp\" + guid + ".rtf", WdSaveFormat.wdFormatRTF);
                result = File.ReadAllText(FileDirectory + @"Temp\" + guid + ".rtf", Encoding.UTF8);
                File.Delete(FileDirectory + @"Temp\" + guid + ".rtf");
            }
            catch { result = null; }
            return result;
        }

        public static string ConvertToHtml(byte[] byteArray)
        {
            string result = null;
            List<Bitmap> documentPictures = new List<Bitmap>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(byteArray, 0, byteArray.Length);
                using (WordprocessingDocument wDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    //var destFileName = new FileInfo(fi.Name.Replace(".docx", ".html"));
                    //if (outputDirectory != null && outputDirectory != string.Empty)
                    //{
                    //    DirectoryInfo di = new DirectoryInfo(outputDirectory);
                    //    if (!di.Exists)
                    //    {
                    //        throw new OpenXmlPowerToolsException("Output directory does not exist");
                    //    }
                    //    destFileName = new FileInfo(Path.Combine(di.FullName, destFileName.Name));
                    //}
                    //var imageDirectoryName = destFileName.FullName.Substring(0, destFileName.FullName.Length - 5) + "_files";
                    int imageCounter = 0;

                    var pageTitle = "RoboBrailleDoc";
                    var part = wDoc.CoreFilePropertiesPart;
                    if (part != null)
                    {
                        pageTitle = (string)part.GetXDocument().Descendants(DC.title).FirstOrDefault() ?? "RoboBrailleDoc";
                    }

                    // TODO: Determine max-width from size of content area.
                    HtmlConverterSettings settings = new HtmlConverterSettings()
                    {
                        AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
                        PageTitle = pageTitle,
                        FabricateCssClasses = true,
                        CssClassPrefix = "pt-",
                        RestrictToSupportedLanguages = false,
                        RestrictToSupportedNumberingFormats = false,
                        ImageHandler = imageInfo =>
                        {
                            //DirectoryInfo localDirInfo = new DirectoryInfo(imageDirectoryName);
                            //if (!localDirInfo.Exists)
                            //    localDirInfo.Create();
                            ++imageCounter;
                            string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                            ImageFormat imageFormat = null;
                            if (extension == "png")
                                imageFormat = ImageFormat.Png;
                            else if (extension == "gif")
                                imageFormat = ImageFormat.Gif;
                            else if (extension == "bmp")
                                imageFormat = ImageFormat.Bmp;
                            else if (extension == "jpeg")
                                imageFormat = ImageFormat.Jpeg;
                            else if (extension == "tiff")
                            {
                                // Convert tiff to gif.
                                extension = "gif";
                                imageFormat = ImageFormat.Gif;
                            }
                            else if (extension == "x-wmf")
                            {
                                extension = "wmf";
                                imageFormat = ImageFormat.Wmf;
                            }

                            // If the image format isn't one that we expect, ignore it,
                            // and don't return markup for the link.
                            if (imageFormat == null)
                                return null;
                            try
                            {
                                //imageInfo.Bitmap.Save(imageFileName, imageFormat);
                                documentPictures.Add(imageInfo.Bitmap);
                            }
                            catch (ExternalException)
                            {
                                return null;
                            }
                            string imageFileName = "/image" + imageCounter.ToString() + "." + extension;
                            XElement img = new XElement(Xhtml.img,
                                new XAttribute(NoNamespace.src, imageFileName),
                                imageInfo.ImgStyleAttribute,
                                imageInfo.AltText != null ?
                                    new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                            return img;
                        }
                    };
                    XElement htmlElement = HtmlConverter.ConvertToHtml(wDoc, settings);

                    // Produce HTML document with <!DOCTYPE html > declaration to tell the browser
                    // we are using HTML5.
                    var html = new XDocument(
                        new XDocumentType("html", null, null, null),
                        htmlElement);

                    // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
                    // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
                    // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
                    // for detailed explanation.
                    //
                    // If you further transform the XML tree returned by ConvertToHtmlTransform, you
                    // must do it correctly, or entities will not be serialized properly.

                    var htmlString = html.ToString(SaveOptions.DisableFormatting);
                    //File.WriteAllText(destFileName.FullName, htmlString, Encoding.UTF8);
                    result = htmlString;
                }
            }
            return result;
        }

        /// <summary>
        /// This method and the method below it are used to convert a word document with numbered lists into a xml string with indentation specified as a tag and is not used at the moment
        /// </summary>
        /// <param name="documentPath"></param>
        /// <returns></returns>
        private static string DocWithListsToXML(string documentPath)
        {
            using (WordprocessingDocument wDoc = WordprocessingDocument.Open(documentPath, false))
            {
                int abstractNumId = 1;

                XElement xml = ConvertDocToXml(wDoc, abstractNumId);
                Console.WriteLine(xml);
                //xml.Save("Out.xml");
                return xml.ToString();
            }
        }

        /// <summary>
        /// http://openxmldeveloper.org/blog/b/openxmldeveloper/archive/2014/04/20/transforming-a-docx-with-hierarchical-numbering-into-different-xml.aspx
        /// </summary>
        /// <param name="wDoc"></param>
        /// <param name="abstractNumId"></param>
        /// <returns></returns>
        private static XElement ConvertDocToXml(WordprocessingDocument wDoc, int abstractNumId)
        {
            XDocument xd = wDoc.MainDocumentPart.GetXDocument();

            // First, call RetrieveListItem so that all paragraphs are initialized with ListItemInfo
            var firstParagraph = xd.Descendants(W.p).FirstOrDefault();
            var listItem = ListItemRetriever.RetrieveListItem(wDoc, firstParagraph);

            XElement xml = new XElement("Root");
            var current = new Stack<XmlStackItem>();
            current.Push(
                new XmlStackItem()
                {
                    Element = xml,
                    LevelNumbers = new int[] { },
                });
            foreach (var paragraph in xd.Descendants(W.p))
            {
                // The following does not take into account documents that have tracked revisions.
                // As necessary, call RevisionAccepter.AcceptRevisions before converting to XML.
                var text = paragraph.Descendants(W.t).Select(t => (string)t).StringConcatenate();
                ListItemRetriever.ListItemInfo lii =
                    paragraph.Annotation<ListItemRetriever.ListItemInfo>();
                if (lii.IsListItem && lii.AbstractNumId == abstractNumId)
                {
                    ListItemRetriever.LevelNumbers levelNums =
                        paragraph.Annotation<ListItemRetriever.LevelNumbers>();
                    if (levelNums.LevelNumbersArray.Length == current.Peek().LevelNumbers.Length)
                    {
                        current.Pop();
                        var levelNumsForThisIndent = levelNums.LevelNumbersArray;
                        string levelText = levelNums
                            .LevelNumbersArray
                            .Select(l => l.ToString() + ".")
                            .StringConcatenate()
                            .TrimEnd('.');
                        var newCurrentElement = new XElement("Indent",
                            new XAttribute("Level", levelText));
                        current.Peek().Element.Add(newCurrentElement);
                        current.Push(
                            new XmlStackItem()
                            {
                                Element = newCurrentElement,
                                LevelNumbers = levelNumsForThisIndent,
                            });
                        current.Peek().Element.Add(new XElement("Heading", text));
                    }
                    else if (levelNums.LevelNumbersArray.Length > current.Peek().LevelNumbers.Length)
                    {
                        for (int i = current.Peek().LevelNumbers.Length;
                            i < levelNums.LevelNumbersArray.Length;
                            i++)
                        {
                            var levelNumsForThisIndent = levelNums
                                .LevelNumbersArray
                                .Take(i + 1)
                                .ToArray();
                            string levelText = levelNums
                                .LevelNumbersArray
                                .Select(l => l.ToString() + ".")
                                .StringConcatenate()
                                .TrimEnd('.');
                            var newCurrentElement = new XElement("Indent",
                                new XAttribute("Level", levelText));
                            current.Peek().Element.Add(newCurrentElement);
                            current.Push(
                                new XmlStackItem()
                                {
                                    Element = newCurrentElement,
                                    LevelNumbers = levelNumsForThisIndent,
                                });
                            current.Peek().Element.Add(new XElement("Heading", text));
                        }
                    }
                    else if (levelNums.LevelNumbersArray.Length < current.Peek().LevelNumbers.Length)
                    {
                        for (int i = current.Peek().LevelNumbers.Length;
                            i > levelNums.LevelNumbersArray.Length;
                            i--)
                            current.Pop();
                        current.Pop();
                        var levelNumsForThisIndent = levelNums.LevelNumbersArray;
                        string levelText = levelNums
                            .LevelNumbersArray
                            .Select(l => l.ToString() + ".")
                            .StringConcatenate()
                            .TrimEnd('.');
                        var newCurrentElement = new XElement("Indent",
                            new XAttribute("Level", levelText));
                        current.Peek().Element.Add(newCurrentElement);
                        current.Push(
                            new XmlStackItem()
                            {
                                Element = newCurrentElement,
                                LevelNumbers = levelNumsForThisIndent,
                            });
                        current.Peek().Element.Add(new XElement("Heading", text));
                    }
                }
                else
                {
                    current.Peek().Element.Add(new XElement("Paragraph", text));
                }
            }
            return xml;
        }

        private class XmlStackItem
        {
            public XElement Element;
            public int[] LevelNumbers;
        }
    }
}