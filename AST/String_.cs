namespace Hulk
{
    public class String_: ASTnode
    {
        public string Value {get; private set; }

        public String_ (string value)
        {
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}