namespace Hulk
{
    public class Print: ASTnode
    {
        public ASTnode Expr { get; private set; }

        public Print(ASTnode expr)
        {
            Expr = expr;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}