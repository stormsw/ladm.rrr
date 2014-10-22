using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    [Table("lr_rrr")]
    public abstract class RRR
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Additional aspect
        /// </summary>
        [Obsolete("RRR concrete class have to be enouh!")]
        public virtual RRRMetaData MetaType { get; set; }
        [Obsolete("Temporary while class selectors been implemented")]
        public string TypeName { get; set; }
        /// <summary>
        /// Container for properties
        /// </summary>
        public virtual LAUnit LAUnit { get; set; }
        /// <summary>
        /// Party as Right Holder
        /// </summary>
        public virtual Party Party { get; set; }
        /// <summary>
        /// Transaction created RRR
        /// </summary>
        public virtual Transaction CreatedBy { get; set; }
        /// <summary>
        /// Helper to address first created transaction
        /// </summary>
        public virtual Transaction Origin { get; set; }
        /// <summary>
        /// Cancellation info
        /// </summary>
        public virtual Transaction CancelledBy { get; set; }
        /// <summary>
        /// RRR Valid from
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// RRR Valid till
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Exoirable
        /// </summary>
        public bool CanExpire { get; set; }
        /// <summary>
        /// For expirable
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// Version start date
        /// </summary>
        public DateTime? BeginLifeSpan { get; set; }
        /// <summary>
        /// Version end date
        /// </summary>
        public DateTime? EndLifeSpan { get; set; }
        /// <summary>
        /// Version number
        /// </summary>
        public int Version { get; set; }

    }

    public abstract class Right:RRR
    {
        public int Numerator { get; set; }
        public int Denominator { get; set; }
    }

    public abstract class Restriction:RRR
    {

    }

    public abstract class Responsibility:RRR
    {

    }

#region Configuration generation part
    public class Occupancy:Right
    {
        public Occupancy():base()
        {
            TypeName = "Occupancy";
        }
    }

    public class Mortgage:Right
    {
        public Mortgage():base()
        {
            TypeName = "Mortgage";
        }

    }

    public class Lease:Right
    {
        public Lease():base()
        {
            TypeName = "Lease";
        }
    }

    public class Consent:Responsibility
    {
        public Consent():base()
        {
            TypeName = "Consent";
        }
    }

    public class Caveat:Restriction
    {
        public Caveat()
            : base()
        {
            TypeName = "Caveat";
        }
    }
#endregion
}
