namespace Hulk
{
    public class LetStmt: ASTnode
    {
        public List<Assignment> Declarations { get; private set; } //lista de declaraciones de variable
        public ASTnode Body { get; private set; }//expresion del cuerpo

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