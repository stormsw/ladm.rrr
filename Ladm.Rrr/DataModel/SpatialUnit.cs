using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    public abstract class SpatialUnit
    {
        public string SuId { get; set; }

        public float Area { get; set; }

        public virtual ICollection<SpatialUnit> Properties { get; set; }
    }

    public class Parcel:SpatialUnit
    {
        public Parcel():base()
        {

        }
        
    }

    public class Building: SpatialUnit
    {
        public Building()
            : base()
        {

        }

    }

}
