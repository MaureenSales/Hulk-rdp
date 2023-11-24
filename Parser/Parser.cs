using System.Diagnostics.Contracts;

namespace Hulk
{
    public class Parser
    {
        public Lexer Tokenized { get; private set; } // instancia de la clase lexer para tener un objeto de este tipo y acceder a sus propiedades
        private int position = 0; //iterador a traves de la lista de tokens

        public Parser(Lexer tokenized)
        {
            Tokenized = tokenized;
        }

        //crea una lista de nodos que son construidos con los tokens
        public List<ASTnode> Parse()
        {
            List<ASTnode> statements = new List<ASTnode>();

            while (!IsAtEnd())
            {
                statements.Add(Fun());
            }

            int count = Tokenized.Tokens.Count;
            //verifico que la linea de codigo termine con ';'
            if (Tokenized.Tokens[count - 2].Type != TokenType.Semicolon)
            {
                throw Error.Error_(Error.ErrorType.SINTACTIC, "Expect ';' at end expression");
            }
            return statements;
        }

        private ASTnode Primary()
        {
            //dado el tipo del token actual...
            switch (Current().Type)
            {
                case TokenType.False:
                    position++;
                    return new Boolean(false);
                case TokenType.True:
                    position++;
                    return new Boolean(true);
                case TokenType.PI:
                    position++;
                    return new MathExpr(Math.PI);
                case TokenType.Euler:
                    position++;
                    return new MathExpr(Math.E);
                case TokenType.Number:
                    position++;
                    return new Num(Previous());
                case TokenType.String:
                    position++;
                    return new String_(Previous().Lexeme);
                case TokenType.Identifier:
                    string name = Current().Lexeme;
                    position++;
                    if (Current().Type == TokenType.OpParenthesis)
                    {
                        return CallFunction();
                    }
                    else if (Current().Type == TokenType.Assignment)
                    {
                        throw Error.Error_(Error.ErrorType.SINTACTIC, "Can only assign value to a variable within an expression let-in and cann't reassign valor to the variable");
                    }
                    else return new VariableReference(name);
                case TokenType.OpParenthesis:
                    position++;
                    ASTnode node = LevelFour();
                    Consume(TokenType.ClParenthesis, "Expect ')' after expression.");
                    return new Grouping(node);
                case TokenType.Let:
                    position++;
                    return LetStatement();
                case TokenType.If:
                    position++;
                    return IfStatement();

                default:
                    switch (Current().Type)
                    {
                        case TokenType.Sum:
                        case TokenType.Subtraction:
                        case TokenType.Pow:
                        case TokenType.Product:
                        case TokenType.Division:
                        case TokenType.Modulo:
                        case TokenType.LessThan:
                        case TokenType.LessOrEqual:
                        case TokenType.GreaterThan:
                        case TokenType.GreaterOrEqual:
                        case TokenType.Concat:
                        case TokenType.Disjunction:
                        case TokenType.Conjunction:
                        case TokenType.Assignment:
                        case TokenType.Equality:
                        case TokenType.NotEqual:
                            throw Error.Error_(Error.ErrorType.SINTACTIC, "Missing left-hand or right-hand operand '" + Current().Lexeme + "'. ");
                        default:
                            throw Error.Error_(Error.ErrorType.SINTACTIC, "Unexpected character '" + Current().Lexeme + "'. ");
                    }



            }

        }


        //verifica que ';' solo este al final de la linea de codigo
        private void Semicolon()
        {
            if (position != Tokenized.Tokens.Count - 2)
            {
                throw Error.Error_(Error.ErrorType.SINTACTIC, "Unexpected character ';' is only placed at the end expression");
            }
            else
            {
                position++;
            }

        }

        private ASTnode CallFunction()
        {
            //estructura de una llamada: nombre de la funcion, lista de parametros entre parentesis
            //callee es el nombre de la funcion llamada que es el token anterior al actual
            Token callee = Previous();
            Consume(TokenType.OpParenthesis, "Expect '(' after call function ");
            List<ASTnode> arguments = new List<ASTnode>();

            if (!Check(TokenType.ClParenthesis))
            {
                do
                {
                    arguments.Add(Or());
                }
                while (Eat(TokenType.Comma));
            }

            Token paren = Consume(TokenType.ClParenthesis, "Expect ')' after arguments");

            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return new CallFunction(callee, paren, arguments);
        }

