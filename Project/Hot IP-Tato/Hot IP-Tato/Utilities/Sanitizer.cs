using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot_IP_Tato.CS_Scripts.Utilities
{
    public class Sanitizer
    {
        public static bool Test()
        {
            // Arrange
            string input = "Rocksalt<<>>GHT";
            string expected = "Rocksalt____GHT";

            // Act
            string actual = Sanitizer.FilePath(input);

            // Assert
            Console.WriteLine("input: {0} output: {1} expected {2}", input, actual, expected);
            if (actual.Equals(expected))
                return true;
            else
                return false;
        }
        public static string FilePath(string path)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            Console.WriteLine("Invalid Characters: {0}", invalidChars);
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(path, invalidRegStr, "_");
        }
    }
}
