using System.Reflection.Metadata;

namespace Hulk
{
    public class Error
    {
        public static void Error_( int line, ErrorType type, string error)
        {
            report(line, "", type, error);
        }

        private static void report( int line, string where, ErrorType type, string error)
        {
            System.Console.WriteLine($"{type} ERROR: line {line} {where} {error}.");
            HadError = true;
        }

        public static bool HadError = false;

        public enum ErrorType
        {
           LEXICAL,
           SINTACTIC,
           SEMANTIC
        }

    }
}