using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Game1.Source.Core
{

    /*
     * TODO's:
     *  - Add Walls
     *  - Add Objectives
     *  - Add Player Health
     */


    /// <summary>
    /// Map size constants
    /// </summary>
   internal enum MapSize
    {
        X = 25,
        Y = 25
    };

    // Y 23 = Valid player spawn point
    // Y 24 = Ground

    // Y 22< = Sky
    
    /// <summary>
    /// Player definition/struct
    /// </summary>
   internal struct Player
    {
        private char Representation;
        private (int x, int y) Origin;
        public (int x, int y) Position;
        public bool HasJumped { get; set;}

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

        public bool CheckHit(Npc Enemy)
        {
            if (Position.y == Enemy.Position.y - 1 && (Position.x == Enemy.Position.x - 1 || Position.x == Enemy.Position.x + 1))
            {
                return true;
            }

            return false;
        }

        public bool CheckDeath(Npc Enemy)
        {
            return Enemy.Position == Position;
        }

    };

    /// <summary>
    /// Game Renderer 
    /// </summary>
    internal static class Render
    {

        private static List<List<char>> Map; // 2D Letter Map
     //   private static List<List<char>> BackMap;
        private static System.Timers.Timer time;
        private static Player player;
        private static ConsoleKey CurrentKey;
        private static List<Npc> Enemies;
        private static string reason;

        private static void ReadInput()
        {
            CurrentKey = Console.ReadKey().Key;
        }

        private static List<Npc> InitEnemies(int max = 1)
        {
            List<Npc> temp = new();
            Random xCoords = new();

            for (int x = 0;x < max; x++)
            {
                
                temp.Add(new('X', (xCoords.Next(0, (int)MapSize.X), (int)MapSize.Y - 2), true));
            }

            return temp;
        }

        private static void InitiateMap()
        {
            int EnemyCount = 0;

            // Initialize variables
            Map = new();
            time = new() { Interval=75,Enabled=true };
            player = new Player('O',(12,23));
            Enemies = InitEnemies(2);
            
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

                    if(EnemyCount < Enemies.Count && y == (int)MapSize.Y - 2)
                    {
                        Npc CurrentEnemy = Enemies[EnemyCount];
                        
                        if(CurrentEnemy.Position.x == x)
                        {
                            Row.Add(CurrentEnemy.Representation);
                            EnemyCount++;
                            continue;
                        }

                        
                    }

                    Row.Add(' ');
                }

                Map.Add(Row);
            }
        }

        private static void HandleEnemyMove() // Enemy Movement Handler
        {
            byte Delay = 75; // Delay when animating
            


            foreach(Npc Enemy in Enemies)
            {

                if (Enemy.CurrentState == State.DEAD)
                {
                    Enemies.Remove(Enemy);
                    continue;

                } 

                if (!Npc.IsPlayerNear(Enemy, player))
                {
                    Enemy.CurrentState = State.IDLE;

                    Enemy.Move(Enemy.Position.x - 1, Enemy.Position.y); // Frame 1
                    Paint(Enemy.Position, Enemy.Representation);
                    Paint((Enemy.Position.x + 1, Enemy.Position.y), ' ');

                    Thread.Sleep(Delay);

                    Enemy.Move(Enemy.Position.x + 1, Enemy.Position.y);
                    Paint(Enemy.Position, Enemy.Representation);
                    Paint((Enemy.Position.x - 1, Enemy.Position.y), ' ');

                    Thread.Sleep(Delay);

                    Enemy.Move(Enemy.Position.x + 1, Enemy.Position.y);
                    Paint(Enemy.Position, Enemy.Representation);
                    Paint((Enemy.Position.x - 1, Enemy.Position.y), ' ');

                    Thread.Sleep(Delay);

                    Enemy.Move(Enemy.Position.x - 1, Enemy.Position.y);
                    Paint(Enemy.Position, Enemy.Representation);
                    Paint((Enemy.Position.x + 1, Enemy.Position.y), ' ');

                    Thread.Sleep(Delay);
                }
                else
                {
                    int Distance = Enemy.Position.x - player.Position.x;
                    Enemy.CurrentState = State.HUNT;

                    for (int i = 0;i < Enemy.ViewRange;i++)
                    {

                        if(Enemy.CurrentState == State.DEAD)
                        {
                            Enemies.Remove(Enemy);
                            continue;

                        }

                        if (Distance >= 0)
                        {
                            if (player.Position.y == Enemy.Position.y - 1)
                            {
                                Enemy.CurrentState = State.DEAD;
                                continue;
                            }

                            Enemy.Move(Enemy.Position.x - 1, Enemy.Position.y);
                            Paint(Enemy.Position, Enemy.Representation);
                            Paint((Enemy.Position.x + 1, Enemy.Position.y), ' ');
                        }
                        else
                        {
                            Enemy.Move(Enemy.Position.x + 1, Enemy.Position.y);
                            Paint(Enemy.Position, Enemy.Representation);
                            Paint((Enemy.Position.x - 1, Enemy.Position.y), ' ');
                        }
                       
                    }
                }

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

            if (Arrow.Equals(ConsoleKey.RightArrow) && player.Position.x < (int)MapSize.X - 1)
            {
                player.SetPosition(player.Position.x + 1, player.Position.y);
                Paint(player.Position, player.GetRepresentation());
                Paint((player.Position.x - 1, player.Position.y), ' ');
            }

            if (Arrow.Equals(ConsoleKey.Spacebar) && player.Position.y == (int)MapSize.Y - 2)
            {
                int jumpheight = 2;
                for (int x = 0; x < jumpheight; x++){

                    player.SetPosition(player.Position.x, player.Position.y - 1);
                    Paint(player.Position, player.GetRepresentation());
                    Paint((player.Position.x, player.Position.y + 1), ' ');
                }
            }
        }

        private static void Gravity()
        {
            int ground = (int)MapSize.Y - 2;

            if (player.Position.y != ground)
            {

                

                for(int x=0;x < (ground - player.Position.y);x++)
                {

                    player.SetPosition(player.Position.x, player.Position.y + 1);
                    Paint(player.Position, player.GetRepresentation());
                    Paint((player.Position.x, player.Position.y - 1), ' ');

                }

            }
        }

        public static bool Death()
        {
            foreach(Npc Enemy in Enemies)
            {
                if (player.CheckDeath(Enemy))
                {
                    return true;
                }
            }

            return false;
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

                    HandleEnemyMove();
                    Gravity();

                    Show();
                    Console.SetCursorPosition(0, 0);

                };

                time.Start();

                while (true)
                {

                    if (Console.KeyAvailable)
                    {
                        ReadInput();
                        Move(CurrentKey);
                    }

                    if (Death())
                    {
                        reason = "Death";
                        time.Stop();
                        break;
                    }

                    if(Enemies.Count == 0)
                    {
                        reason = "Win";
                        time.Stop();
                        break;
                    }

                    if (CurrentKey.Equals(ConsoleKey.Escape))
                    {
                        reason = "Escape";
                        time.Stop();
                        break;
                    }

                }

                if (reason.Equals("Escape"))
                {
                    Console.WriteLine("Good Bye...");
                }else if (reason.Equals("Death"))
                {
                    Console.WriteLine("You Died to an Enemy");
                }else if (reason.Equals("Win"))
                {
                    Console.WriteLine("Congrats you Killed All of the Enemies, You won");
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
