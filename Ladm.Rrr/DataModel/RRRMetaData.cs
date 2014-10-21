using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    [Table("lr_rrr_metadata")]
    public class RRRMetaData
    {
        [Key]
        public int Id { get; set; }
        public string RightType { get; set; }

    }
}
