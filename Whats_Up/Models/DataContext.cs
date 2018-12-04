﻿using System;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}