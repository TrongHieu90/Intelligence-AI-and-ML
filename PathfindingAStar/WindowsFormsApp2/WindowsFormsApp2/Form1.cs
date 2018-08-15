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
        private int rows;
        private int cols;
        //private Spot[][] grid; //jagged array
        private Spot[,] grid;
        private List<Spot> openSet = new List<Spot>();
        private List<Spot> closedSet = new List<Spot>();
        
        private int c_width; //width of each spot/cell
        private int c_height; //width of each spot/cell

        Spot Start;
        Spot End;
        Spot Current;

        public Form1()
        {
            InitializeComponent();          
        }

        private void panel1_Paint(object sender, PaintEventArgs e) //main canvas/panel to draw
        {

        }

        private void button1_Click(object sender, EventArgs e) //draw button
        {
            int? input = null;
            try
            {
                input = int.Parse(InputBox.Text);
            }
            catch (FormatException error)
            {
                Console.WriteLine($"FormatException: {error.Source}");
            }
            if (input != null)
            {
                rows = (int)input;
                cols = (int)input;
                c_width = Canvas.Width / cols;
                c_height = Canvas.Height / rows;

                Setup();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Pathfind();
        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
           
        }

        public class Spot:Form1
        {
            public int i;
            public int j;
            public int f;
            public int g;
            public int h;
            public List<Spot> neighbors = new List<Spot>();
            public Spot previous = null;

            public Spot(int i, int j)
            {
                this.i = i;
                this.j = j;
               
            }
            
            public void Display(SolidBrush brushColor)
            {
                //Console.WriteLine($"Displaying Spot: {i} and {j}");
                Spot spot = new Spot(i, j);
                Graphics grph = Canvas.CreateGraphics();           
                Rectangle rect = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                grph.FillRectangle(brushColor, rect);

            }


            public void AddNeighbors()
            {
                int i = this.i;
                int j = this.j;

                //right adjacent spot
                if (i < cols - 1)
                {
                    neighbors.Add(grid[i + 1,j]);
                }

                //left adjacent spot
                if (i > 0)
                {
                    neighbors.Add(grid[i - 1,j]);
                }

                //up adjacent spot
                if (j < rows - 1)
                {
                    neighbors.Add(grid[i,j + 1]);
                }

                //down adjacent spot
                if (j > 0)
                {
                    neighbors.Add(grid[i,j - 1]);
                }
            }
        }

        private void Setup()
        {
            Graphics g = Canvas.CreateGraphics();
            Pen myPen = new Pen(Brushes.Black, 1);

            //2d array
            grid = new Spot[rows, cols];

            //initizaling each cell/spot in our grid
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    grid[i,j] = new Spot(i,j);

                    //drawing our grid
                    Rectangle rect = new Rectangle(i * c_width, j * c_height, c_width, c_height);
                    g.DrawRectangle(myPen, rect);

                }
            }

            //Adding neighbors
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    grid[i,j].AddNeighbors();
                }
            }

            //Set Start point and End Point and fill them with purple color
            Start = grid[0, 0];
            SolidBrush purpleBrush = new SolidBrush(Color.Purple);
            Rectangle startRect = new Rectangle(0 * c_width, 0 * c_height, c_width, c_height);
            g.FillRectangle(purpleBrush, startRect);

            End = grid[rows - 1, cols - 1];
            Rectangle endRect = new Rectangle((rows - 1) * c_width, (cols -1) * c_height, c_width, c_height);
            g.FillRectangle(purpleBrush, endRect);

            //Putting Start point as openSet's first element
            openSet.Add(Start);
        }

        private void Pathfind()
        {
            while(true)
            {              
                if (openSet.Count > 0)
                {
                    int winner = 0;
                    for(int i = 0; i < openSet.Count; i++)
                    {
                        if(openSet[i].f < openSet[winner].f)
                        {
                            winner = i;
                        }
                    }

                    Current = openSet[winner];

                    if(Current == End)
                    {
                        Console.WriteLine("Pathfinding done");
                        break;
                        
                    }
                    
                    RemoveElement(openSet, Current);
                    
                    closedSet.Add(Current);
                    
                    var neighbors = Current.neighbors;                  

                    
                    //checking each neighbor Spot
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        Console.WriteLine("checking neighbors");
                        var neighbor = neighbors[i];
                        if(!closedSet.Contains(neighbor))
                        {
                            var tempG = Current.g + 1;
                            var newPath = false;

                            if(openSet.Contains(neighbor))
                            {
                                if(tempG < neighbor.g)
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
                            if(newPath)
                            {
                                neighbor.h = Heuristic(neighbor, End);
                                neighbor.f = neighbor.g + neighbor.h;
                                neighbor.previous = Current;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("no solution");
                    return;
                }
            }
            VisualizingPath();

        }

        private int Heuristic(Spot a, Spot b)
        {
            //Euclidean distance function
            var dist = Math.Sqrt(((a.i - b.i) * (a.i - b.i) + (a.j - b.j) * (a.j - b.j)));
            return (int)dist;
        }

        private void VisualizingPath()
        {          
            for (int i = 0; i < closedSet.Count; i++)
            {
                closedSet[i].Display(new SolidBrush(Color.Green));
            }

            for (int i = 0; i < closedSet.Count; i++)
            {
                closedSet[i].Display(new SolidBrush(Color.Red));
            }

            List<Spot> path = new List<Spot>();
            Spot temp = Current;
            path.Add(temp);

            while(temp.previous != null)
            {
                path.Add(temp.previous);
                temp = temp.previous;
            }

            for (int i = 0; i < closedSet.Count; i++)
            {
                path[i].Display(new SolidBrush(Color.Blue));
            }
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
    }
}
