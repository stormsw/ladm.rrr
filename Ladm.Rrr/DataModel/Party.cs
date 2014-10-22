using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    /// <summary>
    /// This is Party prototype to keep it simple (real one has references to Individuals/Organizations etc)
    /// </summary>
    [Table("lr_party")]
    public class Party
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        /// <summary>
        /// Map to SpatialUnit uids
        /// </summary>
        public string TargetSUIds { get; set; }
        /// <summary>
        /// Map to LAUnits
        /// </summary>
        public string TargetUIDs { get; set; }
    }
}
