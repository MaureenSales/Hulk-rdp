namespace Hulk
{
    public class String_: ASTnode
    {
        public string Value {get; private set; }

        public String_ (string value)
        {
            Value = value;
        }
    }
}