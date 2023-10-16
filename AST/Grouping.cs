namespace Hulk
{
     public class Grouping : ASTnode
        {
            public ASTnode expression;
             
            public Grouping(ASTnode expression)
            {
                this.expression = expression;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
}