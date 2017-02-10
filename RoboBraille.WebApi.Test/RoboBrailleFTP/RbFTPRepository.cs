using EnterpriseDT.Net.Ftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test.RoboBrailleFTP
{
    public static class RbFTPRepository
    {
        private static readonly string destFTPUser = "dediconftp";
        public static byte[] ProcessRoboBrailleFTP(SensusRequest sr) {
            byte[] result = null;
            string sreqFile = CreateServiceRequestFile(sr);
            int resp = UploadFileToFTPServer(Path.GetFileName(sr.SourcePath), Path.GetDirectoryName(sr.SourcePath), sr.FTPServer, destFTPUser, sr.FTPPassword);
            if (resp == 0)
            {
                resp = UploadFileToFTPServer(Path.GetFileName(sreqFile), Path.GetDirectoryName(sreqFile), sr.FTPServer, destFTPUser, sr.FTPPassword);
                if (resp != 0)
                    throw new Exception("Unable to upload sreq file Code: " + resp);
            }
            else throw new Exception("Unable to upload source file Code: " + resp);
            sr.TempSResp = Path.GetFileName(Path.ChangeExtension(sreqFile,"srsp"));
            //get result maybe needs a loop to constantly check something?
            DateTime dt = DateTime.Now.AddMinutes(3);
            bool run = true;
            while (run)
            {
                Task.Delay(1000);
                byte[] res = DownloadFileFromFTP(sr);
                if (res != null)
                {
                    DeleteFilesFromFTP(sr);
                    result = res;
                } else
                {
                    if (dt.CompareTo(DateTime.Now)<0)
                        run = false;
                }
            }
            if (run==false)
            {
                throw new TimeoutException("no result file on ftp");
            }
            return result;
        }
        private static bool DeleteFilesFromFTP(SensusRequest sr) {
            bool res = false;

            FTPConnection ftp = new FTPConnection();
            ftp.ServerAddress = sr.FTPServer;
            ftp.UserName = sr.FTPUser;
            ftp.Password = sr.FTPPassword;
            ftp.ConnectMode = FTPConnectMode.PASV;
            ftp.TransferType = FTPTransferType.BINARY;

            try
            {
                ftp.Connect();
                res = ftp.DeleteFile(sr.DestinationFile);
                res &= ftp.DeleteFile(sr.TempSResp);
            }
            catch (Exception ex)
            {
                res = false;
            }
            finally
            {
                ftp.Close();
                ftp.Dispose();
            }
            return res;
        }
        private static byte[] DownloadFileFromFTP(SensusRequest sr)
        {
            byte[] res = null;
            FTPConnection ftp = new FTPConnection();

            ftp.ServerAddress = sr.FTPServer;
            ftp.UserName = sr.FTPUser;
            ftp.Password = sr.FTPPassword;
            ftp.ConnectMode = FTPConnectMode.PASV;
            ftp.TransferType = FTPTransferType.BINARY;

            try
            {
                //DisplayLine("Connecting to FTP server " + FTPServer);
                ftp.Connect();
                //DisplayLine("Connected to FTP server " + FTPServer);
                res = ftp.DownloadByteArray(sr.DestinationFile);
            }
            catch (Exception ex)
            {
                //DisplayLine("Connecting to FTP server " + FTPServer + " failed");
                res = null;
            }
            finally
            {
                ftp.Close();
                ftp.Dispose();
            }
            return res;
        }

        private static int UploadFileToFTPServer(string Filename, string FilePath, string FTPServer, string FTPUser, string FTPPassword)
        {
            FTPConnection ftp = new FTPConnection();

            ftp.ServerAddress = FTPServer;
            ftp.UserName = FTPUser;
            ftp.Password = FTPPassword;
            ftp.ConnectMode = FTPConnectMode.PASV;
            ftp.TransferType = FTPTransferType.BINARY;

            try
            {
                //DisplayLine("Connecting to FTP server " + FTPServer);
                ftp.Connect();
                //DisplayLine("Connected to FTP server " + FTPServer);
            }
            catch (Exception ex)
            {
                //DisplayLine("Connecting to FTP server " + FTPServer + " failed");
                ftp.Dispose();
                return -1;
            }

            try
            {
                //DisplayLine("Uploading file " + FilePath + "\\" + Filename);
                ftp.UploadFile(FilePath + "\\" + Filename, Filename);
                //DisplayLine("File uploaded");
            }
            catch (Exception ex)
            {
                //DisplayLine("Upload failed");
                ftp.Dispose();
                return -2;
            }

            //DisplayLine("Closing connection");
            ftp.Close();
            ftp.Dispose();
            //DisplayLine("Connection closed");
            return 0;
        }

        private static string CreateServiceRequestFile(SensusRequest sr)
        {
            string FileName = Path.ChangeExtension(Path.GetTempFileName(), "sreq");
            StreamWriter sw = new StreamWriter(FileName, false, Encoding.UTF8);

            //DisplayLine("Creating SRF: " + FileName);
            sw.WriteLine(sr.ToString());

            sw.Close();
            //DisplayLine("SRF created");

            return FileName;
        }

    }
}
