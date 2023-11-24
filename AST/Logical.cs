namespace Hulk
{
    public class Logical: ASTnode
    {
        public ASTnode Left { get; private set; }//nodo de expresion a la izquierda
        public Token Operator { get; private set; }//operador binario logico
        public ASTnode Right { get; private set; }//nodo de expresion a la derecha

        public Logical( ASTnode left, Token op, ASTnode right)
        {
            Left = left;
            Operator = op;
            Right = right;

        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}