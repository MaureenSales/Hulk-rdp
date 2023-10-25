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
            T Visit(Variable _var);

        }
        public abstract T Accept<T>(IVisitor<T> visitor);
    }

    public abstract class Stmt
    {
        public interface IVisitor<R> 
        {
            R Visit(FunctionStmt _stmt);
            R Visit(IfStmt _stmt);
            R Visit(Print _print);
            R Visit(VariableStmt _stmt);
            R Visit(ExpressionStmt _stmt);
            R Visit(Let _let);

        }  
            public abstract R Accept<R>(IVisitor<R> visitor);
    }
            

}