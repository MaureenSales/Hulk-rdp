
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
            if (Tokenized.Tokens[Tokenized.Tokens.Count - 2].Value != ";")
            {
                throw new Exception("; excpected");
            }
        }
        private ASTnode Factor()
        {
            Token current = Current();

            if (Current().Type == TokenType.Number)
            {
                Eat(Current(), TokenType.Number);
                var node = new Num(current);
                return node;
            }
            else if (Current().Type == TokenType.OpParenthesis)
            {
                Eat(Current(), TokenType.OpParenthesis);
                var node = LevelFour();
                Eat(Current(), TokenType.ClParenthesis);
                return node;
            }
            throw new Exception("BAD");
        }

        private ASTnode LevelTwo()
        {
            var node = Factor();
            while (Current().Type == TokenType.Pow)
            {
                var op = Current();
                Eat(Current(), TokenType.Pow);
                var right = Factor();
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
            Semicolon();
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

        private ASTnode String_()
        {
            var node = Current();
            Eat(Current(), TokenType.String);
            return new String_(node.Value);
        }

        private Token Current()
        {
            return Tokenized.Tokens[position];
        }

        private Token Advance()
        {
            return Tokenized.Tokens[position++];
        }

        private bool IsAtEnd()
        {
            if (position == Tokenized.Tokens.Count - 1) return true;
            return false;
        }

        private void Eat(Token current, TokenType type)
        {
            if (current.Type == type && !IsAtEnd())
            {
                Advance();
            }
            else throw new Exception("error");
        }


    }
}