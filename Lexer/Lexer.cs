namespace Hulk
{
    public class Lexer
    {
        private readonly string Line;
        private int position = 0;
        public List<Token> Tokens = new List<Token>();
        private int start = 0;
        private int line = 1;

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

            AddToken(TokenType.Eof, "EOF", -1, line);
            return Tokens;
        }

        public void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '+':
                    AddToken(TokenType.Sum, "+", position - 1, line);
                    break;
                case '-':
                    AddToken(TokenType.Subtraction, "-", position - 1, line);
                    break;
                case '"':
                    ScanString();
                    break;
                case '*':
                    AddToken(TokenType.Product, "*", position - 1, line);
                    break;
                case '/':
                    AddToken(TokenType.Division, "/", position - 1, line);
                    break;
                case '%':
                    AddToken(TokenType.Modulo, "%", position - 1, line);
                    break;
                case '^':
                    AddToken(TokenType.Pow, "^", position - 1, line);
                    break;
                case '=':
                    ScanAssignment(c);
                    break;
                case '<':
                    ScanComparative(c);
                    break;
                case '>':
                    ScanComparative(c);
                    break;
                case '(':
                    AddToken(TokenType.OpParenthesis, "(", position - 1, line);
                    break;
                case ')':
                    AddToken(TokenType.ClParenthesis, ")", position - 1, line);
                    break;
                case '!':
                    ScanNegation(c);
                    break;
                case '&':
                    AddToken(TokenType.Conjunction, "&", -1, line);
                    break;
                case '|':
                    AddToken(TokenType.Disjunction, "|", -1, line);
                    break;
                case ',':
                    AddToken(TokenType.Comma, ",", position - 1, line);
                    break;
                case ';':
                    AddToken(TokenType.Semicolon, ";", position - 1, line);
                    break;
                case ':':
                    AddToken(TokenType.Colon, ":", position - 1, line);
                    break;
                case '\n':
                    line++;
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
                        ScanNumber(c);
                    }
                    else
                    {
                        // unexpected character
                        Error.Error_(line, Error.ErrorType.LEXICAL, "'" + c + "'", "Unexpected character.");
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
                Error.Error_(line, Error.ErrorType.LEXICAL, " at '" + result + "'", "Expected '\"' but not found.");
            }

            AddToken(TokenType.String, result, -1, line);
        }


        private void ScanNumber(char c)
        {
            string result = "";

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
                Error.Error_(line, Error.ErrorType.LEXICAL, result, " Input not valid");
            }
            result = Line.Substring(start, position - start);
            AddToken(TokenType.Number, result, -1, line);
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
                { "false", TokenType.False },
                { "sqrt", TokenType.Sqrt },
                { "print", TokenType.Print}
            };

            if (Identifiers.Keys.Contains(result))
            {
                AddToken(Identifiers[result], result, -1, line);
            }
            else
            {
                AddToken(TokenType.Identifier, result, -1, line);
            }
        }

        private void ScanAssignment(char c)
        {
            string result = "";
            result += c;
            if (Current() == '>')
            {
                result += Current();
                AddToken(TokenType.Imply, result, -1, line);
                Advance();
            }
            else if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.Equality, result, -1, line);
                Advance();
            }
            else AddToken(TokenType.Assignment, result, position - 1, line);
        }

        private void ScanComparative(char c)
        {
            string result = "";
            result += c;
            if (Current() == '=' && c == '<')
            {
                result += Current();
                AddToken(TokenType.LessOrEqual, result, -1, line);
                Advance();
            }
            else if (Current() == '=' && c == '>')
            {
                result += Current();
                AddToken(TokenType.GreaterOrEqual, result, -1, line);
                Advance();
            }
            else if (Current() != '=' && c == '<') AddToken(TokenType.LessThan, result, position - 1, line);
            else AddToken(TokenType.GreaterThan, result, position - 1, line);
        }

        private void ScanNegation(char c)
        {
            string result = "";
            result += c;
            if (Current() == '=')
            {
                result += Current();
                AddToken(TokenType.NotEqual, result, -1, line);
                Advance();
            }

            else AddToken(TokenType.Negation, result, position - 1, line);
        }

        private void AddToken(TokenType type, string value, int column, int line)
        {
            Tokens.Add(new Token(type, value, column, line));
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