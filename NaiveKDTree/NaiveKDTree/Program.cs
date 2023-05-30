using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaiveKDTree
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //KDTree k = new KDTree(2, 100, 100);
            //k.Print();
            //k.Add(new Point(23.5f, 40f));
            //k.Print();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            string a = Console.ReadLine();

        }
    }
}
