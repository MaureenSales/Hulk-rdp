namespace Hulk
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Lexeme { get; set; }
       
        public Token(TokenType type, string value)
        {
            Type = type;
            Lexeme = value;
        }
    }
}