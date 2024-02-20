using Numbers.Agent;
using Numbers.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers
{
	public interface IDemos
    {
        SKWorkspaceMapper NextTest(MouseAgent agent);
        SKWorkspaceMapper PreviousTest(MouseAgent agent);
        SKWorkspaceMapper Reload(MouseAgent agent);

        SKWorkspaceMapper LoadTest(int index, MouseAgent mouseAgent);
    }
}
