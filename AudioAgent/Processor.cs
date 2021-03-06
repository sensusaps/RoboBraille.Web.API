﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAgent
{
    class Processor
    {
        public static void CheckForAudioAgentInstances() {
            var instancesRunning = System.Diagnostics.Process.GetProcessesByName("AudioAgent").Count();
            if (instancesRunning < 6)
            {
                var dif = 6 - instancesRunning;
                if (dif > 0)
                {
                    for (int i = 0; i < dif; i++)
                    {
                        System.Diagnostics.Process.Start(@"C:\AudioAgent\AudioAgent.exe", "startup");
                    }
                }
            }
        }
        public static Encoding GetEncoding(byte[] bom,Language lang)
        {
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            //extend for language and code page mapping 
            return GetEncodingByCountryCode(lang);
            //return Encoding.ASCII;
        }

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
                case Language.esCO:
                case Language.frFR:
                case Language.itIT:
                case Language.ptPT:
                case Language.daDK:
                case Language.nlNL:
                case Language.nbNO:
                case Language.isIS:
                case Language.svSE:
                case Language.klGL:
                case Language.cyGB:
                    res = Encoding.GetEncoding(1252);
                    break;
                case Language.fiFI:
                    res = Encoding.GetEncoding(1252);
                    break;
                case Language.slSI:
                case Language.huHU:
                case Language.plPL:
                case Language.roRO:
                case Language.czCZ:
                case Language.skSK:
                    res = Encoding.GetEncoding(1250);
                    break;
                case Language.ltLT:
                    res = Encoding.GetEncoding(1257);
                    break;
                case Language.arEG:
                    res = Encoding.GetEncoding(1256);
                    break;
                case Language.zhCN:
                case Language.zhHK:
                case Language.zhTW:
                    res = Encoding.Unicode;
                    break;
                case Language.jaJP:
                    res = Encoding.UTF8;
                    break;
                case Language.koKR:
                    res = Encoding.UTF8;
                    break;
                default:
                    res = Encoding.Unicode;
                    break;
            }
            return res;
        }
    }
}
