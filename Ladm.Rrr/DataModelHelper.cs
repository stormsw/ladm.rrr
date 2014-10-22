using Ladm.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm
{
    public class DataModelHelper
    {        
        /// <summary>
        /// This method returns active registered RRR object agianst spatial unid filtered by specified right type
        /// This is not DRY but get filtered res on DS side
        /// Alternatively we may populate data from GetAllActiveSpatialUnitsInterests and enumerate it, then add this filter on local data
        /// </summary>
        /// <param name="uid">SpatialUnit UID</param>
        /// <param name="rightType"></param>
        /// <param name="context">Instance of Db Context</param>
        /// <returns></returns>
        public static IEnumerable<RRR> GetActiveSpatialUnitInterestsByRightType(string uid, string rightType, LadmDbContext context)
        {
            var rrrs = (
                from r in context.RRRs
                where r.BeginLifeSpan != null
                    && (r.EndDate <= DateTime.Now || r.EndDate == null)
                && r.LAUnit.Properties.Where(item => item.SuId == uid).Count() > 0
                && r.TypeName == rightType
                select r);
            return rrrs;
        }
        
        public static IEnumerable<RRR> GetAllActiveSpatialUnitsInterests(string uid, LadmDbContext context)
        {
            var rrrs = (
                from r in context.RRRs
                where r.BeginLifeSpan != null
                    && (r.EndDate <= DateTime.Now || r.EndDate == null)
                && r.LAUnit.Properties.Where(item => item.SuId == uid).Count() > 0
                select r);
            return rrrs;
        }
        /// <summary>
        /// Additional filter, make sure source dataset from EF is enumerated
        /// </summary>
        /// <param name="rrrs"></param>
        /// <param name="rightType"></param>
        /// <returns></returns>
        public static IEnumerable<RRR> FilterInterestsByRight(IEnumerable<RRR> rrrs, string rightType)
        {
            /// dont like to have there rrrs.ToList()
            return rrrs.Where(r => r.TypeName == rightType);
        }
    }
}
