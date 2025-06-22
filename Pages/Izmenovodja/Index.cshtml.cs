using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace diplomska.Pages.Izmenovodja
{
    [Authorize(Roles = "Admin, Izmenovodja")]

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Transport> Transport { get; set; } = new List<Transport>();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty]
        public List<IFormFile> ExcelFiles { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Transport.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(t =>
                    t.StTransporta.ToString().Contains(SearchString) ||
                    t.Registracija.Contains(SearchString) ||
                    t.Voznik.Contains(SearchString));
            }

            Transport = await query
                .Select(t => new Transport
                {
                    Id = t.Id,
                    StTransporta = t.StTransporta,
                    Registracija = t.Registracija,
                    Voznik = t.Voznik,
                    Sp = t.Sp,
                    SK = t.SK,
                    Skladisce = t.Skladisce,
                    VrstaTransporta = t.VrstaTransporta,
                    PlaniranPrihod = t.PlaniranPrihod,
                    PavzaVoznika = t.PavzaVoznika,
                    VrstaPrevoznegaSredstva = t.VrstaPrevoznegaSredstva,
                    NAVISZacetekSklada = t.NAVISZacetekSklada,
                    NAVISKonecSklada = t.NAVISKonecSklada
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostImportExcelAsync()
        {
            if (ExcelFiles == null || !ExcelFiles.Any())
            {
                TempData["Message"] = "No files selected.";
                return RedirectToPage();
            }

            foreach (var file in ExcelFiles)
            {
                if (file.Length == 0)
                    continue;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                Console.WriteLine($"Processing uploaded file: {file.FileName}");

                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;

                        if (extension == ".xlsx")
                        {
                            using (var workbook = new XLWorkbook(stream))
                            {
                                var worksheet = workbook.Worksheets.First();
                                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

                                foreach (var row in rows)
                                {
                                    if (row.Cells().Count() < 12) continue;

                                    string registracija = row.Cell(8).GetString()?.Trim();
                                    if (string.IsNullOrWhiteSpace(registracija)) continue;

                                    string stTransportaStr = row.Cell(5).GetString();
                                    string numericStTransportaStr = new string(stTransportaStr.Where(char.IsDigit).ToArray());
                                    if (!long.TryParse(numericStTransportaStr, out var stTransporta)) continue;

                                    DateTime? planiranPrihod = DateTime.TryParse(row.Cell(6).GetString(), out var prihod) ? prihod : (DateTime?)null;
                                    DateTime? navStart = DateTime.TryParse(row.Cell(11).GetString(), out var ns) ? ns : (DateTime?)null;
                                    DateTime? navEnd = DateTime.TryParse(row.Cell(12).GetString(), out var ne) ? ne : (DateTime?)null;

                                    var transport = new Transport
                                    {
                                        Sp = row.Cell(1).GetBoolean(),
                                        SK = row.Cell(2).GetString()?.Trim(),
                                        Skladisce = row.Cell(3).GetString()?.Trim(),
                                        VrstaTransporta = row.Cell(4).GetString()?.Trim(),
                                        StTransporta = stTransporta,
                                        PlaniranPrihod = planiranPrihod,
                                        PavzaVoznika = null,
                                        Registracija = registracija,
                                        VrstaPrevoznegaSredstva = row.Cell(9).GetString()?.Trim(),
                                        Voznik = row.Cell(10).GetString()?.Trim(),
                                        NAVISZacetekSklada = navStart,
                                        NAVISKonecSklada = navEnd
                                    };

                                    if (_context.Transport.Any(t => t.StTransporta == transport.StTransporta))
                                        continue;

                                    _context.Transport.Add(transport);
                                }

                                await _context.SaveChangesAsync();
                            }
                        }
                        else if (extension == ".csv" || extension == ".txt")
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                int lineIndex = 0;
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    lineIndex++;

                                    if (string.IsNullOrWhiteSpace(line)) continue;
                                    if (lineIndex == 1 && line.StartsWith("TRUE", StringComparison.OrdinalIgnoreCase)) continue;

                                    var parts = line.Split('\t');
                                    if (parts.Length < 13) continue;

                                    try
                                    {
                                        string stTransportaStr = parts[4];
                                        string numericStTransportaStr = new string(stTransportaStr.Where(char.IsDigit).ToArray());
                                        if (!long.TryParse(numericStTransportaStr, out var stTransporta)) continue;

                                        if (_context.Transport.Any(t => t.StTransporta == stTransporta))
                                            continue;

                                        DateTime? planiranPrihod = DateTime.TryParse(parts[5], out var prihod) ? prihod : (DateTime?)null;
                                        DateTime? navStart = DateTime.TryParse(parts[10], out var ns) ? ns : (DateTime?)null;
                                        DateTime? navEnd = DateTime.TryParse(parts[11], out var ne) ? ne : (DateTime?)null;

                                        var transport = new Transport
                                        {
                                            Sp = bool.TryParse(parts[0], out var sp) && sp,
                                            SK = parts[1]?.Trim(),
                                            Skladisce = parts[2]?.Trim(),
                                            VrstaTransporta = parts[3]?.Trim(),
                                            StTransporta = stTransporta,
                                            PlaniranPrihod = planiranPrihod,
                                            PavzaVoznika = null,
                                            Registracija = parts[6]?.Trim(),
                                            VrstaPrevoznegaSredstva = parts[7]?.Trim(),
                                            Voznik = parts[8]?.Trim(),
                                            NAVISZacetekSklada = navStart,
                                            NAVISKonecSklada = navEnd
                                        };

                                        _context.Transport.Add(transport);
                                    }
                                    catch (Exception parseEx)
                                    {
                                        Console.WriteLine($"Line {lineIndex} skipped due to error: {parseEx.Message}");
                                        continue;
                                    }
                                }

                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file '{file.FileName}': {ex.Message}");
                }
            }

            TempData["Message"] = "Files processed successfully.";
            return RedirectToPage();
        }
    }
}
