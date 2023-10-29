using System.Linq.Expressions;

namespace Hulk
{
    public class ExpressionStmt: ASTnode
    {
        public ASTnode expression { get; private set; }
        public ExpressionStmt(ASTnode expr)
        {
            this.expression = expr;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}