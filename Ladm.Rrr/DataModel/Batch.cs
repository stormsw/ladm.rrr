using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    [Table("lr_batch")]
    public class Batch
    {
        //TODO: required for example with tech transactions complete
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
