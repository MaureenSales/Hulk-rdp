namespace Hulk
{
    public class VariableReference : ASTnode
    {
        public string Name { get; private set; }//nombre de la variable

        public VariableReference(string name)
        {
            Name = name;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}