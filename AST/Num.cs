namespace Hulk
{
    public class Num: ASTnode
    {
        public Token Token_ { get; private set; }
        public double Value { get; private set; }

        public Num( Token token)
        {
            Token_ = token;
            Value  = double.Parse(token.Value);
        }
    }
}