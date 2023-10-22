
using System.Linq.Expressions;
using System.Text.RegularExpressions;

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

        public ASTnode Parse()
        {
            return LevelFour();
        }

        private ASTnode Primary()
        {
            if (Current().Type == TokenType.Number)
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
            throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "at '" + Current().Lexeme + "'", "Expect expression");
        }

        private ASTnode Unary()
        {
            if (Current().Type == TokenType.Subtraction)
            {
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

            Token paren = Consume(TokenType.ClParenthesis,
                                  "Expect ')' after arguments.");

            return new CallFunction(callee, paren, arguments);
        }

        private ASTnode LevelTwo()
        {
            var node = Unary();
            while (Current().Type == TokenType.Pow)
            {
                Token op = Previous();
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
                Token op = Previous();
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
                Token op = Previous();
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

        private ASTnode Boolean()
        {
            if (Current().Type == TokenType.True)
            {
                Eat(Current(), TokenType.True);
                return new Boolean(true);
            }
            else if (Current().Type == TokenType.False)
            {
                Eat(Current(), TokenType.False);
                return new Boolean(false);
            }

            throw new Exception("boolean");

        }

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
                ASTnode right = Comparison();

                node = new BinOp(node, op, right);

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

        private ASTnode String_()
        {
            var node = Current();
            Eat(Current(), TokenType.String);
            return new String_(node.Lexeme);
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

            throw Error.Error_(Current().Line, Error.ErrorType.SINTACTIC, "at '" + Current().Lexeme + "'", message);
        }
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Current().Type == type;
        }

        List<Stmt> parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Statement());
            }

            return statements;
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

            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            Eat(Current(), TokenType.OpParenthesis);
            ASTnode value = Expr();
            Consume(TokenType.Semicolon, "Expect ';' after value.");
            return new Print(value);
        }

        private ASTnode Expr()
        {
            return Assignment();
        }

        private Stmt ExpressionStatement()
        {
            ASTnode expr = LevelFour();
            Consume(TokenType.Semicolon, "Expect ';' after expression.");
            return new ExpressionStmt(expr);
        }

        private Stmt Declaration()
        {
            if (Current().Type == TokenType.Identifier)
            {
                Eat(Current(), TokenType.Identifier);
                return VarDeclaration();
            }
            return Statement();
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.Identifier, "Expect variable name.");

            ASTnode initializer = null;
            if (Current().Type == TokenType.Assignment)
            {
                Eat(Current(), TokenType.Assignment);
                initializer = Expr();
            }

            Consume(TokenType.Semicolon, "Expect ';' after variable declaration.");
            return new VariableStmt(name, initializer);
        }

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

    }

}
