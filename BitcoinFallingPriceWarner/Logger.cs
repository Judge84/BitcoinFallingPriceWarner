using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace BitcoinFallingPriceWarner
{
    /// <summary>
    /// small logger for this shit ;-)
    /// </summary>
    public  class Logger
    {
        private readonly string loggerFilename = "BitcoinFallingPriceWarner.log";

        public enum LogLevel : short
        {
            Trace = 0,
            Warn = 1,
            Error = 2
        };

        StreamWriter logFile; 
        //init the logger
        public Logger(string folder){

            string fullLogfilename = $"{folder}/{loggerFilename}";
            logFile = File.AppendText(fullLogfilename);
            Trace.Listeners.Add(new TextWriterTraceListener(logFile));
            
            Trace.AutoFlush = true;
        }

        public static void Log(LogLevel logLevel, object who,string logEntry) { 

            Trace.WriteLine(
                String.Format($"{System.DateTime.Now.ToString()} {logLevel} {who} {logEntry}" ));
        }

        public void closeAndDispose()
        {
            logFile.Close();
            logFile.Dispose();

        }







    }
}
