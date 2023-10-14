namespace Hulk
{
    public class MathFunctions
    {
        public string MathExpr { get; private set; }
        public List<string> MathExprs { get; private set; }
        public List<string> Arguments { get; private set; }


        public MathFunctions( string math_expr, List<string> math_exprs)
        {
            MathExpr = math_expr;
            List<string> MathExprs = new List<string>(){ "sen", "cos", "tan", "PI", "E", "log", "sqrt"};
        }
        public void Evaluate ()
        {

        }
    }
}