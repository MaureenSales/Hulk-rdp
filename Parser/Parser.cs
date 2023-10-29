namespace Hulk
{
    public class Parser
    {
        public Lexer Tokenized { get; private set; }
        private int position = 0;
        public Parser(Lexer tokenized)
        {
            Tokenized = tokenized;
        }

        public List<ASTnode> Parse()
        {
            List<ASTnode> statements = new List<ASTnode>();
            while (!IsAtEnd())
            {
                statements.Add(Statement());
            }
            return statements;
        }

        private ASTnode Primary()
        {
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
                case TokenType.Identifier:
                    position++;
                    return new Variable(Previous());
                case TokenType.Number:
                    position++;
                    return new Num(Previous());
                case TokenType.String:
                    position++;
                    return new String_(Previous().Lexeme);
                case TokenType.OpParenthesis:
                    position++;
                    var node = LevelFour();
                    Consume(TokenType.ClParenthesis, "Expect ')' after expression.");
                    return new Grouping(node);

                default: throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "at '" + Current().Lexeme + "'", "Missing left-hand or right-hand operand.");

            }



        }
        private ASTnode Unary()
        {
            while (Eat(TokenType.Subtraction))
            {
                Token op = Previous();
                ASTnode right = Unary();
                return new UnaryExpr(op, right);
            }
            return Primary();
        }

        private ASTnode LevelTwo()
        {
            var node = Unary();
            while (Eat(TokenType.Pow))
            {
                Token op = Previous();
                ASTnode right = LevelTwo();
                node = new BinOp(node, op, right);
            }
            return node;
        }

        private ASTnode LevelThree() // factor
        {
            var node = LevelTwo();

            while (Eat(TokenType.Product) || Eat(TokenType.Division) || Eat(TokenType.Modulo))
            {
                Token op = Previous();
                ASTnode right = LevelTwo();
                node = new BinOp(node, op, right);
            }
            return node;
        }

        private ASTnode LevelFour() //term
        {
            ASTnode node = LevelThree();

            while (Eat(TokenType.Sum) || Eat(TokenType.Subtraction))
            {
                Token op = Previous();
                ASTnode right = LevelThree();
                node = new BinOp(node, op, right);
            }
            return node;
        }

        private ASTnode Or()
        {
            ASTnode expr = And();

            while (Eat(TokenType.Disjunction))
            {
                Token op = Previous();
                ASTnode right = And();
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        private ASTnode And()
        {
            ASTnode expr = Equality();

            while (Eat(TokenType.Conjunction))
            {
                Token op = Previous();
                ASTnode right = Equality();
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        private ASTnode Equality()
        {
            ASTnode node = Comparison();
            while (Eat(TokenType.Equality) || Eat(TokenType.NotEqual))
            {
                Token op = Previous();
                ASTnode right = Comparison();
                node = new BinOp(node, op, right);

            }
            return node;
        }

        private ASTnode Comparison()
        {
            ASTnode node = Concat(); //concat
            while (Eat(TokenType.LessThan) || Eat(TokenType.LessOrEqual) || Eat(TokenType.GreaterThan) || Eat(TokenType.GreaterOrEqual))
            {
                Token op = Previous();
                ASTnode right = LevelFour(); //concat
                node = new BinOp(node, op, right);
            }
            return node;
        }

        private ASTnode Concat()
        {
            ASTnode node = LevelFour();
            while (Eat(TokenType.Concat))
            {
                Token op = Previous();
                node = new BinOp(node, op, LevelFour());
            }
            return node;
        }


        private ASTnode Statement()
        {

            if (Eat(TokenType.If))
            {
                return IfStatement();
            }
            else if (Eat(TokenType.Print))
            {
                return PrintStatement();
            }
            else if (Eat(TokenType.Let))
            {
                return LetStatement();
            }
            return ExpressionStatement();
        }

        private ASTnode PrintStatement()
        {
            Consume(TokenType.OpParenthesis, "Expect '(' after expression");
            ASTnode value = Expr();
            Consume(TokenType.ClParenthesis, "Expect ')' after value");
            return new Print(value);
        }

        private ASTnode Expr()
        {
            return Or();
        }

        private ASTnode ExpressionStatement()
        {
            ASTnode expr = Expr();
            return new ExpressionStmt(expr);
        }



        private ASTnode IfStatement()
        {
            Consume(TokenType.OpParenthesis, "Expect '(' after 'if'.");
            ASTnode condition = Expr();
            Consume(TokenType.ClParenthesis, "Expect ')' after if condition.");

            ASTnode then_body = Statement();
            Consume(TokenType.Else, "Expect 'else declaration' after if condition");
            ASTnode else_body = Statement();

            return new IfStmt(condition, then_body, else_body);
        }

        private ASTnode LetStatement()
        {
            List<Assignment> let_declarations = Assignment();
            Consume(TokenType.In, " Expected 'in' after let statement");

            ASTnode in_body = Statement();
            return new LetStmt(let_declarations, in_body);

        }

        private List<Assignment> Assignment()
        {
            List<Assignment> let_declarations = new List<Assignment>();

            while (!Eat(TokenType.In))
            {
                string name = Consume(TokenType.Identifier, "Invalid assignment target.").Lexeme;
                Consume(TokenType.Assignment, "Expect '=' after variable name");
                ASTnode right = Expr();

                if (!Eat(TokenType.In))
                {
                    Consume(TokenType.Comma, "Expect 'in' or ',' after expression ");
                    if (Eat(TokenType.In))
                    {
                        throw Error.Error_(Previous().Line, Error.ErrorType.SINTACTIC, "at '" + Previous().Lexeme + "'", "Invalid token 'in' after ','");
                    }
                }

                let_declarations.Add(new Hulk.Assignment(name, right));
            }

            return let_declarations;

        }
        private Token Current()
        {
            return Tokenized.Tokens[position];
        }

        private Token Previous()
        {
            return Tokenized.Tokens[position - 1];
        }

        private Token Advance()
        {
            if (!IsAtEnd()) position++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            if (Current().Type == TokenType.Eof) return true;
            return false;
        }

        private bool Eat(TokenType type)
        {
            if (Check(type))
            {
                position++;
                return true;
            }
            else return false;
        }
        private Token Consume(TokenType type, String message)
        {
            if (Check(type)) return Advance();

            throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "at '" + Previous().Lexeme + "'", message);
        }
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Current().Type == type;
        }
    }

}
