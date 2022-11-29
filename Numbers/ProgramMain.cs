using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Numbers
{
    static class ProgramMain
    {
        [STAThread]
        static void Main()
        {
	        Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CoreForm());

        }
    }
}
