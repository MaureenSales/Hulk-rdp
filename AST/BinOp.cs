namespace Hulk
{
    public class BinOp: ASTnode
    {
        public ASTnode Left { get; private set; }
        public Token Op { get; private set; }
        public ASTnode Right { get; private set; }

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