namespace Hulk
{
    public class Assignment: ASTnode
    {
        public string Name { get; private set; }
        public ASTnode? Value { get; set; }

        public Assignment (string name, ASTnode value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}