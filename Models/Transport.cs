using diploma.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Transport
{
    public int Id { get; set; }
    public bool Sp { get; set; }

    [Column("Sk")]
    public long? SK { get; set; }
    public string? Skladisce { get; set; }
    public string? VrstaTransporta { get; set; }
    public int? StTransporta { get; set; }
    public DateTime PlaniranPrihod { get; set; }
    public string? PavzaVoznika { get; set; }

    [Column("DolocenSkladiscnik")] // No special characters
    public long? DoločenSkladiščnik { get; set; }

    public string? Registracija { get; set; }
    public string? VrstaPrevoznegaSredstva { get; set; }
    public string? Voznik { get; set; }
    public DateTime? NAVISZacetekSklada { get; set; }
    public DateTime? NAVISKonecSklada { get; set; }

    public User? DolocenSkladiscnikNavigation { get; set; }
}
