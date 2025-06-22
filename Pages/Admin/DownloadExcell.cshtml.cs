using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using diplomska.Data;
using diplomska.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DownloadExcellModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DownloadExcellModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Transport> Transports { get; set; } = new();
        public List<Izkladisceno> Izkladisceno { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Transports = _context.Transport.ToList();
            Izkladisceno = _context.Izkladisceno.ToList();

            var users = _userManager.Users.ToList();
            var userMap = users
                .Where(u => !string.IsNullOrEmpty(u.Id))
                .ToDictionary(u => u.Id, u => $"{u.UserName} ({u.Email})");

            ViewData["SkladiscnikMap"] = userMap;
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadAllDataAsync()
        {
            var transports = _context.Transport.ToList();
            var izkList = _context.Izkladisceno.ToList();

            var users = _userManager.Users.ToList();
            var userMap = users
                .Where(u => !string.IsNullOrEmpty(u.Id))
                .ToDictionary(u => u.Id, u => $"{u.UserName} ({u.Email})");

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Transport + Izkladisceno");

            int transportStartCol = 1;
            int izkladiscenoStartCol = 20;

            WriteTransportData(sheet, transports, transportStartCol, userMap);
            WriteIzkladiscenoData(sheet, izkList, userMap, izkladiscenoStartCol);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Transports_And_Izkladisceno_SideBySide.xlsx");
        }

        private void WriteTransportData(IXLWorksheet sheet, List<Transport> transports, int startCol, Dictionary<string, string> userMap)
        {
            var headers = new[]
            {
                "Id", "Sp", "SK", "Skladisce", "VrstaTransporta", "StTransporta",
                "PlaniranPrihod", "PavzaVoznika", "DolocenSkladiščnik", "Registracija",
                "VrstaPrevoznegaSredstva", "Voznik", "NAVISZacetekSklada", "NAVISKonecSklada"
            };

            // Header row
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cell(1, startCol + i);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data rows
            for (int i = 0; i < transports.Count; i++)
            {
                var t = transports[i];
                int row = i + 2;

                sheet.Cell(row, startCol + 0).Value = t.Id;
                sheet.Cell(row, startCol + 1).Value = t.Sp;
                sheet.Cell(row, startCol + 2).Value = t.SK;
                sheet.Cell(row, startCol + 3).Value = t.Skladisce;
                sheet.Cell(row, startCol + 4).Value = t.VrstaTransporta;
                sheet.Cell(row, startCol + 5).Value = t.StTransporta;
                sheet.Cell(row, startCol + 6).Value = t.PlaniranPrihod;
                sheet.Cell(row, startCol + 7).Value = t.PavzaVoznika;
                sheet.Cell(row, startCol + 8).Value =
                    !string.IsNullOrEmpty(t.DolocenSkladiscnikId) && userMap.TryGetValue(t.DolocenSkladiscnikId, out var displayName)
                        ? displayName
                        : t.DolocenSkladiscnikId ?? "Ni dodeljen";
                sheet.Cell(row, startCol + 9).Value = t.Registracija;
                sheet.Cell(row, startCol + 10).Value = t.VrstaPrevoznegaSredstva;
                sheet.Cell(row, startCol + 11).Value = t.Voznik;
                sheet.Cell(row, startCol + 12).Value = t.NAVISZacetekSklada;
                sheet.Cell(row, startCol + 13).Value = t.NAVISKonecSklada;

                if (i % 2 == 0)
                {
                    for (int col = 0; col < headers.Length; col++)
                        sheet.Cell(row, startCol + col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                }
            }

            sheet.Columns(startCol, startCol + headers.Length - 1).AdjustToContents();
        }

        private void WriteIzkladiscenoData(IXLWorksheet sheet, List<Izkladisceno> izkList, Dictionary<string, string> userMap, int startCol)
        {
            var headers = new[]
            {
                "Id", "Kolicina", "Palete", "Skladiščnik", "Datum", "SkladiščnikId", "TransportId"
            };

            // Header row
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cell(1, startCol + i);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data rows
            for (int i = 0; i < izkList.Count; i++)
            {
                var z = izkList[i];
                int row = i + 2;

                sheet.Cell(row, startCol + 0).Value = z.Id;
                sheet.Cell(row, startCol + 1).Value = z.Kolicina;
                sheet.Cell(row, startCol + 2).Value = z.Palete;
                sheet.Cell(row, startCol + 3).Value = z.Skladiscnik;
                sheet.Cell(row, startCol + 4).Value = z.Datum;
                sheet.Cell(row, startCol + 5).Value =
                    !string.IsNullOrEmpty(z.SkladiscnikId) && userMap.TryGetValue(z.SkladiscnikId, out var displayName)
                        ? displayName
                        : z.SkladiscnikId ?? "Ni dodeljen";
                sheet.Cell(row, startCol + 6).Value = z.TransportId;

                if (i % 2 == 0)
                {
                    for (int col = 0; col < headers.Length; col++)
                        sheet.Cell(row, startCol + col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                }
            }

            sheet.Columns(startCol, startCol + headers.Length - 1).AdjustToContents();
        }
    }
}