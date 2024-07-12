using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace AvaliadorSalas.Models
{
    public class Sensor
    {
        [Required]
        public int SensorId { get; set; }
        [Required]
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public virtual ICollection<Leitura> Leituras { get; set; }
    }

    public enum TipoLeituraEnum
    {
        Temperatura = 1,
        Humidade = 2,
        Co2 = 3
    }
}
