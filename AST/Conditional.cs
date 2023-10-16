namespace Hulk
{
    public class Conditional: ASTnode
    {
        public ASTnode Condition { get; private set; }  
        public ASTnode ThenExpr { get; private set; }
        public ASTnode ElseExpr {get; private set; }

        public Conditional(ASTnode condition, ASTnode then, ASTnode else_)
        {
            Condition = condition;
            ThenExpr = then;
            ElseExpr = else_;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}