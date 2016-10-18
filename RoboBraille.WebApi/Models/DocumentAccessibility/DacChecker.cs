using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models.DocumentAccessibility
{
    public class DacChecker
    {

        public DacReport isPdfAccessible(String sourceFile)
        {
            DacReport dr = new DacReport();
            PdfReader reader = new PdfReader(sourceFile);
            if (reader.IsTagged())
            {
                dr.IsTagged = true;
                var trailerKeys = reader.Trailer.Keys;

                var catalogKeys = reader.Catalog.Keys;

                //continue checking for accessibility, possibly follow the Matterhorn Protocol
                /*
                 * 01-003 Content marked as Artifact is present inside tagged content.
                 * 01-004 Tagged content is present inside content marked as Artifact.
                 * 01-005 Content is neither marked as Artifact nor tagged as real content.
                 * 01-007 Suspect entry has a value of true.
                 * 02-001 One or more non-standard tag’s mapping does not terminate with a standard type.
                 * 02-003 A circular mapping exists.
                 * 02-004 One or more standard types are remapped.
                 * 06-001 Document does not contain XMP metadata stream
                 * 06-002 The metadata stream in the Catalog dictionary does not include the PDF/UA identifier.
                 * 06-003 Metadata stream does not contain dc:title
                 * 07-001 ViewerPreferences dictionary of the Catalog dictionary does not contain DisplayDocTitle key.
                 * 07-002 ViewerPreferences dictionary of the Catalog dictionary contains DisplayDocTitle key with a value of false.
                 * 09-004 A table-related structure element is used in a way that does not conform to the syntax defined in ISO 32000-1, Table 337.
                 * 09-005 A list-related structure element is used in a way that does not conform to Table 336 in ISO 32000-1.
                 * 09-006 A TOC-related structure element is used in a way that does not conform to Table 333 in ISO 32000-1.
                 * 09-007 A Ruby-related structure element is used in a way that does not conform to Table 338 in ISO 32000-1.
                 * 09-008 A Warichu-related structure element is used in a way that does not conform to Table 338 in ISO 32000-1.
                 * 10-001 Character code cannot be mapped to Unicode.
                 * 11-001 Natural language for text in page content cannot be determined.
                 * 11-002 Natural language for text in “Alt”, “ActualText” and “E” attributes cannot be determined.
                 * 11-003 Natural language in the Outline entries cannot be determined.
                 * 11-004 Natural language in the “Contents” entry for annotations cannot be determined.
                 * 11-005 Natural language in the TU key for form fields cannot be determined.
                 * 11-006 Natural language for document metadata cannot be determined.
                 * 13-004 Figure tag alternative or replacement text missing.
                 * 14-002 Does use numbered headings, but the first heading tag is not H1.
                 * 14-003 Numbered heading levels in descending sequence are skipped (Example: H3 follows directly after H1).
                 */
            }
            else
            {
                dr.IsTagged = false;
            }
            return dr;
        }

        //according to the Matterhorn Protocol a list of tests that can only be done by humans
        public void ProcessedByHuman()
        {
            /*
             * 01-001 Artifact is tagged as real content.
             * 01-002 Real content is marked as artifact.
             * 01-006 The structure type and attributes of a structure element are not semantically appropriate for the structure element. 
             *        All of the following structure types must be taken into account....
             * 02-002 The mapping of one or more non-standard types is semantically inappropriate.
             * 03-001 One or more Actions lead to flickering.
             * 03-002 One or more multimedia objects contain flickering content.
             * 03-003 One or more JavaScript actions lead to flickering.
             * 04-001 Information is conveyed by contrast, color, format or layout, or some combination thereof but the content is not tagged to reflect all meaning conveyed by the use of contrast, color, format or layout, or some combination thereof.
             * 05-001 Media annotation present, but audio content not available in another form.
             * 05-002 Audio annotation present, but content not available in another form.
             * 05-003 JavaScript uses beep function but does not provide another means of notification.
             * 06-004 dc:title does not clearly identify the document
             * 08-001 OCR-generated text contains significant errors.
             * 08-002 OCR-generated text is not tagged. 01-006
             * 09-001 Tags are not in logical reading order.
             * 09-002 Structure elements are nested in a semantically inappropriate manner. (e.g., a table inside a heading).
             * 09-003 The structure type (after applying any role-mapping as necessary) of a structure element is not semantically appropriate. 01-006
             * 11-007 Natural language is not appropriate.
             * 12-001 Stretched characters are not represented appropriately.
             * 13-001 Graphics objects other than text objects and artifacts are not tagged with a Figure tag.
             * 13-002 A link with a meaningful background does not include alternative text describing both the link and the graphic’s purpose.
             * 13-003 A caption is not tagged with a Caption tag.
             * 13-005 Actual text used for a Figure for which Alternative text is more appropriate.
             * 13-006 Graphics objects that possess semantic value only within a group of graphics objects is tagged on its own.
             * 13-007 A more accessible representation is not used.
             * 14-001 Headings are not tagged. 01-006 
             * 14-004 Numbered heading tags do not use Arabic numerals. 01-006
             * 14-005 Content representing a 7th level (or higher) heading does not use an “H7” (or higher) tag.
             * 14-006 A node contains more than one H tag.
             * 14-007 Document uses both H and H# tags.
             */
        }

        //just an example of creating an accessible pdf, may become relevant at a later stage
        /** The resulting PDF. */
        public static String DEST = "results/pdfa/pdfua.pdf";
        /** An image resource. */
        public static String FOX = "resources/images/fox.bmp";
        /** An image resource. */
        public static String DOG = "resources/images/dog.bmp";
        /** A font that will be embedded. */
        public static String FONT = "resources/fonts/FreeSans.ttf";


        /**
         * Creates an accessible PDF with images and text.
         */
        public void createPdf(String dest)
        {
            Document document = new Document(PageSize.A4.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(document, File.OpenRead(dest));
            writer.PdfVersion = PdfWriter.VERSION_1_7;
            //TAGGED PDF
            //Make document tagged
            writer.SetTagged();
            //===============
            //PDF/UA
            //Set document metadata
            writer.ViewerPreferences = PdfWriter.DisplayDocTitle;
            document.AddLanguage("en-US");
            document.AddTitle("English pangram");
            writer.CreateXmpMetadata();
            //=====================
            document.Open();

            Paragraph p = new Paragraph();
            //PDF/UA
            //Embed font
            Font font = FontFactory.GetFont(FONT, BaseFont.WINANSI, BaseFont.EMBEDDED, 20);
            p.Font = font;
            //==================
            Chunk c = new Chunk("The quick brown ");
            p.Add(c);
            Image i = Image.GetInstance(FOX);
            c = new Chunk(i, 0, -24);
            //PDF/UA
            //Set alt text
            c.SetAccessibleAttribute(PdfName.ALT, new PdfString("Fox"));
            //==============
            p.Add(c);
            p.Add(new Chunk(" jumps over the lazy "));
            i = Image.GetInstance(DOG);
            c = new Chunk(i, 0, -24);
            //PDF/UA
            //Set alt text
            c.SetAccessibleAttribute(PdfName.ALT, new PdfString("Dog"));
            //==================
            p.Add(c);
            document.Add(p);

            p = new Paragraph("\n\n\n\n\n\n\n\n\n\n\n\n", font);
            document.Add(p);
            List list = new List(true);
            list.Add(new ListItem("quick", font));
            list.Add(new ListItem("brown", font));
            list.Add(new ListItem("fox", font));
            list.Add(new ListItem("jumps", font));
            list.Add(new ListItem("over", font));
            list.Add(new ListItem("the", font));
            list.Add(new ListItem("lazy", font));
            list.Add(new ListItem("dog", font));
            document.Add(list);
            document.Close();
        }
    }
}