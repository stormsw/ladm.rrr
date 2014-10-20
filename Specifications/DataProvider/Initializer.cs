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
                IList<Transaction> transactions = new List<Transaction>();
                transactions.Add(new Transaction() {TransactionNumber="TRN-000001", SourcePropertiesIds = string.Empty, TargetPropertiesIds=string.Empty,Status=Transaction.TransactionStatus.Lodged });
                context.Transactions.AddRange(transactions);
                context.SaveChanges();

                base.Seed(context);
            }
        }
}
