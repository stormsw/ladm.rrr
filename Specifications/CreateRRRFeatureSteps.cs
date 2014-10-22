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

        [Given(@"Transaction No\.""(.*)"" has party ""(.*)"" with role ""(.*)"" associated with LAUnit ""(.*)""")]
        public void GivenTransactionNo_HasPartyWithRole(string transactionNumber, string fullName, string partyRole, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                var party = getOrCreateTransactionParty(transaction, fullName);                
                party.Role = partyRole;
                /// we may check wheter such LAUnit exists, but this will not define parties before properties in test
                party.TargetUIDs = uid;
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
                var lau = transaction.Properties.Where(la => la.SpatialUnits.ToList().Exists(su => su.SuId == uid)).FirstOrDefault();
                Assert.NotNull(lau);
                transaction.Parties.ToList().ForEach(p => p.TargetUIDs = lau.Uid);
                context.SaveChanges();
            }
        }
        [Given(@"Transaction No\.""(.*)"" has target LAUnit ""(.*)""")]
        public void GivenTransactionNo_HasTargetLAUnit(string transactionNumber, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);

                var launit = transaction.Properties.Where(la => la.Uid == uid).FirstOrDefault();
                if (launit == null)
                {
                    launit = new LAUnit() { SpatialUnits = new List<SpatialUnit>() };
                    transaction.Properties.Add(launit);
                }
                launit.Uid = uid;

                var targetlist = (transaction.TargetPropertiesIds ?? string.Empty).Split(',').ToList();
                if (!targetlist.Contains(uid))
                {
                    targetlist.Add(uid);
                    transaction.TargetPropertiesIds = string.Join(",", targetlist);
                }

                context.SaveChanges();
            }
        }

        [Given(@"Transaction No\.""(.*)"" has source LAUnit ""(.*)""")]
        public void GivenTransactionNo_HasSourceLAUnit(string transactionNumber, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);

                var launit = getOrCreateTransactionLaUnit(transaction,uid );

                launit.Uid = uid;

                var sourcelist = (transaction.SourcePropertiesIds?? string.Empty).Split(',').ToList();
                if (!sourcelist.Contains(uid))
                {
                    sourcelist.Add(uid);
                    transaction.SourcePropertiesIds = string.Join(",", sourcelist);
                }

                context.SaveChanges();
            }
        }
        [Given(@"Transaction No\.""(.*)"" has property with Uid = ""(.*)"" in LAUnit ""(.*)""")]
        public void GivenTransactionNo_HasTargetPropertyWithUid(string transactionNumber, string suid, string la_uid)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                var launit = getOrCreateTransactionLaUnit(transaction, la_uid);
                launit.Uid = la_uid;
                /// whether it already added
                /// we check need only against this particular LAUnit
                if (launit.SpatialUnits == null)
                {
                    launit.SpatialUnits = new List<SpatialUnit>();
                }
                var property = launit.SpatialUnits.Where(su => su.SuId == suid).FirstOrDefault();
                if (property == null)
                {
                    /// wether exists in db
                    /// get latest from db
                    property = (from p in context.SpatialUnits 
                                    where p.SuId==suid 
                                        && p.BeginLifeSpanVersion!=null 
                                        && p.EndLifeSpanVersion==null 
                                select p).FirstOrDefault() 
                                ?? new Parcel() {Area = 1,SuId = suid};

                    if (launit.SpatialUnits == null)
                    {
                        launit.SpatialUnits = new List<SpatialUnit>();
                    }

                    launit.SpatialUnits.Add(property);
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
                var rrrs = Ladm.DataModelHelper.GetActiveSpatialUnitInterestsByRightType(uid,rightType, context);                
                Assert.Equal(count, rrrs.Count());
            }
        }

        [Then(@"Party ""(.*)"" have active ""(.*)"" rights on ""(.*)""")]
        public void ThenPartyHaveActiveRightsOn(string fullName, string rightType, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var result = Ladm.DataModelHelper.GetActiveSpatialUnitInterestsByRightType(uid, rightType, context).ToList().
                    Where(r => r.Party.FullName == fullName).Count();
                Assert.True(result > 0);
            }
        }


        [Given(@"We have Parcel with Uid = ""(.*)""")]
        public void GivenWeHaveParcelWithUid(string uid)
        {
            using (var context = new LadmDbContext())
            {
                var parcel = (from p in context.SpatialUnits where p.SuId==uid select p).FirstOrDefault();
                if (parcel == null)
                {
                    parcel = new Parcel() { SuId = uid.ToString() };
                    context.SpatialUnits.Add(parcel);
                }
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
        [Obsolete("Dont create hanged parties!",true)]
        [Given(@"Party ""(.*)"" with role ""(.*)""")]
        public void GivenPartyWithRole(string fullName, string partyRole)
        {
            throw new MissingMethodException("REFACTOR!");

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
        /// <summary>
        /// Get or create transaction Party by FullName
        /// </summary>
        /// <param name="transaction">Instance of Transaction</param>
        /// <param name="fullName">Party.FullName</param>
        /// <returns></returns>
        private static Party getOrCreateTransactionParty(Transaction transaction, string fullName)
        {
            var result = transaction.Parties.Where(party => party.FullName == fullName).FirstOrDefault();

            if (result == null)
            {
                Party newParty = new Party()
                {
                    FullName = fullName,
                };
                transaction.Parties.Add(newParty);
                result = newParty;
            }
            return result;
        }
        /// <summary>
        /// Get or Create transaction LAUnit with given UID
        /// </summary>
        /// <param name="transaction">Instance of Transaction</param>
        /// <param name="uid">LAUnit.Uid</param>
        /// <returns></returns>
        private static LAUnit getOrCreateTransactionLaUnit(Transaction transaction, string uid)
        {
            var result = transaction.Properties.Where(laUnit => laUnit.Uid == uid).FirstOrDefault();
            if (result == null)
            {
                result = new LAUnit() { SpatialUnits = new List<SpatialUnit>() };
                transaction.Properties.Add(result);
            }

            return result;
        }

        public void Dispose()
        {
            sharedContext.Dispose();
        }
        #endregion
    }
}