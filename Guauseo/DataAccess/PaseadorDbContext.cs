using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guauseo.Models;

namespace Guauseo.DataAccess
{
    public class PaseadorDbContext:DbContext
    {

        public DbSet<PaseadorModel> Paseadores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conexionDB = "Server=server-guauseo.database.windows.net;Database=Guauseo;User Id=andress;Password=Pecoso7.";

            optionsBuilder.UseSqlServer(conexionDB);

        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaseadorModel>().HasKey(c => c.Codigo);
        modelBuilder.Entity<PaseadorModel>().Property(c => c.Codigo).ValueGeneratedOnAdd();
        
    }

}
}
