namespace Hulk
{
    public class Num: ASTnode
    {
        public Token Token_ { get; private set; }
        public double Value { get; private set; }

        public Num( Token token)
        {
            Token_ = token;
            Value  = double.Parse(token.Lexeme);
        }

          public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}