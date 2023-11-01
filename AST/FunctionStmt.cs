using System.Reflection.Metadata;

namespace Hulk
{
    public class FunctionStmt : ASTnode
    {
        public string Name { get; private set; }
        public List<Token> Params { get; private set; }
        public ASTnode Body { get; private set; }
        public int Arity { get; private set; }

        public FunctionStmt(string name, List<Token> parameters, List<ASTnode> body)
        {
            Name = name;
            Params = parameters;
            Body = body;
            Arity = parameters.Count;

        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

    }

    public class Function
    {
        public static Dictionary<string, FunctionStmt> functions = new Dictionary<string, FunctionStmt>();

        public Function(string name, FunctionStmt function)
        {
            functions.Add(name, function);
        }

        public static bool FunctionDeclaration(string name)
        {
                if (functions.ContainsKey(name))
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }

        public static bool FunctionArity(string name, int arity)
        {
            if(arity == functions[name].Arity)
            {
                return true;
            }
            else return false;
        }


    }
}