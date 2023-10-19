namespace Hulk
{
    public class IfStmt: Stmt
    {
        public ASTnode Condition { get; private set; }
        public Stmt ThenBody { get; private set; }
        public Stmt ElseBody { get; private set; }

        public IfStmt(ASTnode condition, Stmt then_body, Stmt else_body)
        {
            Condition = condition;
            ThenBody = then_body;
            ElseBody = else_body;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }
}