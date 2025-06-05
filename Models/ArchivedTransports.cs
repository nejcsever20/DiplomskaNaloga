using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace diplomska.Models
{
    public class ArchivedTransport
    {
        public string? CallbackReason { get; set; }

        public long Id { get; set; }
        public bool Sp { get; set; }
        public string? SK { get; set; }
        public string? Skladisce { get; set; }
        public string? VrstaTransporta { get; set; }
        public long? StTransporta { get; set; }
        public DateTime? PlaniranPrihod { get; set; }
        public string? PavzaVoznika { get; set; }
        // We archive the ID but skip the navigation to avoid dependency on AspNetUsers
        public string? DolocenSkladiscnikId { get; set; }
        public string? Registracija { get; set; }
        public string? VrstaPrevoznegaSredstva { get; set; }
        public string? Voznik { get; set; }
        public DateTime? NAVISZacetekSklada { get; set; }
        public DateTime? NAVISKonecSklada { get; set; }
        public bool IsCallback { get; set; }
        public DateTime ArchivedAt { get; set; } = DateTime.UtcNow;
    }
}