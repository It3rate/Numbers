using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Numbers
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Trait t0 = new Trait();
            var unit = t0.AddFocalByValues(100, 200);
            var unot = t0.AddFocalByIndexes(unit.EndId, unit.StartId);
            var range = t0.AddFocalByValues(-900, 1100);
            var domain = t0.AddDomain(unit.Id, unot.Id, range.Id);

            var val = t0.AddFocalByIndexValue(unit.StartId, 650);

            var num = new Number(t0, domain.Id, val.Id);

            Console.WriteLine(num.Value);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }
}
