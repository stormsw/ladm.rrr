using Ladm.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specifications
{
    public class SharedContext:IDisposable
    {
        LadmDbContext dbContext = new LadmDbContext();

        public LadmDbContext DbContext
        {
            get { return dbContext; }
            set { dbContext = value; }
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
