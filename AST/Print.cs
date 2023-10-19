namespace Hulk
{
    public class Print: Stmt
    {
        public ASTnode Expr { get; private set; }

        public Print(ASTnode expr)
        {
            Expr = expr;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }
}