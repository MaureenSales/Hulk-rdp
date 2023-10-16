namespace Hulk
{
    public class Token
    {
        public TokenType Type { get; private set; }
        public string Lexeme { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }
       
        public Token(TokenType type, string value, int column, int line)
        {
            Type = type;
            Lexeme = value;
            Line = line;
            Column = column;
        }
    }
}