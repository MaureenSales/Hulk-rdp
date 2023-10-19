namespace Hulk
{
    public class VariableStmt: Stmt
    {
        public Token Name { get; private set; }
        public ASTnode Initializer { get; private set; }

        public VariableStmt(Token name, ASTnode initializer)
        {
            Name  = name;
            Initializer = initializer;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }
}