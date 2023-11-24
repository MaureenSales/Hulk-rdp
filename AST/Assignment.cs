namespace Hulk
{
    public class Assignment: ASTnode
    {
        public string Name { get; private set; } //nombre de la variable
        public ASTnode? Value { get; set; } //valor de la variable, de tipo nodo

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