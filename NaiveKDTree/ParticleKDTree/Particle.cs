using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleKDTree
{
    public class Particle
    {
        public float x, y, vel_x, vel_y;
        public float diameter;
        public float radius;
        public int windowWidth, windowHeight;
        public Brush color = Brushes.Red;
        public double initMagnitude = 0.0;
        public List<Node> parents = new List<Node>();
        public int id = 0;
        public Particle(float x, float y, float vel_x, float vel_y, float diameter, int windowWidth, int windowHeight)
        {
            //Starting particle location
            this.x = x;
            this.y = y;

            //Particle speed
            this.vel_x = vel_x;
            this.vel_y = vel_y;
            //Magnitude for initial velocity
            initMagnitude = Math.Sqrt(this.vel_x * this.vel_x + this.vel_y * this.vel_y);

            //Particle size
            this.diameter = diameter;
            radius = diameter / 2.0f;

            //Window bounds checking
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
            //color = Brushes.Red;
        }
        private void BoundsCheck(ref float pos, ref float vel, float maxPos)
        {
            //Check to make sure particle doesn't leave the window


            //todo: update this to use magnitude? or still valid?
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
        public void Adjust()
        {
            vel_x *= -1;
            vel_y *= -1;
        }
        public void Move()
        {
            BoundsCheck(ref x, ref vel_x, windowWidth);
            BoundsCheck(ref y, ref vel_y, windowHeight);
        }

        public bool Intersect(Particle other)
        {
            double dist = (other.y - y) * (other.y - y) + (other.x - x) * (other.x - x);
            return dist <= (other.radius + radius) * (other.radius + radius);
        }
    }
}
