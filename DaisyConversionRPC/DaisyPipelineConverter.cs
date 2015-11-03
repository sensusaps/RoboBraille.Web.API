using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaisyConversionRPC
{
    public class DaisyPipelineConverter
    {
        public string jobid { get; set; }

        private static string FileDirectory = ConfigurationManager.AppSettings.Get("FileDirectory");
        private string _javadir = "", XmlDirectory, BookDirectory, Epub3Directory, ZipFilePath;
        private static string DaisyOutputDirectory = FileDirectory + @"Daisy\";
        private static string Pipeline2Directory = FileDirectory + @"DaisyPipeline2\cli";
        private static string Pipeline1Directory = FileDirectory + @"DaisyPipeline";
        private static string OfficeAutomotionDirectory = FileDirectory + @"Daisy";

        /// <summary>
        /// Initialize the converter.
        /// </summary>
        /// <param name="id">A unique id used to track the job in the file system.</param>
        public DaisyPipelineConverter(string id)
        {
            jobid = id;
            XmlDirectory = DaisyOutputDirectory + @"DTBOOK\" + jobid;
            BookDirectory = DaisyOutputDirectory + @"Book\" + jobid;
            Epub3Directory = DaisyOutputDirectory + @"Epub3\" + jobid;
            ZipFilePath = DaisyOutputDirectory + @"Zip\" + jobid + ".zip";
        }

        public byte[] ManageDaisyConversion(byte[] inputDoc, bool isEpub3)
        {
            string DocSourcePath = FileDirectory + @"Source\" + jobid + @".docx";
            File.WriteAllBytes(DocSourcePath, inputDoc);
            byte[] zipContent = null;
            zipContent = ManageDaisyConversion(DocSourcePath, isEpub3);
            if (File.Exists(DocSourcePath))
            {
                File.Delete(DocSourcePath);
            }
            return zipContent;
        }

        public byte[] ManageDaisyConversion(string docFilePath, bool isEpub3)
        {
            byte[] content = null;
            this.DeleteDirectories(isEpub3);
            string result = ConvertDocxToDTBOOK(docFilePath);
            ConvertDTBOOKToTalkingBook(result);
            if (isEpub3)
                content = ConvertTalkingBookToEPUBWMO(BookDirectory);
            else
                if (Directory.Exists(BookDirectory))
                {
                    ZipFile.CreateFromDirectory(BookDirectory, ZipFilePath);
                    content = File.ReadAllBytes(ZipFilePath);
                }
            this.DeleteDirectories(isEpub3);
            return content;
        }

        public string ConvertDocxToDTBOOK(string docFilePath)
        {
            if (Directory.Exists(XmlDirectory))
            {
                Directory.Delete(XmlDirectory, true);
            }
            string SourceFile = docFilePath;
            Directory.CreateDirectory(XmlDirectory);
            string CommandLineArgs = @" """ + SourceFile + @""" """ + XmlDirectory + @"""";

            try
            {
                string result = null;
                Process p = new Process();
                p.StartInfo.FileName = OfficeAutomotionDirectory + @"\OfficeAutomationSample.exe";
                p.StartInfo.Arguments = CommandLineArgs;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.WorkingDirectory = OfficeAutomotionDirectory;
                bool flag = p.Start();
                p.WaitForExit();
                File.AppendAllText(FileDirectory + @"errorLog.txt", "flag= " + flag + Environment.NewLine);
                if (p.HasExited)
                {
                    result = XmlDirectory + @"\" + Path.GetFileNameWithoutExtension(docFilePath) + ".xml";
                }
                return result;
            }
            catch (Exception ex)
            {
                File.AppendAllText(FileDirectory + @"errorLog.txt", ex.Message + Environment.NewLine);
                return ex.Message;
            }
        }

        public void ConvertDTBOOKToTalkingBook(string dtbookFile)
        {
            LocateJava();
            string SourceFile = dtbookFile;
            if (Directory.Exists(BookDirectory))
            {
                Directory.Delete(BookDirectory, true);
            }
            if (File.Exists(ZipFilePath))
            {
                File.Delete(ZipFilePath);
            }
            string CommandLineArgs = @"-classpath """ + Pipeline1Directory + @"\pipeline.jar"";""."" org.daisy.pipeline.ui.CommandLineUI " + Pipeline1Directory + @"\scripts\create_distribute\dtb\Narrator-DtbookToDaisy.taskScript ""--input=" + SourceFile + @""" """ + @"--outputPath=" + BookDirectory + @""" ""--multiLang=true""";

            try
            {
                string temp = Environment.CurrentDirectory;
                Environment.CurrentDirectory = Pipeline1Directory;
                ProcessStartInfo start = new ProcessStartInfo();
                if (!_javadir.Equals(String.Empty))
                {
                    start.FileName = this._javadir + "java.exe";
                }
                else
                {
                    start.FileName = "java.exe";
                }
                start.FileName = "java";
                start.Arguments = CommandLineArgs;
                start.UseShellExecute = false;
                start.RedirectStandardInput = false;
                start.RedirectStandardOutput = false;
                start.CreateNoWindow = true;
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.WorkingDirectory = Pipeline1Directory;

                Process p = new Process();
                p.StartInfo = start;
                p.Start();
                p.WaitForExit();
                Environment.CurrentDirectory = temp;
                if (p.HasExited)
                {
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ConvertDTBOOKToTalkingBook " + ex.Message);
            }
        }

        public byte[] ConvertTalkingBookToEPUBWMO(string BookDirectory)
        {
            string CommandName = null, CommandLine = null;
            string SourceFile = BookDirectory + @"\daisy202\ncc.html";
            if (!File.Exists(SourceFile))
            {
                return null;
            }
            if (Directory.Exists(Epub3Directory))
            {
                Directory.Delete(Epub3Directory, true);
            }
            CommandName = Pipeline2Directory + @"\dp2.exe";
            CommandLine = "daisy202-to-epub3 --x-href file:///" + SourceFile + " --output " + Epub3Directory + " --x-mediaoverlay true --x-compatibility-mode true";

            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.WorkingDirectory = Pipeline2Directory;
                start.FileName = CommandName;
                start.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = new Process();
                p.StartInfo = start;
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
            }

            byte[] result = null;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = CommandName;
                start.Arguments = CommandLine;
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.WorkingDirectory = Pipeline2Directory;
                Process p = new Process();
                p.StartInfo = start;
                p.Start();
                p.WaitForExit();
                if (p.HasExited)
                {
                    if (Directory.Exists(Epub3Directory))
                    {
                        File.Copy(Directory.EnumerateFiles(Epub3Directory + @"\output").First(), Epub3Directory + @".epub");
                        result = File.ReadAllBytes(Epub3Directory + @".epub");
                    }
                    else return null;
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteDirectories(bool alsoEpub)
        {
            bool succes = true;
            if (Directory.Exists(DaisyOutputDirectory + @"DTBOOK\" + jobid))
                Directory.Delete(DaisyOutputDirectory + @"DTBOOK\" + jobid, true);
            else succes = false;
            if (Directory.Exists(DaisyOutputDirectory + @"Book\" + jobid))
                Directory.Delete(DaisyOutputDirectory + @"Book\" + jobid, true);
            else succes = false;
            if (File.Exists(DaisyOutputDirectory + @"Zip\" + jobid + ".zip"))
                File.Delete(DaisyOutputDirectory + @"Zip\" + jobid + ".zip");
            else succes = false;
            if (alsoEpub && Directory.Exists(DaisyOutputDirectory + @"Epub3\" + jobid))
                Directory.Delete(DaisyOutputDirectory + @"Epub3\" + jobid, true);
            if (alsoEpub && File.Exists(Epub3Directory + @".epub"))
                File.Delete(Epub3Directory + @".epub");
            else succes = false || alsoEpub;
            return succes;
        }

        private void LocateJava()
        {
            String path = Environment.GetEnvironmentVariable("path");
            String[] folders = path.Split(';');
            foreach (String folder in folders)
            {
                if (File.Exists(folder + "java.exe"))
                {
                    this._javadir = folder;
                    return;
                }
                else if (File.Exists(folder + "\\java.exe"))
                {
                    this._javadir = folder + "\\";
                    return;
                }
            }
        }
    }
}