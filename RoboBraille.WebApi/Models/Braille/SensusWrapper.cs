////Uncomment this class to implement the Sensus Braille conversion SB4

//using Sensus.Braille;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RoboBraille.WebApi.Models
//{
//    internal class SensusWrapper
//    {
//        private IContractor contractor;

//        public SensusWrapper(Language language, BrailleContraction contractionLevel, BrailleFormat dots)
//        {
//            contractor = InitContractionResources(language, contractionLevel, dots);
//        }
//        public string ConvertText(string toConvert)
//        {
//            string res = null;
//            //try
//            //{
//            res = contractor.ContractText(toConvert);
//            //}
//            //catch (Exception e)
//            //{
//            //    Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
//            //}
//            return res;
//        }

//        private IContractor InitContractionResources(Language language, BrailleContraction contractionLevel, BrailleFormat dots)
//        {
//            BrailleContractionLevel bcl = BrailleContractionLevel.Uncontracted;
//            Sensus.Braille.BrailleFormat format = Sensus.Braille.BrailleFormat.SixDot;

//            switch (dots)
//            {
//                case BrailleFormat.sixdot:
//                    format = Sensus.Braille.BrailleFormat.SixDot;
//                    break;

//                case BrailleFormat.eightdot:
//                    format = Sensus.Braille.BrailleFormat.EightDot;
//                    break;
//                default:
//                    format = Sensus.Braille.BrailleFormat.FullText;
//                    break;
//            }
//            switch (language.ToString())
//            {
//                case "daDK":
//                    switch (contractionLevel.ToString())
//                    {
//                        case "small":
//                            bcl = BrailleContractionLevel.Small;
//                            break;
//                        case "large":
//                            bcl = BrailleContractionLevel.Large;
//                            break;
//                        case "full":
//                            bcl = BrailleContractionLevel.Uncontracted;
//                            break;
//                        case "user":
//                            bcl = BrailleContractionLevel.User;
//                            break;
//                        default: throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    }
//                    break;
//                case "svSE":
//                    switch (contractionLevel.ToString())
//                    {
//                        case "small":
//                            bcl = BrailleContractionLevel.Small;
//                            break;
//                        case "large":
//                            bcl = BrailleContractionLevel.Large;
//                            break;
//                        case "full":
//                            bcl = BrailleContractionLevel.Uncontracted;
//                            break;
//                        case "user":
//                            bcl = BrailleContractionLevel.User;
//                            break;
//                        default: throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    }
//                    break;
//                case "enGB":
//                    switch (contractionLevel.ToString())
//                    {
//                        case "grade1":
//                            bcl = BrailleContractionLevel.Grade1;
//                            break;
//                        case "grade2":
//                            bcl = BrailleContractionLevel.Grade2;
//                            break;
//                        case "grade2i":
//                            bcl = BrailleContractionLevel.Grade2I;
//                            break;
//                        case "grade2b":
//                            bcl = BrailleContractionLevel.Grade2B;
//                            break;
//                        default: throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    }
//                    break;
//                case "nnNO":
//                    switch (contractionLevel.ToString())
//                    {
//                        case "full":
//                        case "level0":
//                            bcl = BrailleContractionLevel.FullNN;
//                            break;

//                        case "level1":
//                            bcl = BrailleContractionLevel.Level1;
//                            break;

//                        case "level2":
//                            bcl = BrailleContractionLevel.Level2;
//                            break;

//                        case "level3":
//                            bcl = BrailleContractionLevel.Level3;
//                            break;
//                        default: throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    }
//                    break;
//                case "isIS":
//                    if (contractionLevel.ToString().Equals("grade1"))
//                        bcl = BrailleContractionLevel.Grade1IS;
//                    else
//                        throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    break;
//                case "ptPT":
//                    if (contractionLevel.ToString().Equals("grade1"))
//                        bcl = BrailleContractionLevel.Grade1;
//                    else
//                        throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    break;
//                case "itIT":
//                    if (contractionLevel.ToString().Equals("grade1"))
//                        bcl = BrailleContractionLevel.Grade1;
//                    else
//                        throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    break;
//                case "elGR":
//                    if (contractionLevel.ToString().Equals("grade1"))
//                        bcl = BrailleContractionLevel.Grade1;
//                    else
//                        throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    break;
//                case "deDE":
//                    switch (contractionLevel.ToString())
//                    {
//                        case "grade1":
//                            bcl = BrailleContractionLevel.Grade1;
//                            break;
//                        case "grade2":
//                            bcl = BrailleContractionLevel.Grade2;
//                            break;
//                        default:
//                            throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//                    }
//                    break;
//                default:
//                    throw new ArgumentException("Invalid argument. The contraction level " + contractionLevel.ToString() + " is not valid for the language " + language.ToString() + ".");
//            }
//            string inLang = language.ToString().Insert(2, "-");
//            CSensusBraille sbraille = new CSensusBraille(inLang, bcl, CharacterEncoding.Iso8859_1Braille, Sensus.Braille.SBFormatInterpretation.FORMAT_INTERPRETATION_LOOSE, Sensus.Braille.SBTagProcessing.TAGS_PROCESS_AND_IGNORE);

//            IContractor contractor = sbraille.GetContractor(format);

//            return contractor;
//        }
//    }
//}
