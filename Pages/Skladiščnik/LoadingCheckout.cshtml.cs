using diplomska.Models;
using diplomska.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace diplomska.Pages.Skladiščnik
{
    public class LoadingCheckOutModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoadingCheckOutModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public long TransportId { get; set; }

        [BindProperty]
        public Transport TransportInfo { get; set; }

        [BindProperty]
        public List<Izkladisceno> IzkladiscenoItems { get; set; } = new();

        [BindProperty]
        public DateTime StartLoading { get; set; }

        [BindProperty]
        public DateTime EndLoading { get; set; }

        [BindProperty]
        public string? CmrNumber { get; set; }

        [BindProperty]
        public string? TransportNumber { get; set; }

        [BindProperty]
        public string? LoadedQuantity { get; set; }

        [BindProperty]
        public string? RegistrationPlates { get; set; }

        [BindProperty]
        public string? Seal { get; set; }

        [BindProperty]
        public string? WarehouseSignature { get; set; }

        [BindProperty]
        public string? DriverSignature { get; set; }

        [BindProperty]
        public List<ChecklistItem> ChecklistItems { get; set; } = new()
        {
            new() { Id = 1, Question = "The vehicle is clean" },
            new() { Id = 2, Question = "The vehicle is clean (no wet, greasy, or damp areas)" },
            new() { Id = 3, Question = "The loading surface is adequate" },
            new() { Id = 4, Question = "The walls of the vehicle are adequate" },
            new() { Id = 5, Question = "The vehicle has intermediate fastening rail" },
            new() { Id = 6, Question = "The vehicle has securing" },
            new() { Id = 7, Question = "Protective equipment used during loading" },
            new() { Id = 8, Question = "The transport vehicle is suitable" }
        };

        public void OnGet()
        {
            TransportInfo = _context.Transport.FirstOrDefault(t => t.Id == TransportId);
            IzkladiscenoItems = _context.Izkladisceno.Where(x => x.TransportId == TransportId).ToList();

            if (TransportInfo != null)
            {
                StartLoading = TransportInfo.NAVISZacetekSklada ?? DateTime.MinValue;
                EndLoading = TransportInfo.NAVISKonecSklada ?? DateTime.MinValue;
                TransportNumber = TransportInfo.StTransporta?.ToString();
                RegistrationPlates = TransportInfo.Registracija;
                Seal = TransportInfo.Sp ? "Yes" : "No";
            }

            var item = IzkladiscenoItems.FirstOrDefault();
            if (item != null)
            {
                Console.WriteLine($"LoadedQuantity (Kolicina): {item.Kolicina}");
                LoadedQuantity = item.Kolicina?.ToString() ?? "No data available";
            }
            else
            {
                Console.WriteLine("No items in IzkladiscenoItems.");
            }
        }

        public IActionResult OnPost()
        {
            foreach (var item in IzkladiscenoItems)
            {
                _context.Izkladisceno.Add(new Izkladisceno
                {
                    Kolicina = item.Kolicina ?? 0,
                    Palete = item.Palete,
                    Skladiscnik = item.Skladiscnik ?? "Unknown",
                    Datum = DateTime.Now,
                    TransportId = TransportId
                });
            }

            _context.SaveChanges();

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12, XFontStyle.Regular);
            var headerFont = new XFont("Verdana", 16, XFontStyle.Bold);
            int y = 40;

            gfx.DrawString("Loading Check-Out Form", headerFont, XBrushes.Black,
                new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
            y += 40;

            gfx.DrawString($"{TransportInfo.Sp} {TransportInfo.SK} {TransportInfo.Skladisce} {TransportInfo.VrstaTransporta} " +
                $"{TransportInfo.StTransporta} {TransportInfo.PlaniranPrihod} {TransportInfo.PavzaVoznika} " +
                $"{TransportInfo.Registracija} {TransportInfo.VrstaPrevoznegaSredstva} {TransportInfo.Voznik} " +
                $"{TransportInfo.NAVISZacetekSklada} {TransportInfo.NAVISKonecSklada}", font, XBrushes.Black, new XPoint(40, y));
            y += 30;

            gfx.DrawString($"Start Loading: {StartLoading}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"End Loading: {EndLoading}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"CMR Number: {CmrNumber}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Transport Number: {TransportNumber}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Loaded Quantity: {LoadedQuantity}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Registration Plates: {RegistrationPlates}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Seal: {Seal}", font, XBrushes.Black, new XPoint(40, y)); y += 30;

            foreach (var item in ChecklistItems)
            {
                var answer = string.IsNullOrWhiteSpace(item.Answer) ? "Not answered" : item.Answer;
                var comment = string.IsNullOrWhiteSpace(item.Comment) ? "No comment" : item.Comment;
                gfx.DrawString($"- {item.Question}: {answer} | Comment: {comment}", font, XBrushes.Black, new XPoint(60, y));
                y += 20;
            }

            y += 20;

            foreach (var i in IzkladiscenoItems)
            {
                gfx.DrawString($"Qty: {i.Kolicina}, Pallets: {i.Palete}, By: {i.Skladiscnik}, On: {i.Datum:dd.MM.yyyy HH:mm}", font, XBrushes.Black, new XPoint(60, y));
                y += 20;
            }

            y += 20;
            gfx.DrawString($"Warehouse Signature: {WarehouseSignature}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Driver Signature: {DriverSignature}", font, XBrushes.Black, new XPoint(40, y));

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return File(stream.ToArray(), "application/pdf", $"LoadingCheckOutForm_Transport_{TransportInfo.StTransporta}.pdf");
        }

        public class ChecklistItem
        {
            public int Id { get; set; }
            public string? Question { get; set; }
            public string? Answer { get; set; }
            public string? Comment { get; set; }
        }
    }
}
