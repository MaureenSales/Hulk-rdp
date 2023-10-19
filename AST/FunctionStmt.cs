namespace Hulk
{
    public class FunctionStmt: Stmt
    {
        public Token Name { get; private set; }
        public List<Token> Params { get; private set; }
        public List<Stmt> Body { get; private set; }

        public FunctionStmt(Token name, List<Token> paramet, List<Stmt> body)
        {
            Name = name;
            Params = paramet;
            Body = body;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }

    }
}