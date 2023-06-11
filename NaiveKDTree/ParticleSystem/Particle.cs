using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ParticleSystem
{
    internal class Particle
    {
        public float x, y, vel_x, vel_y;
        public float diameter;
        public float radius;
        public int windowWidth, windowHeight;
        public Brush color = Brushes.Red;
        public Particle(float x, float y, float vel_x, float vel_y, float diameter, int windowWidth, int windowHeight)
        {
            this.x = x;
            this.y = y;
            this.vel_x = vel_x;
            this.vel_y = vel_y;
            this.diameter = diameter;
            this.radius = diameter / 2.0f;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

        }
        public void Draw(Graphics g)
        {
            //x, y center of ellipse
            //drawing expects top left corner
            //Expects size of rectange, need to use diameter
            g.FillEllipse(color, new RectangleF(x - radius, y - radius, diameter, diameter));
            g.FillEllipse(Brushes.Black, new RectangleF(x - 2.5f, y - 2.5f, 5, 5));
            color = Brushes.Red;
        }
        private void BoundsCheck(ref float pos, ref float vel, float maxPos)
        {
            float nextPos = pos + vel;
            if (nextPos < radius || nextPos > maxPos - radius)
            {
                if (nextPos < radius)
                {
                    nextPos = radius;
                }
                else
                {
                    nextPos = maxPos - radius;
                }
                vel *= -1;
            }
            pos = nextPos;
        }
        public void Move()
        {
            BoundsCheck(ref x, ref vel_x, windowWidth);
            BoundsCheck(ref y, ref vel_y, windowHeight);

        }
        public bool Intersect(Particle other)
        {
            
            double dist = Math.Sqrt((other.y - y) * (other.y - y) + (other.x - x) * (other.x - x));
            bool ret = dist <= (other.radius + radius);

            if (ret)
            {
                Console.WriteLine("Intersect: " + x.ToString() + "," + y.ToString() + " " + other.x.ToString() + "," + other.y.ToString());
                return true;
            }
            return ret;
        }

        public void Run(Graphics g)
        {
            Move();
            Draw(g);
        }
    }
}
