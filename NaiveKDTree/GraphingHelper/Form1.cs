using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphingHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void DrawEllipse(Graphics g, float x, float y, float rad, Brush brush)
        {
            g.FillEllipse(brush, new RectangleF(new PointF(x - rad, y - rad), new SizeF(2 * rad, 2 * rad)));
        }

        void DrawEllipses(Graphics g, float x1, float y1, float x2, float y2)
        {
            Console.WriteLine(string.Format("{0} {1} {2} {3}", x1, y1, x2, y2));

            double dist = Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
            float rad = (float)(dist / 2.0);

            //Initial ellipses
            DrawEllipse(g, x1, y1, rad, Brushes.Red);
            DrawEllipse(g, x2, y2, rad, Brushes.Red);


            float midRad = 2.5f;
            DrawEllipse(g, x1, y1, midRad, Brushes.Black);
            DrawEllipse(g, x2, y2, midRad, Brushes.White);

            //Triangle
            g.DrawLine(new Pen(Brushes.Black), new PointF(x1, y1), new PointF(x2, y2));
            g.DrawLine(new Pen(Brushes.Black), new PointF(x1, y1), new PointF(x2, y1));
            g.DrawLine(new Pen(Brushes.Black), new PointF(x2, y1), new PointF(x2, y2));
            g.DrawLine(new Pen(Brushes.Black), new PointF(x1, y2), new PointF(x2, y2));
            g.DrawLine(new Pen(Brushes.Black), new PointF(x1, y1), new PointF(x1, y2));

            //Outline
            double step = Math.PI / 12.0;
            double cur = 0;
            while (cur <= 2 * Math.PI)
            {
                float x = (float)(rad * Math.Cos(cur));
                float y = (float)(rad * Math.Sin(cur));

                DrawEllipse(g, x + x1, y + y1, midRad, Brushes.Black);
                DrawEllipse(g, x + x2, y + y2, midRad, Brushes.Black);

                cur += step;
            }
            //Test angle point
            double angleTest = Math.PI / 2.0;
            DrawEllipse(g, (float)(rad * Math.Cos(angleTest) + x2), (float)(rad * Math.Sin(angleTest) + y2), 2 * midRad, Brushes.Orange);


            //y2 > y1, x2 > x1
            //p1 = -1*asin(h/d)
            //p2 = PI + -1*asin(h/d)

            //y2 < y1, x2 < x1
            //p1 = PI + -1*asin(h/d)
            //p2 = -1*asin(h/d)

            //y2 < y1, x2 > x1
            //p1 = asin(h/d)
            //p2 = PI + asin(h/d)

            //y2 > y1, x2 < x1
            //p1 = PI + asin(h/d)
            //p2 = asin(h/d)




            double height = Math.Abs(y2 - y1);
            double width = Math.Abs(x2 - x1);   //needed at all?
            double asin = Math.Asin(height / dist);


            double theta_P1, theta_P2;

            if (y2 > y1 && x2 > x1)
            {
                Console.WriteLine("case 1");
                theta_P1 = asin;
                theta_P2 = Math.PI + asin;
            }
            else if (y2 < y1 && x2 < x1)
            {
                Console.WriteLine("case 2");
                theta_P1 = Math.PI + asin;
                theta_P2 = asin;
            }
            else if (y2 < y1 && x2 > x1)
            {
                Console.WriteLine("case 3");
                theta_P1 = -1 * asin;
                theta_P2 = Math.PI + -1 * asin;
            }
            else if (y2 > y1 && x2 < x1)
            {
                Console.WriteLine("case 4");
                theta_P1 = Math.PI + -1 * asin;
                theta_P2 = -1 * asin;
            }
            else if (x1 == x2)
            {
                Console.WriteLine("case 5");
                if (y1 < y2)
                {
                    Console.WriteLine("case 5a");
                    theta_P1 = Math.PI + -1 * asin;
                    theta_P2 = -1 * asin;
                }
                else
                {
                    Console.WriteLine("case 5b");
                    theta_P1 = -1 * asin;
                    theta_P2 = Math.PI + -1 * asin;
                }
            }
            else if (y1 == y2)
            {
                Console.WriteLine("case 6");
                if (x1 < x2)
                {
                    Console.WriteLine("case 6a");
                    theta_P1 = 0;
                    theta_P2 = Math.PI;
                }
                else
                {
                    Console.WriteLine("case 6b");
                    theta_P1 = Math.PI;
                    theta_P2 = 0;
                }
            }
            else
            {
                Console.WriteLine("todo - if x1==x2 or y1==y1");
                theta_P1 = 0;
                theta_P2 = 0;
            }




            float x3 = (float)(rad * Math.Cos(theta_P1) + x1);
            float y3 = (float)(rad * Math.Sin(theta_P1) + y1);
            DrawEllipse(g, x3, y3, 2 * midRad, Brushes.Blue);

            x3 = (float)(rad * Math.Cos(theta_P2) + x2);
            y3 = (float)(rad * Math.Sin(theta_P2) + y2);
            DrawEllipse(g, x3, y3, 2 * midRad, Brushes.Green);


            Console.WriteLine(string.Format("{0},{1} {2},{3} {4},{5} theta_P1:{6} theta_P2:{7}", x1, y1, x2, y2, x3, y3, theta_P1, theta_P2));
        }

        public void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //4 cases
            float x1 = 111;
            float y1 = 74;
            float x2 = 194;
            float y2 = 141;

            DrawEllipses(g, x1, y1, x2, y2);

            x2 = 76;
            y2 = 256;
            x1 = 189;
            y1 = 354;

            DrawEllipses(g, x1, y1, x2, y2);

            x2 = 456;
            y2 = 175;
            x1 = 577;
            y1 = 95;

            DrawEllipses(g, x1, y1, x2, y2);

            x1 = 550;
            y1 = 377;
            x2 = 631;
            y2 = 307;

            DrawEllipses(g, x1, y1, x2, y2);

            x1 = 333;
            y1 = 260;
            x2 = 333;
            y2 = 350;

            DrawEllipses(g, x1, y1, x2, y2);

            x2 = 830;
            y2 = 100;
            x1 = 830;
            y1 = 200;
            DrawEllipses(g, x1, y1, x2, y2);



            x1 = 740;
            y1 = 360;
            x2 = 830;
            y2 = 360;
            DrawEllipses(g, x1, y1, x2, y2);

            x2 = 590;
            y2 = 490;
            x1 = 690;
            y1 = 490;
            DrawEllipses(g, x1, y1, x2, y2);

            //===========================


            //float x1 = 592;
            //float y1 = 276;
            //float x2 = 368;
            //float y2 = 186;

            //float x2 = 592;
            //float y2 = 276;
            //float x1 = 368;
            //float y1 = 186;

            //float x1 = 554;
            //float y1 = 132;
            //float x2 = 364;
            //float y2 = 246;

            //float x2 = 554;
            //float y2 = 132;
            //float x1 = 364;
            //float y1 = 246;








        }

        public void Form1_Click(object sender, MouseEventArgs e)
        {
            Console.WriteLine(string.Format("{0} {1}", e.X, e.Y));
        }

    }
}
