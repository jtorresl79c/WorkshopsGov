using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace WorkshopsGov.Services
{
    public class InspectionPdfGenerator
    {
        private static readonly string _pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Formats");

        public static string GenerateAndSavePdf(int inspectionId)
        {
            try
            {
                // ✅ Asegurar que la carpeta de PDFs existe
                if (!Directory.Exists(_pdfDirectory))
                {
                    Directory.CreateDirectory(_pdfDirectory);
                }

                // ✅ Definir la ruta del archivo PDF
                string fileName = $"Inspeccion_{inspectionId}.pdf";
                string filePath = Path.Combine(_pdfDirectory, fileName);

                // ✅ Si el archivo ya existe, elimínalo para evitar problemas de lectura
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // ✅ Crear el archivo PDF con "Hello World"
                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    document.Add(new Paragraph("Hello, World! 🚀 PDF generado con éxito."));
                }

                // ✅ Confirmar que el archivo se ha generado correctamente
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"✅ PDF generado correctamente en: {filePath}");
                    return fileName;
                }
                else
                {
                    Console.WriteLine("❌ Error: No se encontró el archivo generado.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PDF Generation Error] {ex.Message}");
                return null;
            }
        }
    }
}
