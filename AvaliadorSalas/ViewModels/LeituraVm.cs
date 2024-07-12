using AvaliadorSalas.Models;

namespace AvaliadorSalas.ViewModels
{
    public class LeituraVm
    {
        public DateTime? DataMedicao { get; set; }
        //public string SalaNome { get; set; }
        public decimal? Temperatura { get; set; }
        public decimal? Humidade { get; set; }
        public decimal? Co2 { get; set; }

        public string DateOnlyString => DataMedicao?.ToString("yyyy-MM-dd") ?? string.Empty;
        public string TimeOnlyString => DataMedicao?.ToString("hh:mm:ss tt") ?? string.Empty;

        public string GoogleDate => (DataMedicao.HasValue)
          ? string.Format("Date({0})", DataMedicao.Value.ToString("yyyy,M,d,H,m,s,f"))
          : string.Empty;

        public string TempString => (Temperatura != null) ? Temperatura.Value.ToString("###.0") : "0.0";
        public string HumidString => (Humidade != null) ? Humidade.Value.ToString("###.0") : "0.0";
        public string Co2String => (Co2 != null) ? Co2.Value.ToString("###.0") : "0.0";

        //public virtual Sensor Sensores { get; set; }

    }
}
