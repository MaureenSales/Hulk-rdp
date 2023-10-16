namespace Hulk
{
    public enum TokenType
    {
        // Operators
        Sum, Subtraction, Product, Modulo, Division, Pow, Sqrt, Assignment,

        // Boolean operators
        Equality, NotEqual, LessThan, GreaterThan, LessOrEqual, GreaterOrEqual, Negation, Imply,
        Conjunction, Disjunction,

        // Literals
        Identifier, String, Number, 
        
        //KeyWords
        Function, Let, In, Print, If, Else, PI, Euler, True, False,

        // Symbols    
        OpParenthesis, ClParenthesis, Comma, Semicolon, Colon,

        // EOF
        Eof
    }
}