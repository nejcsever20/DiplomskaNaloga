using System.ComponentModel.DataAnnotations;
namespace diploma.Models
{
    public class User
    {
        public long Id { get; set; } // <-- changed from int to long
        public string? Ime { get; set; }
        public string? Priimek { get; set; }
        public string? Email { get; set; }
        public string? Geslo { get; set; }
        public long Pravica { get; set; }
    }

}
