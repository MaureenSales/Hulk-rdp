namespace Hulk
{
    public class IfStmt: ASTnode
    {
        public ASTnode Condition { get; private set; }
        public ASTnode ThenBody { get; private set; }
        public ASTnode ElseBody { get; private set; }

        public IfStmt(ASTnode condition, ASTnode then_body, ASTnode else_body)
        {
            Condition = condition;
            ThenBody = then_body;
            ElseBody = else_body;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}