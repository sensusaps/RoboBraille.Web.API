using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test.RoboBrailleFTP
{
    public class SensusRequest
    {
        public string Process { get; set; }
        public string SubProcess { get; set; }
        public string Option { get; set; }
        public string Language { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string SourcePath { get; set; }
        public string DestinationFile { get; set; }
        public string Prefix { get; set; }
        public string RequesterID { get; set; }
        public string FTPServer { get; set; }
        public string FTPUser { get; set; }
        public string FTPPassword { get; set; }
        public string TempSResp { get; set; }

        public override string ToString()
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<SReq>");
            xml.AppendLine("<Process>" + this.Process + "</Process>");
            xml.AppendLine("<Subprocess>" + this.SubProcess + "</Subprocess>");
            xml.AppendLine("<Option>" + this.Option + "</Option>");
            xml.AppendLine("<Language>" + this.Language + "</Language>");
            xml.AppendLine("<Gender>" + this.Gender + "</Gender>");
            xml.AppendLine("<Age>" + this.Age + "</Age>");
            xml.AppendLine("<Source>" + Path.GetFileName(this.SourcePath) + "</Source>");
            xml.AppendLine("<Destination>" + this.DestinationFile + "</Destination>");
            xml.AppendLine("<Prefix>" + this.Prefix + "</Prefix>");
            xml.AppendLine("<RequesterID>" + this.RequesterID + "</RequesterID>");
            xml.AppendLine("<FTP-Server>" + this.FTPServer + "</FTP-Server>");
            xml.AppendLine("<FTP-User>" + this.FTPUser + "</FTP-User>");
            xml.AppendLine("<FTP-Password>" + this.FTPPassword + "</FTP-Password>");
            xml.AppendLine("</SReq>");
            return xml.ToString();
        }
    }
}
