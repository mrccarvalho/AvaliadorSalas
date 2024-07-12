using AvaliadorSalas.Models;

namespace AvaliadorSalas.ViewModels
{
    public class LeituraCo2Vm
    {
        public DateTime? DataMedicao { get; set; }
        public decimal? Co2 { get; set; }
        public string DateOnlyString => DataMedicao?.ToString("yyyy-MM-dd") ?? string.Empty;
        public string TimeOnlyString => DataMedicao?.ToString("hh:mm:ss tt") ?? string.Empty;

        public string GoogleDate => (DataMedicao.HasValue)
          ? string.Format("Date({0})", DataMedicao.Value.ToString("yyyy,M,d,H,m,s,f"))
          : string.Empty;
        public string Co2String => (Co2!= null) ? Co2.Value.ToString("###.0") : "0.0";


    }
}
