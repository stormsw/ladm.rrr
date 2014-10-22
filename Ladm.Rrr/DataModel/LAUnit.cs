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
        public virtual ICollection<SpatialUnit> Properties { get; set; }
        /// <summary>
        /// In case of multi RRR operations it Helps to target each concrete RRR type
        /// </summary>
        public virtual RRRMetaData RightPrototype { get; set; }
        public string UId { get; set; }
    }
}
