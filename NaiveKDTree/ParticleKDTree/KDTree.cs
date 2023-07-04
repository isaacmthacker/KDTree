using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParticleKDTree
{
    public class Node
    {
        //topleft
        public float x, y;
        public float width, height;
        public float wrad, hrad;
        public Node left = null;
        public Node right = null;
        public Node parent = null;
        public bool leaf = false;
        public Dictionary<int, Particle> particles = new Dictionary<int, Particle>();
        public List<Node> leaves = new List<Node>();

        public Node(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            width = w;
            height = h;
            wrad = this.width / 2.0f;
            hrad = this.height / 2.0f;
        }
        public bool Intersect(float in_x, float in_y)
        {
            return x <= in_x && in_x <= (x + width) &&
                y <= in_y && in_y <= (y + height);
        }

        public bool CheckIntersection(double val, double calcMid, double checkMid, double minRange, double maxRange, double prad)
        {
            //Check if particle intersects one of the rectangle's edges
            double s = prad * prad - (val - calcMid) * (val - calcMid);
            if (s >= 0)
            {
                s = Math.Sqrt(s);
                double checkVal = checkMid + s;
                if (minRange <= checkVal && checkVal <= maxRange)
                {
                    return true;
                }

                checkVal = checkMid - s;
                if (minRange <= checkVal && checkVal <= maxRange)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IntersectParticle(Particle p)
        {
            //Cases:
            //1) inside square
            //2) intersect square
            //3) outside square

            float midx = x + wrad;
            float midy = y + hrad;


            //inside square

            if (midx - wrad <= p.x - p.radius && p.x + p.radius <= midx + wrad)
            {
                if (midy - hrad <= p.y - p.radius && p.y + p.radius <= midy + hrad)
                {
                    return true;
                }
            }

            //Intersect square
            //plug in x
            //y = a +/- sqrt(r^2 - (x-b)^2), x = rx +/- rwrad, y in [ry+/-rhrad]
            if (CheckIntersection(midx - wrad, p.x, p.y, midy - hrad, midy + hrad, p.radius))
            {
                return true;
            }

            if (CheckIntersection(midx + wrad, p.x, p.y, midy - hrad, midy + hrad, p.radius))
            {
                return true;
            }

            //check y
            //x = b +/- sqrt(r^2 - (y-a)^2), y = ry +/- rhrad, x in [rx +-/ rwrad]
            if (CheckIntersection(midy - hrad, p.y, p.x, midx - wrad, midx + wrad, p.radius))
            {
                return true;
            }
            if (CheckIntersection(midy + hrad, p.y, p.x, midx - wrad, midx + wrad, p.radius))
            {
                return true;
            }
            return false;
        }

        public void Add(Particle p)
        {
            particles.Add(p.id, p);
        }
        public void Remove(Particle p)
        {
            if(particles.ContainsKey(p.id))
            {
                particles.Remove(p.id);
            }
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
                foreach (int id in particles.Keys)
                {
                    ret += particles[id].ToString() + ",";
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
                foreach (int id in particles.Keys)
                {
                    Particle p = particles[id];
                    p.Draw(g);
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
        public Node GetNode(float x, float y)
        {
            Node cur = root;

            while (!cur.leaf)
            {
                if (cur.left.Intersect(x, y))
                {
                    cur = cur.left;
                }
                else if (cur.right.Intersect(x, y))
                {
                    cur = cur.right;
                }
                else
                {
                    Console.WriteLine("huh??????????");
                }
            }
            return cur;
        }

        public void GetNodesHelper(Particle p, Node cur, ref List<Node> retList)
        {
            if (cur.leaf)
            {
                retList.Add(cur);
            }
            else
            {
                if (cur.left.IntersectParticle(p))
                {
                    GetNodesHelper(p, cur.left, ref retList);
                }
                if (cur.right.IntersectParticle(p))
                {
                    GetNodesHelper(p, cur.right, ref retList);
                }
            }
        }
        public List<Node> GetNodes(Particle p)
        {
            //Get all leaf nodes that intersect the particle
            List<Node> retList = new List<Node>();
            GetNodesHelper(p, root, ref retList);
            return retList;

        }

        public void Add(ref Particle p)
        {
            List<Node> nodes = GetNodes(p);
            foreach(Node node in nodes)
            {
                node.Add(p);
                p.parents.Add(node);
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
            //Draws boxes that make up the tree
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
