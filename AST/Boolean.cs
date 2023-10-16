namespace Hulk
{
    public class Boolean: ASTnode
    {
        public bool Value { get; private set; }

        public Boolean( bool value)
        {
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}