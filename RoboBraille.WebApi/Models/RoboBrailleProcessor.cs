using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// A repository class of text processing methods
    /// </summary>
    public class RoboBrailleProcessor
    {
        private static readonly Dictionary<int, char> SixDotNumberMapping = new Dictionary<int, char> {
            {0, (char)0x281A},
            {1, (char)0x2801},
            {2, (char)0x2803},
            {3, (char)0x2809},
            {4, (char)0x2819},
            {5, (char)0x2811},
            {6, (char)0x280B},
            {7, (char)0x281B},
            {8, (char)0x2813},
            {9, (char)0x280A},
            {-1, (char)0x283C}
    };
        public static byte[] GetInputFileHash(byte[] file)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(file);
            }
        }
        public static bool IsSameJobProcessing(Job job, RoboBrailleDataContext context)
        {
            byte[] fileHash = null;
            using (var md5 = MD5.Create())
            {
                fileHash = md5.ComputeHash(job.FileContent);
            }
            //using (var context = new RoboBrailleDataContext())
            //{
            List<Job> sameJobs = (from j in context.Jobs where j.Status == JobStatus.Started && j.FileName == job.FileName && j.MimeType == job.MimeType && j.FileExtension == job.FileExtension && j.InputFileHash == fileHash select j).ToList();
            if (sameJobs.Count > 0)
                return true;
            else return false;
            //}
        }

        public static Guid getUserIdFromJob(string jobParameter)
        {
            string authParam = jobParameter;
            int i = authParam.IndexOf("id=\"") + 4;
            authParam = authParam.Substring(i);
            i = authParam.IndexOf("\"");
            string userId = authParam.Substring(0, i);
            Guid resultGuid;
            Guid.TryParse(userId, out resultGuid);
            return resultGuid;
        }
        public static void UpdateDownloadCounterInDb(Guid jobId, RoboBrailleDataContext context)
        {
            var task = Task.Factory.StartNew(j =>
            {
                //using (var context = new RoboBrailleDataContext())
                //{
                try
                {
                    Job job = context.Jobs.Find(jobId);
                    if (job != null)
                        if (job.Status.Equals(JobStatus.Done))
                        {
                            job.DownloadCounter = job.DownloadCounter + 1;
                            context.Jobs.Attach(job);
                            context.Entry(job).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                }
                catch (Exception)
                {
                    // ignored
                }
                //}
            }, jobId);
        }
        public static Job CheckForJobInDatabase(Guid jobId, RoboBrailleDataContext context)
        {
            //using (var context = new RoboBrailleDataContext())
            //{
            try
            {
                Job existingJob = context.Jobs.Find(jobId);
                if (existingJob != null)
                    if (existingJob.Status.Equals(JobStatus.Done))
                        return existingJob;
                    else return null;
                else return null;
            }
            catch (Exception)
            {
                // ignored
                return null;
            }
            //}
        }
        public static void SetJobFaulted(Job job, RoboBrailleDataContext context)
        {
            //using (var context = new RoboBrailleDataContext())
            //{
            try
            {
                job.Status = JobStatus.Error;
                job.FinishTime = DateTime.Now;
                context.Jobs.Attach(job);
                context.Entry(job).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception)
            {
                // ignored
            }
            //}
        }

        public static Encoding GetEncoding(byte[] bom, Language lang)
        {
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            return GetEncodingByCountryCode(lang);
        }
        /// <summary>
        /// Programatically determines the encoding based on the input language and the known encoding used in that country/region
        /// </summary>
        /// <param name="lang">A language parameter</param>
        /// <returns>The approprieate encoding</returns>
        public static Encoding GetEncodingByCountryCode(Language lang)
        {
            Encoding res = Encoding.ASCII;
            switch (lang)
            {
                case Language.ruRU:
                case Language.bgBG:
                    res = Encoding.GetEncoding(1251);
                    break;
                case Language.elGR:
                    res = Encoding.GetEncoding(1253);
                    break;
                case Language.enGB:
                case Language.enUS:
                case Language.deDE:
                case Language.esES:
                case Language.frFR:
                case Language.itIT:
                case Language.ptPT:
                case Language.daDK:
                case Language.nnNO:
                case Language.nbNO:
                case Language.isIS:
                case Language.svSE:
                    res = Encoding.GetEncoding(1252);
                    break;
                case Language.slSI:
                case Language.huHU:
                case Language.plPL:
                case Language.roRO:
                    res = Encoding.GetEncoding(1250);
                    break;
                case Language.ltLT:
                    res = Encoding.GetEncoding(1257);
                    break;
                default:
                    res = Encoding.ASCII;
                    break;
            }
            return res;
        }

        /// <summary>
        /// Formats the braille text input based on the braille job parameters
        /// </summary>
        /// <param name="source">the input text</param>
        /// <param name="job">the BrailleJob class</param>
        /// <returns>the formatted text</returns>
        public static string FormatBraille(string source, BrailleJob job)
        {
            int cols = job.CharactersPerLine;
            int rows = job.LinesPerPage;
            //bool isPef = OutputFormat.Pef.Equals(job.OutputFormat);
            //bool isUni = OutputFormat.Unicode.Equals(job.OutputFormat);
            string result = source;
            try
            {
                Encoding enc = RoboBrailleProcessor.GetEncodingByCountryCode(job.BrailleLanguage);
                switch (job.OutputFormat)
                {
                    case OutputFormat.Pef:
                        {
                            result = RoboBrailleProcessor.ConvertBrailleToUnicode(enc.GetBytes(result));

                            if (cols > 10 && rows > 2 && !String.IsNullOrWhiteSpace(result))
                            {
                                result = RoboBrailleProcessor.CreatePagination(result, cols);
                                if (PageNumbering.none.Equals(job.PageNumbering))
                                    result = RoboBrailleProcessor.CreatePefFromUnicode(result, cols, rows, 0, true);
                                else
                                    result = RoboBrailleProcessor.CreatePefFromUnicodeWithPageNumber(result, cols, rows, 0, true, PageNumbering.right.Equals(job.PageNumbering));
                            }
                            break;
                        }
                    case OutputFormat.Unicode:
                        {
                            result = RoboBrailleProcessor.ConvertBrailleToUnicode(enc.GetBytes(result));

                            if (cols > 10 && rows > 2 && !String.IsNullOrWhiteSpace(result))
                            {
                                result = RoboBrailleProcessor.CreatePagination(result, cols);
                                result = RoboBrailleProcessor.CreateNewLine(result, rows);
                            }
                            break;
                        }
                    case OutputFormat.NACB:
                        {
                            result = RoboBrailleProcessor.ConvertBrailleToUnicode(enc.GetBytes(result));
                            result = enc.GetString(RoboBrailleProcessor.ConvertUnicodeToNACB(result)).ToLowerInvariant();

                            if (cols > 10 && rows > 2 && !String.IsNullOrWhiteSpace(result))
                            {
                                result = RoboBrailleProcessor.CreatePagination(result, cols);
                                result = RoboBrailleProcessor.CreateNewLine(result, rows);
                            }
                            break;
                        }
                    default:
                        {
                            if (cols > 10 && rows > 2 && !String.IsNullOrWhiteSpace(result))
                            {
                                result = RoboBrailleProcessor.CreatePagination(result, cols);
                                result = RoboBrailleProcessor.CreateNewLine(result, rows);
                            }
                            break;
                        }
                }
                return result;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return source;
            }
        }

        /// <summary>
        /// Adds a new line to the text
        /// </summary>
        /// <param name="source">Must not be empty or null</param>
        /// <param name="LinesPerPage">Must be greater than 1</param>
        /// <returns>the formatted text</returns>

        public static string CreateNewLine(string source, int LinesPerPage)
        {
            string sourceCopy = source;
            try
            {
                if (LinesPerPage > 1)
                {
                    string[] arrSource = source.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    List<string> result = new List<string>();
                    for (int i = 1; i <= arrSource.Length; i++)
                    {
                        result.Add(arrSource[i - 1] + Environment.NewLine);
                        if (i % LinesPerPage == 0)
                        {
                            result.Add(Environment.NewLine);
                        }
                    }
                    string finalString = null;
                    foreach (string s in result)
                    {
                        finalString += s;
                    }
                    for (int i = 0; i <= 2; i++)
                        finalString = finalString.Replace("\r\n\r\n\r\n", "\r\n\r\n");
                    return finalString;
                }
                else return sourceCopy;
            }
            catch
            {
                return sourceCopy;
            }
        }

        /// <summary>
        /// Creates pagination
        /// </summary>
        /// <param name="source">input text</param>
        /// <param name="CharPerLine">greater than 10</param>
        /// <returns>The formatted text</returns>
        public static string CreatePagination(string source, int CharPerLine)
        {
            try
            {
                string result = "";
                string[] words = source.Split(new char[] { ' ', '\u2800', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i <= words.Length - 1; i++)
                {
                    string oneLine = "";
                    if (words[i].Length <= CharPerLine)
                    {
                        int lineLength = oneLine.Length + words[i].Length;
                        while (lineLength <= CharPerLine)
                        {
                            oneLine += words[i] + " ";
                            i++;
                            if (i >= words.Length - 1)
                            {
                                i--;
                                break;
                            }
                            lineLength = oneLine.Length + words[i].Length;
                        }
                        if (lineLength > CharPerLine)
                        {
                            i--;
                        }
                    }
                    else
                    {
                        string longWord = words[i];
                        while (longWord.Length > CharPerLine)
                        {
                            result += longWord.Substring(0, CharPerLine) + Environment.NewLine;
                            longWord = longWord.Substring(CharPerLine + 1);
                        }
                    }
                    oneLine = oneLine.Trim();
                    result += oneLine + Environment.NewLine;
                }
                return result;
            }
            catch (Exception e)
            {
                return source;
            }
        }

        /// <summary>
        /// Creates a pef document
        /// </summary>
        /// <param name="source">the input text</param>
        /// <param name="cols">the number of columns</param>
        /// <param name="rows">number of rows</param>
        /// <param name="rowgap">set to 0</param>
        /// <param name="duplex">set to true</param>
        /// <param name="isNumberOnRight">true if page numbering is on right, false if it is on the left</param>
        /// <returns>the pef document</returns>
        public static string CreatePefFromUnicodeWithPageNumber(string source, int cols, int rows, int rowgap, bool duplex, bool isNumberOnRight)
        {

            string pefHeader = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<pef version=""2008-1"" xmlns=""http://www.daisy.org/ns/2008/pef"">
<head>
  <meta xmlns:dc=""http://purl.org/dc/elements/1.1/"">
    <dc:format>application/x-pef+xml</dc:format>
    <dc:identifier>Sensus Braille</dc:identifier>
  </meta>
</head>
<body>
  <volume cols=""" + cols + @""" rows=""" + rows + @""" rowgap=""" + rowgap + @""" duplex=""" + duplex + @""">
    <section>" + Environment.NewLine;
            string pefFooter = @"    </section>
  </volume>
</body>
</pef>";
            string pefPageStart = @"      <page>" + Environment.NewLine;
            string pefPageEnd = @"      </page>" + Environment.NewLine;
            string pefRowStart = @"        <row>";
            string pefRowEnd = @"</row>" + Environment.NewLine;
            string sourceCopy = source;
            //TODO always keep the last row empty for page numbering
            try
            {
                string space = "";
                for (int j = 0; j < cols; j++)
                    space += " ";
                string[] arrSource = source.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                List<string> result = new List<string>();
                result.Add(pefHeader + pefPageStart);
                for (int i = 0; i < arrSource.Length; i++)
                {
                    int rSpace = cols - arrSource[i].Length;
                    string difSpace = "";
                    for (int j = 0; j < rSpace; j++)
                        difSpace += " ";
                    if ((i + 1) % rows == 0)
                    {
                        string brNum = CreateSixBrailleFromNumber((i + 1) / 10);
                        if (isNumberOnRight)
                            result.Add(pefRowStart + space.Substring(brNum.Length) + brNum + pefRowEnd);
                        else
                            result.Add(pefRowStart + brNum + space.Substring(brNum.Length) + pefRowEnd);
                        result.Add(pefPageEnd + pefPageStart);
                        result.Add(pefRowStart + arrSource[i] + difSpace + pefRowEnd);
                    }
                    else
                        result.Add(pefRowStart + arrSource[i] + difSpace + pefRowEnd);

                }
                if (arrSource.Length % rows != 0)
                {
                    int remainRows = rows - arrSource.Length % rows;
                    for (int i = 1; i < remainRows; i++)
                        result.Add(pefRowStart + space + pefRowEnd);
                    string lastPageBrNum = CreateSixBrailleFromNumber(arrSource.Length / rows + 1);
                    if (isNumberOnRight)
                        result.Add(pefRowStart + space.Substring(lastPageBrNum.Length) + lastPageBrNum + pefRowEnd);
                    else
                        result.Add(pefRowStart + lastPageBrNum + space.Substring(lastPageBrNum.Length) + pefRowEnd);

                }
                result.Add(pefPageEnd + pefFooter);
                string finalString = null;
                foreach (string s in result)
                {
                    finalString += s;
                }
                return finalString;
            }
            catch
            {
                return sourceCopy;
            }
        }

        /// <summary>
        /// Creates numbers in six dot braille
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static string CreateSixBrailleFromNumber(int num)
        {
            StringBuilder result = new StringBuilder();
            result.Append(SixDotNumberMapping[-1]);
            foreach (int i in digitArr(num))
            {
                result.Append(SixDotNumberMapping[i]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Used to create the Six dot braille numbers
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static int[] digitArr(int n)
        {
            if (n == 0) return new int[1] { 0 };

            var digits = new List<int>();

            for (; n != 0; n /= 10)
                digits.Add(n % 10);

            var arr = digits.ToArray();
            Array.Reverse(arr);
            return arr;
        }

        /// <summary>
        /// creates a Pef document without page numbering. same as above.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <param name="rowgap"></param>
        /// <param name="duplex"></param>
        /// <returns></returns>
        public static string CreatePefFromUnicode(string source, int cols, int rows, int rowgap, bool duplex)
        {
            string pefHeader = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<pef version=""2008-1"" xmlns=""http://www.daisy.org/ns/2008/pef"">
<head>
  <meta xmlns:dc=""http://purl.org/dc/elements/1.1/"">
    <dc:format>application/x-pef+xml</dc:format>
    <dc:identifier>Sensus Braille</dc:identifier>
  </meta>
</head>
<body>
  <volume cols=""" + cols + @""" rows=""" + rows + @""" rowgap=""" + rowgap + @""" duplex=""" + duplex + @""">
    <section>" + Environment.NewLine;
            string pefFooter = @"    </section>
  </volume>
</body>
</pef>";
            string pefPageStart = @"      <page>" + Environment.NewLine;
            string pefPageEnd = @"      </page>" + Environment.NewLine;
            string pefRowStart = @"        <row>";
            string pefRowEnd = @"</row>" + Environment.NewLine;
            string sourceCopy = source;
            //TODO always keep the last row empty for page numbering
            try
            {
                string[] arrSource = source.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                List<string> result = new List<string>();
                result.Add(pefHeader + pefPageStart);
                for (int i = 0; i < arrSource.Length; i++)
                {
                    int rSpace = cols - arrSource[i].Length;
                    StringBuilder space = new StringBuilder();
                    for (int j = 0; j < rSpace; j++)
                        space.Append('\u2800');
                    result.Add(pefRowStart + arrSource[i] + space.ToString() + pefRowEnd);
                    if ((i + 1) % rows == 0)
                        result.Add(pefPageEnd + pefPageStart);
                }
                if (arrSource.Length % rows != 0)
                {
                    StringBuilder space = new StringBuilder();
                    for (int j = 0; j < cols; j++)
                        space.Append('\u2800');
                    int remainRows = rows - arrSource.Length % rows;
                    for (int i = 0; i < remainRows; i++)
                        result.Add(pefRowStart + space.ToString() + pefRowEnd);
                }
                result.Add(pefPageEnd + pefFooter);
                string finalString = null;
                foreach (string s in result)
                {
                    finalString += s;
                }
                return finalString;
            }
            catch
            {
                return sourceCopy;
            }
        }

        /// <summary>
        /// Converts unicode braille into North American Computer Braille
        /// </summary>
        /// <param name="source">Input text</param>
        /// <returns>a byte array with containing the converted text</returns>
        public static byte[] ConvertUnicodeToNACB(string source)
        {
            byte[] nacbbraille =
            {
            32, 65, 49, 66, 39, 75, 50, 76, 64, 67, 73, 70, 47, 77, 83, 80,
            34, 69, 51, 72, 57, 79, 54, 82, 94, 68, 74, 71, 62, 78, 84, 81,
            44, 42, 53, 60, 45, 85, 56, 86, 46, 37, 91, 36, 43, 88, 33, 38,
            59, 58, 52, 92, 48, 90, 55, 40, 95, 63, 87, 93, 35, 89, 41, 61,//--> mapping for unicode 6 dot range to nacb
            
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32//--> invalid mapppings outside of the 6 dot range
        };

            string result = null;
            foreach (string s1 in source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                byte[] bsource = Encoding.Unicode.GetBytes(s1); //UTF-16 little eadian
                byte[] data = new byte[bsource.Length / 2];
                int k = 0;
                for (int i = 0; i < bsource.Length; i = i + 2)
                {
                    data[k] = bsource[i];
                    k++;
                }
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = nacbbraille[data[i]];
                }
                result += Encoding.GetEncoding(1252).GetString(data) + Environment.NewLine;
            }
            return Encoding.GetEncoding(1252).GetBytes(result);
        }

        /// <summary>
        /// Converts Unicode text into Octobraille ANSI
        /// </summary>
        /// <param name="source">input string</param>
        /// <returns>byte array with unicode text</returns>
        public static byte[] ConvertUnicodeToOctoBraille(string source2)
        {
            byte[] octobraille =
            {
            32, 97, 44, 98, 46, 107, 59, 108, 39, 99,
            105, 102, 191, 109, 115, 112, 96, 101, 58,
            104, 42, 111, 33, 114, 129, 100, 106, 103,
            230, 110, 116, 113, 133, 229, 63, 234, 150,
            117, 181, 118, 152, 238, 248, 235, 158, 120,
            232, 231, 168, 251, 161, 252, 176, 122, 34,
            224, 139, 244, 119, 239, 190, 121, 249, 233,
            186, 65, 143, 66, 149, 75, 147, 76, 145, 67,
            73, 70, 92, 77, 83, 80, 171, 69, 13, 72, 144,
            79, 134, 82, 130, 68, 74, 71, 198, 78, 84, 81,
            165, 197, 62, 202, 151, 85, 164, 86, 126, 206,
            216, 203, 142, 88, 200, 199, 187, 219, 157, 220,
            141, 90, 175, 192, 155, 212, 87, 207, 172, 89,
            217, 201, 0, 49, 185, 50, 183, 254, 178, 163,
            146, 51, 57, 54, 47, 253, 154, 94, 131, 53,
            179, 56, 60, 156, 43, 15, 189, 52, 48, 55,
            228, 188, 160, 177, 25, 226, 20, 237, 45, 11,
            40, 29, 180, 227, 246, 242, 243, 215, 223, 38,
            148, 236, 247, 250, 41, 18, 61, 225, 124, 245,
            5, 241, 35, 240, 255, 182, 24, 1, 22, 2, 173,
            222, 132, 12, 64, 3, 9, 6, 166, 221, 138, 16,
            167, 128, 162, 8, 23, 140, 135, 174, 19, 4,
            37, 7, 196, 14, 153, 17, 10, 194, 27, 205,
            95, 21, 91, 123, 184, 195, 214, 210, 211, 169,
            170, 30, 136, 204, 36, 218, 93, 26, 31, 193,
            28, 213, 137, 209, 125, 208, 159, 127
            };
            string result = null;
            foreach (string source in source2.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                byte[] bsource = Encoding.Unicode.GetBytes(source); //UTF-16 little eadian
                byte[] data = new byte[bsource.Length / 2];
                int k = 0;
                for (int i = 0; i < bsource.Length; i = i + 2)
                {
                    data[k] = bsource[i];
                    k++;
                }
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = octobraille[data[i]];
                }
                result += Encoding.GetEncoding(1252).GetString(data) + Environment.NewLine;
            }
            return Encoding.GetEncoding(1252).GetBytes(result);
        }

        /// <summary>
        /// Converts Octobraille byte array into a unicode braille string
        /// </summary>
        /// <param name="data">The input byte array of the text to input</param>
        /// <returns>the string in unicode braille</returns>
        public static string ConvertBrailleToUnicode(byte[] data)
        {
            byte[] unibraille =
                    {
                        128, 193, 195, 201, 217, 186, 203, 219, 211, 202,
                        224, 165, 199, 82, 221, 151, 207, 223, 181, 216,
                        162, 229, 194, 212, 192, 160, 245, 226, 248, 167,
                        239, 246, 0, 22, 54, 188, 242, 218, 175, 8,
                        166, 180, 20, 150, 2, 164, 4, 140, 154, 129,
                        131, 137, 153, 145, 139, 155, 147, 138, 18, 6,
                        148, 182, 98, 34, 200, 65, 67, 73, 89, 81,
                        75, 91, 83, 74, 90, 69, 71, 77, 93, 85,
                        79, 95, 87, 78, 94, 101, 103, 122, 109, 125,
                        117, 230, 76, 244, 143, 228, 16, 1, 3, 9,
                        25, 17, 11, 27, 19, 10, 26, 5, 7, 13,
                        29, 21, 15, 31, 23, 14, 30, 37, 39, 58,
                        45, 61, 53, 231, 184, 252, 104, 255, 209, 24,
                        88, 144, 198, 32, 86, 214, 240, 250, 206, 56,
                        213, 116, 108, 66, 84, 72, 136, 70, 176, 68,
                        36, 100, 40, 222, 142, 120, 149, 114, 44, 254,
                        158, 50, 210, 135, 102, 96, 204, 208, 48, 237,
                        238, 80, 124, 196, 215, 118, 52, 159,134, 146,
                        168, 38, 191, 132, 232, 130, 64, 112, 157, 152,
                        60, 12, 119, 247, 225, 233, 220, 97, 92, 111,
                        110, 127, 99, 107, 241, 227, 105, 123, 253, 251,
                        235, 236, 121, 249, 234, 173, 106, 126, 243, 113,
                        115, 205, 197, 174, 55, 183, 161, 169, 156, 33,
                        28, 47, 46, 63, 35, 43, 177, 163, 41, 59,
                        189, 187, 171, 172, 57, 185, 170, 178, 42, 62,
                        179, 49, 51, 141, 133, 190
                    };
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = unibraille[data[i]];
            }
            char[] unidata = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                unidata[i] = (char)(0x2800 | data[i]);
            }
            return new String(unidata);
        }
    }
}