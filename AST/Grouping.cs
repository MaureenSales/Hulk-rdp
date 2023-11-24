namespace Hulk
{
     public class Grouping : ASTnode
        {
            public ASTnode Expression; //expresion entre parentesis
             
            public Grouping(ASTnode expression)
            {
                Expression = expression;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
}