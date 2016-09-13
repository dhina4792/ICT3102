using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using StowagePlanAnalytics_ITP_2016.Models;

namespace StowagePlanAnalytics_ITP_2016.DAL
{
    public class StowageDBContext : DbContext
    {
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<Port> Port { get; set; }
        public virtual DbSet<Vessel> Vessel { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Voyage> Voyage { get; set; }
        public virtual DbSet<UsefulInfo> UsefulInfo { get; set; }
        public virtual DbSet<Models.FileModel.UploadedFile> Files { get; set; }

        public StowageDBContext() 
            : base("StowageDB")
        {
        }

        public StowageDBContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Remove plural naming convention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Map Entities to database table name
            modelBuilder.Entity<Service>().ToTable("Service");
            modelBuilder.Entity<Port>().ToTable("Port");
            modelBuilder.Entity<Vessel>().ToTable("Vessel");
            modelBuilder.Entity<Class>().ToTable("Class");
            modelBuilder.Entity<Voyage>().ToTable("Voyage");
            modelBuilder.Entity<UsefulInfo>().ToTable("UsefulInfo");
            modelBuilder.Entity<Models.FileModel.UploadedFile>().ToTable("Files");
        }
    }
}