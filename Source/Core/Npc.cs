using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Source.Core
{
    
    internal class Npc
    {
        public char Representation { get; set; }

        private const int MapY = (int)MapSize.Y;

        private const int MapX = (int)MapSize.X;

        public bool isDead { get; set; }
        public bool isEnemy { get; set; }

        public (int x, int y) Position;

        public Npc(char Rep,(int x,int y) StartPos,bool isOps = false)
        {
            if((StartPos.x > MapX || StartPos.y > MapY) || StartPos.x < 0)
            {
                throw new Exception($"Cannot spawn NPC at {StartPos}");
            }

            Representation = Rep;

            Position = StartPos;

            isDead = false;

            isEnemy = isOps;
        }

        public void Move(int x,int y) // Moves enemy back and forth by 1 tile
        {

            if(y > (int)MapSize.Y || x > (int)MapSize.X || x < 0 || y < 0)
            {
                throw new Exception($"Cannot Move Npc to {Position}");
            }

            Position = (x, y);
        }

        
    }
}
