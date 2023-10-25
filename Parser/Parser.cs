using System.Runtime.InteropServices;
using System.Xml;
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

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Statement());
            }
            return statements;
        }

        private ASTnode Primary()
        {
            if (Current().Type == TokenType.False)
            {
                return new Boolean(false);
            }
            else if (Current().Type == TokenType.True)
            {
                return new Boolean(true);
            }
            else if (Current().Type == TokenType.Number)
            {
                Eat(Current(), TokenType.Number);
                return new Num(Previous());
            }
            else if (Current().Type == TokenType.String)
            {
                Eat(Current(), TokenType.String);
                return new String_(Previous().Lexeme);
            }
            else if (Current().Type == TokenType.Identifier)
            {
                Eat(Current(), TokenType.Identifier);
                return new Variable(Previous());
            }
            else if (Current().Type == TokenType.OpParenthesis)
            {
                Eat(Current(), TokenType.OpParenthesis);
                var node = LevelFour();
                Consume(TokenType.ClParenthesis, "Expect ')' after expression.");
                return new Grouping(node);
            }
            else if (Current().Type == TokenType.GreaterThan || Current().Type == TokenType.GreaterOrEqual || Current().Type == TokenType.LessThan || Current().Type == TokenType.LessOrEqual || Current().Type == TokenType.NotEqual || Current().Type == TokenType.Equality || Current().Type == TokenType.Sum || Current().Type == TokenType.Pow || Current().Type == TokenType.Product || Current().Type == TokenType.Division || Current().Type == TokenType.Modulo)
            {
                throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "at '" + Current().Lexeme + "'", "Missing left-hand operand.");
            }

            throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "at '" + Current().Lexeme + "'", "Expect expression");
        }
        private ASTnode LevelTwo()
        {
            var node = Unary();
            if (Current().Type == TokenType.Pow)
            {
                Token op = Current();
                Eat(Current(), TokenType.Pow);
                var right = Unary();
                node = new BinOp(node, op, right);
            }
            return node;
        }
        private ASTnode LevelThree()
        {
            var node = LevelTwo();

            while (Current().Type == TokenType.Product || Current().Type == TokenType.Division || Current().Type == TokenType.Modulo)
            {
                Token op = Current();
                if (Current().Type == TokenType.Product)
                {
                    Eat(Current(), TokenType.Product);
                }
                else if (Current().Type == TokenType.Division)
                {
                    Eat(Current(), TokenType.Division);
                }
                else if (Current().Type == TokenType.Modulo)
                {
                    Eat(Current(), TokenType.Modulo);
                }

                node = new BinOp(node, op, LevelTwo());
            }
            return node;
        }

        private ASTnode LevelFour()
        {
            var node = LevelThree();

            while (Current().Type == TokenType.Sum || Current().Type == TokenType.Subtraction)
            {
                Token op = Current();
                if (Current().Type == TokenType.Sum)
                {
                    Eat(Current(), TokenType.Sum);
                }
                else if (Current().Type == TokenType.Subtraction)
                {
                    Eat(Current(), TokenType.Subtraction);
                }

                node = new BinOp(node, op, LevelThree());
            }
            return node;
        }

        private ASTnode Unary()
        {
            if (Current().Type == TokenType.Subtraction)
            {
                Advance();
                Token op = Previous();
                return new UnaryExpr(op, Unary());
            }
            return Call(); ;
        }

        private ASTnode Call()
        {
            ASTnode expr = Primary();

            while (true)
            {
                if (Current().Type == TokenType.OpParenthesis)
                {
                    Eat(Current(), TokenType.OpParenthesis);
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private ASTnode FinishCall(ASTnode callee)
        {
            List<ASTnode> arguments = new List<ASTnode>();
            if (!Check(TokenType.ClParenthesis))
            {
                do
                {
                    arguments.Add(Expr());
                } while (Eat(Current(), TokenType.Comma));
            }

            Token paren = Consume(TokenType.ClParenthesis, "Expect ')' after arguments.");

            return new CallFunction(callee, paren, arguments);
        }

        // private Stmt Function()
        // {
        //     Token name = Consume(TokenType.Identifier, "Expect function name.");
        //     Consume(TokenType.OpParenthesis, "Expect '(' after function name.");
        //     List<Token> parameters = new List<Token>();
        //     if (!Check(TokenType.ClParenthesis))
        //     {
        //         do
        //         {
        //             parameters.Add(Consume(TokenType.Identifier, "Expect parameter name."));
        //         }
        //         while (Eat(Current(), TokenType.Comma));
        //     }
        //     Consume(TokenType.ClParenthesis, "Expect ')' after parameters.");
        //     Consume(TokenType.Imply, "Expect '=>' after function declaration.");
        //     List<Stmt> body_function = new List<Stmt>();

        // }

        // private Stmt LetStmt()
        // {
        //     ASTnode LetExpr = VarDeclaration();

        // }


        private ASTnode Equality()
        {
            var node = Comparison();
            while (Current().Type == TokenType.Equality || Current().Type == TokenType.NotEqual)
            {
                Token op = Previous();
                if (Current().Type == TokenType.Equality)
                {
                    Eat(Current(), TokenType.Equality);
                }
                else if (Current().Type == TokenType.NotEqual)
                {
                    Eat(Current(), TokenType.NotEqual);
                }

                node = new BinOp(node, op, Comparison());

            }
            return node;
        }

        private ASTnode Comparison()
        {
            ASTnode node = LevelFour();
            while (Current().Type == TokenType.LessThan || Current().Type == TokenType.LessOrEqual || Current().Type == TokenType.GreaterThan || Current().Type == TokenType.GreaterOrEqual)
            {
                Token op = Previous();
                if (Current().Type == TokenType.LessThan)
                {
                    Eat(Current(), TokenType.LessThan);
                }
                else if (Current().Type == TokenType.LessOrEqual)
                {
                    Eat(Current(), TokenType.LessOrEqual);
                }
                else if (Current().Type == TokenType.GreaterThan)
                {
                    Eat(Current(), TokenType.GreaterThan);
                }
                else if (Current().Type == TokenType.GreaterOrEqual)
                {
                    Eat(Current(), TokenType.GreaterOrEqual);
                }
                node = new BinOp(node, op, LevelFour());
            }
            return node;
        }


        private Stmt Statement()
        {

            if (Current().Type == TokenType.If)
            {
                Eat(Current(), TokenType.If); return IfStatement();
            }
            else if (Current().Type == TokenType.Print)
            {
                Eat(Current(), TokenType.Print); return PrintStatement();
            }
            // else if (Current().Type == TokenType.Let)
            // {
            //     Eat(Current(), TokenType.Let); return LetStmt();
            // }
            // else if (Current().Type == TokenType.Function)
            // {
            //     Eat(Current(), TokenType.Function); return Function();
            // }

            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            Consume(TokenType.OpParenthesis, "Expect '(' after expression");
            ASTnode value = Expr();
            Consume(TokenType.ClParenthesis, "Expect ')' after value");
            Consume(TokenType.Semicolon, "Expect ';' after expression.");
            return new Print(value);
        }

        private ASTnode Expr()
        {
            return Assignment();
        }

        private Stmt ExpressionStatement()
        {
            ASTnode expr = Expr();
            Consume(TokenType.Semicolon, "Expect ';' after expression.");
            return new ExpressionStmt(expr);
        }

        // private Stmt Declaration()
        // {
        //     if (Current().Type == TokenType.Identifier)
        //     {
        //         Eat(Current(), TokenType.Identifier);
        //         return VarDeclaration();
        //     }
        //     return Statement();
        // }

        // private Stmt VarDeclaration()
        // {
        //     Token name = Consume(TokenType.Identifier, "Expect variable name.");
        //     ASTnode initializer = null;

        //     if (Eat(Current(), TokenType.Assignment))
        //     {
        //         initializer = Expr();
        //     }
        //     else
        //     {
        //         throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "at '" + Previous().Lexeme + "'", "Expect '=' after variable name");
        //     }
        //     List<ASTnode> declarations = new List<ASTnode>
        //     {
        //         initializer
        //     };
        //     if (Eat(Current(), TokenType.Comma))
        //     {
        //         Stmt other_initializer = VarDeclaration();
        //     }
        //     else
        //     {
        //         return 
        //     }
        //     Consume(TokenType.In, "Expect 'in' expression after variable declaration");

        // }

        // private Stmt FinishDeclaration()
        // {

        // }

        private ASTnode Assignment()
        {
            ASTnode expr = Or();

            if (Current().Type == TokenType.Assignment)
            {
                Token equals = Previous();
                ASTnode value = Assignment();

                if (expr is Variable)
                {
                    Token name = ((Variable)expr).Name;
                    return new Assignment(name.Lexeme, value);
                }

                throw Error.Error_(equals.Line, Error.ErrorType.SINTACTIC, "", "Invalid assignment target.");
            }

            return expr;

        }

        private ASTnode Or()
        {
            ASTnode expr = And();

            while (Current().Type == TokenType.Disjunction)
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

            while (Current().Type == TokenType.Conjunction)
            {
                Eat(Current(), TokenType.Conjunction);
                Token op = Previous();
                ASTnode right = Equality();
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        private Stmt IfStatement()
        {
            Consume(TokenType.OpParenthesis, "Expect '(' after 'if'.");
            ASTnode condition = Expr();
            Consume(TokenType.ClParenthesis, "Expect ')' after if condition.");

            Stmt then_body = Statement();
            Stmt else_body = null;

            if (Current().Type == TokenType.Else)
            {
                Eat(Current(), TokenType.Else);
                else_body = Statement();
            }
            else
            {
                throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "", "Expect 'else declaration' after if condition");
            }

            return new IfStmt(condition, then_body, else_body);
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

        private bool Eat(Token current, TokenType type)
        {
            if (current.Type == type)
            {
                Advance();
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
