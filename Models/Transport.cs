using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace diplomska.Models
{
    [Table("diplomska_Transports")]
    public class Transport
    {
        public long Id { get; set; }

        public bool Sp { get; set; }

        public string? SK { get; set; }

        public string? Skladisce { get; set; }

        public string? VrstaTransporta { get; set; }

        public long? StTransporta { get; set; }

        public DateTime? PlaniranPrihod { get; set; }

        public string? PavzaVoznika { get; set; }

        // Foreign key to AspNetUsers table
        public string? DolocenSkladiscnikId { get; set; }

        [ForeignKey("DolocenSkladiscnikId")]
        public IdentityUser? DolocenSkladiscnikNavigation { get; set; }

        public string? Registracija { get; set; }

        public string? VrstaPrevoznegaSredstva { get; set; }

        public string? Voznik { get; set; }

        public DateTime? NAVISZacetekSklada { get; set; }

        public DateTime? NAVISKonecSklada { get; set; }

        public bool IsCallback { get; set; }
    }
}
