namespace Hulk
{
    public class Logical: ASTnode
    {
        public ASTnode Left { get; private set; }
        public Token Operator { get; private set; }
        public ASTnode Right { get; private set; }

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