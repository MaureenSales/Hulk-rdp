namespace Hulk
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Lexeme { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
       
        public Token(TokenType type, string value, int column, int line)
        {
            Type = type;
            Lexeme = value;
            Line = line;
            Column = column;
        }
    }
}