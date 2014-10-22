using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    [Table("lr_launit")]
    public class LAUnit
    {
        [Key]
        public int Id { get; set; }
        public virtual ICollection<SpatialUnit> SpatialUnits { get; set; }
        /// <summary>
        /// In case of multi RRR operations it Helps to target each concrete RRR type
        /// </summary>
        public virtual RRRMetaData RightPrototype { get; set; }
        /// <summary>
        /// This attribute supposed to be unique for different RightType (like Right-Number())
        /// </summary>
        public string Uid { get; set; }

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
    }
}