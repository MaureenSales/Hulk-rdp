using System.Reflection.Metadata;

namespace Hulk
{
    public class Error: Exception
    {
        public static Exception Error_(int line, ErrorType type, string where, string error)
        {
            HadError = true;
            throw new Exception($"{type} ERROR: line {line} {where} {error}.");
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