using System.Linq.Expressions;

namespace Hulk
{
    public abstract class ASTnode
    {
        public abstract bool CheckSemantic();
    }
}