using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaiveKDTree
{
    public partial class Form1 : Form
    {
        KDTree k;
        List<Point> clicked =  new List<Point>();
        public Form1()
        {
            DoubleBuffered = true;
            Width = 1000;
            InitializeComponent();

            k = new KDTree(7, ClientSize.Width, ClientSize.Height);
            k.Print();

            Random r = new Random();
            float xOffset = ClientSize.Width / 2.0f;
            float yOffset = ClientSize.Height / 2.0f;
            Console.WriteLine("Offset: " + xOffset.ToString() + "," + yOffset.ToString());
            for (int i = 0; i < 50000; ++i)
            {
                float x = r.Next(ClientSize.Width);
                float y = r.Next(ClientSize.Height);

                k.Add(new Point(x, y));
            }
            //k.Print();
        }

        public void KDTree_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.White, new Rectangle(0,0,ClientSize.Width,ClientSize.Height)); 
            k.Draw(g);
            foreach(Point p in clicked)
            {
                float rad = 5f;
                g.FillEllipse(Brushes.Green, new RectangleF(
                    new PointF(p.x - rad / 2.0f, p.y - rad / 2.0f),
                    new SizeF(rad, rad)));
            }
        }

        public void KDTree_Click(object sender, MouseEventArgs e) {
            int mouseX = e.X;
            int mouseY = e.Y;
            Console.WriteLine("Clicked");
            Timer t = new Timer();
            t.Start();
            Node n = k.GetNode(new Point(mouseX, mouseY));
            clicked = k.GetPoints(mouseX, mouseY);
            //foreach(Point p in clicked)
            //{
            //    Console.WriteLine(p.ToString());
            //}
            t.Stop();
            Console.WriteLine("Done " + t.Interval.ToString());
            //Invalidate(new Region(new RectangleF(new PointF(n.x, n.y), new SizeF(n.width, n.height))), false);
            Invalidate();
        }
    }
}
