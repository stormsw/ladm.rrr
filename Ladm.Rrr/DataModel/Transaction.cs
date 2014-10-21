using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    [Table("lr_transaction")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public enum TransactionStatus
        { Lodged, Completed, Withdrawed}

        public string TransactionNumber { get; set; }
        public TransactionStatus Status { get; set; }
        /// <summary>
        /// Addittional non class aspect
        /// </summary>
        public virtual TransactionMetaData TransactionType { get; set; }
        /// <summary>
        /// Collection of Parties whatever role they have
        /// </summary>
        public virtual ICollection<Party> Parties { get; set; }
        /// <summary>
        /// List of Grouping objects
        /// </summary>
        public virtual ICollection<LAUnit> Properties { get; set; }
        /// <summary>
        /// Target LAUnit UIds
        /// </summary>
        public string TargetPropertiesIds{ get; set; }
        /// <summary>
        /// Source LAUnit UIds
        /// </summary>
        public string SourcePropertiesIds { get; set; }

        public DateTime? ExpirationDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    /// <summary>
    /// Helpers extends Transaction objects
    /// </summary>
    public static class TransactionExtension
    {
        public static List<LAUnit> GetPartyTargetLaUnit(this Transaction transaction, Party party)
        {
            var result = new List<LAUnit>();
            if (transaction.Parties.Contains(party))
            {
                if (!string.IsNullOrEmpty(party.TargetUIDs))
                {
                    var filter = party.TargetUIDs.Split(',');
                    result.AddRange(
                        transaction.Properties.Where(item => filter.Contains(item.UId))
                    );
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Specify party from this transaction");
            }

            return result;
        }
        #region Transaction->SpatialUnit operations
        /// <summary>
        /// Get filtered SU by SourcePropertiesIds attribute of transaction
        /// </summary>
        /// <param name="transaction">Transaction instance</param>
        /// <returns></returns>
        public static List<SpatialUnit> GetSourceProperties(this Transaction transaction)
        {
            var result = new List<SpatialUnit>();
            result.AddRange(FilterPropertiesByIds(transaction.Properties, transaction.SourcePropertiesIds));
            return result;
        }
        /// <summary>
        /// Get filtered SU by TargetPropertiesIds attribute of transaction
        /// </summary>
        /// <param name="transaction">Transaction instance</param>
        /// <returns></returns>
        public static List<SpatialUnit> GetTargetProperties(this Transaction transaction)
        {
            var result = new List<SpatialUnit>();
            result.AddRange(
                FilterPropertiesByIds(transaction.Properties,transaction.TargetPropertiesIds)
            );
            return result;
        }
        /// <summary>
        /// Get SU filtered by CSV list
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static IEnumerable<SpatialUnit> FilterPropertiesByIds(IEnumerable<LAUnit> source, string filter)
        {
            var ids = filter.Split(',');
            var target = source.Where(item => ids.Contains(item.UId));
            return target.SelectMany(laUnit => laUnit.Properties);
        }
        #endregion
    }
}
