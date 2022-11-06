using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Numbers.Core;

namespace Numbers
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
	        Trait t0 = new Trait();
	        //var unit = t0.AddFocalByValues(100, 200);
	        //var range = t0.AddFocalByValues(-900, 1100);
	        var unit = t0.AddFocalByValues(0, 100);
	        var range = t0.AddFocalByValues(-1000, 1000);
            var domain = t0.AddDomain(unit.Id, range.Id);
            var domain2 = t0.AddDomain(unit.Id, range.Id);
            var val2 = t0.AddFocalByValues(900, 200);
            var val3 = t0.AddFocalByValues(-400, 600);
            //var val2 = t0.AddFocalByValues(400, 500);
            //var val3 = t0.AddFocalByValues(0, 200);


            var num2 = new Number(domain, val2.Id);
            var num3 = new Number(domain2, val3.Id);
            var sel = new Selection(num2);
            var transform = t0.AddTransform(sel, num3, TransformKind.Blend);

            //var val0 = t0.AddFocalByIndexValue(unit.StartId, 650);
            //var val1 = t0.AddFocalByValueIndex(-300, unit.StartId);
            //var num0 = new Number(domain, val0.Id);
            //var num1 = new Number(domain, val1.Id);
            //Console.WriteLine(num0);
            //Console.WriteLine(num1);
            //Console.WriteLine(num2);
            //var num3 = num2.Clone();
            //num3.Add(num1);
            //Console.WriteLine(num3);
            //num3.Multiply(num0);
            //Console.WriteLine(num3);
            //num3.Divide(num0);
            //Console.WriteLine(num3);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CoreForm());

        }
    }
}
