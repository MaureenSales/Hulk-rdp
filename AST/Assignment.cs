namespace Hulk
{
    public class Assignment: ASTnode
    {
        public string Name { get; private set; }
        public ASTnode Value { get; private set; }

        public Assignment (string name, ASTnode value)
        {
            Name = name;
            Value = value;
        }
    }
}