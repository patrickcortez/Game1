using Game1.Source.Core;

namespace Game1.Source
{
    public static class Program
    {
        private static Dictionary<string, Delegate> Commands = new();

        public static int Main(params string[] args) 
        {
            try
            {
                if (args.Count() < 1)
                {
                    return Render.Run();
                }


                return 0;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"{ex.Message} at {ex.StackTrace}");
                return 1;
            }

        }
    }
}
