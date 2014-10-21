using Ladm.DataModel;
using System;
using TechTalk.SpecFlow;
using System.Linq;

namespace Specifications
{
    [Binding]
    public class CreateRRRFeatureSteps:IDisposable
    {
        LadmDbContext dbContext = new LadmDbContext();

        [Given(@"We have Parcel with Uid = (.*)")]
        public void GivenWeHaveParcelWithUid(int p0)
        {        
            var parcel = new Parcel() { SuId = p0.ToString() };
            dbContext.SpatialUnits.Add(parcel);
            dbContext.SaveChanges();
        }

        [Given(@"Registration transaction ""(.*)"" with No\.""(.*)""")]
        public void GivenRegistrationTransactionWithNo_(string transactionCode, string transactionNumber)
        {
            using(var context = new LadmDbContext())
            {

                TransactionMetaData trType = context.TransactionMetaData.Where(item => item.Code == transactionCode).FirstOrDefault();
                    //from tmd in context.TransactionMetaData where tmd.code = transactionCode select tmd;  

                
                var transaction = new Transaction()
                {
                    RegistrationDate = DateTime.Now, 
                    StartDate = DateTime.Now, 
                    TransactionType = trType,
                };
            }
        }

        [Given(@"Party ""(.*)"" with id=(.*)")]
        public void GivenPartyWithId(string p0, int p1)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"Registration Transaction with No\.""(.*)"" for ""(.*)"" right")]
        public void GivenRegistrationTransactionWithNo_ForRight(string p0, string p1)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have add target Party with id\.(\d+) to transaction ""(.*)""")]
        public void GivenIHaveAddTargetPartyWithId_ToTransaction(int p0, string p1)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have add target Property with uid\.""(.*)"" to transaction ""(.*)""")]
        public void GivenIHaveAddTargetPropertyWithUid_ToTransaction(int p0, string p1)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"transaction ""(.*)"" is completed")]
        public void WhenTransactionIsCompleted(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"""(.*)"" ""(.*)"" RRR produced")]
        public void ThenRRRProduced(int p0, string p1)
        {
            ScenarioContext.Current.Pending();
        }
        #region Technical magics
        public void Dispose()
        {
            dbContext.Dispose();
        }
        #endregion
    }
}