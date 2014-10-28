using Ladm.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Specifications
{
    public class SharedContext:IDisposable
    {
        private static SharedContext instance;
        private LadmDbContext context;
        /// <summary>
        /// string - TransactionNumber, thread safe access
        /// </summary>
        private ICollection<KeyValuePair<string,Transaction>> hashedTransactions;

        private SharedContext()
        {
            // looks like it program for localDB to have opened connections
            //context = new LadmDbContext();            
            hashedTransactions = new ConcurrentDictionary<string,Transaction>();
        }

        public static SharedContext GetInstance()
        {
            instance = new SharedContext();
            return instance;
        }

         LadmDbContext DbContext
        {
            get { return context; }
            set { context = value; }
        }
        /// <summary>
        /// Helper too Keep current transaction
        /// </summary>
        public Transaction CurrentTransaction { get; set; }
        /// <summary>
        /// permanent storage for affected in scenarion transactions
        /// </summary>
        public ICollection<KeyValuePair<string,Transaction>> TransactionsHash {
            get
            {
                return hashedTransactions;
            }
        }

        public void Dispose()
        {
            //context.Dispose();
        }
    }
}

