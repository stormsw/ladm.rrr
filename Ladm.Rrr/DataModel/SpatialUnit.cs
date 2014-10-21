using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    [Table("lr_spatial_unit")]
    public abstract class SpatialUnit
    {
        [Key]
        public int Id { get; set; }
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
