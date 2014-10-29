using Ladm;
using Ladm.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using Xunit;

namespace Specifications
{
    [Binding]
    public class RRRFeatureSteps : IDisposable
    {
        private static SharedContext sharedContext = SharedContext.GetInstance();

        [Given(@"Transaction No""(.*)"" has party ""(.*)"" with role ""(.*)"" associated with LAUnit ""(.*)""")]
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

        [Given(@"Transaction No""(.*)"" target parties reference property with Uid = ""(.*)""")]
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

        [Given(@"Transaction No""(.*)"" has target LAUnit ""(.*)""")]
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
                    context.LAUnit.Add(launit);
                }
                launit.Uid = uid;


                var targetlist = new List<string>();
                if (!string.IsNullOrEmpty(transaction.TargetPropertiesIds))
                {
                    targetlist = transaction.TargetPropertiesIds.Split(',').ToList();
                }

                if (!targetlist.Contains(uid))
                {
                    targetlist.Add(uid);
                    transaction.TargetPropertiesIds = string.Join(",", targetlist);
                }

                context.SaveChanges();
            }
        }

        [Given(@"Transaction No""(.*)"" has source LAUnit ""(.*)""")]
        public void GivenTransactionNo_HasSourceLAUnit(string transactionNumber, string uid)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);

                var launit = getOrCreateTransactionLaUnit(transaction, uid);

                launit.Uid = uid;

                var sourcelist = new List<string>();
                if (!string.IsNullOrEmpty(transaction.SourcePropertiesIds))
                {
                    sourcelist = transaction.SourcePropertiesIds.Split(',').ToList();
                }

                if (!sourcelist.Contains(uid))
                {
                    sourcelist.Add(uid);
                    transaction.SourcePropertiesIds = string.Join(",", sourcelist);
                }

                context.SaveChanges();
            }
        }

        [Given(@"Transaction No""(.*)"" has property with Uid = ""(.*)"" in LAUnit ""(.*)""")]
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
                                where p.SuId == suid
                                    && p.BeginLifeSpanVersion != null
                                    && p.EndLifeSpanVersion == null
                                select p).FirstOrDefault();
                    /// otherwise create new
                    if (property == null)
                    {
                        property = new Parcel() { Area = 1, SuId = suid,Status = SpatialUnit.SpatialUnitStatus.New };
                        context.SpatialUnits.Add(property);
                    }

                    if (launit.SpatialUnits == null)
                    {
                        launit.SpatialUnits = new List<SpatialUnit>();
                    }

                    launit.SpatialUnits.Add(property);
                }

                context.SaveChanges();
            }
        }

        [Then(@"Against property ""(.*)"" registered (.*) ""(.*)"" rights")]
        public void ThenAgainsPropertyRegisteredRights(string uid, int count, string rightType)
        {
            ///TODO: refator out RRRMeta to Entity classes detection
            using (var context = new LadmDbContext())
            {
                var rrrs = Ladm.DataModelHelper.GetActiveSpatialUnitInterestsByRightType(uid, rightType, context);
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
        [Obsolete("At usual hanged SpatialUnit useless!")]
        [Given(@"We have Parcel with Uid = ""(.*)""")]
        public void GivenWeHaveParcelWithUid(string uid)
        {
            using (var context = new LadmDbContext())
            {
                var parcel = (from p in context.SpatialUnits where p.SuId == uid select p).FirstOrDefault();
                if (parcel == null)
                {
                    parcel = new Parcel() { 
                        SuId = uid.ToString(), 
                        Status = SpatialUnit.SpatialUnitStatus.Normal, 
                        Area = 100, 
                        BeginLifeSpanVersion = DateTime.Now, 
                        Version = 1 };
                    context.SpatialUnits.Add(parcel);
                }
                context.SaveChanges();
            }
        }

        [Given(@"Registration transaction ""(.*)"" with No\.""(.*)"" is set current")]
        public void GivenRegistrationTransactionWithNo_IsSetCurrent(string transactionCode, string transactionNumber)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = NewTransaction(transactionCode, transactionNumber, context);
                sharedContext.TransactionsHash.Add(new KeyValuePair<string,Transaction>(transactionNumber,transaction));
                sharedContext.CurrentTransaction = transaction;
                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }

        [Given(@"Registration transaction ""(.*)"" with No\.""(.*)""")]
        public void GivenRegistrationTransactionWithNo(string transactionCode, string transactionNumber)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = NewTransaction(transactionCode, transactionNumber, context);

                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }

        private static Transaction NewTransaction(string transactionCode, string transactionNumber, LadmDbContext context)
        {
                TransactionMetaData trType = context.TransactionMetaData.Where(item => item.Code == transactionCode).Single();
                //from tmd in context.TransactionMetaData where tmd.code = transactionCode select tmd;
                var transaction = new Transaction()
                {
                    TransactionNumber = transactionNumber,
                    RegistrationDate = DateTime.Now,
                    StartDate = DateTime.Now,
                    TransactionType = trType,
                Status = Transaction.TransactionStatus.Lodged
                };
            return transaction;
            }

        [Obsolete("Dont create hanged parties!", true)]
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

        [When(@"transaction ""(.*)"" is completed")]
        public void WhenTransactionIsCompleted(string transactionNumber)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                RegistrationOperations.CompleteTransaction(transaction, context);
            }
        }

        [Then(@"Cancelled by ""(.*)"" (.*) ""(.*)"" rights")]
        public void ThenCancelledByRights(string transactionNumber, int total, string rightType)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                Assert.Equal(transaction.Status, Transaction.TransactionStatus.Completed);

                var rrr = transaction.CancelledRights(context);
                /// lets make sure only our right Type used
                foreach (var r in rrr)
                {
                    Assert.Equal(r.TypeName, rightType);
                }
                Assert.Equal(total, rrr.Count());
            }
        }

        [Given(@"Property ""(.*)"" in Transaction No""(.*)"" Status set ""(.*)""")]
        public void GivenPropertyInTransactionNo_StatusSet(string uid, string transactionNumber, string state)
        {
            using (var context = new LadmDbContext())
            {
                var transaction = (from t in context.Transactions where t.TransactionNumber == transactionNumber select t).FirstOrDefault();
                Assert.NotNull(transaction);
                Assert.NotNull(transaction.Properties);
                //single will throw ex if != 1 found
                var property = transaction.Properties.SelectMany(la => la.SpatialUnits).Single(su => su.SuId == uid);
                property.Status = SpatialUnit.SpatialUnitStatus.Archived;
                context.SaveChanges();
            }
        }

        [Then(@"Property ""(.*)"" is archived")]
        public void ThenPropertyIsArchived(string p0)
        {
            using (var context = new LadmDbContext())
            {
                var property = (from su in context.SpatialUnits where su.SuId == p0 && su.Status == SpatialUnit.SpatialUnitStatus.Archived select su).Single();
                Assert.NotNull(property);
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
                //it have to alter existing versions if any
                //var ex = from la in context.LAUnits where la.uid=uid && la.Beginlifespanversion!=null && la.endlifespanversion==null
                //ex.tolist().foreach(la=>la.EndLifespanVersion = DateTime.now)
                result = new LAUnit() {Uid=uid, SpatialUnits = new List<SpatialUnit>(), BeginLifeSpanVersion=DateTime.Now,Version=-1 };
                transaction.Properties.Add(result);
            }

            return result;
        }

        public void Dispose()
        {
            sharedContext.Dispose();
        }

        #endregion Technical magics
    }
}