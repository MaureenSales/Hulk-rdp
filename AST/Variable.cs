namespace Hulk
{
    public class Variable: ASTnode
    {
        public Token Name { get; private set; }

        public Variable (Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

    }
}