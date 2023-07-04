using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ParticleKDTree
{
    public class ParticleSystem
    {
        KDTree tree;
        int width, height;
        List<Particle> particles;
        float maxVelocity = 3;


        Dictionary<int, Particle> particlesChecked = new Dictionary<int, Particle>();

        public ParticleSystem(int windowWidth, int windowHeight)
        {
            int depth = 5;

            width = windowWidth;
            height = windowHeight;
            tree = new KDTree(depth, width, height);

            int numParticles = 50;

            particles = new List<Particle>();
            Console.WriteLine("Creating particles");
            CreateParticles(numParticles, width, height);
            Console.WriteLine("Created particles");
        }

        public void CreateParticles(int numParticles, int windowWidth, int windowHeight)
        {
            Console.WriteLine("w: " + windowWidth.ToString() + " h: " + windowHeight.ToString());
            Random r = new Random();
            float size = 50;
            for (int i = 0; i < numParticles; i++)
            {
                float x, y, vel_x, vel_y;

                //Random position
                x = windowWidth * (r.Next(1, 100) / 100.0f);
                y = windowHeight * (r.Next(1, 100) / 100.0f);
                //Random velocities
                vel_x = maxVelocity * (r.Next(1, 100) / 100.0f);
                vel_y = maxVelocity * (r.Next(1, 100) / 100.0f);

                if (i == 0)
                {
                    vel_x = 1;
                    vel_y = 1;
                }

                Particle p = new Particle(x, y, vel_x, vel_y, size, windowWidth, windowHeight);
                //Set particle id
                p.id = i;
                particles.Add(p);
                tree.Add(ref p);
            }
        }
        public void Move()
        {
            particlesChecked.Clear();

            //todo: when moving particles, update tree with new particle location if they left a specified node
            HashSet<int> velocityAdjusted = new HashSet<int>();
            foreach (Particle p in particles)
            {
                int particleCheckCount = 0;
                if (!velocityAdjusted.Contains(p.id))
                {
                    foreach (Node n in p.parents)
                    {
                        foreach (int id in n.particles.Keys)
                        {
                            if (p.id == 0)
                            {
                                if (!particlesChecked.ContainsKey(id))
                                {
                                    particlesChecked.Add(id, n.particles[id]);
                                }

                            }

                            particleCheckCount += 1;
                            if (!velocityAdjusted.Contains(id) && id != p.id && p.Intersect(n.particles[id]))
                            {
                                //todo merge in smart particle intersection
                                Particle iParticle = p;
                                Particle jParticle = n.particles[id];

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

                                double i_velAngle = theta_P1 + Math.PI;
                                //double mag = Math.Sqrt(iParticle.vel_x*iParticle.vel_x + iParticle.vel_y*iParticle.vel_y);

                                double mag = iParticle.initMagnitude;

                                double newvelx = mag * Math.Cos(i_velAngle);
                                double newvely = mag * Math.Sin(i_velAngle);
                                iParticle.vel_x = (float)newvelx;
                                iParticle.vel_y = (float)newvely;


                                double j_velAngle = theta_P2 + Math.PI;
                                //mag = Math.Sqrt(iParticle.vel_x * iParticle.vel_x + iParticle.vel_y * iParticle.vel_y);
                                mag = jParticle.initMagnitude;

                                newvelx = mag * Math.Cos(j_velAngle);
                                newvely = mag * Math.Sin(j_velAngle);
                                jParticle.vel_x = (float)newvelx;
                                jParticle.vel_y = (float)newvely;


                                velocityAdjusted.Add(id);
                                velocityAdjusted.Add(p.id);
                                break;
                            }
                        }
                    }
                }
                //Console.WriteLine(p.id.ToString() + ": " + particleCheckCount.ToString());
                p.Move();
                Particle refP = p;
                ResetTreePosition(ref refP);
            }
        }
        public void ResetTreePosition(ref Particle p)
        {
            foreach (Node n in p.parents)
            {
                n.particles.Remove(p.id);
            }
            p.parents.Clear();
            tree.Add(ref p);
        }
        public void Draw(Graphics g)
        {
            //foreach(Particle p in particles)
            //{
            //    p.color = Brushes.Red;
            //}

            //foreach (int id in particlesChecked.Keys)
            //{
            //    particlesChecked[id].color = Brushes.Purple;
            //}

            foreach (Particle p in particles)
            {
                if (p.id == 0)
                {
                    p.color = Brushes.Green;
                    foreach (Node n in p.parents)
                    {
                        n.Draw(g);
                    }

                    //List<Node> nodes = tree.GetNodes(p);
                    //foreach (Node n in nodes)
                    //{
                    //    n.Draw(g);
                    //}

                }
                p.Draw(g);
            }
        }
    }
}
