namespace Hulk
{
    public class Let: ASTnode
    {
        public List<ASTnode> Declarations { get; private set; }
        public List<ASTnode> Body { get; private set; }

        public Let( ASTnode declaration, List<ASTnode> body)
        {
            List<ASTnode> Declarations = new List<ASTnode>();
            Declarations.Add(declaration);
            Body = body;
        }

        public Let( List<ASTnode> declarations, List<ASTnode> body)
        {
            Declarations = declarations;
            Body = body;
        }
    }


}