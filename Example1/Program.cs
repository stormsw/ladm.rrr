using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladm.DataModel;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new Ladm.DataModel.LadmDbContext())
            {
                Transaction transaction = new Transaction() { TransactionNumber = "TRN-0001", TransactionType = new TransactionMetaData() { Code="TRN" } };
                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }
    }
}
