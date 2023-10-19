namespace Hulk
{
    public class UnaryExpr: ASTnode
    {
        public Token Operator { get; private set; }
        public ASTnode Right { get; private set; }

        public UnaryExpr(Token op, ASTnode expr)
        {
            Operator = op;
            Right = expr;
        }
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    } 
}