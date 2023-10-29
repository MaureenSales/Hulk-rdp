namespace Hulk
{
    public class FunctionStmt: ASTnode
    {
        public Token Name { get; private set; }
        public List<Token> Params { get; private set; }
        public List<ASTnode> Body { get; private set; }

        public FunctionStmt(Token name, List<Token> paramet, List<ASTnode> body)
        {
            Name = name;
            Params = paramet;
            Body = body;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

    }
}