using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParticleSystem
{
    public class ParticleSystem
    {
        public ArrayList particles = new ArrayList();
        public ParticleSystem() { }
        public void CreateParticles(int numParticles, int windowWidth, int windowHeight)
        {
            Console.WriteLine("w: " + windowWidth.ToString() + " h: " + windowHeight.ToString());
            Random r = new Random();
            float size = 50;
            for (int i = 0; i < numParticles; i++)
            {
                float x = 0;
                float y = 0;
                float vel_x = 0;
                float vel_y = 0;

                x = (float)windowWidth * ((float)r.Next(1, 100) / 100.0f);
                y = (float)windowHeight * ((float)r.Next(1, 100) / 100.0f);

                vel_x = 10 * ((float)r.Next(1, 100) / 100.0f);
                vel_y = 10 * ((float)r.Next(1, 100) / 100.0f);



                particles.Add(new Particle(x, y, vel_x, vel_y, size, windowWidth, windowHeight));
            }

            //Test code
            //int steps = 50;
            //Opposite velx and vely
            /*
            float velx = (windowWidth * 0.5f - windowWidth * 0.25f) / (float)steps;
            float vely = (windowHeight * 0.5f - windowHeight * 0.25f) / (float)steps;
            particles.Add(new Particle(windowWidth *0.25f, windowHeight * 0.25f, velx, vely, size, windowWidth, windowHeight));
            velx = (windowWidth * 0.5f - windowWidth * 0.75f) / (float)steps;
            vely = (windowHeight * 0.5f - windowHeight * 0.75f) / (float)steps;
            particles.Add(new Particle(windowWidth *0.75f, windowHeight * 0.75f, velx,  vely, size, windowWidth, windowHeight));
            */

            /*
            //velx = 0
            float vely = (windowHeight * 0.5f - windowHeight * 0.25f) / (float)steps;
            particles.Add(new Particle(windowWidth * 0.5f, windowHeight * 0.25f, 0, vely, size, windowWidth, windowHeight));
            vely = (windowHeight * 0.5f - windowHeight * 0.75f) / (float)steps;
            particles.Add(new Particle(windowWidth * 0.5f, windowHeight * 0.75f, 0, vely, size, windowWidth, windowHeight));
            */

            /*
            //vely = 0
            float velx = (windowWidth * 0.5f - windowWidth * 0.25f) / (float)steps;
            particles.Add(new Particle(windowWidth * 0.25f, windowHeight * 0.5f, velx, 0, size, windowWidth, windowHeight));
            velx = (windowWidth * 0.5f - windowWidth * 0.75f) / (float)steps;
            particles.Add(new Particle(windowWidth * 0.75f, windowHeight * 0.5f, velx, 0, size, windowWidth, windowHeight));
            */

            /*
            //same dir velx and vely
            steps = 150;
            float startx = windowWidth * 0.375f;
            float starty = windowHeight * 0.375f;
            float endx = windowWidth*0.5f;
            float endy = windowHeight*0.5f;
            float velx = (startx - endx) / (float)steps;
            float vely = (starty - endy) / (float)steps;
            particles.Add(new Particle(startx, starty, velx, vely, size, windowWidth, windowHeight));

            startx = windowWidth * 0.1f;
            starty = windowHeight * 0.1f;
            endx = windowWidth * 0.75f;
            endy = windowHeight * 0.75f;
            velx = (startx - endx) / (float)steps;
            vely = (starty - endy) / (float)steps;
            particles.Add(new Particle(startx, starty, velx, vely, size, windowWidth, windowHeight));
            */
        }

        public void Resize(int width, int height)
        {
            foreach (Particle particle in particles)
            {
                particle.windowWidth = width;
                particle.windowHeight = height;
            }
        }

        public void Run(Graphics g)
        {
            foreach (Particle particle in particles)
            {
                particle.Run(g);
            }
        }

        public void Move()
        {
            HashSet<Particle> processed = new HashSet<Particle>();
            for (int i = 0; i < particles.Count; ++i)
            {
                Particle iParticle = particles[i] as Particle;
                if (!processed.Contains(iParticle))
                {
                    for (int j = 0; j < particles.Count; ++j)
                    {
                        if (i != j)
                        {
                            Particle jParticle = particles[j] as Particle;
                            if (!processed.Contains(jParticle))
                            {
                                if (iParticle.Intersect(jParticle))
                                {
                                    float x1 = iParticle.x;
                                    float y1 = iParticle.y;
                                    float x2 = jParticle.x;
                                    float y2 = jParticle.y;

                                    double dist = Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));

                                    double height = Math.Abs(y2 - y1);
                                    double width = Math.Abs(x2 - x1);   //needed at all?
                                    double asin = Math.Asin(height / dist);


                                    double theta_P1, theta_P2;

                                    if (y2 > y1 && x2 > x1)
                                    {
                                        //Console.WriteLine("case 1");
                                        theta_P1 = asin;
                                        theta_P2 = Math.PI + asin;
                                    }
                                    else if (y2 < y1 && x2 < x1)
                                    {
                                        //Console.WriteLine("case 2");
                                        theta_P1 = Math.PI + asin;
                                        theta_P2 = asin;
                                    }
                                    else if (y2 < y1 && x2 > x1)
                                    {
                                        //Console.WriteLine("case 3");
                                        theta_P1 = -1 * asin;
                                        theta_P2 = Math.PI + -1 * asin;
                                    }
                                    else if (y2 > y1 && x2 < x1)
                                    {
                                        //Console.WriteLine("case 4");
                                        theta_P1 = Math.PI + -1 * asin;
                                        theta_P2 = -1 * asin;
                                    }
                                    else if (x1 == x2)
                                    {
                                        //Console.WriteLine("case 5");
                                        if (y1 < y2)
                                        {
                                            //Console.WriteLine("case 5a");
                                            theta_P1 = Math.PI + -1 * asin;
                                            theta_P2 = -1 * asin;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("case 5b");
                                            theta_P1 = -1 * asin;
                                            theta_P2 = Math.PI + -1 * asin;
                                        }
                                    }
                                    else if (y1 == y2)
                                    {
                                        //Console.WriteLine("case 6");
                                        if (x1 < x2)
                                        {
                                            //Console.WriteLine("case 6a");
                                            theta_P1 = 0;
                                            theta_P2 = Math.PI;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("case 6b");
                                            theta_P1 = Math.PI;
                                            theta_P2 = 0;
                                        }
                                    }
                                    else
                                    {
                                        //Console.WriteLine("todo - what to do here? Anything?");
                                        theta_P1 = 0;
                                        theta_P2 = 0;
                                    }
                                    processed.Add(iParticle);
                                    processed.Add(jParticle);

                                    double i_velAngle = theta_P1 + Math.PI;
                                    //double mag = Math.Sqrt(iParticle.vel_x*iParticle.vel_x + iParticle.vel_y*iParticle.vel_y);

                                    double mag = iParticle.initMag;

                                    double newvelx = mag*Math.Cos(i_velAngle);
                                    double newvely = mag*Math.Sin(i_velAngle);
                                    iParticle.vel_x = (float)newvelx;
                                    iParticle.vel_y = (float)newvely;


                                    double j_velAngle = theta_P2 + Math.PI;
                                    //mag = Math.Sqrt(iParticle.vel_x * iParticle.vel_x + iParticle.vel_y * iParticle.vel_y);
                                    mag = jParticle.initMag;

                                    newvelx = mag * Math.Cos(j_velAngle);
                                    newvely = mag * Math.Sin(j_velAngle);
                                    jParticle.vel_x = (float)newvelx;
                                    jParticle.vel_y = (float)newvely;



                                }
                                if (i == 0)
                                {
                                    //Console.WriteLine(string.Format("{0} {1}", iParticle.vel_x, iParticle.vel_y));
                                    iParticle.color = Brushes.Purple;
                                }
                            }
                        }
                    }
                }
            }
            foreach (Particle particle in particles)
            {
                particle.Move();
            }
        }

        public void Draw(Graphics g)
        {
            foreach (Particle particle in particles)
            {
                particle.Draw(g);
            }
        }
    }
}
