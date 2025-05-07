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
    }
}