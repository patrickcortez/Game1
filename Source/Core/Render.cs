namespace Game1.Source.Core
{
   internal enum MapSize
    {
        X = 25,
        Y = 25
    };

    // Y 23 = Valid player spawn point
    // Y 24 = Ground

    // Y 22< = Sky

   internal struct Player
    {
        private char Representation;
        private (int x, int y) Origin;
        public (int x, int y) Position;

        public Player(char Rep, (int x, int y) Org)
        {

            if(Org.y > (int)MapSize.Y || Org.x > (int)MapSize.X)
            {
                throw new Exception("Cannot spawn player outside of the map!");
            }

            if (Org.y < 23)
            {
                throw new Exception("Cannot spawn player on the sky");
            }

            Representation = Rep;
            Origin = Org;
            Position = Org;
        }

        public char GetRepresentation()
        {
            return Representation;
        }

        public (int x,int y) GetOrigin()
        {
            return Origin;
        }

        public void SetPosition(int x, int y)
        {
            Position = (x, y);
        }

    };

    internal static class Render
    {

        private static List<List<char>> Map; // 2D Letter Map
     //   private static List<List<char>> BackMap;
        private static System.Timers.Timer time;
        private static Player player;

        private static void InitiateMap()
        {
            // Initialize variables
            Map = new();
            time = new() { Interval=40,Enabled=true };
            player = new Player('O',(12,23));

            // Fill the map w/ whitespaces
            for(int y = 0; y < (int)MapSize.Y; y++)
            {
                List<char> Row = new();
                for (int x = 0; x < (int)MapSize.X; x++)
                {
                    
                    if(x == player.GetOrigin().x && y == player.GetOrigin().y) // spawn player at their origin
                    {
                        Row.Add(player.GetRepresentation());
                        
                        continue;

                    }

                    Row.Add(' ');
                }

                Map.Add(Row);
            }
        }

        private static void Paint((int X,int Y) Position,char letter)
        {
            Map[Position.Y][Position.X] = letter;
        }

        private static void PaintRow(int YPosition, char letter)
        {
            for(int x = 0; x < (int)MapSize.X; x++)
            {
                Map[YPosition][x] = letter;
            }
        }

        private static void Show()
        {
            for (int y = 0; y < (int)MapSize.Y; y++)
            {
                for (int x = 0; x < (int)MapSize.X; x++)
                {
                    Console.Write(Map[y][x]);
                }
                Console.WriteLine();
            }
        }

        private static void Move(ConsoleKey Arrow)
        {
            if (Arrow.Equals(ConsoleKey.LeftArrow) && player.Position.x > 0)
            {
                player.SetPosition(player.Position.x - 1, player.Position.y);
                Paint(player.Position,player.GetRepresentation());
                Paint((player.Position.x + 1,player.Position.y),' ');
            }

            if (Arrow.Equals(ConsoleKey.RightArrow) && player.Position.x < 24)
            {
                player.SetPosition(player.Position.x + 1, player.Position.y);
                Paint(player.Position, player.GetRepresentation());
                Paint((player.Position.x - 1, player.Position.y), ' ');
            }
        }

        public static int Run()
        {

            try
            {
                InitiateMap();
                PaintRow((int)MapSize.Y - 1, '=');
                Console.CursorVisible = false;

                time.Elapsed += (s, e) =>
                {

                    Console.SetCursorPosition(0, 0);

                    Show();


                };

                time.Start();

                while (true)
                {

                    ConsoleKeyInfo key =  Console.ReadKey();

                    Move(key.Key);

                    if (key.Key.Equals(ConsoleKey.Escape))
                    {
                        time.Stop();
                        break;
                    }

                }


                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
