using System.Security.AccessControl;
using System;
using Microsoft.CSharp.RuntimeBinder;

namespace Hulk
{
    public class Interprete : ASTnode.IVisitor<object>, Stmt.IVisitor<object>
    {
        public object Visit(Num num)
        {
            return num.Value;
        }
        public object Visit(String_ string_)
        {
            return string_.Value;
        }
        public Object Visit(Grouping expr)
        {
            return expr.Expression;
        }
        private Object evaluate(ASTnode expr)
        {
            return expr.Accept(this);
        }

        public void Visit(ExpressionStmt stmt)
        {
            evaluate(stmt.expression);
        }

        public Object Visit(UnaryExpr expr)
        {
            Object right = evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Subtraction:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            return null;
        }

        private bool IsTruthy(Object ob)
        {
            if (ob == null) return false;
            if (ob is Boolean) return (bool)ob;
            return true;
        }

        public Object Visit(BinOp expr)
        {
            Object left = evaluate(expr.Left);
            Object right = evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.GreaterThan:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterOrEqual:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LessThan:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                case TokenType.LessOrEqual:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;
                case TokenType.NotEqual: return !IsEqual(left, right);
                case TokenType.Equality: return IsEqual(left, right);
                case TokenType.Subtraction:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left - (double)right;
                case TokenType.Sum:
                    if (left is Double && right is Double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is String && right is String)
                    {
                        return (String)left + (String)right;
                    }
                    throw Error.Error_(expr.Op.Line, Error.ErrorType.SEMANTIC, "", "Operands must be two numbers or two strings.");
                case TokenType.Division:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left / (double)right;
                case TokenType.Product:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                case TokenType.Modulo:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left % (double)right;
                case TokenType.Pow:
                    CheckNumberOperands(expr.Op, left, right);
                    return Math.Pow((double)left, (double)right);
            }

            // Unreachable.
            return null;
        }
        private bool IsEqual(Object a, Object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            if (b == null) return false;

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token op, Object operand)
        {
            if (operand is Double) return;
            throw Error.Error_(op.Line, Error.ErrorType.SEMANTIC, "", "Operand must be a number.");
        }
        private void CheckNumberOperands(Token op, Object left, Object right)
        {
            if (left is Double && right is Double) return;
            throw Error.Error_(op.Line, Error.ErrorType.SEMANTIC, "", "Operands must be numbers.");
        }

        private void interpret(ASTnode expression)
        {
            try
            {
                Object value = evaluate(expression);
            }
            catch (Error)
            {

            }
        }

    }
}