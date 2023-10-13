namespace Hulk
{
    public class Program
    {
        public static void Main()
        {
            string input = Console.ReadLine();
            var lexer = new Lexer(input); 
            var parser = new Parser(lexer);
            System.Console.WriteLine(parser.Parse());
        }
    }
}