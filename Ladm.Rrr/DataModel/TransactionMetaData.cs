using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    [Table("lr_transaction_meta")]
    public class TransactionMetaData
    {
        public enum MetaCode
        {
            Registration, Documentation, Technical
        }

        public enum ActionCode
        {
            Create, Alter, Cancell
        }
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public MetaCode Meta { get; set; }
        public ActionCode Action { get; set; }
        public string TargetPartyRole { get; set; }
        public string RightType { get; set; }
    }
}
