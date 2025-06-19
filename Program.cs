using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilesCompressionProject
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            int[] x = { 5, 5, 2, 1, 1, 1, 1, 3 };
            List<int> y = new List<int>();
            int c = 1;
            for (int i = 0; i < x.Length - 1; i++)
            {
                if (x[i] == x[i + 1])
                {
                    c++;
                }
                else
                {
                    y.Add(c);
                    y.Add(x[i]);
                    c = 1;
                }
            }
            y.Add(c);
            y.Add(x[x.Length - 1]);
            Console.WriteLine(string.Join(", ", y));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
