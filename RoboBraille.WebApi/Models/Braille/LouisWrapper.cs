using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class LouisWrapper
    {
        private static string inputString = "";

        #region Public Methods
        public static unsafe String CallTranslateString(String source, String translationTables, int translationMode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    return null;
                }
                string inputTables = "";
                if (translationTables.Length > 1)
                {
                    foreach (string s in translationTables.Split(','))
                    {
                        inputTables += @"tables\" + s + ",";
                    }
                    inputTables = inputTables.Substring(0, inputTables.Length - 1);
                }
                else
                {
                    inputTables = @"tables\" + translationTables;
                }
                int inputLength = source.Length;
                int outputLength = 1;
                while (outputLength < source.Length)
                {
                    outputLength *= 40960;
                }

                char* outBuffer = (char*)Marshal.AllocHGlobal(outputLength);
                int r_outputLength = outputLength;
                inputString = source;
                int mode = 8; //default value
                mode = (int)translationMode;
                int code = LouisWrapper.lou_translateString(inputTables, inputString, ref inputLength, outBuffer, ref r_outputLength, null, null, mode);
                if (code == 1)
                {
                    string uniString = Marshal.PtrToStringAuto((IntPtr)outBuffer);
                    Marshal.FreeHGlobal((IntPtr)outBuffer);
                    if (uniString.Length > r_outputLength)
                        uniString = uniString.Substring(0, r_outputLength);
                    return uniString;
                }
                else return null;
            }
            catch (Exception e)
            {
                throw e;
                //return null;
            }
        }

        public static unsafe String CallBackTranslateString(String source, String translationTables, int translationMode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    return null;
                }
                string inputTables = "";
                if (translationTables.Length > 1)
                {
                    foreach (string s in translationTables.Split(','))
                    {
                        inputTables += @"tables\" + s + ",";
                    }
                    inputTables = inputTables.Substring(0, inputTables.Length - 1);
                }
                else
                {
                    inputTables = @"tables\" + translationTables;
                }
                int inputLength = source.Length;
                int outputLength = 1;
                while (outputLength < source.Length)
                {
                    outputLength *= 4096;
                }

                char* outBuffer = (char*)Marshal.AllocHGlobal(outputLength);
                int r_outputLength = outputLength;

                int mode = 8; //default value
                mode = (int)translationMode;

                int code = LouisWrapper.lou_backTranslateString(inputTables, source, ref inputLength, outBuffer, ref r_outputLength, null, null, mode);
                string uniString = Marshal.PtrToStringUni((IntPtr)outBuffer);
                Marshal.FreeHGlobal((IntPtr)outBuffer);
                uniString = uniString.Substring(0, r_outputLength);
                return uniString;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //return null;
                throw e;
            }
        }

        public static unsafe string GetVersion()
        {
            return "Library version: " + Marshal.PtrToStringAnsi((IntPtr)LouisWrapper.lou_version());
        }

        public static unsafe bool FreeLibrary()
        {
            try
            {
                LouisWrapper.lou_free();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
        #region liblouis.dlll private Call methods

        /// <summary>
        /// Set the path used for searching for tables and liblouisutdml files. Overrides the installation path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_setDataPath", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern char* lou_setDataPath([MarshalAs(UnmanagedType.LPStr)] string path);

        /// <summary>
        /// Get the path set in the previous function.
        /// </summary>
        /// <returns></returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_getDataPath", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern char* lou_getDataPath();

        /// <summary>
        ///  Original: int EXPORT_CALL lou_translateString(const char *tableList, const widechar *inbuf, int *inlen, widechar * outbuf, int *outlen, formtype *typeform, char *spacing, int mode);
        /// </summary>
        /// <param name="tableList"></param>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <param name="typeform"></param>
        /// <param name="spacing"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "lou_translateString", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern int lou_translateString(
            [MarshalAs(UnmanagedType.LPStr)] string tableList,
            [MarshalAs(UnmanagedType.LPWStr)] string inbuf,
            ref int inlen,
            char* outbuf,
            ref int outlen,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder typeform,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder spacing,
            int mode
            );

        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_version", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern char* lou_version();

        /// <summary>
        /// This function should be called at the end of the application to free all memory allocated by liblouis.
        /// </summary>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_free", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern void lou_free();

        /// <summary>
        /// Original: int EXPORT_CALL lou_backTranslateString (const char *tableList, const widechar * inbuf, int *inlen, widechar * outbuf, int *outlen, formtype *typeform, char  *spacing, int mode);
        /// </summary>
        /// <param name="tableList"></param>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <param name="typeform"></param>
        /// <param name="spacing"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "lou_backTranslateString", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern int lou_backTranslateString(
            [MarshalAs(UnmanagedType.LPStr)] string tableList,
            string inbuf,
            ref int inlen,
            char* outbuf,
            ref int outlen,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder typeform,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder spacing,
            int mode
            );

        /// <summary>
        /// This function enables you to compile a table entry on the fly at run-time. 
        /// The new entry is added to tableList and remains in force until lou_free is called. 
        /// If tableList has not previously been loaded it is loaded and compiled. 
        /// It may be anything valid. Error messages will be produced if it is invalid. 
        /// </summary>
        /// <param name="tableList"></param>
        /// <param name="inString">inString contains the table entry to be added. </param>
        /// <returns>The function returns 1 on success and 0 on failure.</returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_compileString", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern int lou_compileString(
            [MarshalAs(UnmanagedType.LPStr)] string tableList,
            [MarshalAs(UnmanagedType.LPStr)] string inString
            );

        /// <summary>
        /// This function takes a widechar string in inbuf consisting of dot patterns and converts it to a widechar string in outbuf consisting of characters according to the specifications in tableList. 
        /// The dot patterns in inbuf can be in either liblouis format or Unicode braille. 
        /// </summary>
        /// <param name="tableList"></param>
        /// <param name="inbuf"></param>
        /// <param name="outbuf"></param>
        /// <param name="length">length is the length of both inbuf and outbuf.</param>
        /// <param name="mode"></param>
        /// <returns>The function returns 1 on success and 0 on failure.</returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "lou_dotsToChar", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern int lou_dotsToChar(
            [MarshalAs(UnmanagedType.LPStr)] string tableList,
            string inbuf,
            char* outbuf,
            int length,
            int mode
            );

        /// <summary>
        /// This function is the inverse of lou_dotsToChar. 
        /// It takes a widechar string in inbuf consisting of characters and converts it to a widechar string in outbuf consisting of dot patterns according to the specifications in tableList. 
        /// The dot patterns in outbufbuf are in liblouis format if the mode bit ucBrl is not set and in Unicode format if it is set. 
        /// </summary>
        /// <param name="tableList"></param>
        /// <param name="inbuf"></param>
        /// <param name="outbuf"></param>
        /// <param name="length">length is the length of both inbuf and outbuf.</param>
        /// <param name="mode"></param>
        /// <returns>The function returns 1 on success and 0 on failure.</returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "lou_charToDots", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern int lou_charToDots(
            [MarshalAs(UnmanagedType.LPStr)] string tableList,
            string inbuf,
            char* outbuf,
            int length,
            int mode
            );

        /// <summary>
        /// This function checks a table for errors. If none are found it loads the table into memory and returns a pointer to it. if errors are found it returns a null pointer. 
        /// It is called by lou_translateString and lou_backTranslateString and also by functions in liblouisxml
        /// </summary>
        /// <param name="tableList"></param>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_getTable", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern void lou_getTable([MarshalAs(UnmanagedType.LPStr)] string tableList);

        /// <summary>
        /// Read a character from a file, whether big-encian, little-endian or ASCII8, and return it as an integer. EOF at end of file. Mode = 1 on first call, any other value thereafter
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_readCharFromFile", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern int lou_readCharFromFile(
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            ref int mode
            );

        [DllImport("liblouis.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "lou_hyphenate", PreserveSig = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        private unsafe static extern int lou_hyphenate(
             [MarshalAs(UnmanagedType.LPStr)] string tableList,
             string inbuf,
             int inlen,
             char* hyphens,
             int mode);

        #endregion;
        #region Unimplemented C methods
        /* =========================  REMAINING FUNCTIONALITY API ========================= /
        
  int EXPORT_CALL lou_translate (const char *tableList, const widechar
				 * inbuf,
				 int *inlen, widechar * outbuf, int *outlen,
				 formtype *typeform, char *spacing,
				 int *outputPos, int *inputPos,
				 int *cursorPos, int mode);
  
  int EXPORT_CALL lou_backTranslate (const char *tableList, const widechar
				     * inbuf,
				     int *inlen, widechar * outbuf,
				     int *outlen, formtype *typeform,
				     char *spacing, int *outputPos,
				     int *inputPos, int *cursorPos, int mode);

  int EXPORT_CALL lou_translatePrehyphenated (const char *tableList,
					      const widechar * inbuf,
					      int *inlen, widechar * outbuf,
					      int *outlen, formtype 
					      *typeform,
					      char *spacing, int *outputPos,
					      int *inputPos, int *cursorPos,
					      char *inputHyphens,
					      char *outputHyphens, int mode);

void EXPORT_CALL lou_registerTableResolver (char ** (* resolver) (const char *table, const char *base));
/* Register a new table resolver. Overrides the default resolver. /
                  
  /* =========================  LOG API ========================= /
  void EXPORT_CALL lou_logPrint (const char *format, ...);
/* Prints error messages to a file
   @deprecated As of 2.6.0, applications using liblouis should implement
               their own logging system. /

  void EXPORT_CALL lou_logFile (const char *filename);
/* Specifies the name of the file to be used by lou_logPrint. If it is 
* not used, this file is stderr/

  void EXPORT_CALL lou_logEnd ();
  /* Closes the log file so it can be read by other functions. /

      typedef void (*logcallback)(int level, const char *message);
      
        void EXPORT_CALL lou_registerLogCallback(logcallback callback);
      /* Register logging callbacks
       * Set to NULL for default callback.
       /

        typedef enum
        {
          LOG_ALL = -2147483648,
          LOG_DEBUG = 10000,
          LOG_INFO = 20000,
          LOG_WARN = 30000,
          LOG_ERROR = 40000,
          LOG_FATAL = 50000,
          LOG_OFF = 2147483647
        } logLevels;
        
        void EXPORT_CALL lou_setLogLevel(logLevels level);
      /* Set the level for logging callback to be called at */


        /* =========================  END OF LOG API ========================= /

        /* =========================  BETA API ========================= /

        // Use the following two function with care, API is subject to change!

        void EXPORT_CALL lou_indexTables(const char ** tables);
        /* Parses, analyzes and indexes tables. This function must be called prior to
         * lou_findTable(). An error message is given when a table contains invalid or
         * duplicate metadata fields.
         /
        char * EXPORT_CALL lou_findTable(const char * query);
        /* Finds the best match for a query. Returns a string with the table
         * name. Returns NULL when no match can be found. An error message is given
         * when the query is invalid.
         */

        /* ====================== END OF BETA API ====================== /

                 */
        #endregion
    }
}