using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diplomska.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace diplomska.Pages.Admin 
{
    public class DownloadExcellModel : PageModel 
    {
        private readonly ApplicationDbContext _context;

        public DownloadExcellModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostDownloadAllDataAsync()
        {
            using var workbook = new XLWorkbook();

            var transportsSheet = workbook.Worksheets.Add("Transports");
            var transports = _context.Transport.ToList();

            transportsSheet.Cell(1, 1).Value = "Id";
            transportsSheet.Cell(1, 2).Value = "Sp";
            transportsSheet.Cell(1, 3).Value = "SK";
            transportsSheet.Cell(1, 4).Value = "Skladisce";
            transportsSheet.Cell(1, 5).Value = "VrstaTransporta";
            transportsSheet.Cell(1, 6).Value = "StTransporta";
            transportsSheet.Cell(1, 7).Value = "PlaniranPrihod";
            transportsSheet.Cell(1, 8).Value = "PavzaVoznika";
            transportsSheet.Cell(1, 9).Value = "DolocenSkladiscnikId";
            transportsSheet.Cell(1, 10).Value = "Registracija";
            transportsSheet.Cell(1, 11).Value = "VrstaPrevoznegaSredstva";
            transportsSheet.Cell(1, 12).Value = "Voznik";
            transportsSheet.Cell(1, 13).Value = "NAVISZacetekSklada";
            transportsSheet.Cell(1, 14).Value = "NAVISKonecSklada";

            for (int i = 0; i < transports.Count; i++)
            {
                var row = transportsSheet.Row(i + 2);
                var t = transports[i];
                row.Cell(1).Value = t.Id;
                row.Cell(2).Value = t.Sp;
                row.Cell(3).Value = t.SK;
                row.Cell(4).Value = t.Skladisce;
                row.Cell(5).Value = t.VrstaTransporta;
                row.Cell(6).Value = t.StTransporta;
                row.Cell(7).Value = t.PlaniranPrihod;
                row.Cell(8).Value = t.PavzaVoznika;
                row.Cell(9).Value = t.DolocenSkladiscnikId;
                row.Cell(10).Value = t.Registracija;
                row.Cell(11).Value = t.VrstaPrevoznegaSredstva;
                row.Cell(12).Value = t.Voznik;
                row.Cell(13).Value = t.NAVISZacetekSklada;
                row.Cell(14).Value = t.NAVISKonecSklada;
            }

            var izSheet = workbook.Worksheets.Add("Izkladisceno");
            var izk = _context.Izkladisceno.ToList();

            izSheet.Cell(1, 1).Value = "Id";
            izSheet.Cell(1, 2).Value = "Kolicina";
            izSheet.Cell(1, 3).Value = "Palete";
            izSheet.Cell(1, 4).Value = "Skladiscnik";
            izSheet.Cell(1, 5).Value = "Datum";
            izSheet.Cell(1, 6).Value = "SkladiscnikId";
            izSheet.Cell(1, 7).Value = "TransportId";

            for (int i = 0; i < izk.Count; i++)
            {
                var row = izSheet.Row(i + 2);
                var z = izk[i];
                row.Cell(1).Value = z.Id;
                row.Cell(2).Value = z.Kolicina;
                row.Cell(3).Value = z.Palete;
                row.Cell(4).Value = z.Skladiscnik;
                row.Cell(5).Value = z.Datum;
                row.Cell(6).Value = z.SkladiscnikId;
                row.Cell(7).Value = z.TransportId;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "AllTransportAndIzkladiscenoData.xlsx");
        }
    }
}
