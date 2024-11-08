using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guauseo.Models;

namespace Guauseo.DataAccess
{
    public class DueñoDbContext : DbContext
    {

        public DbSet<DueñoModel> Dueños { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=server-guauseo.database.windows.net;Database=Guauseo;User Id=andress;Password=Pecoso7.");
            string conexionDB = "Server=server-guauseo.database.windows.net;Database=Guauseo;User Id=andress;Password=Pecoso7.";

            optionsBuilder.UseSqlServer(conexionDB);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
              modelBuilder.Entity<DueñoModel>().HasKey(c => c.Codigo);
              modelBuilder.Entity<DueñoModel>().Property(c => c.Codigo).ValueGeneratedOnAdd();
        //    //modelBuilder.Entity<DueñoModel>().ToTable("cliente_cab");
        //    modelBuilder.Entity<DueñoModel>(entity => { entity.HasKey(col => col.cli_codigo); });
        }

    }
}
