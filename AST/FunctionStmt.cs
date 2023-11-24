using System.Reflection.Metadata;

namespace Hulk
{
    public class FunctionStmt : ASTnode
    {
        public string Name { get; private set; }//nombre de  la funcion
        public List<Assignment> Params { get; set; }//lista de parametros
        public ASTnode Body { get; private set; }//cuerpo de la funcion

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