using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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

        public static void EventLog(string s)
        {
#if DEBUG
            Debug.WriteLine(s);
#endif
        }

        public LadmDbContext():base("LadmDbConnectionString")
        {


            Database.Log = EventLog;

            // FIXED: remove and use Appconfig
            //Database.SetInitializer<LadmDbContext>(new CreateDatabaseIfNotExists<LadmDbContext>());
            //Database.SetInitializer<LadmDbContext>(new DropCreateDatabaseIfModelChanges<LadmDbContext>());
            //Database.SetInitializer<LadmDbContext>(new DropCreateDatabaseAlways<LadmDbContext>());           
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LAUnit>().HasMany(la => la.SpatialUnits).WithMany(s => s.LAUnitsAttending).Map(m =>
                {
                    m.MapLeftKey("LAUnitId");
                    m.MapRightKey("SpatialUnitId");
                    m.ToTable("lr_launit_spatialunits");
                });
        }
    }
}
