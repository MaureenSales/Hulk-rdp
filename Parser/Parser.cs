
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

        public double Parse()
        {
            return LevelFour();
        }

        public  void Semicolon()
        {
            if( Tokenized.Tokens[Tokenized.Tokens.Count -2].Value != ";" )
            {
                throw new Exception( "; excpected");
            }
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
            if (position == Tokenized.Tokens.Count-1) return true;
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

        private double Factor()
        {
            Token current = Current();
            if (Current().Type == TokenType.Sum)
            {
                Eat(Current(), TokenType.Sum);
            }
            else if (Current().Type == TokenType.Subtraction)
            {
                Eat(Current(), TokenType.Subtraction);
            }
            else if (Current().Type == TokenType.Number)
            {
                Eat(Current(), TokenType.Number);
                return double.Parse(current.Value);
            }
            else if (Current().Type == TokenType.OpParenthesis)
            {
                Eat(Current(), TokenType.OpParenthesis);
                double result = LevelFour();
                Eat(Current(), TokenType.ClParenthesis);
                return result;
            }
            throw new Exception("BAD");
        }

        private double LevelTwo()
        {
            double result = Factor();
            while (Current().Type == TokenType.Pow)
            {
                Eat(Current(), TokenType.Pow);
                result = Math.Pow(result, Factor());

            }
            return result;
        }
        private double LevelThree()
        {
            double result = LevelTwo();

            while (Current().Type == TokenType.Product || Current().Type == TokenType.Division || Current().Type == TokenType.Modulo)
            {
                if (Current().Type == TokenType.Product)
                {
                    Eat(Current(), TokenType.Product);
                    result *= LevelTwo();
                }
                else if (Current().Type == TokenType.Division)
                {
                    Eat(Current(), TokenType.Division);
                    result /= LevelTwo();
                }
                else if (Current().Type == TokenType.Modulo)
                {
                    Eat(Current(), TokenType.Modulo);
                    result %= LevelTwo();
                }
            }
            return result;
        }

        private double LevelFour()
        {
            Semicolon();
            double result = LevelThree();

            while (Current().Type == TokenType.Sum || Current().Type == TokenType.Subtraction)
            {
                if (Current().Type == TokenType.Sum)
                {
                    Eat(Current(), TokenType.Sum);
                    result += LevelThree();
                }
                else if (Current().Type == TokenType.Subtraction)
                {
                    Eat(Current(), TokenType.Subtraction);
                    result -= LevelThree();
                }
            }
            return result;
        }


    }
}