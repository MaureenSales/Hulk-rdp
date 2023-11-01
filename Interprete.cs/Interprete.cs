using System.Security.Cryptography.X509Certificates;

namespace Hulk
{
    public class Interprete : ASTnode.IVisitor<object>
    {
        public static Dictionary<string, FunctionStmt> functions;
        public Stack<Dictionary<string, object>> VariableScopes;

        public Interprete()
        {
            System.Console.WriteLine("constructor");
            functions = new();
            VariableScopes = new();
            EnterScope();
        }

        public void EnterScope()
        {
            System.Console.WriteLine("entrando al scope");
            VariableScopes.Push(new Dictionary<string, object>());
        }

        public void ExitScope()
        {
            System.Console.WriteLine("saliendo del scope");
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
            return Error.Error_(1, Error.ErrorType.SEMANTIC, "", "Variable no declarada");
        }

        public object evaluate(ASTnode expr)
        {
            return expr.Accept(this);
        }

        public object Visit(Num num)
        {
            System.Console.WriteLine(num.Value + "is visited");
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
            object result = evaluate(stmt.expression);
            return result;
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
                    System.Console.WriteLine(left + "algo");
                    System.Console.WriteLine(right);

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
            System.Console.WriteLine("count de la pila " + VariableScopes.Count);
            VariableScopes.Peek().Add(expr.Name, value);
            return value;
        }

        public object Visit(IfStmt stmt)
        {
            object result = null;
            if (IsTruthy(evaluate(stmt.Condition)))
            {
                System.Console.WriteLine(result + "hi");
                System.Console.WriteLine(stmt.ThenBody);
                result = evaluate(stmt.ThenBody);
                System.Console.WriteLine(result + "hello");
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

            if (!functions.ContainsKey(expr.Callee.Lexeme))
            {
                throw Error.Error_(expr.Callee.Line, Error.ErrorType.SEMANTIC, "", "Non defined function");
            }
            else
            {
                FunctionStmt fun = functions[expr.Callee.Lexeme];
                EnterScope();
                if (fun.Params.Count != expr.Arguments.Count)
                {
                    throw Error.Error_(expr.Callee.Line, Error.ErrorType.SEMANTIC, "", "Incorrect amount of parameters");
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

            // switch (expr.Callee.Lexeme)
            // {
            //     case "rand":
            //         Random result = new Random();
            //         return result.NextDouble();
            //     case "cos":
            //         return (double)Math.Cos((double)arguments[0]);
            //     case "sin":
            //         return (double)Math.Sin((double)arguments[0]);
            //     case "sqrt":
            //         return (double)Math.Sqrt((double)arguments[0]);
            //     case "log":
            //         return (double)Math.Log((double)arguments[1], (double)arguments[0]);
            //     default:
            //         throw new NotImplementedException();
            // }
            // //

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
                throw Error.Error_(1, Error.ErrorType.SEMANTIC, "", "Already defined function");
            }
            return null;
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