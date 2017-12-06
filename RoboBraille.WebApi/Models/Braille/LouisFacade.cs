using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class LouisFacade : IDisposable
    {
        private string holder = null;
        private static Dictionary<Language, string[]> tableMapping = new Dictionary<Language, string[]>();

        public LouisFacade()
        {
            tableMapping = new Dictionary<Language, string[]>();
            tableMapping.Add(Language.itIT, new string[] { "it-it-comp8.utb", "it-it-comp6.utb" });
            tableMapping.Add(Language.plPL, new string[] { "Pl-Pl-g1.utb", "pl-pl-comp8.ctb" });
            tableMapping.Add(Language.ptPT, new string[] { "pt-pt-g1.utb", "pt-pt-g2.ctb", "pt-pt-comp8.ctb" });
            tableMapping.Add(Language.slSI, new string[] { "sl-si-g1.utb", "sl-si-comp8.ctb" });
            tableMapping.Add(Language.elGR, new string[] { "gr-gr-g1.utb", });
            tableMapping.Add(Language.svSE, new string[] { "Se-Se-g1.utb", "se-se.ctb", });
            tableMapping.Add(Language.esES, new string[] { "Es-Es-g1.utb", "es-chardefs.cti ", "Es-Es-G0.utb", "es-g1.ctb", "es-new.dis", "es-old.dis", "es-translation.cti " });
            tableMapping.Add(Language.roRO, new string[] { "ro.ctb" });
            tableMapping.Add(Language.bgBG, new string[] { "bg.ctb" });
            tableMapping.Add(Language.huHU, new string[] { "hu-hu-g1.ctb", "hu-backtranslate-correction.dis", "hu-chardefs.cti", "hu-exceptionwords.cti", "hu-hu-comp8.ctb" });
            tableMapping.Add(Language.enUEB, new string[] { "en-ueb-g1.ctb", "en-ueb-chardefs.uti", "en-ueb-g2.ctb" });
            tableMapping.Add(Language.enUS, new string[] { "en-us-g1.ctb", "en-us-brf.dis", "en-us-comp6.ctb", "en-us-comp8.ctb", "en-us-compbrl.ctb", "en-us-g2.ctb", "en-us-interline.ctb", "en-us-mathtext.ctb" });
            tableMapping.Add(Language.frFR, new string[] { "fr-fr-g1.utb", "Fr-Fr-g2.ctb", });
            tableMapping.Add(Language.deDE, new string[] { "de-de-g1.ctb", "de-de-comp8.ctb", "de-de-g0.utb", "de-de-g2.ctb" });
            tableMapping.Add(Language.daDK, new string[] { "da-dk-g16.utb", "da-dk-g18.utb", "da-dk-g26.ctb", "da-dk-g28.ctb" });
            tableMapping.Add(Language.nnNO, new string[] { "no-no-g1.ctb", "no-no-g2.ctb", "no-no-g3.ctb", "no-no-g0.utb" });
            tableMapping.Add(Language.isIS, new string[] { "is.ctb" });
            tableMapping.Add(Language.enGB, new string[] { "en-gb-g1.utb", "en-GB-g2.ctb", "en-gb-comp8.ctb" });
            //add slSL
            holder = Environment.CurrentDirectory;
            Environment.CurrentDirectory = ConfigurationManager.AppSettings.Get("bindirectory");
        }
        public String TranslateString(String source, String translationTables, int mode)
        {
            return LouisWrapper.CallTranslateString(source, translationTables, mode);
        }

        public String BackTranslateString(String source, String translationTables, int mode)
        {
            return LouisWrapper.CallBackTranslateString(source, translationTables, mode);
        }

        public string GetLibraryVersion()
        {
            return LouisWrapper.GetVersion();
        }

        public static List<string> GetTranslationTables()
        {
            List<string> result = new List<string>();
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings.Get("BinDirectory") + @"\tables");
            foreach (String f in files)
            {
                result.Add(Path.GetFileName(f));
            }
            return result;
        }

        public static List<string> GetTranslationTablesWithDescription()
        {
            List<string> result = new List<string>();
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings.Get("BinDirectory") + @"\tables");
            foreach (String f in files)
            {
                string firstLine = "";
                using (StreamReader reader = new StreamReader(f))
                {
                    firstLine = reader.ReadLine();
                }
                firstLine = firstLine.Replace("#", "");
                firstLine = firstLine.Trim();
                result.Add(Path.GetFileName(f) + " " + firstLine);
            }
            return result;
        }

        public void Dispose()
        {
            LouisWrapper.FreeLibrary();
            Environment.CurrentDirectory = holder;
            holder = null;
        }

        internal string getGrade1TranslationTable(Language brailleLanguage)
        {
            return tableMapping[brailleLanguage][0];
        }

        internal string getTranslationTable(Language brailleLanguage, BrailleContraction brailleContraction, BrailleFormat dots)
        {
            List<String> tables = GetTranslationTables();
            string contractionTable = "", eightDotTable = "";
            string searchContraction = "", searchDots = "8";
            switch (brailleContraction)
            {
                case BrailleContraction.grade1: searchContraction = "g1"; break;
                case BrailleContraction.grade2: searchContraction = "g2"; break;
                default: break;
            }
            string[] langTables = tableMapping[brailleLanguage];
            if (langTables.Length == 1)
                contractionTable = langTables[0];
            else
                foreach (string s in langTables)
                {
                    if (dots.Equals(BrailleFormat.eightdot) && s.ToLowerInvariant().Contains(searchDots))
                        eightDotTable = s;
                    if (s.ToLowerInvariant().Contains(searchContraction))
                    {
                        if (tables.Contains(s))
                            contractionTable = s;
                    }
                    else contractionTable = langTables[0];
                }
            if (eightDotTable != "" && !contractionTable.Contains(searchDots))
                return contractionTable + ", " + eightDotTable;
            else
                return contractionTable;
        }
    }
}
