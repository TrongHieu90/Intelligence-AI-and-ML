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
            SetStyle(ControlStyles.ResizeRedraw, true); //redraw during the resize

        }

        public class Spot:Form1
        {
            public int i;
            public int j;
            public int f = 0;
            public int h = 0;
            public int g = 0;
            public List<Spot> neighbors = new List<Spot>();
            public Spot previous;

            public Spot(int i, int j)
            {
                this.i = i;
                this.j = j;

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
            //Pen blackPen = new Pen(Brushes.Black, 1);
            //Pen greenPen = new Pen(Brushes.Green, 1);
            //Pen redPen = new Pen(Brushes.Red, 1);
            //Pen whitePen = new Pen(Brushes.White, 1);
            //SolidBrush whiteBrush = new SolidBrush(Color.White);
            //SolidBrush greenBrush = new SolidBrush(Color.Green);
            //SolidBrush redBrush = new SolidBrush(Color.Red);
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

            openSet.Add(startPoint);


            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Pen blackPen = new Pen(Brushes.Black, 1);
                    Rectangle rect = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                    e.Graphics.DrawRectangle(blackPen, rect);
                }
            }

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
                    

                    Console.WriteLine($"current point pos is: {currentPoint.i},{currentPoint.j} and the neighbors.count is: {neighbors.Count}");
                    //checking each neighbor Spot
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        //Console.WriteLine("checking neighbors");
                        var neighbor = neighbors[i];
                        if (!closedSet.Contains(neighbor))
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
                    return;
                }




                //for (int i = 0; i < openSet.Count; i++)
                //{
                //    for (int j = 0; j < openSet.Count; j++)
                //    {
                //        Rectangle rectOpen = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                //        e.Graphics.FillRectangle(greenBrush, rectOpen);
                //    }
                //}

                //for (int i = 0; i < closedSet.Count; i++)
                //{
                //    for (int j = 0; j < closedSet.Count; j++)
                //    {
                //        Rectangle rectclose = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                //        e.Graphics.FillRectangle(redBrush, rectclose);
                //    }
                //}

            }
        }

        private void button1_Click(object sender, EventArgs e) //draw button
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {

        }
    }       
}
