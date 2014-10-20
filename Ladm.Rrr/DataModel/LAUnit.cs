using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    public class LAUnit
    {
        public virtual ICollection<SpatialUnit> Properties { get; set; }

        public string UId { get; set; }
    }
}
