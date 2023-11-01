using System.Reflection.Metadata;

namespace Hulk
{
    public abstract class ASTnode
    {

        public interface IVisitor<T>
        {
            T Visit(Assignment _assign);
            T Visit(String_ _string);
            T Visit(BinOp _binary);
            T Visit(Boolean _boolean);
            T Visit(UnaryExpr _unary);
            T Visit(Num _num);
            T Visit(Grouping _group);
            T Visit(CallFunction _call);
            T Visit(Logical _logical);
            T Visit(MathExpr _value);

            T Visit(FunctionStmt _stmt);
            T Visit(IfStmt _stmt);
            T Visit(Print _print);
            T Visit(ExpressionStmt _stmt);
            T Visit(LetStmt _let);

        }
        public abstract T Accept<T>(IVisitor<T> visitor);
    }


}