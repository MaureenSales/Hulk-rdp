using System.Security.Cryptography.X509Certificates;

namespace Hulk
{
    public class Interprete : ASTnode.IVisitor<object>
    {
        public static Dictionary<string, FunctionStmt>? functions;
        public Stack<Dictionary<string, object>> VariableScopes;

        public Interprete()
        {
            functions = new();
            VariableScopes = new();
            EnterScope();
        }

        public void EnterScope()
        {
            VariableScopes.Push(new Dictionary<string, object>());
        }

        public void ExitScope()
        {
            VariableScopes.Pop();
        }

        public object FindVariable(string name)
        {
            foreach (var item in VariableScopes)
            {
                if (item.ContainsKey(name))
                {
                    var x = item[name];
                    return x;
                }

            }
            return Error.Error_(Error.ErrorType.SEMANTIC, "Undeclared varible '" + name + "'. ");
        }

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
            return evaluate(expr.Expression);
        }

        public object Visit(ExpressionStmt stmt)
        {
            object result = evaluate(stmt.expression);
            return result;
        }

        public object Visit(UnaryExpr expr)
        {
            object right = evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Subtraction:
                    CheckNumberOperand(right);
                    return -(double)right;
            }

            return right;
        }


        public object Visit(BinOp expr)
        {
            object left = evaluate(expr.Left);
            object right = evaluate(expr.Right);


            switch (expr.Op.Type)
            {
                case TokenType.GreaterThan:
                    CheckNumberOperands(left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterOrEqual:
                    CheckNumberOperands(left, right);
                    return (double)left >= (double)right;
                case TokenType.LessThan:
                    CheckNumberOperands(left, right);
                    return (double)left < (double)right;
                case TokenType.LessOrEqual:
                    CheckNumberOperands(left, right);
                    return (double)left <= (double)right;
                case TokenType.NotEqual: return !IsEqual(left, right);
                case TokenType.Equality: return IsEqual(left, right);
                case TokenType.Subtraction:
                    CheckNumberOperands(left, right);
                    return (double)left - (double)right;
                case TokenType.Sum:

                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Cann't operate between " + left.GetType() + " and " + right.GetType() + "types. " );
                case TokenType.Division:
                    CheckNumberOperands(left, right);
                    return (double)left / (double)right;
                case TokenType.Product:
                    CheckNumberOperands(left, right);
                    return (double)left * (double)right;
                case TokenType.Modulo:
                    CheckNumberOperands(left, right);
                    return (double)left % (double)right;
                case TokenType.Pow:
                    CheckNumberOperands(left, right);
                    return Math.Pow((double)left, (double)right);
                case TokenType.Concat:
                    string concated = "";
                    if (left is string) concated += left;
                    else if (left is double) concated += left;
                    else if (left is bool) concated += left;
                    else
                    {
                        throw Error.Error_(Error.ErrorType.SEMANTIC, " Operator '@' cann't be used between " + left.GetType() + " and " + right.GetType() + "types. ");
                    }
                    if (right is string) concated += right;
                    else if (right is double) concated += right;
                    else if (right is bool) concated += right;
                    else
                    {
                        throw Error.Error_(Error.ErrorType.SEMANTIC, " Operator '@' cann't be used between " + left.GetType() + " and " + right.GetType() + "types. ");
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
            VariableScopes.Peek().Add(expr.Name, value);
            return value;
        }

        public object Visit(IfStmt stmt)
        {
            object? result = null;
            if (IsTruthy(evaluate(stmt.Condition)))
            {
                result = evaluate(stmt.ThenBody);
            }
            else if (stmt.ElseBody != null)
            {
                result = evaluate(stmt.ElseBody);
            }
            return result;
        }


        public object Visit(Boolean _boolean)
        {
            return _boolean.Value;
        }

        public object Visit(CallFunction expr)
        {

            if (expr.Callee.Lexeme == "sin")
            {
                if (expr.Arguments.Count > 1)
                {
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Funtion sin(x) receive a argument");
                }
                else
                {
                    var argument = evaluate(expr.Arguments[0]);
                    if (argument is double)
                    {
                        return Math.Sin((double)argument);
                    }
                    else
                    {
                        throw Error.Error_(Error.ErrorType.SEMANTIC, "Function sin(x) receive a double not a" + argument.GetType());
                    }
                }

            }
            else if (expr.Callee.Lexeme == "cos")
            {
                if (expr.Arguments.Count > 1)
                {
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Funtion cos(x) receive a argument");
                }
                else
                {
                    var argument = evaluate(expr.Arguments[0]);
                    if (argument is double)
                    {
                        return Math.Cos((double)argument);
                    }
                    else
                    {
                        throw Error.Error_(Error.ErrorType.SEMANTIC, "Function cos(x) receive a double not a" + argument.GetType());
                    }
                }
            }
            else if (expr.Callee.Lexeme == "tan")
            {
                if (expr.Arguments.Count > 1)
                {
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Funtion tan(x) receive a argument");
                }
                else
                {
                    var argument = evaluate(expr.Arguments[0]);
                    if (argument is double)
                    {
                        return Math.Tan((double)argument);
                    }
                    else
                    {
                        throw Error.Error_(Error.ErrorType.SEMANTIC, "Function tan(x) receive a double not a" + argument.GetType());
                    }
                }
            }
            else if (expr.Callee.Lexeme == "sqrt")
            {
                if (expr.Arguments.Count > 1)
                {
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Funtion sqrt(x) receive a argument");
                }
                else
                {
                    var argument = evaluate(expr.Arguments[0]);
                    if (argument is double)
                    {
                        return Math.Sqrt((double)argument);
                    }
                    else
                    {
                        throw Error.Error_(Error.ErrorType.SEMANTIC, "Function sqrt(x) receive a double not a" + argument.GetType());
                    }
                }
            }
            else if (expr.Callee.Lexeme == "log")
            {
                if (expr.Arguments.Count > 2)
                {
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Funtion log(x) receive two argument (argument, base)");
                }
                else
                {
                    var argument1 = evaluate(expr.Arguments[0]);
                    var argument2 = evaluate(expr.Arguments[1]);
                    if (argument1 is double && argument2 is double)
                    {
                        return Math.Log((double)argument1, (double)argument2);
                    }
                    else
                    {
                        throw Error.Error_(Error.ErrorType.SEMANTIC, "Function log(x) receive two doubles not " + argument1.GetType() + " and " + argument2.GetType());
                    }
                }
            }

            else if (!functions.ContainsKey(expr.Callee.Lexeme))
            {
                throw Error.Error_(Error.ErrorType.SEMANTIC, "Non defined function" + expr.Callee.Lexeme + ". ");
            }
            else
            {
                FunctionStmt fun = functions[expr.Callee.Lexeme];

                EnterScope();
                if (fun.Params.Count != expr.Arguments.Count)
                {
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Incorrect amount of parameters");
                }
                else
                {
                    for (int i = 0; i < fun.Params.Count; i++)
                    {
                        fun.Params[i].Value = expr.Arguments[i];
                    }


                    foreach (var item in fun.Params)
                    {
                        evaluate(item);
                    }

                    object result = evaluate(fun.Body);
                    ExitScope();
                    return result;
                }
            }

        }
        public object Visit(LetStmt _let)
        {
            EnterScope();
            foreach (var item in _let.Declarations)
            {
                evaluate(item);
            }

            object value = evaluate(_let.Body);
            ExitScope();
            return value;
        }

        public object Visit(VariableReference _reference)
        {
            return FindVariable(_reference.Name);
        }

        public object Visit(FunctionStmt _stmt)
        {
            if (!functions.ContainsKey(_stmt.Name))
            {
                functions.Add(_stmt.Name, _stmt);
            }
            else
            {
                throw Error.Error_(Error.ErrorType.SEMANTIC, "Already defined function" + _stmt.Name + ". ");
            }
            return null;
        }

        public object Visit(MathExpr _value)
        {
            return _value.Value;
        }
        private bool IsTruthy(object ob)
        {
            if (ob == null) return false;
            if (ob is bool) return (bool)ob;
            throw Error.Error_(Error.ErrorType.SEMANTIC, "Operand must be a boolean.");
        }
        private void CheckNumberOperand(object operand)
        {
            if (operand is double) return;
            throw Error.Error_(Error.ErrorType.SEMANTIC, "Operand must be a number.");
        }
        private void CheckNumberOperands(object left, object right)
        {
            if (left is double && right is double) return;
            throw Error.Error_(Error.ErrorType.SEMANTIC, "Operands must be numbers.");
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