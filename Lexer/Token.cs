namespace Hulk
{
    public class Token
    {
        public TokenType Type { get; private set; }
        public string Value { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }
       
        public Token(TokenType type, string value, int column)
        {
            Type = type;
            Value = value;
            Line = 1;
            Column = column;
        }
    }
}