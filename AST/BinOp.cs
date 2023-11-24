namespace Hulk
{
    public class BinOp: ASTnode
    {
        public ASTnode Left { get; private set; } //nodo operando izquierdo
        public Token Op { get; private set; } //operador binario 
        public ASTnode Right { get; private set; } //nodo operando derecho

        public BinOp(ASTnode left, Token op, ASTnode right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

    }
}