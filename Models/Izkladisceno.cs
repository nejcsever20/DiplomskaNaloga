using System.ComponentModel.DataAnnotations.Schema;

namespace diplomska.Models
{
    public class Izkladisceno
    {
        public int Id { get; set; }
        public int? Kolicina { get; set; }
        public int? Palete { get; set; }
        public string? Skladiscnik { get; set; }
        public DateTime Datum { get; set; }
        public string? SkladiscnikId { get; set; }

        public long? TransportId { get; set; }
        [ForeignKey("TransportId")]
        public Transport? Transport { get; set; }

        public int Rampa1 { get; set; }
        public int Rampa2 { get; set; }

        public string? CarinskaVrvicva { get; set; }
        public string? UstreznostVozilca { get; set; }

        public DateTime? ZavrnilZacetek { get; set; }

        // Optional, if not already present:
        public DateTime? ZacetekNaklada { get; set; }
        public DateTime? KonecNaklada { get; set; }
    }
}