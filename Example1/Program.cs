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
            using (var ctx = new Ladm.DataModel.LadmDbContext())
            {
                /*Transaction transaction = new Transaction() { Id = 1, TransactionType = new TransactionMetaData() { Id = 1 } };
                ctx.Transactions.Add(transaction);
                ctx.SaveChanges();*/
            }
        }
    }
}
