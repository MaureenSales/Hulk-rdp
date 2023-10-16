
using System.Linq.Expressions;

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

        public void Semicolon()
        {
            if (Tokenized.Tokens[Tokenized.Tokens.Count - 2].Lexeme != ";")
            {
                throw new Exception("; excpected");
            }
        }
        private ASTnode Primary()
        {
            if (Current().Type == TokenType.Number)
            {
                Eat(Current(), TokenType.Number);
                return new Num(Previous());
            }
            else if(Current().Type == TokenType.String)
            {
                Eat(Current(), TokenType.String);
                return new String_(Previous().Lexeme);
            }
            else if (Current().Type == TokenType.OpParenthesis)
            {
                Eat(Current(), TokenType.OpParenthesis);
                var node = LevelFour();
                Eat(Current(), TokenType.ClParenthesis);
                return new Grouping(node);
            }
            throw new Exception("BAD");
        }

        private ASTnode Unary()
        {
            if(Current().Type == TokenType.Subtraction)
            {
                Token op = Previous();
                return new UnaryExpr(op, Unary());
            }
            return Primary();
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
            Semicolon();
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

        private void Eat(Token current, TokenType type)
        {
            if (current.Type == type)
            {
                Advance();
            }
            else throw new Exception("error");
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Current().Type == type;
        }

    }
}