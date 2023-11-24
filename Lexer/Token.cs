namespace Hulk
{
    public class Token
    {
        //construye el token con las propiedades tipo y valor
        public TokenType Type { get; set; }
        public string Lexeme { get; set; }
       
        public Token(TokenType type, string value)
        {
            Type = type;
            Lexeme = value;
        }
    }
}