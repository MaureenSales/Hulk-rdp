namespace Hulk
{
    public class Lexer
    {
        private readonly string Line;
        private int position = 0;
        public List<Token> Tokens { get; private set; }

        public Lexer(string line)
        {
            Line = line + " ";
            Tokens = new List<Token>();

            while (!IsAtEnd())
            {
                ScanToken();
            }

            AddToken(TokenType.Eof, "EOF", -1);
        }

        public void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '+':
                    AddToken(TokenType.Sum, "+", position - 1);
                    return;
                case '-':
                    AddToken(TokenType.Subtraction, "-", position - 1);
                    return;
                case '"':
                    ScanString();
                    return;
                case '*':
                    AddToken(TokenType.Product, "*", position - 1);
                    return;
                case '/':
                    AddToken(TokenType.Division, "/", position - 1);
                    return;
                case '%':
                    AddToken(TokenType.Modulo, "%", position - 1);
                    return;
                case '^':
                    AddToken(TokenType.Pow, "^", position - 1);
                    return;
                case '=':
                    ScanAssignment(c);
                    return;
                case '<':
                    ScanLess(c);
                    return;
                case '>':
                    ScanGreater(c);
                    return;
                case '(':
                    AddToken(TokenType.OpParenthesis, "(", position - 1);
                    return;
                case ')':
                    AddToken(TokenType.ClParenthesis, ")", position - 1);
                    return;
                case '!':
                    ScanNegation(c);
                    return;
                case ',':
                    AddToken(TokenType.Comma, ",", position - 1);
                    return;
                case ';':
                    AddToken(TokenType.Semicolon, ";", position - 1);
                    return;
                case ':':
                    AddToken(TokenType.Colon, ":", position - 1);
                    return;


                default:
                    if (IsAlphaC(c))
                    {
                        ScanIdentifier(c);
                    }
                    else if (IsDigit(c))
                    {
                        ScanNumber(c);
                    }
                    else
                    {
                        // unexpected character
                        throw new Exception("Unexpected character.");
                    }
                    return;
                case ' ':
                case '\n':
                case '\t':
                    return;

            }
        }

        private void ScanString()
        {
            string result = "";
            bool found = false;
            while (!IsAtEnd())
            {
                char c = Advance();
                if (c == '"')
                {
                    found = true;
                    break;
                }
                result += c;
            }

            if (found == false)
            {
                throw new Exception("Expected '\"' but not found.");
            }

            AddToken(TokenType.String, result, -1);
        }


        private void ScanNumber(char c)
        {
            string result = "";
            int count = 0;
            while (!IsAtEnd())
            {
                result += c;
                if (Current() == '.')
                {
                    count++;
                }
                if (IsAlphaC(Current()) || count == 2)
                {
                    throw new Exception("Undefine character");
                }
                else if (!IsDigit(Current()) && Current() != '.')
                {
                    break;
                }

                c = Advance();
            }
            AddToken(TokenType.Number, result, -1);
        }


        private void ScanIdentifier(char c = '?')
        {
            string result = "";

            while (!IsAtEnd())
            {
                result += c;
                if (!IsAlphaC(Current()) && !IsDigit(Current()))
                {
                    break;
                }
                c = Advance();
            }

            // if else PI 
            Dictionary<string, TokenType> Identifiers = new Dictionary<string, TokenType>
             {
                { "let", TokenType.Let},
                { "in", TokenType.In },
                { "function", TokenType.Function },
                { "if", TokenType.If },
                { "else", TokenType.Else },
                { "PI", TokenType.PI },
                { "E", TokenType.Euler },
                { "true", TokenType.True },
                { "false", TokenType.False }
            };

            if (Identifiers.Keys.Contains(result))
            {
                AddToken(Identifiers[result], result, -1);
            }
            else
            {
                AddToken(TokenType.Identifier, result, -1);
            }
        }

        private void ScanAssignment(char c)
        {
            string result = "";
            result += c;
            if (Current() == '>')
            {
                result += Current();
                AddToken(TokenType.Imply, result, -1);
                Advance();
            }
            else if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.Equality, result, -1);
                Advance();
            }
            else AddToken(TokenType.Assignment, result, position - 1);
        }

        private void ScanLess(char c)
        {
            string result = "";
            result += c;
            if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.LessOrEqual, result, -1);
                Advance();
            }

            else AddToken(TokenType.LessThan, result, position - 1);
        }

        private void ScanGreater(char c)
        {
            string result = "";
            result += c;
            if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.GreaterOrEqual, result, -1);
                Advance();
            }

            else AddToken(TokenType.GreaterThan, result, position - 1);
        }

        private void ScanNegation(char c)
        {
            string result = "";
            result += c;
            if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.NotEqual, result, -1);
                Advance();
            }

            else AddToken(TokenType.Negation, result, position - 1); 
        }

        private void AddToken(TokenType type, string value, int column)
        {
            Tokens.Add(new Token(type, value, column));
        }

        private char Advance()
        {
            return Line[position++];
        }

        private char Current()
        {
            return Line[position];
        }

        private bool IsAlphaC(char c)
        {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || (c == '_');
        }

        private bool IsDigit(char c)
        {
            return '0' <= c && c <= '9';
        }

        private bool IsAtEnd()
        {
            if (position == Line.Length) return true;
            return false;
        }
    }
}
