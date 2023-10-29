namespace Hulk
{
    public class Interprete : ASTnode.IVisitor<object>
    {
        public object evaluate(ASTnode expr)
        {
            return expr.Accept(this);
        }

        public object Visit(Num num)
        {
            return num.Value;
        }
        public object Visit(String_ string_)
        {
            return string_.Value;
        }

        public object Visit(Logical expr)
        {
            object left = evaluate(expr.Left);

            if (expr.Operator.Type == TokenType.Disjunction)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return evaluate(expr.Right);
        }

        public object Visit(Grouping expr)
        {
            return expr.Expression;
        }

        public object Visit(ExpressionStmt stmt)
        {
            evaluate(stmt.expression);
            return null;
        }

        public object Visit(UnaryExpr expr)
        {
            object right = evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Subtraction:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            return null;
        }


        public object Visit(BinOp expr)
        {
            object left = evaluate(expr.Left);
            object right = evaluate(expr.Right);

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
                case TokenType.Concat:
                    string concated = "";
                    if (left is string) concated += left;
                    else if (left is double) concated += left;
                    else if (left is bool) concated += left;
                    else
                    {
                        throw Error.Error_(expr.Op.Line, Error.ErrorType.SEMANTIC, "", " Operator '@' cannot be used between " + left + " " + right);
                    }
                    if (right is string) concated += right;
                    else if (right is double) concated += right;
                    else if (right is bool) concated += right;
                    else
                    {
                        throw Error.Error_(expr.Op.Line, Error.ErrorType.SEMANTIC, "", " Operator '@' cannot be used between " + left + " " + right);
                    }
                    return concated;

            }

            // Unreachable.
            return null;
        }

        public object Visit(Print stmt)
        {
            System.Console.WriteLine(evaluate(stmt.Expr));
            return null;
        }

        public object Visit(Assignment expr)
        {
            object value = evaluate(expr.Value);
            return value;
        }

        public object Visit(IfStmt stmt)
        {
            if (IsTruthy(evaluate(stmt.Condition)))
            {
                evaluate(stmt.ThenBody);
            }
            else if (stmt.ElseBody != null)
            {
                evaluate(stmt.ElseBody);
            }
            return null;
        }


        public object Visit(Boolean _boolean)
        {
            return _boolean.Value;
        }

        public object Visit(CallFunction expr)
        {
            throw new NotImplementedException();
        }
        public object Visit(LetStmt _let)
        {
            foreach (var item in _let.Declarations)
            {
                evaluate(item);
            }
            return evaluate(_let.Body);
        }

        public object Visit(Variable _var)
        {
            throw new NotImplementedException();
        }

        public object Visit(FunctionStmt _stmt)
        {
            throw new NotImplementedException();
        }

        public object Visit(MathExpr _value)
        {
            throw new NotImplementedException();
        }
        private bool IsTruthy(object ob)
        {
            if (ob == null) return false;
            if (ob is bool) return (bool)ob;
            throw Error.Error_(1, Error.ErrorType.SEMANTIC, "", "Operand must be a boolean.");
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
        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            if (b == null) return false;

            return a.Equals(b);
        }

    }
}