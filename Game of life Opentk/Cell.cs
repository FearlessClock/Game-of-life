using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Game_of_life_Opentk
{
    //A cel containing all the information needed to create the simulation
    class Cell
    {
        //Current position of the cell
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }
        //If the cell is currently alive
        bool isAlive;

        //The race of the cell
        public int race;

        /// <summary>
        /// Basic componant in a game of life
        /// </summary>
        /// <param name="pos"></param>
        public Cell(Vector2 pos)
        {
            position = pos;
            isAlive = false;
            race = 0;
        }
        public Cell(Vector2 pos, bool life) : this(pos)
        {
            isAlive = life;
        }

        public Cell(Vector2 pos, bool life, int race) : this(pos, life)
        {
            this.race = race;
        }

        public void Revive()
        {
            isAlive = true;
        }
        public void Revive(int raceAve)
        {
            isAlive = true;
            race = raceAve;
        }
        public bool IsAlive()
        {
            return isAlive ? true : false;
        }

        public void Kill()
        {
            isAlive = false;
        }

        public override string ToString()
        {
            return position.X + ":" + position.Y + " " + isAlive;
        }

        public void Clone(Cell c)
        {
            this.position = c.position;
            this.isAlive = c.isAlive;
            this.race = c.race;
        }
    }
}
