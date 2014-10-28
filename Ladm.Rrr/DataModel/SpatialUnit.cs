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
        public enum SpatialUnitStatus { New, Busy, Normal, Archived };
        [Key]
        public int Id { get; set; }
        public string SuId { get; set; }
        public float Area { get; set; }
        public SpatialUnitStatus Status { get; set; }
        /// <summary>
        /// To keep subsets
        /// </summary>
        public virtual ICollection<SpatialUnit> Members { get; set; }
        #region Versionable attribs
        /// <summary>
        /// Version start date
        /// </summary>        
        public DateTime? BeginLifeSpanVersion { get; set; }
        /// <summary>
        /// Version end date
        /// </summary>
        public DateTime? EndLifeSpanVersion { get; set; }
        /// <summary>
        /// Version number
        /// </summary>
        public int Version { get; set; }
        #endregion
        protected SpatialUnit()
        {
            Status = SpatialUnitStatus.New;
        }
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
