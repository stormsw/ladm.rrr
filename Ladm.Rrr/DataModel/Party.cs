using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    public class Party
    {
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
