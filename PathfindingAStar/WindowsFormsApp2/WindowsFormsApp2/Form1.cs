using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        protected Spot[,] grid;
        protected int rows = 15;
        protected int cols = 15;
        protected int c_width;
        protected int c_height;

        protected List<Spot> openSet = new List<Spot>();
        protected List<Spot> closedSet = new List<Spot>();
        protected List<Spot> path = new List<Spot>();

        protected Spot startPoint;
        protected Spot endPoint;
        protected Spot currentPoint;

        public Form1()
        {
            InitializeComponent();
            //SetStyle(ControlStyles.ResizeRedraw, true); //redraw during the resize

        }

        public class Spot : Form1
        {
            public int i;
            public int j;
            public int f = 0;
            public int h = 0;
            public int g = 0;
            public List<Spot> neighbors = new List<Spot>();
            public Spot previous;
            public bool wall = false;


            public Spot(int i, int j)
            {
                this.i = i;
                this.j = j;

                Random random = new Random();
                if(random.NextDouble() < 0.1)
                {
                    wall = true;
                }

            }

            public void AddNeighbors(Spot[,] grid)
            {
                int i = this.i;
                int j = this.j;

                //right adjacent spot
                if (i < cols - 1)
                {
                    neighbors.Add(grid[i + 1, j]);
                }

                //left adjacent spot
                if (i > 0)
                {
                    neighbors.Add(grid[i - 1, j]);
                }

                //up adjacent spot
                if (j < rows - 1)
                {
                    neighbors.Add(grid[i, j + 1]);
                }

                //down adjacent spot
                if (j > 0)
                {
                    neighbors.Add(grid[i, j - 1]);
                }
            }

            public void Display(PaintEventArgs e)
            {

            }
        }

        private int Heuristic(Spot a, Spot b)
        {
            //Euclidean distance function
            var dist = Math.Sqrt(((a.i - b.i) * (a.i - b.i) + (a.j - b.j) * (a.j - b.j)));
            return (int)dist;
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

        private void panel1_Paint(object sender, PaintEventArgs e) //main canvas/panel to draw
        {
            //Graphics g;
            //Pen blackPen = new Pen(Brushes.Black, 1);
            //Pen greenPen = new Pen(Brushes.Green, 1);
            //Pen redPen = new Pen(Brushes.Red, 1);
            //Pen whitePen = new Pen(Brushes.White, 1);
            //SolidBrush whiteBrush = new SolidBrush(Color.White);

            //SolidBrush blueBrush = new SolidBrush(Color.Blue);

            //Rectangle rect; 

            c_width = Canvas.Width / cols;
            c_height = Canvas.Height / rows;

            grid = new Spot[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    grid[i, j] = new Spot(i, j);
                }
            }

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    grid[i, j].AddNeighbors(grid);
                }
            }

            startPoint = grid[0, 0];
            endPoint = grid[cols - 1, rows - 1];
            startPoint.wall = false;
            endPoint.wall = false;

            openSet.Add(startPoint);

 

        }

        private void button1_Click(object sender, EventArgs e) //draw button
        {
            Graphics g = Canvas.CreateGraphics();
            SolidBrush greenBrush = new SolidBrush(Color.Green);
            SolidBrush redBrush = new SolidBrush(Color.Red);
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            Pen blackPen = new Pen(Brushes.Black, 1);


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

                var temp = currentPoint;
                path.Add(temp);
                while (temp.previous != null)
                {
                    path.Add(temp.previous);
                    temp = temp.previous;
                }

                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {   
                        if(!grid[i,j].wall)
                        {
                            Rectangle rect = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                            //blackPen = new Pen(Brushes.Black, 1);
                            g.DrawRectangle(blackPen, rect);
                        }
                        else
                        {
                            Rectangle wallRect = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                            g.FillRectangle(blackBrush, wallRect);
                        }
                    }
                }

                for (int i = 0; i < openSet.Count; i++)
                {
                    //Console.WriteLine($"openSet.Count is: {openSet.Count}");
                    //Console.WriteLine($"openSet loc is {openSet[i].i} and {openSet[i].j}");
                    Rectangle rectOpen = new Rectangle(openSet[i].i * c_width, openSet[i].j * c_height, c_width, c_height);
                    g.FillRectangle(greenBrush, rectOpen);
                }

                for (int i = 0; i < closedSet.Count; i++)
                {
                    //Console.WriteLine($"openSet.Count is: {openSet.Count}");
                    //Console.WriteLine($"closedSet loc is {closedSet[i].i} and {closedSet[i].j}");
                    Rectangle rectClose = new Rectangle(closedSet[i].i * c_width, closedSet[i].j * c_height, c_width, c_height);
                    g.FillRectangle(redBrush, rectClose);
                }

                //this.Invalidate();

                for (var i = 0; i < path.Count; i++)
                {
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
