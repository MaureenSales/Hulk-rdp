namespace Hulk
{
    public enum TokenType
    {
        // Operators
        Sum, Subtraction, Product, Modulo, Division, Pow, Sqrt,

        Assignment, If, Else, PI, Euler, True, False,

        // Boolean operators
        Equality, NotEqual, LessThan, GreaterThan, LessOrEqual, GreaterOrEqual, Negation, Imply,
        Conjunction, Disjunction,

        // Types
        Identifier, String, Number, Function, Let, In,

        // Symbols    
        OpParenthesis, ClParenthesis, Comma, Semicolon, Colon,

        // EOF
        Eof
    }
}