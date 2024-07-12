using System.ComponentModel.DataAnnotations;

namespace AvaliadorSalas.Models
{
    public class Leitura
    {
        [Required]
        public int LeituraId { get; set; }

        [Required]
        public int SensorId { get; set; }

        [Required]
        public string Sala { get; set; }

        [Required]
        public decimal Medicao { get; set; }
        [Required]
        public System.DateTime DataMedicao { get; set; }
      
        public virtual Sensor Sensor { get; set; }


    }
}
