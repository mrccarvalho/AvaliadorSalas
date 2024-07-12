using AvaliadorSalas.Data;
using AvaliadorSalas.Models;

namespace AvaliadorSalas.Migrations
{
    public class InitializeDatabase
    {
        public static void SeedData(IApplicationBuilder app)
        {
            using (var serviceScope =
                app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;

                using (var db = serviceProvider.GetService<ArduinoDbContext>())
                {

                    SeedSensores(db);

                }
            }
        }

       

        private static void SeedSensores(ArduinoDbContext db)
        {
            if (!db.Sensores.Any())
            {
                db.Sensores.Add(new Sensor
                {
                    SensorId = 1,
                    Nome = "Temperatura",
                    Descricao = "Temperature em Farhenheit"
                });
                db.Sensores.Add(new Sensor
                {
                    SensorId = 2,
                    Nome = "Humidade",
                    Descricao = "Humidade(percentagem)"
                });
                db.Sensores.Add(new Sensor
                {
                    SensorId = 3,
                    Nome = "Co2",
                    Descricao = "Co2"
                });
                db.SaveChanges();
            }
        }

      
        }
}
