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
        /// List of Grouping objects (BusinessAuthorities - like RRR prototypes)
        /// </summary>
        public virtual ICollection<LAUnit> Properties { get; set; }
        /// <summary>
        /// Target LAUnit UIds (it's better to have attrib in LAUnit)
        /// </summary>
        /// <remarks>They are target BusinessAuthorities. It's like a target RRR prototypes</remarks>
        public string TargetPropertiesIds{ get; set; } //Target
        /// <summary>
        /// Source LAUnit UIds (it's better to have attrib in LAUnit)
        /// </summary>
        /// <remarks>It better to have it SourceBusinessAuthorities (in a way as Source RRRs)</remarks>
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
        public static IEnumerable<LAUnit> GetPartyTargetLaUnit(this Transaction transaction, Party party)
        {
            var result = new List<LAUnit>();
            if (transaction.Parties.Contains(party))
            {
                if (!string.IsNullOrEmpty(party.TargetUIDs))
                {
                    var filter = party.TargetUIDs.Split(',');
                    result.AddRange(
                        //additional check if it put into target
                        transaction.GetTransactionTargetLAUnits().Where(item => filter.Contains(item.Uid))
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
            var target = source.Where(item => ids.Contains(item.Uid));
            return target.SelectMany(laUnit => laUnit.SpatialUnits);
        }

        /// <summary>
        /// Get Only Transaction Target LAUnits
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static IEnumerable<LAUnit> GetTransactionTargetLAUnits(this Transaction transaction)
        {
            var result = new List<LAUnit>();            
            if (string.IsNullOrEmpty(transaction.TargetPropertiesIds))
                return result;            
            var ids = transaction.TargetPropertiesIds.Split(',');            
            result.AddRange(transaction.Properties.DefaultIfEmpty().ToList().Where(item => ids.Contains(item.Uid)));
            return result;
        }

        /// <summary>
        /// Get Only Transaction Source LAUnits (req for mutation)
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static IEnumerable<LAUnit> GetTransactionSourceLAUnits(this Transaction transaction)
        {
            var result = new List<LAUnit>();
            if (string.IsNullOrEmpty(transaction.SourcePropertiesIds))
                return result;
            var ids = transaction.SourcePropertiesIds.Split(',');
            result.AddRange(transaction.Properties.DefaultIfEmpty().ToList().Where(item => ids.Contains(item.Uid)));
            return result;
        }
        
        #endregion
    }
}
