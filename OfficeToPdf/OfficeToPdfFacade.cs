using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OfficeToPDF
{
    // Set up the exit codes
    [Flags]
    public enum ExitCode : int
    {
        Success = 0,
        Failed = 1,
        UnknownError = 2,
        PasswordFailure = 4,
        InvalidArguments = 8,
        FileOpenFailure = 16,
        UnsupportedFileFormat = 32,
        FileNotFound = 64,
        DirectoryNotFound = 128
    }
    public class OfficeToPdfFacade
    {
        private static string message = null;
        public static string GetMessage()
        {
            return message;
        }
        public static int ConvertOfficeToPDF(string[] args)
        {
            string[] files = new string[2];
            int filesSeen = 0;
            Hashtable options = new Hashtable
            {

                // Loop through the input, grabbing switches off the command line
                ["hidden"] = false,
                ["markup"] = false,
                ["readonly"] = false,
                ["bookmarks"] = false,
                ["print"] = true,
                ["screen"] = false,
                ["pdfa"] = false,
                ["verbose"] = false,
                ["excludeprops"] = false,
                ["excludetags"] = false,
                ["noquit"] = false,
                ["merge"] = false,
                ["template"] = "",
                ["password"] = "",
                ["excel_show_formulas"] = false,
                ["excel_show_headings"] = false,
                ["excel_auto_macros"] = false,
                ["excel_max_rows"] = (int)0,
                ["word_header_dist"] = (float)-1,
                ["word_footer_dist"] = (float)-1
            };
            Regex switches = new Regex(@"^/(hidden|markup|readonly|bookmarks|merge|noquit|print|screen|pdfa|template|writepassword|password|help|verbose|exclude(props|tags)|excel_max_rows|excel_show_formulas|excel_show_headings|excel_auto_macros|word_header_dist|word_footer_dist|\?)$", RegexOptions.IgnoreCase);
            for (int argIdx = 0; argIdx < args.Length; argIdx++)
            {
                string item = args[argIdx];
                // see if this starts with a /
                Match m = Regex.Match(item, @"^/");
                if (m.Success)
                {
                    // This is an option
                    Match itemMatch = switches.Match(item);
                    if (itemMatch.Success)
                    {
                        if (itemMatch.Groups[1].Value.ToLower().Equals("help") ||
                            itemMatch.Groups[1].Value.Equals("?"))
                        {
                            ShowHelp();
                        }
                        switch (itemMatch.Groups[1].Value.ToLower())
                        {
                            case "template":
                                // Only accept the next option if there are enough options
                                if (argIdx + 2 < args.Length)
                                {
                                    if (File.Exists(args[argIdx + 1]))
                                    {
                                        FileInfo templateInfo = new FileInfo(args[argIdx + 1]);
                                        options[itemMatch.Groups[1].Value.ToLower()] = templateInfo.FullName;
                                    }
                                    else
                                    {
                                        message = "Unable to find "+ itemMatch.Groups[1].Value.ToLower()+" "+ args[argIdx + 1];
                                    }
                                    argIdx++;
                                }
                                break;
                            case "excel_max_rows":
                                // Only accept the next option if there are enough options
                                if (argIdx + 2 < args.Length)
                                {
                                    if (Regex.IsMatch(args[argIdx + 1], @"^\d+$"))
                                    {
                                        options[itemMatch.Groups[1].Value.ToLower()] = (int)Convert.ToInt32(args[argIdx + 1]);
                                    }
                                    else
                                    {
                                        message = "Maximum number of rows ("+args[argIdx + 1]+") is invalid";
                                        return ((int)(ExitCode.Failed | ExitCode.InvalidArguments));
                                    }
                                    argIdx++;
                                }
                                break;
                            case "word_header_dist":
                            case "word_footer_dist":
                                // Only accept the next option if there are enough options
                                if (argIdx + 2 < args.Length)
                                {
                                    if (Regex.IsMatch(args[argIdx + 1], @"^[\d\.]+$"))
                                    {
                                        try
                                        {

                                            options[itemMatch.Groups[1].Value.ToLower()] = (float)Convert.ToDouble(args[argIdx + 1]);
                                        }
                                        catch (Exception)
                                        {
                                            message = "Header/Footer distance ("+args[argIdx + 1]+") is invalid";
                                            return ((int)(ExitCode.Failed | ExitCode.InvalidArguments));
                                        }
                                    }
                                    else
                                    {
                                        message = "Header/Footer distance ("+args[argIdx + 1]+") is invalid";
                                        return ((int)(ExitCode.Failed | ExitCode.InvalidArguments));
                                    }
                                    argIdx++;
                                }
                                break;
                            case "password":
                            case "writepassword":
                                // Only accept the next option if there are enough options
                                if (argIdx + 2 < args.Length)
                                {
                                    options[itemMatch.Groups[1].Value.ToLower()] = args[argIdx + 1];
                                    argIdx++;
                                }
                                break;
                            case "screen":
                                options["print"] = false;
                                options[itemMatch.Groups[1].Value.ToLower()] = true;
                                break;
                            case "print":
                                options["screen"] = false;
                                options[itemMatch.Groups[1].Value.ToLower()] = true;
                                break;
                            default:
                                options[itemMatch.Groups[1].Value.ToLower()] = true;
                                break;
                        }
                    }
                    else
                    {
                        message = "Unknown option: "+item;
                        return ((int)(ExitCode.Failed | ExitCode.InvalidArguments));
                    }
                }
                else if (filesSeen < 2)
                {
                    files[filesSeen++] = item;
                }
            }

            // Need to error here, as we need input and output files as the
            // arguments to this script
            if (filesSeen != 1 && filesSeen != 2)
            {
                ShowHelp();
            }

            // Make sure we only choose one of /screen or /print options
            if ((Boolean)options["screen"] && (Boolean)options["print"])
            {
                message = "You can only use one of /screen or /print - not both";
                return ((int)(ExitCode.Failed | ExitCode.InvalidArguments));
            }

            // Make sure the input file looks like something we can handle (i.e. has an office
            // filename extension)
            Regex fileMatch = new Regex(@"\.(((ppt|pps|pot|do[ct]|xls|xlt)[xm]?)|od[cpt]|rtf|csv|vsd[xm]?|pub|msg|vcf|ics|mpp|svg|txt|html?)$", RegexOptions.IgnoreCase);
            if (fileMatch.Matches(files[0]).Count != 1)
            {
                message = "Input file can not be handled. Must be Word, PowerPoint, Excel, Outlook, Publisher or Visio";
                return ((int)(ExitCode.Failed | ExitCode.UnsupportedFileFormat));
            }

            if (filesSeen == 1)
            {
                files[1] = Path.ChangeExtension(files[0], "pdf");
            }

            String inputFile = "";
            String outputFile;

            // Make sure the input file exists and is readable
            FileInfo info;
            try
            {
                info = new FileInfo(files[0]);

                if (info == null || !info.Exists)
                {
                    message = "Input file doesn't exist";
                    return ((int)(ExitCode.Failed | ExitCode.FileNotFound));
                }
                inputFile = info.FullName;
            }
            catch
            {
                message = "Unable to open input file";
                return ((int)(ExitCode.Failed | ExitCode.FileOpenFailure));
            }

            // Make sure the destination location exists
            FileInfo outputInfo = new FileInfo(files[1]);
            if (outputInfo != null && outputInfo.Exists)
            {
                System.IO.File.Delete(outputInfo.FullName);
            }
            if (!System.IO.Directory.Exists(outputInfo.DirectoryName))
            {
                message = "Output directory does not exist";
                return ((int)(ExitCode.Failed | ExitCode.DirectoryNotFound));
            }
            outputFile = outputInfo.FullName;

            // Now, do the cleverness of determining what the extension is, and so, which
            // conversion class to pass it to
            int converted = (int)ExitCode.UnknownError;
            Match extMatch = fileMatch.Match(inputFile);
            if (extMatch.Success)
            {
                if ((Boolean)options["verbose"])
                {
                    message = "Converting "+inputFile+" to "+ outputFile;
                }
                switch (extMatch.Groups[1].ToString().ToLower())
                {
                    case "rtf":
                    case "odt":
                    case "doc":
                    case "dot":
                    case "docx":
                    case "dotx":
                    case "docm":
                    case "dotm":
                    case "txt":
                    case "html":
                    case "htm":
                        // Word
                        if ((Boolean)options["verbose"])
                        {
                            message = "Converting with Word converter";
                        }
                        converted = WordConverter.Convert(inputFile, outputFile, options);
                        break;
                    case "csv":
                    case "odc":
                    case "xls":
                    case "xlsx":
                    case "xlt":
                    case "xltx":
                    case "xlsm":
                    case "xltm":
                        // Excel
                        if ((Boolean)options["verbose"])
                        {
                            message = "Converting with Excel converter";
                        }
                        converted = ExcelConverter.Convert(inputFile, outputFile, options);
                        break;
                    case "odp":
                    case "ppt":
                    case "pptx":
                    case "pptm":
                    case "pot":
                    case "potm":
                    case "potx":
                    case "pps":
                    case "ppsx":
                    case "ppsm":
                        // Powerpoint
                        if ((Boolean)options["verbose"])
                        {
                            message = "Converting with Powerpoint converter";
                        }
                        converted = PowerpointConverter.Convert(inputFile, outputFile, options);
                        break;
                    case "vsd":
                    case "vsdm":
                    case "vsdx":
                    case "svg":
                        // Visio
                        if ((Boolean)options["verbose"])
                        {
                            message = "Converting with Visio converter";
                        }
                        converted = VisioConverter.Convert(inputFile, outputFile, options);
                        break;
                    case "pub":
                        // Publisher
                        if ((Boolean)options["verbose"])
                        {
                            message = "Converting with Publisher converter";
                        }
                        converted = PublisherConverter.Convert(inputFile, outputFile, options);
                        break;
                    case "msg":
                    case "vcf":
                    case "ics":
                        // Outlook
                        if ((Boolean)options["verbose"])
                        {
                            message = "Converting with Outlook converter";
                        }
                        converted = OutlookConverter.Convert(inputFile, outputFile, options);
                        break;
                    case "mpp":
                        // Project
                        if ((Boolean)options["verbose"])
                        {
                            message = "Converting with Project converter";
                        }
                        converted = ProjectConverter.Convert(inputFile, outputFile, options);
                        break;
                }
            }
            if (converted != (int)ExitCode.Success)
            {
                message = "Did not convert";
                // Return the general failure code and the specific failure code
                return ((int)ExitCode.Failed | converted);
            }
            else if ((Boolean)options["verbose"])
            {
                message = "Completed Conversion";
            }
            return ((int)ExitCode.Success);
        }

        public static string ShowHelp()
        {
            return @"Converts Office documents to PDF from the command line.
Handles Office files:
  doc, dot, docx, dotx, docm, dotm, rtf, odt, txt, htm, html, ppt, pptx,
  pptm, pps, ppsx, ppsm, pot, potm, potx, odp, xls, xlsx, xlsm, xlt, xltm,
  xltx, csv, odc, vsd, vsdm, vsdx, svg, pub, mpp, ics, vcf, msg

OfficeToPDF.exe [/bookmarks] [/hidden] [/readonly] input_file [output_file]

  /bookmarks    - Create bookmarks in the PDF when they are supported by the
                  Office application
  /hidden       - Attempt to hide the Office application window when converting
  /markup       - Show document markup when creating PDFs with Word
  /readonly     - Load the input file in read only mode where possible
  /print        - Create high-quality PDFs optimised for print (default)
  /screen       - Create PDFs optimised for screen display
  /pdfa         - Produce ISO 19005-1 (PDF/A) compliant PDFs
  /excludeprops - Do not include properties in the PDF
  /excludetags  - Do not include tags in the PDF
  /noquit       - Do not quit already running Office applications once the conversion is done
  /verbose      - Print out messages as it runs
  /password <pass>          - Use <pass> as the password to open the document with
  /writepassword <pass>     - Use <pass> as the write password to open the document with
  /template <template_path> - Use a .dot, .dotx or .dotm template when
                              converting with Word
  /excel_auto_macros        - Run Auto_Open macros in Excel files before conversion
  /excel_show_formulas      - Show formulas in the generated PDF
  /excel_show_headings      - Show row and column headings
  /excel_max_rows <rows>    - If any worksheet in a spreadsheet document has more
                              than this number of rows, do not attempt to convert
                              the file. Applies when converting with Excel.
  /word_header_dist <pts>   - The distance (in points) from the header to the top of
                              the page.
  /word_footer_dist <pts>   - The distance (in points) from the footer to the bottom
                              of the page.
  
  input_file  - The filename of the Office document to convert
  output_file - The filename of the PDF to create. If not given, the input filename
                will be used with a .pdf extension
";
        }
    }
}
