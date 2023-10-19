namespace Hulk
{
    public class CallFunction: ASTnode
    {
        public ASTnode Callee { get; private set; }
        public Token Paren { get; private set; }
        public List<Token> Parameters { get; private set; }

        public CallFunction( ASTnode callee, Token paren, List<Token> parameters)
        {
            Callee = callee;
            Paren = paren;
            Parameters = parameters;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}