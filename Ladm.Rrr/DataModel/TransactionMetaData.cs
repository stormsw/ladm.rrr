using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
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

        public string Code { get; set; }
        public string Label { get; set; }
        public MetaCode Meta { get; set; }
        public ActionCode Action { get; set; }
        public string TargetPartyRole { get; set; }
        public string RightType { get; set; }
    }
}
