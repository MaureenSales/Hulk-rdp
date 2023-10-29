namespace Hulk
{
    public class LetStmt: ASTnode
    {
        public List<Assignment> Declarations { get; private set; }
        public ASTnode Body { get; private set; }

        public LetStmt ( List<Assignment> declaration, ASTnode body )
        {
            Declarations = declaration;
            Body = body;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }


}