namespace Hulk
{
    public class Lexer
    {
        private readonly string Line;
        private int position = 0;
        public List<Token> Tokens = new List<Token>();
        private int start = 0;

        public Lexer(string line_)
        {
            Line = line_;
            ScanTokens();
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = position;
                ScanToken();
            }

            AddToken(TokenType.Eof, "EOF");
            return Tokens;
        }

        public void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '+':
                    AddToken(TokenType.Sum, "+");
                    break;
                case '-':
                    AddToken(TokenType.Subtraction, "-");
                    break;
                case '"':
                    ScanString();
                    break;
                case '*':
                    AddToken(TokenType.Product, "*");
                    break;
                case '/':
                    AddToken(TokenType.Division, "/");
                    break;
                case '%':
                    AddToken(TokenType.Modulo, "%");
                    break;
                case '^':
                    AddToken(TokenType.Pow, "^");
                    break;
                case '=':
                    ScanAssignment(c);
                    break;
                case '@':
                    AddToken(TokenType.Concat, "@");
                    break;
                case '<':
                    ScanComparative(c);
                    break;
                case '>':
                    ScanComparative(c);
                    break;
                case '(':
                    AddToken(TokenType.OpParenthesis, "(");
                    break;
                case ')':
                    AddToken(TokenType.ClParenthesis, ")");
                    break;
                case '!':
                    ScanNegation(c);
                    break;
                case '&':
                    AddToken(TokenType.Conjunction, "&");
                    break;
                case '|':
                    AddToken(TokenType.Disjunction, "|");
                    break;
                case ',':
                    AddToken(TokenType.Comma, ",");
                    break;
                case ';':
                    AddToken(TokenType.Semicolon, ";");
                    break;
                case ':':
                    AddToken(TokenType.Colon, ":");
                    break;
                case ' ':
                case '\t':
                case '\r':
                    break;

                default:
                    if (IsAlphaC(c))
                    {
                        ScanIdentifier(c);
                    }
                    else if (IsDigit(c))
                    {
                        ScanNumber();
                    }
                    else
                    {
                        // unexpected character
                        Error.Error_(Error.ErrorType.LEXICAL, "Unexpected character '" + c + "'.");
                    }
                    break;

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
                Error.Error_(Error.ErrorType.LEXICAL, "Expected '\"' after '" + result + "' but not found.");
            }

            AddToken(TokenType.String, result);
        }


        private void ScanNumber()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }
            if (IsAlphaC(Peek()))
            {
                Error.Error_(Error.ErrorType.LEXICAL, " Input not valid. ");
            }
            string result = Line.Substring(start, position - start);
            AddToken(TokenType.Number, result);
        }

        private void ScanIdentifier(char c = '?')
        {
            string result = "";
            result += c;
            while (!IsAtEnd())
            {

                if (!IsAlphaC(Current()) && !IsDigit(Current()))
                {
                    break;
                }
                c = Advance();
                result += c;
            }

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
                { "false", TokenType.False },
                { "print", TokenType.Print}
            };

            if (Identifiers.Keys.Contains(result))
            {
                AddToken(Identifiers[result], result);
            }
            else
            {
                AddToken(TokenType.Identifier, result);
            }
        }

        private void ScanAssignment(char c)
        {
            string result = "";
            result += c;
            if (Current() == '>')
            {
                result += Current();
                AddToken(TokenType.Imply, result);
                Advance();
            }
            else if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.Equality, result);
                Advance();
            }
            else AddToken(TokenType.Assignment, result);
        }

        private void ScanComparative(char c)
        {
            string result = "";
            result += c;
            if (Current() == '=' && c == '<')
            {
                result += Current();
                AddToken(TokenType.LessOrEqual, result);
                Advance();
            }
            else if (Current() == '=' && c == '>')
            {
                result += Current();
                AddToken(TokenType.GreaterOrEqual, result);
                Advance();
            }
            else if (Current() != '=' && c == '<') AddToken(TokenType.LessThan, result);
            else AddToken(TokenType.GreaterThan, result);
        }

        private void ScanNegation(char c)
        {
            string result = "";
            result += c;
            if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.NotEqual, result);
                Advance();
            }

            else AddToken(TokenType.Negation, result);
        }

        private void AddToken(TokenType type, string value)
        {
            Tokens.Add(new Token(type, value));
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

        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            return Current();
        }
        private char PeekNext()
        {
            if (position + 1 >= Line.Length)
            {
                return '\0';
            }
            return Line[position + 1];
        }

        private bool IsAtEnd()
        {
            return position >= Line.Length;
        }
    }
}