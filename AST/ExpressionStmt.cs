using System.Linq.Expressions;

namespace Hulk
{
    public class ExpressionStmt: Stmt
    {
        public ASTnode expression { get; private set; }
        public ExpressionStmt(ASTnode expr)
        {
            this.expression = expr;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }
}