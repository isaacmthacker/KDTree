using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaiveKDTree
{
    class Point
    {
        public float x;
        public float y;
        public Point(float xx, float yy)
        {
            x = xx;
            y = yy;
        }
        public override string ToString()
        {
            return "(" + x.ToString() + "," + y.ToString() + ")";
        }
    }
    class Node
    {
        //topleft
        public float x, y;
        public float width, height;
        public Node left = null;
        public Node right = null;
        public Node parent = null;
        public bool leaf = false;
        public List<Point> points = new List<Point>();

        public Node(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            width = w;
            height = h;
        }
        public bool Intersect(Point p)
        {
            return x <= p.x && p.x <= (x + width) &&
                y <= p.y && p.y <= (y + height);
        }
        public void Add(Point p)
        {
            points.Add(p);
        }
        public override string ToString()
        {
            string ret = "[" + x.ToString() + "," + y.ToString() +
                "->" + (x + width).ToString() + "," + (y + height).ToString() +
                " " + width.ToString() + "," + height.ToString() + "]";
            if (leaf)
            {
                ret += " Leaf: ";
                ret += "{";
                foreach (Point p in points)
                {
                    ret += p.ToString() + ",";
                }
                ret += "}";
            }
            return ret;
        }
        public void Draw(Graphics g)
        {
            //top
            g.DrawLine(Pens.Black, new PointF(x, y), new PointF(x + width, y));
            //left
            g.DrawLine(Pens.Black, new PointF(x, y), new PointF(x, y + height));
            //right
            g.DrawLine(Pens.Black, new PointF(x + width, y), new PointF(x + width, y + height));
            //bottom
            g.DrawLine(Pens.Black, new PointF(x, y + height), new PointF(x + width, y + height));

            if (leaf)
            {
                foreach (Point point in points)
                {
                    float rad = 5f;
                    g.FillEllipse(Brushes.Red, new RectangleF(
                        new PointF(point.x-rad/2.0f, point.y-rad/2.0f),
                        new SizeF(rad, rad)));
                }
            }
        }


    }
    internal class KDTree
    {
        int depth;
        float width, height;
        Node root;
        public List<Node> leaves = new List<Node>();
        public KDTree(int d, float w, float h)
        {
            depth = d;
            width = w;
            height = h;
            CreateTree();
        }
        public Node GetNode(Point p)
        {
            Node cur = root;

            while(!cur.leaf)
            {
                //bool l = cur.left.Intersect(p);
                //bool r = cur.right.Intersect(p);
                //if (l && r)
                //{
                //    Console.WriteLine("Double");
                //}
                if (cur.left.Intersect(p))
                {
                    cur = cur.left;
                }
                else if (cur.right.Intersect(p))
                {
                    cur = cur.right;
                }
                else
                {
                    Console.WriteLine("huh");
                }
            }
            return cur;
        }
        public void Add(Point p)
        {
            Node n = GetNode(p);
            n.points.Add(p);
        }
        public List<Point> GetPoints(int x, int y)
        {
            Node n = GetNode(new Point(x, y));
            return n.points;
        }


        private void AddHelper(Node cur, Point p)
        {
            if (cur.leaf)
            {
                cur.Add(p);
            }
            else
            {
                bool l = cur.left.Intersect(p);
                bool r  = cur.right.Intersect(p);
                if(l && r)
                {
                    Console.WriteLine("Double");
                }
                if (cur.left.Intersect(p))
                {
                    AddHelper(cur.left, p);
                }
                else if (cur.right.Intersect(p))
                {
                    AddHelper(cur.right, p);
                }
                else
                {
                    Console.WriteLine("huh");
                }
            }
        }
        private void CreateTree()
        {
            root = new Node(0, 0, width, height);
            CreateTreeHelper(root, 0, 0);
        }
        private void CreateTreeHelper(Node cur, int curDepth, int curDim)
        {
            if (curDepth == depth)
            {
                cur.leaf = true;
                leaves.Add(cur);
                return;
            }

            //X
            if (curDim == 0)
            {
                float newWidth = cur.width / 2.0f;
                cur.left = new Node(cur.x, cur.y, newWidth, cur.height);
                cur.right = new Node(cur.x + newWidth, cur.y, newWidth, cur.height);
            }
            else
            {
                //Y
                float newHeight = cur.height / 2.0f;
                cur.left = new Node(cur.x, cur.y, cur.width, newHeight);
                cur.right = new Node(cur.x, cur.y + newHeight, cur.width, newHeight);
            }
            cur.left.parent = cur;
            cur.right.parent = cur;
            CreateTreeHelper(cur.left, curDepth + 1, 1 - curDim);
            CreateTreeHelper(cur.right, curDepth + 1, 1 - curDim);
        }
        public void Print()
        {
            PrintHelper(root, "");
        }
        private void PrintHelper(Node cur, string indent)
        {
            Console.WriteLine(indent + cur.ToString());
            if (!cur.leaf)
            {
                PrintHelper(cur.left, indent + "\t");
                PrintHelper(cur.right, indent + "\t");
            }
        }
        public void Draw(Graphics g)
        {
            DrawHelper(root, g);
        }
        private void DrawHelper(Node cur, Graphics g)
        {
            cur.Draw(g);
            if (!cur.leaf)
            {
                DrawHelper(cur.left, g);
                DrawHelper(cur.right, g);
            }
        }
    }
}
