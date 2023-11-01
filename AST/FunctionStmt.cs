using System.Reflection.Metadata;

namespace Hulk
{
    public class FunctionStmt : ASTnode
    {
        public string Name { get; private set; }
        public List<Assignment> Params { get; set; }
        public ASTnode Body { get; private set; }

        public FunctionStmt(string name, List<Assignment> parameters, ASTnode body)
        {
            Name = name;
            Params = parameters;
            Body = body;

        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

    }

}