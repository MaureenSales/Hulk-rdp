namespace Hulk
{
    public abstract class ASTnode
    {

        public abstract T Accept<T>(IVisitor<T> visitor);
        public interface IVisitor<T>
        {
            T Visit(Assignment _assign);
            T Visit(String_ _string);
            T Visit(BinOp _binary);
            T Visit(Boolean _logical);
            T Visit(Conditional _conditional);
            T Visit(UnaryExpr _unary);
            T Visit(Num _num);
            T Visit(Grouping _group);
        }

    }

}