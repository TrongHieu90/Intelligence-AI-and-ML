using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        protected Spot[][] grid; //jagged array
        protected int rows = 30;
        protected int cols = 30;
        protected int c_width;
        protected int c_height;

        protected List<Spot> openSet; //= new List<Spot>();
        protected List<Spot> closedSet= new List<Spot>();
        protected List<Spot> path;

        protected Spot startPoint;
        protected Spot endPoint;
        protected Spot currentPoint;

        Graphics g;
        Pen blackPen;
        SolidBrush blackBrush;
        SolidBrush greenBrush;
        SolidBrush redBrush;
        SolidBrush blueBrush;

        public Form1()
        {
            InitializeComponent();
            //SetStyle(ControlStyles.ResizeRedraw, true); //redraw during the resize
            g = Canvas.CreateGraphics();
            blackPen = new Pen(Brushes.Black, 1);
            blackBrush = new SolidBrush(Color.Black);
            greenBrush = new SolidBrush(Color.Green);
            redBrush = new SolidBrush(Color.Red);
            blueBrush = new SolidBrush(Color.Blue);

        }

        public class Spot : Form1
        {
            public int i;
            public int j;
            public int f = 0;
            public int h = 0;
            public int g = 0;
            public List<Spot> neighbors = new List<Spot>();
            public Spot previous = null;
            public bool wall = false;


            public Spot(int i, int j)
            {
                this.i = i;
                this.j = j;

                //Default system rand is weak. It shows repeated pattern on our wall / obstacle spot
                //Random random = new Random();             
                //if(random.NextDouble() < 0.1)
                //{
                //    wall = true;
                //}

                //Using better crypto rand. Very good random wall/spot generation
                RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();
                if (RandomInteger(Rand, 0, 10) < 3)
                {
                    wall = true;
                }

            }

            public void AddNeighbors(Spot[][] grid)
            {
                int i = this.i;
                int j = this.j;

                //right adjacent spot
                if (i < cols - 1)
                {
                    neighbors.Add(grid[i + 1][j]);
                }

                //left adjacent spot
                if (i > 0)
                {
                    neighbors.Add(grid[i - 1][j]);
                }

                //up adjacent spot
                if (j < rows - 1)
                {
                    neighbors.Add(grid[i][j + 1]);
                }

                //down adjacent spot
                if (j > 0)
                {
                    neighbors.Add(grid[i][j - 1]);
                }

                //Diagonal Movement
                if (i > 0 && j > 0)
                {
                    neighbors.Add(grid[i - 1][j - 1]);
                }
                if (i < cols - 1 && j > 0)
                {
                    neighbors.Add(grid[i + 1][j - 1]);
                }
                if (i > 0 && j < rows - 1)
                {
                    neighbors.Add(grid[i - 1][j + 1]);
                }
                if (i < cols - 1 && j < rows - 1)
                {
                    neighbors.Add(grid[i + 1][j + 1]);
                }
            }
        }

        private int Heuristic(Spot node, Spot goal)
        {
            //for implementing different Heuristic functions see here: http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#S7

            //Euclidean square distance function
            var dist = Math.Sqrt(((node.i - goal.i) * (node.i - goal.i) + (node.j - goal.j) * (node.j - goal.j)));
            return (int)dist;

            //Diagonal distance
            //var D = 1;
            //var D2 = 1;
            //var dx = Math.Abs(node.i - goal.i);
            //var dy = Math.Abs(node.j - goal.j);
            //var dist = D * Math.Max(dx, dy) + (D2 - D) * Math.Min(dx, dy);
            //return dist;

            ////Manhattan distance
            //var dx = Math.Abs(node.i - goal.i);
            //var dy = Math.Abs(node.j - goal.j);
            //return (int)(dx + dy);
        }

        private void RemoveElement(List<Spot> list, Spot element)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == element)
                {
                    list.RemoveAt(i);
                }
            }
        }

        private int RandomInteger(RNGCryptoServiceProvider Rand, int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                Rand.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
        }

        private void panel1_Paint(object sender, PaintEventArgs e) //main canvas/panel to draw
        {
            c_width = Canvas.Width / cols;
            c_height = Canvas.Height / rows;

            //Creating the grid
            grid = new Spot[cols][];

            for(int i = 0; i < cols; i++)
            {
                grid[i] = new Spot[rows];
            }

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    grid[i][j] = new Spot(i, j);
                }
            }

            //Adding neighbors to each spot in grid
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    grid[i][j].AddNeighbors(grid);
                }
            }

            //Setting some start point and end point arbitrarily
            startPoint = grid[0][0];
            endPoint = grid[cols -1][rows -1];
            startPoint.wall = false;
            endPoint.wall = false;

            //Adding the predefined start point to the openSet
            openSet = new List<Spot>();
            openSet.Add(startPoint);

            //Visualizing our grid 
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (!grid[i][j].wall)
                    {
                        Rectangle rect = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                        g.DrawRectangle(blackPen, rect);
                    }
                    else //coloring each wall spot with black color
                    {
                        Rectangle wallRect = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                        g.FillRectangle(blackBrush, wallRect);
                    }
                }
            }


        }

        private void button1_Click(object sender, EventArgs e) //draw button
        {
            while (true)
            {
                if (openSet.Count > 0)
                {
                    int winner = 0;
                    for (int i = 0; i < openSet.Count; i++)
                    {
                        if (openSet[i].f < openSet[winner].f)
                        {
                            winner = i;
                        }
                    }

                    currentPoint = openSet[winner];

                    if (currentPoint == endPoint)
                    {
                        Console.WriteLine("Pathfinding done");
                        break;
                    }

                    RemoveElement(openSet, currentPoint);
                    //openSet.RemoveAt(winner);

                    //closedSet = new List<Spot>();
                    closedSet.Add(currentPoint);

                    var neighbors = currentPoint.neighbors;


                    //Console.WriteLine($"current point pos is: {currentPoint.i},{currentPoint.j} and the neighbors.count is: {currentPoint.neighbors.Count}");
                    //checking each neighbor Spot
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        //Console.WriteLine("checking neighbors");
                        var neighbor = neighbors[i];
                        if (!closedSet.Contains(neighbor) && !neighbor.wall)
                        {
                            var tempG = currentPoint.g + 1;
                            var newPath = false;

                            if (openSet.Contains(neighbor))
                            {
                                if (tempG < neighbor.g)
                                {
                                    neighbor.g = tempG;
                                    newPath = true;
                                }
                            }
                            else
                            {
                                neighbor.g = tempG;
                                newPath = true;
                                openSet.Add(neighbor);
                            }
                            if (newPath)
                            {
                                neighbor.h = Heuristic(neighbor, endPoint);
                                neighbor.f = neighbor.g + neighbor.h;
                                neighbor.previous = currentPoint;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No pathfinding solution");
                    break;
                }

                path = new List<Spot>();
                var temp = currentPoint;
                path.Add(temp);
                while (temp.previous != null)
                {
                    path.Add(temp.previous);
                    temp = temp.previous;
                }

                for (int i = 0; i < openSet.Count; i++)
                {                   
                    Rectangle rectOpen = new Rectangle(openSet[i].i * c_width, openSet[i].j * c_height, c_width, c_height);
                    g.FillRectangle(greenBrush, rectOpen);
                }

                for (int i = 0; i < closedSet.Count; i++)
                {
                    Rectangle rectClose = new Rectangle(closedSet[i].i * c_width, closedSet[i].j * c_height, c_width, c_height);
                    g.FillRectangle(redBrush, rectClose);
                }

                for (var i = 0; i < path.Count; i++)
                {
                    //Graphics r = Canvas.CreateGraphics();
                    Rectangle pathRect = new Rectangle(path[i].i * c_width, path[i].j * c_height, c_width, c_height);
                    g.FillRectangle(blueBrush, pathRect);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {

        }


    
    }
}
