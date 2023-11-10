using System.Reflection.Metadata;

namespace Hulk
{
    public class Error: Exception
    {
        public static Exception Error_( ErrorType type, string error)
        {
            HadError = true;
            string type_ = "";
            if(type == ErrorType.LEXICAL) type_ = "LEXICAL";
            else if(type == ErrorType.SINTACTIC) type_ = "SINTACTIC";
            else if(type == ErrorType.SEMANTIC) type_ = "SEMANTIC";
            throw new Exception($"{type_} ERROR: {error}.");
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