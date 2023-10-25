namespace Hulk
{
    public class Let: Stmt
    {
        public List<ASTnode> Declarations { get; private set; }
        public List<ASTnode> Body { get; private set; }

        public ASTnode Declaration { get; private  set; }
        public ASTnode Body_ { get; private set; }

        public Let( List<ASTnode> declarations, List<ASTnode> body)
        {
            Declarations = declarations;
            Body = body;
        }

        public Let ( ASTnode declaration, ASTnode body_ )
        {
            Declaration = declaration;
            Body_ = body_;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }


}