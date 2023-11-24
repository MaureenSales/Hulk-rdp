namespace Hulk
{
    public class UnaryExpr: ASTnode
    {
        public Token Operator { get; private set; }//operador - o !
        public ASTnode Right { get; private set; }//expresion derecha

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