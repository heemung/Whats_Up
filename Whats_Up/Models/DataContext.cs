using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Whats_Up.Models
{
    public class DataContext : DbContext
    {
        public DataContext() : base("Whats_Up")
        {
        }

        public DbSet<SatelliteN2YO> SatelliteN2YOs { get; set; }
        //public DbSet<User> Users { get; set; }
        //public DbSet<Favorite> Favorites {get; set;}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //fluent API
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<SatelliteN2YO>().ToTable("SatelliteN2YOs");
            //modelBuilder.Entity<User>().ToTable("Users");
            //modelBuilder.Entity<Favorite>().ToTable("Favorites");


        }

    }
}