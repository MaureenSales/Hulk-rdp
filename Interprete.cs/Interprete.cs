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
        private object evaluate(ASTnode expr)
        {
            return expr.Accept(this);
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

        private bool IsTruthy(object ob)
        {
            if (ob == null) return false;
            if (ob is Boolean) return (bool)ob;
            return true;
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
            }

            // Unreachable.
            return null;
        }
        private bool IsEqual(object a, object b)
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

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public object Visit(Print stmt)
        {
            evaluate(stmt.Expr);
            return null;
        }

        public object Visit(VariableStmt stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = evaluate(stmt.Initializer);
            }
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
                Execute(stmt.ThenBody);
            }
            else if (stmt.ElseBody != null)
            {
                Execute(stmt.ElseBody);
            }
            return null;
        }

        public object Visit(CallFunction expr)
        {
            object callee = evaluate(expr.Callee);

            List<object> arguments = new List<object>();
            foreach (ASTnode argument in expr.Parameters)
            {
                arguments.Add(evaluate(argument));
            }
            return callee;//arreglar
        }

        public object Visit(Boolean _boolean)
        {
            throw new NotImplementedException();
        }

        public object Visit(Let _let)
        {
            throw new NotImplementedException();
        }

        public object Visit(Variable _var)
        {
            throw new NotImplementedException();
        }

        public object Visit(FunctionStmt _stmt)
        {
            throw new NotImplementedException();
        }
    }
}