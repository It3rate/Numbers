namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Selection
    {
        public int[] ValueIds { get; }

        public Selection(params int[] ids)
        {
	        ValueIds = ids;
        }
    }
}
