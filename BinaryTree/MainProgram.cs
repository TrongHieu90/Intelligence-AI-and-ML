/*This is a translation from Javascript to C# of the same topic 
 * from Daniel Shiffman
 * at https://github.com/shiffman/NOC-S17-2-Intelligence-Learning/tree/master/week1-graphs/01_binary_tree
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BinaryTree
{
    class Node
    {
        public int value;
        private Node leftNode = null;
        private Node rightNode = null;

        //constructor
        public Node(int val)
        {
            value = val;
        }

        //methods
        public void AddNode(Node n)
        {
            if(n.value < value)
            {
                if(leftNode == null)
                {
                    leftNode = n;
                }
                else
                {
                    leftNode.AddNode(n);
                }
            }
            else if(n.value > value)
            {
                if(rightNode == null)
                {
                    rightNode = n;
                }
                else
                {
                    rightNode.AddNode(n);
                }
            }
        }

        public void VisitNode()
        {
            if(leftNode != null)
            {
                leftNode.VisitNode();
            }
            Console.WriteLine(value);
            if(rightNode != null)
            {
                rightNode.VisitNode();
            }
        }

        public Node SearchNode(int val)
        {
            if(value == val)
            {
                return this;
            } else if(val < value && leftNode != null)
            {
                return this.leftNode.SearchNode(val); //recursively searching on left side
            } else if(val > value && rightNode != null)
            {
                return this.rightNode.SearchNode(val);//recursively searching on right side
            }
            return null;
        }

        
    }

    class Tree
    {
        private Node root;
        
        public void AddValue(int val)
        {
            Node n = new Node(val);
            
            if(root == null)
            {
                root = n;
            }
            else
            {
                root.AddNode(n);
            }
        }

        public void Traverse()
        {
            root.VisitNode();
        }

        public Node Search(int val)
        {
            Node found = root.SearchNode(val);
            return found;
        }

    }

    class MainProgram
    {
        static void Main(string[] args) 
        {
            Tree tree = new Tree();

            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                tree.AddValue(random.Next(0,100));
            }

            Console.WriteLine("Binary tree in sorted order\n");
            tree.Traverse();
            
            int j = 7; //hardcoded value to search on binary tree
            Node result = tree.Search(j);

            Console.WriteLine($"\nsearching for {j} in binary tree");

            if (result == null)
            {
                Console.WriteLine("not found");
            }
            else
            {
                Console.WriteLine($"found object: {result} with {result.value}");
            }

            Console.ReadLine();

        }
    }
}
