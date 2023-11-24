namespace Hulk
{
    public class MathExpr: ASTnode
    {
        public double Value { get; private set; }//expresion de tipo matematica: Euler,Pi

        public MathExpr( double value)
        {
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}