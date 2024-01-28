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
        SKWorkspaceMapper NextTest(MouseAgent agent, bool isReload = false);
        SKWorkspaceMapper Reload(MouseAgent agent);
    }
}
