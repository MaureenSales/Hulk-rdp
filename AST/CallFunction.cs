using System.Reflection.Emit;
namespace Hulk
{
    public class CallFunction: ASTnode
    {
        public Token Callee { get; private set; }
        public Token Paren { get; private set; }
        public List<ASTnode> Arguments { get; private set; }

        public CallFunction( Token callee, Token paren, List<ASTnode> arguments)
        {
            Callee = callee;
            Paren = paren;
            Arguments = arguments;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}