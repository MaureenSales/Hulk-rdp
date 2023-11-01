using System.Reflection.Metadata.Ecma335;

namespace Hulk
{
    public class AstPrinter : ASTnode.IVisitor<string>
    {
        public void Print_(ASTnode stmt)
        {
            Console.WriteLine(stmt.Accept(this));
        }
        private string Print(ASTnode node)
        {
            return node.Accept(this);
        }

        public string Visit(IfStmt _stmt)
        {
            throw new NotImplementedException();
        }

        public string Visit(Print _print)
        {
            return Print(_print.Expr);
        }

        public string Visit(ExpressionStmt _stmt)
        {
            return Print(_stmt.expression);
        }

        public string Visit(LetStmt _let)
        {
            throw new NotImplementedException();
        }

        public string Visit(Assignment _assign)
        {
            throw new NotImplementedException();
        }

        public string Visit(String_ _string)
        {
            return _string.Value;
        }

        public string Visit(BinOp _binary)
        {
            return $"( {Print(_binary.Left)} {_binary.Op.Lexeme} {Print(_binary.Right)} )";
        }

        public string Visit(Boolean _boolean)
        {
            return _boolean.Value.ToString();
        }

        public string Visit(UnaryExpr _unary)
        {
            return $"( {_unary.Operator.Lexeme} {Print(_unary.Right)} )";
        }

        public string Visit(Num _num)
        {
            return _num.Value.ToString();
        }

        public string Visit(Grouping _group)
        {
            return $"( {Print(_group.Expression)} )";
        }

        public string Visit(CallFunction _call)
        {
            throw new NotImplementedException();
        }

        public string Visit(Logical _logical)
        {
            return $"( {Print(_logical.Left)}  {_logical.Operator.Lexeme}  {Print(_logical.Right)} ) ";
        }

        string Visit(FunctionStmt _stmt)
        {
            throw new NotImplementedException();
        }

        public string Visit(MathExpr _value)
        {
            if (_value.Value == Math.E) return _value.Value.ToString();
            else if (_value.Value == Math.PI) return _value.Value.ToString();
            return "";
        }

        string ASTnode.IVisitor<string>.Visit(FunctionStmt _stmt)
        {
            throw new NotImplementedException();
        }
    }
}