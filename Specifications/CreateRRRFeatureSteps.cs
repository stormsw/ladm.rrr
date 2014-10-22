using Ladm.DataModel;
using System;
using TechTalk.SpecFlow;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using Ladm;

namespace Specifications
{
    [Binding]
    public class CreateRRRFeatureSteps:IDisposable
    {
        static SharedContext sharedContext = new SharedContext();

        [Given(@"Transaction No\.""(.*)"" has party ""(.*)"" with role ""(.*)""")]
        public void GivenTransactionNo_HasPartyWithRole(string transactionNumber, string fullName, string partyRole)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                
                var trparty = transaction.Parties.Where(party=>party.FullName==fullName).FirstOrDefault();

                if (trparty==null)
                {
                    Party newParty = new Party()
                    {
                        FullName = fullName,
                        Role = partyRole
                    };
                    transaction.Parties.Add(newParty);
                } else 
                {
                    trparty.Role = partyRole;
                }

                context.SaveChanges();    
            }
        }

        [Given(@"Transaction No\.""(.*)"" target parties reference property with Uid = ""(.*)""")]
        public void GivenAllTargetPartiesReferencePropertyWithUid(string transactionNumber, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                var lau = transaction.Properties.Where(la => la.Properties.ToList().Exists(su => su.SuId == uid)).FirstOrDefault();
                Assert.NotNull(lau);
                transaction.Parties.ToList().ForEach(p => p.TargetUIDs = lau.UId);
                context.SaveChanges();
            }
        }


        [Given(@"Transaction No\.""(.*)"" has target property with Uid = ""(.*)""")]
        public void GivenTransactionNo_HasTargetPropertyWithUid(string transactionNumber, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);

                var property = transaction.Properties.SelectMany(item => item.Properties).Where(su => su.SuId == uid).FirstOrDefault();
                /// wether already added
                if (property == null)
                {
                    /// wether exists in db
                    /// 
                    property = (from p in context.SpatialUnits where p.SuId==uid select p).FirstOrDefault();
                    if (property == null)
                    {
                        property = new Parcel()
                        {
                            Area = 1,
                            SuId = uid
                        };
                    }
                    transaction.Properties.Add(new LAUnit() { UId = "bbb", Properties = new List<SpatialUnit>() { property } });
                    transaction.TargetPropertiesIds = "bbb";
                }

                context.SaveChanges();
            }                
        }

        [Then(@"Agains property ""(.*)"" registered (.*) ""(.*)"" rights")]
        public void ThenAgainsPropertyRegisteredRights(string uid, int count, string rightType)
        {
            ///TODO: refator out RRRMeta to Entity classes detection
            using (var context = new LadmDbContext())
            {
                var rrrs = ActiveRRRsOnSpatialUnitsByUidAndRightType(uid,rightType, context);                
                Assert.Equal(count, rrrs.Count());
            }
        }

        public static IEnumerable<RRR> FilterRRRsByRight(IEnumerable<RRR> rrrs, string rightType)
        {
            return rrrs.Where(r => r.TypeName == rightType);
        }
        /// <summary>
        /// This is not DRY but get filtered res on DS side
        /// Alternatively we may populate data from ActiveRRRsOnSpatialUnitsByUid and enumerate it, then add this filter on local data
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="rightType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static IEnumerable<RRR> ActiveRRRsOnSpatialUnitsByUidAndRightType(string uid, string rightType, LadmDbContext context)
        {
            var rrrs = (
                from r in context.RRRs
                where r.BeginLifeSpan != null
                    && (r.EndDate <= DateTime.Now || r.EndDate == null)
                && r.LAUnit.Properties.Where(item => item.SuId == uid).Count() > 0
                && r.TypeName  == rightType
                select r);
            return rrrs;
        }

        private static IEnumerable<RRR> ActiveRRRsOnSpatialUnitsByUid(string uid, LadmDbContext context)
        {
            var rrrs = (
                from r in context.RRRs
                where r.BeginLifeSpan != null
                    && (r.EndDate <= DateTime.Now || r.EndDate == null)
                && r.LAUnit.Properties.Where(item => item.SuId == uid).Count() > 0
                select r);
            return rrrs;
        }

        [Then(@"Party ""(.*)"" have active ""(.*)"" rights on ""(.*)""")]
        public void ThenPartyHaveActiveRightsOn(string fullName, string rightType, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var result = ActiveRRRsOnSpatialUnitsByUidAndRightType(uid,rightType, context).ToList().
                    Where(r => r.Party.FullName == fullName).Count();
                Assert.True(result > 0);
            }
        }


        [Given(@"We have Parcel with Uid = ""(.*)""")]
        public void GivenWeHaveParcelWithUid(string uid)
        {
            using (var context = new LadmDbContext())
            {

                var parcel = new Parcel() { SuId = uid.ToString() };
                context.SpatialUnits.Add(parcel);
                context.SaveChanges();
            }
        }

        [Given(@"Registration transaction ""(.*)"" with No\.""(.*)""")]
        public void GivenRegistrationTransactionWithNo(string transactionCode, string transactionNumber)
        {
            using(var context = new LadmDbContext())
            {

                TransactionMetaData trType = context.TransactionMetaData.Where(item => item.Code == transactionCode).FirstOrDefault();
                    //from tmd in context.TransactionMetaData where tmd.code = transactionCode select tmd;  
                
                var transaction = new Transaction()
                {
                    TransactionNumber = transactionNumber,
                    RegistrationDate = DateTime.Now, 
                    StartDate = DateTime.Now, 
                    TransactionType = trType,
                };

                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }

        [Given(@"Party ""(.*)"" with role ""(.*)""")]
        public void GivenPartyWithRole(string fullName, string partyRole)
        {
            using (var context = new LadmDbContext())
            {
                Party p = new Party()
                {
                    FullName = fullName,
                    Role = partyRole,
                    TargetSUIds = string.Empty,
                    TargetUIDs = string.Empty
                };

                context.Parties.Add(p);
                context.SaveChanges();
            }
        }


        [Given(@"Registration Transaction with No\.""(.*)"" for ""(.*)"" right")]
        public void GivenRegistrationTransactionWithNo_ForRight(string transactionNumber, string rightType)
        {
            using (var context = new LadmDbContext())
            {

                var query = (from t in context.TransactionMetaData 
                            where (
                                t.RightType==rightType 
                                && t.Meta==TransactionMetaData.MetaCode.Registration 
                                && t.Action == TransactionMetaData.ActionCode.Create
                           ) select t).FirstOrDefault();
                
                var tm = context.TransactionMetaData.Where(item => item.Action == TransactionMetaData.ActionCode.Create
                    && item.Meta == TransactionMetaData.MetaCode.Registration
                    && item.RightType == rightType).FirstOrDefault();

                Assert.NotNull(tm);

                var transaction = new Transaction()
                {
                    Status=Transaction.TransactionStatus.Lodged,
                    StartDate = DateTime.Now,
                    TransactionNumber = transactionNumber,
                    TransactionType = tm
                };

                context.Transactions.Add(transaction);

                context.SaveChanges();
            }
        }

        [When(@"transaction ""(.*)"" is completed")]
        public void WhenTransactionIsCompleted(string transactionNumber)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                RegistrationOperations.CompleteTransaction(transaction,context);
            }
        }

        #region Technical magics
        public void Dispose()
        {
            sharedContext.Dispose();
        }
        #endregion
    }
}