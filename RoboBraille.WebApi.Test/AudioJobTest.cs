using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using RoboBraille.WebApi.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace RoboBraille.WebApi.Test
{
    [TestClass]
    public class AudioJobTest
    {
        [TestMethod]
        public void TestSupportedLanguages()
        {
            List<string> res = AudioJobRepository.GetInstalledLangs();
            string txt ="";
            foreach (string s in res)
            {
                txt += s+Environment.NewLine;
            }
            File.WriteAllText(@"C:\Users\Paul\Desktop\test\res.txt",txt);
            Assert.IsTrue(res.Count>0);
        }
    }
}
