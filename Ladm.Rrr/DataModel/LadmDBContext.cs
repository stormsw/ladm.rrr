using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladm.DataModel
{
    public class LadmDbContext:DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionMetaData> TransactionMetaData { get; set; }
        public DbSet<SpatialUnit> SpatialUnits { get; set; }
        public DbSet<LAUnit> LAUnit { get; set; }
        public DbSet<RRRMetaData> RRRMetaData { get; set; }
        public DbSet<RRR> RRRs { get; set; }
        public DbSet<Party> Parties { get; set; }

        public LadmDbContext():base("LadmDbConnectionString")
        {
            // TODO: remove and use Appconfig
            //Database.SetInitializer<LadmDbContext>(new CreateDatabaseIfNotExists<LadmDbContext>());
            //Database.SetInitializer<LadmDbContext>(new DropCreateDatabaseIfModelChanges<LadmDbContext>());
            //Database.SetInitializer<LadmDbContext>(new DropCreateDatabaseAlways<LadmDbContext>());           
        }
    }
}
