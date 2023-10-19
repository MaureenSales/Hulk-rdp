namespace Hulk
{
    public class ASTPrinter: ASTnode.IVisitor<string>
    {
         public string Print(ASTnode expression)
        {
            return expression.Accept(this);
        }
    }
}