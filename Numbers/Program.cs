using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var range = t0.AddFocalByValues(-900, 1100);
            var domain = t0.AddDomain(unit.Id, range.Id);

            var val0 = t0.AddFocalByIndexValue(unit.StartId, 650);
            var val1 = t0.AddFocalByValueIndex(-300, unit.StartId);
            var val2 = t0.AddFocalByValues(-200, 950);

            var num0 = new Number(domain, val0.Id);
            var num1 = new Number(domain, val1.Id);
            var num2 = new Number(domain, val2.Id);

            Console.WriteLine(num0);
            Console.WriteLine(num1);
            Console.WriteLine(num2);
            num2.Add(num1);
            Console.WriteLine(num2);
            num2.Multiply(num0);
            Console.WriteLine(num2);
            num2.Divide(num0);
            Console.WriteLine(num2);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

        }
    }
}
