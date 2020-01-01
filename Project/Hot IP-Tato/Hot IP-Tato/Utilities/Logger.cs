using System;
using System.IO;

namespace Hot_IP_Tato.CS_Scripts.Utilities
{
    public class Logger
    {
        public static void Test()
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log("Test1", "Log.txt");
                Log("Test2", "Log.txt");
            }

            using (StreamReader r = File.OpenText("log.txt"))
            {
                DumpLog(r);
            }
        }

        public static void Log(string logMessage, string LogFileName="Log.txt")
        {
            //Sanitize Input
            TextWriter w = File.AppendText(Sanitizer.FilePath(LogFileName));
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("--------------------------------");
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
// The example creates a file named "log.txt" and writes the following lines to it,
// or appends them to the existing "log.txt" file:

// Log Entry : <current long time string> <current long date string>
//  :
//  :Test1
// -------------------------------

// Log Entry : <current long time string> <current long date string>
//  :
//  :Test2
// -------------------------------

// It then writes the contents of "log.txt" to the console.

// Retrieved from: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-open-and-append-to-a-log-file