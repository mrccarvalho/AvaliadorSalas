using AvaliadorSalas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace AvaliadorSalas.Data
{
    public class ArduinoDbContext : DbContext
    {
        public ArduinoDbContext(DbContextOptions<ArduinoDbContext> options) : base(options)
        {
        }

        public DbSet<Sensor> Sensores { get; set; }
        public DbSet<Leitura> Medicoes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Sensor>(d =>
            { d.Property(e => e.SensorId).ValueGeneratedNever(); });


            modelBuilder.Entity<Leitura>(d =>
            { d.Property(e => e.LeituraId).ValueGeneratedOnAdd(); });

            modelBuilder.Entity<Leitura>()
                .HasOne(d => d.Sensor)
                .WithMany(m => m.Leituras)
                .OnDelete(DeleteBehavior.Restrict);

        }

     
    }
}
