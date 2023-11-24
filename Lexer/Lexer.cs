namespace Hulk
{
    public class Lexer
    {
        private readonly string Line; // linea de codigo recibida del usuario
        private int position = 0; // iterador de la posicion en el string
        public List<Token> Tokens = new List<Token>(); // lista donde se van a guardar los tokens
        private int start = 0; // iterador que indica el inicio de un nuevo token

        public Lexer(string line_)
        {
            Line = line_;
            ScanTokens();
        }

        public List<Token> ScanTokens()
        {
            //si no estoy al final de la linea voy a seguir buscando tokens
            while (!IsAtEnd())
            {
                start = position;
                ScanToken();
            }
            //si estoy al final agregar un token que lo indique en este caso Eof
            AddToken(TokenType.Eof, "EOF");
            return Tokens;
        }

        public void ScanToken()
        {
            //guardo en c el primer caracter y avanzo
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
            //va guardando el valor del string mientras busca el cierre de comillas
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
            //voy guardando el valor de mi number tal que si aparece una letra o caracter inseperado de error 
            //si es un punto verifica que luego venga un numero y que no hayan mas puntos
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
            //un identificador puede contener numeros mas no al principio, de este modo se escanea 
            //si coincide con un identificador  especial se guarda como tal
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
            //si luego del '=' matchea un '>' es un implica y asi de agrega 
            // si es otro '=' seria de igualdad
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
            //si mi actual es '=' y el anterion '<' tendria un token de tipo menor  igual
            //analogamente para mayor igual, mayor que y menor que
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
            //si luego de un '!' tengo '=' entonces es un no igual sino es un caracter de negacion
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

        //recibe el typo y valor del token, genera un token y lo agrega a la lista
        private void AddToken(TokenType type, string value)
        {
            Tokens.Add(new Token(type, value));
        }

        //retorna el caracter actual y avanza
        private char Advance()
        {
            return Line[position++];
        }

        //retorna el caracter actual
        private char Current()
        {
            return Line[position];
        }

        //verifica si tengo una letra o guion bajo 
        private bool IsAlphaC(char c)
        {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || (c == '_');
        }

        //verifica si tengo un digito
        private bool IsDigit(char c)
        {
            return '0' <= c && c <= '9';
        }

        //si no estoy al final devuelve el actual
        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            return Current();
        }

        //si no estoy al final avanza al caracter siguiente y lo devuelve
        private char PeekNext()
        {
            if (position + 1 >= Line.Length)
            {
                return '\0';
            }
            return Line[position + 1];
        }

        //verifica que este al final o no de la linea
        private bool IsAtEnd()
        {
            return position >= Line.Length;
        }
    }
}