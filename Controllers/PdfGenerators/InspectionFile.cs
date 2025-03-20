using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using WorkshopsGov.Models; // Asegúrate de incluir tu modelo
using Microsoft.EntityFrameworkCore;
using WorkshopsGov.Controllers.Global;
using WorkshopsGov.Data;
using IOFile = System.IO.File;

namespace WorkshopsGov.Controllers.PdfGenerators
{
    public class InspectionFile
    {
        public static dynamic GenerateFile(Inspection inspection, ApplicationDbContext db)
        {
            try
            {
                if (inspection == null)
                {
                    throw new ArgumentNullException(nameof(inspection), "La inspección no puede ser nula.");
                }

                // ✅ Obtener la carpeta específica para la inspección
                string inspectionPath = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(
                    Utilidades.GetFullPathInspection(inspection.Id),
                    "RECEPCION_ENTREGA"
                );

                // ✅ Definir la ruta del archivo PDF
                string fileName = $"Inspeccion_{inspection.Id}.pdf";
                string filePath = Path.Combine(inspectionPath, fileName);

                // ✅ Si el archivo ya existe, elimínalo
                if (IOFile.Exists(filePath))
                {
                    IOFile.Delete(filePath);
                }

                // ✅ Crear el archivo PDF con contenido de inspección
                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    document.Add(new Paragraph($"Inspección ID: {inspection.Id}"));
                    document.Add(new Paragraph($"Número de Memo: {inspection.MemoNumber}"));
                    document.Add(new Paragraph($"Fecha de Inspección: {inspection.InspectionDate:dd/MM/yyyy}"));
                    document.Add(new Paragraph($"Operador: {inspection.OperatorName}"));
                    document.Add(new Paragraph($"Observaciones: {inspection.FailureReport}"));
                    document.Add(new Paragraph("🚀 PDF generado con éxito."));
                }

                // ✅ Confirmar que el archivo se ha generado correctamente
                if (!IOFile.Exists(filePath))
                {
                    throw new Exception("❌ Error: No se encontró el archivo generado.");
                }

                return new
                {
                    Filename = fileName,
                    Formato = ".pdf",
                    Ruta = filePath
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [PDF Generation Error] {ex.Message}");
                return null;
            }
        }
    }
}
