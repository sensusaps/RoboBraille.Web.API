using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models.DocumentAccessibility
{
    public class DacReport
    {
        private Dictionary<string, object> reportItems = new Dictionary<string, object>();
        public bool IsTagged { get; set; }
        public void AddReportItemAsText(string name,string value)
        {
            reportItems.Add(name, value);
        }

        public string PrintReport()
        {
            string res = "Propriety -:- Result of testing the propriety"+Environment.NewLine;
            foreach (KeyValuePair<string, object> item in reportItems)
            {
                res += item.Key + " -:- " + item.Value.ToString() + Environment.NewLine;
            }
            return res;
        }
    }
}