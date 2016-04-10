using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_life_Opentk
{
    /// <summary>
    /// Object holding all the cells in the simulation. 
    /// </summary>
    class Grid
    {
        //Array storing all the cells in simulation
        Cell[,] grid;

        //Random generator object
        Random rand;

        int ID;

        //Races
        int nmbrOfRaces;

        //Size of the grid
        public int X
        {
            get { return grid.GetLength(0); }
        }
        public int Y
        {
            get { return grid.GetLength(1); }
        }

        /// <summary>
        /// Initiate the grid with a certain size
        /// </summary>
        /// <param name="size">Size of the grid to use</param>
        public Grid(Vector2 size, int id)
        {
            grid = new Cell[(int)size.X, (int)size.Y];
            rand = new Random();
            for(int i = 0; i < grid.GetLength(0); i++)
                for(int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = new Cell(new Vector2(i, j));
                }
            ID = id;
            nmbrOfRaces = 2;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="percent">Percent of the cells alive between 0 - 1</param>
        public Grid(Vector2 size, double percent, int id) : this(size, id)
        {
            int total = (int)(size.X * size.Y);
            int cellsToFill = (int)Math.Ceiling((total * percent));
            while(cellsToFill > 0)
            {
                int x = rand.Next(0, grid.GetLength(0));
                int y = rand.Next(0, grid.GetLength(1));
                if (!grid[x, y].IsAlive())
                {
                    grid[x, y].Revive();
                    cellsToFill--;
                }
            }
        }

        public Grid(Vector2 size, double percent, int id, int raceRandomMax) : this(size, percent, id)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j].race = rand.Next(0,raceRandomMax);
                }
            nmbrOfRaces = raceRandomMax;
        }

        public void Update()
        {
            Grid gridClone = new Grid(Vector2.Zero, 1);
            gridClone.Clone(this);

            foreach(Cell c in gridClone.grid)
            {
                ApplyRules(c);
            }

            this.Clone(gridClone);
        }

        private void ApplyRules(Cell c)
        {
            List<Cell> neighborCells = new List<Cell>();
            int nmbrNeigh = GetNeighbors(c, out neighborCells);
            if (nmbrNeigh < 2)
                c.Kill();
            else if (c.IsAlive() && (nmbrNeigh == 2 || nmbrNeigh == 3))
                c.Revive();
            else if (!c.IsAlive() && (nmbrNeigh == 3))
                c.Revive(GetRaceAve(neighborCells));
            else if (nmbrNeigh > 3)
                c.Kill();
        }

        public void ReviveAtPos(Vector2 pos)
        {
            grid[(int)(pos.X/Camera.scale.X), (int)(pos.Y/Camera.scale.Y)].Revive();
        }

        private int GetNeighbors(Cell c, out List<Cell> cells)
        {
            int sum = 0;
            cells = new List<Cell>();
            for(int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {

                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    int x = (int)(c.Position.X + i) % grid.GetLength(0);
                    if (x < 0)
                        x = grid.GetLength(0) - 1;
                    int y = (int)(c.Position.Y + j) % grid.GetLength(1);
                    if (y < 0)
                        y = grid.GetLength(1) - 1;
                    if (grid[x, y].IsAlive())
                    {
                        sum++;
                        cells.Add(grid[x, y]);
                    }
                }
            }
            return sum;
        }

        /// <summary>
        /// Get the race that is prominent in the surrounding cells
        /// </summary>
        /// <param name="c">Current cell</param>
        /// <returns></returns>
        private int GetRaceMax(List<Cell> cells)
        {
            int[] sum = new int[nmbrOfRaces];
            foreach(Cell c in  cells)
            {
                sum[c.race]++;
            }
            int raceMax = 0;
            for(int i = 0; i < nmbrOfRaces; i++)
            { 
                if(sum[raceMax] < sum[i])
                {
                    raceMax = i;
                }
            }
            return raceMax;
        }
        /// <summary>
        /// Get the average color of the surrounding cells
        /// </summary>
        /// <param name="c">Current cell</param>
        /// <returns></returns>
        private int GetRaceAve(List<Cell> cells)
        {
            int color = 0;
            foreach(Cell c in cells)
            {
                color += c.race;
            }
            color /= cells.Count;
            return color;
        }

        public void Clone(Grid toCopy)
        {
            Grid middleMan = new Grid(new Vector2(toCopy.grid.GetLength(0), toCopy.grid.GetLength(1)), toCopy.ID);
            for(int i = 0; i < middleMan.X; i++)
            {
                for(int j  = 0; j < middleMan.Y; j++)
                {
                    middleMan.grid[i, j].Clone(toCopy.grid[i, j]);
                }
            }
            this.grid = middleMan.grid;
        }

        public void Draw(GraphicsBuffer buf, out GraphicsBuffer outBuf)
        {
            List<Vertex> vert = new List<Vertex>();
            List<uint> index = new List<uint>();
            
            foreach (Cell c in grid)
            {
                if (c.IsAlive())
                {
                    Vector2 vec = new Vector2(c.Position.X - Camera.cameraPos.X, c.Position.Y - Camera.cameraPos.Y);
                    vec.X *= Camera.scale.X;
                    vec.Y *= Camera.scale.Y;

                    vert.Add(new Vertex(vec, new Vector2(0, 0)) { Color = System.Drawing.Color.FromArgb(0, c.race, 0)});
                    vec.X += Camera.scale.X;
                    vert.Add(new Vertex(vec, new Vector2(1, 0)) { Color = System.Drawing.Color.FromArgb(0, c.race, 0) });
                    vec.Y += Camera.scale.Y;
                    vert.Add(new Vertex(vec, new Vector2(1, 1)) { Color = System.Drawing.Color.FromArgb(0, c.race, 0) });
                    vec.X -= Camera.scale.X;
                    vert.Add(new Vertex(vec, new Vector2(0, 1)) { Color = System.Drawing.Color.FromArgb(0, c.race, 0) });

                    for (int i = 4; i > 0; i--)
                    {
                        index.Add((uint)(vert.Count - i));
                    }
                }
            }
            outBuf = new GraphicsBuffer();
            outBuf = buf;
            outBuf.vertBuffer = vert.ToArray<Vertex>();
            outBuf.indexBuffer = index.ToArray<uint>();
        }
    }
}
