using System.IO;
namespace Hulk
{
    public class Program
    {
        public static void Main()
        {
            System.Console.Write("> ");
           string input = Console.ReadLine();
           try
           {
                run(input);
           }
           catch (Exception ex)
           {
                System.Console.WriteLine(ex.Message);
           }
        }

        private static void run (string input)
        {
            Lexer lexer = new Lexer(input);
            List<Token> tokens = lexer.Tokens;
            foreach (var item in tokens)
            {
                System.Console.WriteLine($"{item.Type}  {item.Lexeme}");
            }
            Parser parser = new Parser(lexer);
            ASTnode expression = parser.parse();
            if(Error.HadError) return;
        }
    }
}