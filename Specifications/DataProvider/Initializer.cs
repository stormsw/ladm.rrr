using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladm.DataModel;
using System.Data.Entity;


namespace Specifications.DataProvider
{
    /// <summary>
    /// For test fixtures we need to revert DB and prefill with test data
    /// </summary>
        public class Initializer : DropCreateDatabaseAlways<LadmDbContext>
        {
            protected override void Seed(LadmDbContext context)
            {
                //sample data
                IList<RRRMetaData> rmd = new List<RRRMetaData>()
                {
                    new RRRMetaData()
                    {
                        RightType = "Occupancy"
                    },
                    new RRRMetaData()
                    {
                        RightType = "Mortgage"
                    },
                    new RRRMetaData()
                    {
                        RightType = "Lease"
                    },
                    new RRRMetaData()
                    {
                        RightType = "Caveat"
                    },
                    new RRRMetaData()
                    {
                        RightType = "Attorney"
                    }
                };

                IList<TransactionMetaData> tmd = new List<TransactionMetaData>()
                {
                    /// Occupancy
                    new TransactionMetaData(){
                        RightType = "Occupancy",
                        Action = TransactionMetaData.ActionCode.Create,
                        Meta = TransactionMetaData.MetaCode.Registration,
                        Code = "REGO",
                        Label = "Register Occupancy",
                        TargetPartyRole = "Grantee",                        
                    },

                    new TransactionMetaData(){
                        RightType = "Occupancy",
                        Action = TransactionMetaData.ActionCode.Alter,
                        Meta = TransactionMetaData.MetaCode.Registration,
                        Code = "ALTO",
                        Label = "Alter Occupancy",
                        TargetPartyRole = "Grantee"                       
                    },

                    new TransactionMetaData(){
                        RightType = "Occupancy",
                        Action = TransactionMetaData.ActionCode.Cancell,
                        Meta = TransactionMetaData.MetaCode.Registration,
                        Code = "CANO",
                        Label = "Cancel Occupancy",
                        TargetPartyRole = null
                    },
                    /// Mortgage
                    new TransactionMetaData(){
                        RightType = "Mortgage",
                        Action = TransactionMetaData.ActionCode.Create,
                        Meta = TransactionMetaData.MetaCode.Registration,
                        Code = "REGM",
                        Label = "Register Mortgage",
                        TargetPartyRole = "Mortgagee"                       
                    },

                    new TransactionMetaData(){
                        RightType = "Mortgage",
                        Action = TransactionMetaData.ActionCode.Alter,
                        Meta = TransactionMetaData.MetaCode.Registration,
                        Code = "ALTM",
                        Label = "Alter Mortgage",
                        TargetPartyRole = "Mortgagee"                       
                    },

                    new TransactionMetaData(){
                        RightType = "Mortgage",
                        Action = TransactionMetaData.ActionCode.Cancell,
                        Meta = TransactionMetaData.MetaCode.Registration,
                        Code = "CANM",
                        Label = "Cancel Mortgage",
                        TargetPartyRole = null
                    },
                };

                context.TransactionMetaData.AddRange(tmd);
                context.SaveChanges();

                base.Seed(context);
            }
        }
}
