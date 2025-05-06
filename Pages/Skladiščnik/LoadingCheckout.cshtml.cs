using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace diplomska.Pages.Skladiščnik
{
    public class LoadingCheckOutModel : PageModel
    {
        public class ChecklistItem
        {
            public int Id { get; set; }
            public string? Question { get; set; }
            public string? Answer { get; set; }
            public string? Comment { get; set; }
        }

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
        public List<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>
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

        public void OnGet() { }

        public IActionResult OnPost()
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12, XFontStyle.Regular);
            var headerFont = new XFont("Verdana", 16, XFontStyle.Bold);
            int yPoint = 40;

            // Title (Centered)
            gfx.DrawString("Loading Check-Out Form", headerFont, XBrushes.Black,
                new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopCenter);
            yPoint += 40;

            // Basic Information Section
            gfx.DrawString("Basic Information", new XFont("Verdana", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 20;
            gfx.DrawString($"Start Loading: {StartLoading}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 20;
            gfx.DrawString($"End Loading: {EndLoading}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 20;
            gfx.DrawString($"CMR Number: {CmrNumber}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 20;
            gfx.DrawString($"Transport Number: {TransportNumber}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 20;
            gfx.DrawString($"Loaded Quantity: {LoadedQuantity}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 20;
            gfx.DrawString($"Registration Plates: {RegistrationPlates}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 20;
            gfx.DrawString($"Seal: {Seal}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 30;

            // Checklist Section
            gfx.DrawString("Checklist", new XFont("Verdana", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 20;
            foreach (var item in ChecklistItems)
            {
                var answer = string.IsNullOrWhiteSpace(item.Answer) ? "Not answered" : item.Answer;
                var comment = string.IsNullOrWhiteSpace(item.Comment) ? "No comment" : item.Comment;
                gfx.DrawString($"- {item.Question}: {answer} | Comment: {comment}", font, XBrushes.Black, new XPoint(60, yPoint));
                yPoint += 20;
            }

            yPoint += 20;

            // Signatures Section
            gfx.DrawString("Signatures", new XFont("Verdana", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(40, yPoint));
            yPoint += 20;
            gfx.DrawString($"Warehouse Signature: {WarehouseSignature}", font, XBrushes.Black, new XPoint(40, yPoint)); yPoint += 20;
            gfx.DrawString($"Driver Signature: {DriverSignature}", font, XBrushes.Black, new XPoint(40, yPoint));

            // Save to memory stream
            using var stream = new MemoryStream();
            document.Save(stream, false);
            return File(stream.ToArray(), "application/pdf", "LoadingCheckOutForm.pdf");
        }
    }
}