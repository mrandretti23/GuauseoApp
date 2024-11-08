using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guauseo.Models;

namespace Guauseo.DataAccess
{
    public class PaseoDbContext : DbContext
    {
        public DbSet<PaseoModel> Paseo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=server-guauseo.database.windows.net;Database=Guauseo;User Id=andress;Password=Pecoso7.");
            string conexionDB = "Server=server-guauseo.database.windows.net;Database=Guauseo;User Id=andress;Password=Pecoso7.";

            optionsBuilder.UseSqlServer(conexionDB);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaseoModel>().HasKey(c => c.Codigo);
            modelBuilder.Entity<PaseoModel>().Property(c => c.Codigo).ValueGeneratedOnAdd();
        }
    }
}