        private ASTnode Unary()
        {
            //estructura de un operador unario: '-' + 'op unario' o '!' + 'expresion primaria'
            while (Eat(TokenType.Subtraction))
            {
                Token op = Previous();
                ASTnode right = Unary();
                if (Check(TokenType.Semicolon))
                {
                    Semicolon();
                }
                return new UnaryExpr(op, right);
            }
            if (Eat(TokenType.Negation))
            {
                Token op = Previous();
                ASTnode right = Primary();
                return new UnaryExpr(op, right);
            }
            return Primary();
        }

        private ASTnode LevelTwo()
        {
            //se encarga de crear el operador binario de potencia que se asocia a la izquierda: a^a^b = a^(a^b) 
            ASTnode node = Unary();
            while (Eat(TokenType.Pow))
            {
                Token op = Previous();
                ASTnode right = LevelTwo();

                node = new BinOp(node, op, right);
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return node;
        }

        private ASTnode LevelThree()
        {
            //se encarga de  crear el operador binario referente al producto, la division y el modulo que se asocian a la izquierda
            ASTnode node = LevelTwo();

            while (Eat(TokenType.Product) || Eat(TokenType.Division) || Eat(TokenType.Modulo))
            {
                Token op = Previous();
                ASTnode right = LevelTwo();
                node = new BinOp(node, op, right);
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return node;
        }

        private ASTnode LevelFour() 
        {
            //se encarga de crear el operador binario de mas bajo nivel que seria la suma y la resta asociadas a la derecha
            ASTnode node = LevelThree();

            while (Eat(TokenType.Sum) || Eat(TokenType.Subtraction))
            {
                Token op = Previous();
                ASTnode right = LevelThree();
                node = new BinOp(node, op, right);
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return node;
        }

        private ASTnode Or()
        {
            //comprueba si tenemos una expresion que llame logica en este caso el Or
            ASTnode expr = And();

            while (Eat(TokenType.Disjunction))
            {
                Token op = Previous();
                ASTnode right = And();
                expr = new Logical(expr, op, right);
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return expr;
        }

        private ASTnode And()
        {
            //se comprueba si estamos en presencia de una expresion logica con And
            ASTnode expr = Equality();

            while (Eat(TokenType.Conjunction))
            {
                Token op = Previous();
                ASTnode right = Equality();
                expr = new Logical(expr, op, right);
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return expr;
        }

        private ASTnode Equality()
        {
            //se encarga de crear la expresion binaria para la igualdad o la diferencia
            ASTnode node = Comparison();
            while (Eat(TokenType.Equality) || Eat(TokenType.NotEqual))
            {
                Token op = Previous();
                ASTnode right = Comparison();
                node = new BinOp(node, op, right);

            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return node;
        }

        private ASTnode Comparison()
        {
            //expresiones de comparacion mayor que, menor que, mayor igual, menor igual
            ASTnode node = Concat(); 
            while (Eat(TokenType.LessThan) || Eat(TokenType.LessOrEqual) || Eat(TokenType.GreaterThan) || Eat(TokenType.GreaterOrEqual))
            {
                Token op = Previous();
                ASTnode right = Concat(); 
                node = new BinOp(node, op, right);
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return node;
        }

        private ASTnode Concat()
        {
            //expresion binaria de concatencion con operador @
            ASTnode node = LevelFour();
            while (Eat(TokenType.Concat))
            {
                Token op = Previous();
                node = new BinOp(node, op, LevelFour());
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return node;
        }

        private ASTnode Fun()
        {
            //verifica si hay una declaracion de funcion y llama al metodo q se encarga de consumirla 
            if (Eat(TokenType.Function))
            {
                return Function();
            }
            return Statement();
        }

        private ASTnode Statement()
        {
            //verifica si hay un comando print y llama al metodo que lo consume y crea el nodo de la expresion
            if (Eat(TokenType.Print))
            {
                return PrintStatement();
            }

            return ExpressionStatement();
        }

        private ASTnode PrintStatement()
        {
            //consume la llamada a print y devuelve su nodo 
            Consume(TokenType.OpParenthesis, "Expect '(' after function print");
            ASTnode value = Or();
            Consume(TokenType.ClParenthesis, "Expect ')' after value in function print");
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return new Print(value);
        }

        private ASTnode ExpressionStatement()
        {
            //inicia los llamados para una nueva expresion (mas simple que un llamado print por ejemplo)
            ASTnode expr = Or();
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return new ExpressionStmt(expr);
        }


        private ASTnode IfStatement()
        {
            //Consume la estructura de una expresion condicional: if(expresion condicional) expresion else(expresion)
            Consume(TokenType.OpParenthesis, "Expect '(' before if condition.");
            ASTnode condition = Or();
            Consume(TokenType.ClParenthesis, "Expect ')' after if condition.");

            ASTnode then_body = Statement();
            Consume(TokenType.Else, "Expect 'else declaration' after if condition");
            ASTnode else_body = Statement();
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return new IfStmt(condition, then_body, else_body);
        }

        private ASTnode LetStatement()
        {
            //consume la primera mitad de una expresion let-in: let asignacion  
            List<Assignment> let_declarations = Assignment();
            Consume(TokenType.In, " Expected 'in' after let statement");

            ASTnode in_body = Statement();
           
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return new LetStmt(let_declarations, in_body);

        }

        private List<Assignment> Assignment()
        {
            //segunda mitad de la expresion let-in y declaraciones de variables con su asignacion de valor
            List<Assignment> let_declarations = new List<Assignment>();

            while (Current().Type != TokenType.In)
            {
                string name = Current().Lexeme; Consume(TokenType.Identifier, "Expect variable name not token type " + Current().Type);
                Consume(TokenType.Assignment, "Expect '=' after variable name");
                ASTnode right = Or();

                if (Current().Type != TokenType.In)
                {
                    Consume(TokenType.Comma, "Expect 'in' or ',' after expression ");

                    if (Check(TokenType.In))
                    {
                        Consume(TokenType.In, " Invalid token 'in' after ',' ");
                    }
                }

                let_declarations.Add(new Hulk.Assignment(name, right));
            }
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return let_declarations;

        }

        private ASTnode Function()
        {
            //consume la declaracion de una funcion: function identificador(argumentos) => expresion general que no puede ser otra declaracion
            string function_name = Consume(TokenType.Identifier, "Expect function name after 'function'").Lexeme;
            Consume(TokenType.OpParenthesis, "Expect '(' after function name.");
            List<Assignment> parameters = new List<Assignment>();

            if (!Check(TokenType.ClParenthesis))
            {
                do
                {
                    string name = Consume(TokenType.Identifier, "Expect parameter name.").Lexeme;
                    parameters.Add(new Hulk.Assignment(name, null));
                }
                while (Eat(TokenType.Comma));
            }

            Consume(TokenType.ClParenthesis, "Expect ')' after the function parameters");
            Consume(TokenType.Imply, " Expect '=>' after function declaration");

            ASTnode body = Statement();
            if (Check(TokenType.Semicolon))
            {
                Semicolon();
            }
            return new FunctionStmt(function_name, parameters, body);

        }

        private Token Current()
        {
            //token actual
            return Tokenized.Tokens[position];
        }

        private Token Previous()
        {
            //token anterior
            return Tokenized.Tokens[position - 1];
        }

        private Token Advance()
        {
            //avanza y retorna el token de la posicion antes de avanzar
            if (!IsAtEnd()) position++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            //verifica si esta al final de la lista de tokens
            if (Current().Type == TokenType.Eof) return true;
            return false;
        }

        private bool Eat(TokenType type)
        {
            //chequea un tipo determinado y avanza si es el mismo tipo
            if (Check(type))
            {
                position++;
                return true;
            }
            else return false;
        }
        private Token Consume(TokenType type, String message)
        {
            //chequea un tipo determinado y avanza devolviendo el anterior o lanza un error
            if (Check(type)) return Advance();

            throw Error.Error_(Error.ErrorType.SINTACTIC, message);
        }
        private bool Check(TokenType type)
        {
            //recibe un tipo y lo compara con el tipo del token actual
            if (IsAtEnd()) return false;
            return Current().Type == type;
        }
    }

}

