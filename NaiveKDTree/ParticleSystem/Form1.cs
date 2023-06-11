using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParticleSystem
{
    public class TimerRunner
    {
        Form1 form;
        public TimerRunner(Form1 f)
        {
            form = f;
        }
        public void Run()
        {
            while (true)
            {
                Thread.Sleep(17);
                //Thread.Sleep(100);
                form.particleSystem.Move();
                form.Invalidate();
            }
        }
    }
    public partial class Form1 : Form
    {
        public ParticleSystem particleSystem = new ParticleSystem();
        Thread timerThread;
        public Form1()
        {
            DoubleBuffered = true;
            InitializeComponent();
            particleSystem.CreateParticles(50, ClientSize.Width, ClientSize.Height);
            TimerRunner timerRunner = new TimerRunner(this);
            timerThread = new Thread(new ThreadStart(timerRunner.Run));
            timerThread.Start();
        }

        public void DrawParticles(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //particleSystem.Run(g);
            particleSystem.Draw(g);
        }
        public void UpdateWindow(object sender, EventArgs e)
        {
            Invalidate();
        }
        public void CloseWindow(object sender, FormClosingEventArgs e)
        {
            timerThread.Abort();
        }
        public void Form1_Resize(object sender, EventArgs e)
        {
            particleSystem.Resize(ClientSize.Width, ClientSize.Height);
        }
    }
}
