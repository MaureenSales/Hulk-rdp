using System.Security.Cryptography.X509Certificates;

namespace Hulk
{
    public class Interprete : ASTnode.IVisitor<object>
    {
        public static Dictionary<string, FunctionStmt>? functions; //diccionario con las funciones declaradas nombre-nodo
        public Stack<Dictionary<string, object>> VariableScopes; //pila de diccionarios con nombre-valor para las variables y funciones

        public Interprete()
        {
            //se inicializan tanto el diccionario como la pila
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
            //verifica si una variable fue declarada es decir si esta en la pila, tambien para las funciones
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
            //visita el nodo a evaluar determinando su tipo y ejecutando la evaluacion correspodiente 
            return expr.Accept(this);
        }

        public object Visit(Num num)
        {
            //dado un nodo de tipo numero devuelve su valor que es double
            return num.Value;
        }
        public object Visit(String_ string_)
        {
            //dado un nodo de tipo string devuelve su valor que es string
            return string_.Value;
        }

        public object Visit(Logical expr)
        {
            //dado un nodo de epresion logica, evalua la parte izquierda y la derecha, determina si son expresiones booleanas 
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
            //dado uun nodo tipo agrupado es decir expresion dentro de parentesis, se evalua dicha expresion
            return evaluate(expr.Expression);
        }

        public object Visit(ExpressionStmt stmt)
        {
            //nodo de expresion a ser evaluado
            object result = evaluate(stmt.expression);
            return result;
        }

        public object Visit(UnaryExpr expr)
        {
            //nodo de expresion unaria, se evalua su parte derecha
            //si es de la forma -expresion, se verifica que expresion sea un numero y si es de la forma !expresion, se verifica que sea una expresion booleana
            object right = evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Subtraction:
                    CheckNumberOperand(right);
                    return -(double)right;
                case TokenType.Negation:
                    IsTruthy(right);
                    if (right is false) return true;
                    else return false;
            }

            return right;
        }


        public object Visit(BinOp expr)
        {
            //dado un nodo de operador binario se evalua su parte derecha e izquierda
            object left = evaluate(expr.Left);
            object right = evaluate(expr.Right);


            switch (expr.Op.Type)
            {
                //si es de comparacion se chequea que los operandos sean numeros
                //si es de sustraccion,division,producto,potencia y modulo tambien
                //si es de suma verifica que ambos operandos sean o numeros o strings
                //si es de concatenacion de verifican ls distintas combinaciones de string, numero y booleano
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
                    throw Error.Error_(Error.ErrorType.SEMANTIC, "Cann't operate between " + left.GetType() + " and " + right.GetType() + "types. ");
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
            //nodo de llamada a la funcion identidad(print), evalua la expresion de argumento y la imprime en consola
            System.Console.WriteLine(evaluate(stmt.Expr));
            return null;
        }

        public object Visit(Assignment expr)
        {
            //dado un nodo de asignacion se evalua la expresion valor de la variable
            object value = evaluate(expr.Value);
            //se agrega a la pila la nueva variable declarada
            VariableScopes.Peek().Add(expr.Name, value);
            return value;
        }

        public object Visit(IfStmt stmt)
        {
            //dado un nodo de expresion condicional, verifica si la condicion es una expresion booleana
            //si es falsa entonces evalua la expresion del else
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
            //dado un nodo booleano retorna su valor de verdad
            return _boolean.Value;
        }

        public object Visit(CallFunction expr)
        {
            //dado un nodo de llamada de funcion si es una funcion predeterminada: sin(x), cos(x), tan(x), sqrt(x), log(b,a)
            //verifica la cantidad de argumentos es decir la aridad y el tipo de los mismos
            //si es una funcion no predeterminada, revisa en la pila que este declarada, agrega los argumentos a la pila, verifica su aridad y los evalua
            //evalua el cuerpo y saca de la pila las variables usadas en la llamada
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
            //agrega a la pila el diccionario de las variables declaradas 
            //evalua cada asignacion de valor
            //evalua el cuerpo del let 
            //saca de la pila el diccionario de las variables declaradas 
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
            //dado un nodo de variable, busca en la pila si en algun contexto padre fue declarada y retorna su valor
            return FindVariable(_reference.Name);
        }

        public object Visit(FunctionStmt _stmt)
        {
            //dada una declaracion de funcion se agrega al dicionario de funciones si aun no esta, no tiene valor de retorno
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
            //dado un nodo de eexpresion matematica retorna su valor
            return _value.Value;
        }
        private bool IsTruthy(object ob)
        {
            //comprueba si un objeto tiene valor de verdad y lo devuelve
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