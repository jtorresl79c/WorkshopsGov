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
using iText.IO.Image;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Borders;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Canvas; // Necesario para trabajar con coordenadas
using iText.Kernel.Colors;

namespace WorkshopsGov.Controllers.PdfGenerators
{
    public class MemoFile
    {

        private void AddVehicleCell(Table vehicleTable, string label, string value)
        {
            // 🔹 Celda de la etiqueta (alineada a la derecha)
            vehicleTable.AddCell(new Cell()
                .Add(new Paragraph()
                    .Add(new Text(label).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(11))
                )
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.RIGHT)
            );

            // 🔹 Celda del valor
            var valueCell = new Cell()
                .SetBorder(Border.NO_BORDER)
                .SetWidth(100); // 🔹 Limita el ancho de la celda para evitar que desplace las demás

            // 🔹 Dividir el valor en partes de 10 caracteres
            for (int i = 0; i < value.Length; i += 14)
            {
                string substring = value.Substring(i, Math.Min(14, value.Length - i));

                // 🔹 Agregar cada parte como un nuevo párrafo
                valueCell.Add(new Paragraph(substring)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(9) // 🔹 Texto más pequeño para que quepa más
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetMarginBottom(0)); // 🔹 Sin margen inferior para que esté pegado
            }

            // 🔹 Agregar la línea solo si el valor tiene 10 caracteres o menos
            if (value.Length <= 14)
            {
                valueCell.Add(new Paragraph("_______________") // Línea separada
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(9) // 🔹 Tamaño de la línea igual al texto
                    .SetPaddingTop(-10)); // 🔹 Acerca la línea al texto para que no se vea desplazada
            }

            // 🔹 Añadir la celda a la tabla
            vehicleTable.AddCell(valueCell);
        }
        private void AddFuelGauge(PdfDocument pdfDoc, Document document, float fuelLevel)
        {
            PdfPage page = pdfDoc.GetFirstPage();
            PdfCanvas canvas = new PdfCanvas(page);

            ImageData odometerImage = ImageDataFactory.Create("wwwroot/images/odometro.png");
            Image odometer = new Image(odometerImage)
                .SetWidth(80)
                .SetFixedPosition(200, 620);

            document.Add(odometer);

            float odometerX = 200;
            float odometerY = 620;

            float centerX = odometerX + 41;
            float centerY = odometerY + 2;

            // Ángulo de 150° (E) a 30° (F)
            float angleDegrees = 150 - (fuelLevel / 100f) * 120;
            float angleRadians = (float)(Math.PI / 180f * angleDegrees);

            float needleLength = 30;
            float endX = centerX + (float)(needleLength * Math.Cos(angleRadians));
            float endY = centerY + (float)(needleLength * Math.Sin(angleRadians));

            canvas.SaveState();
            canvas.SetStrokeColor(new DeviceRgb(255, 0, 0))
                  .SetLineWidth(2)
                  .MoveTo(centerX, centerY)
                  .LineTo(endX, endY)
                  .Stroke();

            canvas.Circle(centerX, centerY, 2)
                  .SetFillColor(new DeviceRgb(255, 0, 0))
                  .Fill();
            canvas.RestoreState();
        }
        private void AddInspectionDetails(PdfDocument pdfDoc, Document document)
        {
            document.Add(new Paragraph("INSPECCIÓN E INVENTARIO")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10));

            float[] mainColumnWidths = { 2, 1 };
            Table mainTable = new Table(UnitValue.CreatePercentArray(mainColumnWidths)).UseAllAvailableWidth();

            float[] columnWidths = { 4, 1, 1 };
            Table inspectionTable = new Table(UnitValue.CreatePercentArray(columnWidths))
                .SetWidth(300);

            inspectionTable.AddCell(new Cell().Add(new Paragraph("CONCEPTO")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(11))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingBottom(5));

            inspectionTable.AddCell(new Cell().Add(new Paragraph("SÍ")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(11))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingBottom(5));

            inspectionTable.AddCell(new Cell().Add(new Paragraph("NO")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(11))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingBottom(5));

            string[] conceptos = {
                "SE ENTREGA UNIDAD LAVADA",
                "SE ENTREGAN REFACCIONES USADAS",
                "SE ENTREGA CON CUBRETORRETAS Y LOGOS",
                "SE RECIBE LA UNIDAD",
                "LA UNIDAD SE ENCUENTRA LISTA PARA ENTREGARSE A SSPCM",
                "INDICADORES DE TABLERO",
                "CARROCERÍA Y PINTURA",
                "TAPICERÍA",
                "BATERÍA"
            };

            foreach (var concepto in conceptos)
            {
                inspectionTable.AddCell(new Cell().Add(new Paragraph(concepto)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(11))
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPaddingBottom(3));

                inspectionTable.AddCell(new Cell().SetBorder(new SolidBorder(0)).SetHeight(12));
                inspectionTable.AddCell(new Cell().SetBorder(new SolidBorder(0)).SetHeight(12));
            }

            mainTable.AddCell(new Cell().Add(inspectionTable).SetBorder(Border.NO_BORDER));

            Table imageTable = new Table(1).UseAllAvailableWidth();

            string[] imagePaths = {
                "wwwroot/images/lateral_frontal.png",
                "wwwroot/images/lateral_izquierda.png",
                "wwwroot/images/lateral_derecha.png",
                "wwwroot/images/lateral_atras.png"
            };

            foreach (var path in imagePaths)
            {
                ImageData imageData = ImageDataFactory.Create(path);
                Image referenceImage = new Image(imageData)
                    .SetWidth(90)
                    .SetAutoScale(true)
                    .SetMarginBottom(5);

                imageTable.AddCell(new Cell().Add(referenceImage).SetBorder(Border.NO_BORDER));
            }

            mainTable.AddCell(new Cell().Add(imageTable).SetBorder(Border.NO_BORDER));
            document.Add(mainTable);
        }

        public static dynamic GenerateFile(Inspection inspection, ApplicationDbContext db)
        {
            try
            {
                if (inspection == null)
                {
                    throw new ArgumentNullException(nameof(inspection), "La inspección no puede ser nula.");
                }

                string inspectionPath = Utilidades.CreateOrGetDirectoryInsideInspectionDirectory(
                    Utilidades.GetFullPathInspection(inspection.Id),
                    "MEMOS"
                );

                string fileName = $"Memo_{inspection.Id}.pdf";
                string filePath = System.IO.Path.Combine(inspectionPath, fileName);

                if (IOFile.Exists(filePath))
                {
                    IOFile.Delete(filePath);
                }

                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    Table headerTable = new Table(3).UseAllAvailableWidth().SetMarginTop(-20);

                    Image leftLogo = new Image(ImageDataFactory.Create("wwwroot/images/logo_xxv.png"))
                        .SetWidth(110)
                        .SetHorizontalAlignment(HorizontalAlignment.LEFT);

                    Paragraph title = new Paragraph("DIRECCIÓN DE SERVICIOS GENERALES\nTALLERES MUNICIPALES")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(12)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPaddingTop(10);

                    Image rightLogo = new Image(ImageDataFactory.Create("wwwroot/images/logo_policia.jpeg"))
                        .SetWidth(70)
                        .SetHorizontalAlignment(HorizontalAlignment.RIGHT)
                        .SetMarginTop(-10);

                    headerTable.AddCell(
                        new Cell()
                            .Add(leftLogo)
                            .SetBorder(Border.NO_BORDER)
                            .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                            .SetPaddingLeft(-15) 
                    );

                    headerTable.AddCell(
                        new Cell()
                            .Add(title)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetBorder(Border.NO_BORDER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    );

                    headerTable.AddCell(
                        new Cell()
                            .Add(rightLogo)
                            .SetBorder(Border.NO_BORDER)
                            .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                    );

                    // Añadir la tabla al documento
                    headerTable.SetMarginBottom(10);
                    document.Add(headerTable);


                    LineSeparator line = new LineSeparator(new SolidLine());

                    document.Add(line);
                    document.Add(new Paragraph("ENTREGAS-RECEPCIÓN VEHICULAR OFICIAL / INSPECCIÓN E INVENTARIO")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(11)
                        .SetTextAlignment(TextAlignment.CENTER)
                    );
                    document.Add(line);

                    Table inspectionTable = new Table(3).UseAllAvailableWidth();
                    Cell rightCell = new Cell()
                        .Add(new Paragraph("FOLIO: ")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)) 
                            .Add(new Text("_____________") 
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            )
                        )
                        .Add(new Paragraph("FECHA INGRESO: ")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)) 
                            .Add(new Text("____/____/______") 
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            )
                        )
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.RIGHT);

                    inspectionTable.AddCell(rightCell);
                    document.Add(inspectionTable);


                    Table vehicleTable = new Table(8).UseAllAvailableWidth();
                    var memoFile = new MemoFile();
                    memoFile.AddVehicleCell(vehicleTable, "OFICIALÍA: ", $"{inspection.Vehicle.Oficialia}");
                    memoFile.AddVehicleCell(vehicleTable, "PLACAS: ", $"{inspection.Vehicle.LicensePlate}");
                    memoFile.AddVehicleCell(vehicleTable, "MARCA: ", $"{inspection.Vehicle.Brand.Name}");
                    memoFile.AddVehicleCell(vehicleTable, "MODELO: ", $"{inspection.Vehicle.Model.Name}");

                    document.Add(vehicleTable);

                    memoFile.AddFuelGauge(pdf, document, inspection.FuelLevel);
                    document.Add(new Paragraph("NIVEL DE COMBUSTIBLE:")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(12));
                    document.Add(new Paragraph("\n"));

                    memoFile.AddInspectionDetails(pdf, document);

                    Table table = new Table(1).UseAllAvailableWidth();

                    table.AddCell(new Cell()
                            .Add(new Paragraph()
                                .Add(new Text("TIPO DE REPARACIÓN REALIZADA: ") 
                                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                                    .SetFontSize(10))
                                .Add(new Text(new string('_', 60)) 
                                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                    .SetFontSize(10)))
                            .Add(new Paragraph(new string('_', 92)) 
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                .SetFontSize(10)
                                .SetPaddingTop(5)) 
                            .SetMarginBottom(15) 
                            .SetBorder(Border.NO_BORDER)
                        );

                    table.AddCell(new Cell()
                        .Add(new Paragraph()
                            .Add(new Text("OBSERVACIONES: ")
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                                .SetFontSize(10))
                            .Add(new Text(new string('_', 75))
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                .SetFontSize(10)))
                        .Add(new Paragraph(new string('_', 92))
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            .SetFontSize(10)
                            .SetPaddingTop(5))
                        .SetMarginBottom(15)
                        .SetBorder(Border.NO_BORDER)
                    );

                    document.Add(table);

                    document.Add(new Paragraph("\n"));
                    
                    float[] signatureWidths = { 1, 1 }; 
                    Table signatureTable = new Table(UnitValue.CreatePercentArray(signatureWidths)).UseAllAvailableWidth();

                  
                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("ENTREGÓ")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                            .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("RECIBIÓ")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                            .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("____________________________________"))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPaddingTop(10) 
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("____________________________________"))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPaddingTop(10) 
                        .SetBorder(Border.NO_BORDER));

                    // 🔹 Tercera fila: "Nombre y Firma"
                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("NOMBRE Y FIRMA"))
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("NOMBRE Y FIRMA"))
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("____________________________________"))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPaddingTop(20) 
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("____________________________________"))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPaddingTop(20) 
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("NOMBRE DEL TALLER QUE ENTREGA"))
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(Border.NO_BORDER));

                    signatureTable.AddCell(new Cell()
                        .Add(new Paragraph("NOMBRE Y FIRMA\nENCARGADO DE PISO"))
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(Border.NO_BORDER));

                    document.Add(signatureTable);

                    //document.Add(new Paragraph($"Inspeccións ID: {inspection.Id}"));
                    //document.Add(new Paragraph($"Número de Memo: {inspection.MemoNumber}"));
                    //document.Add(new Paragraph($"Fecha de Inspección: {inspection.InspectionDate:dd/MM/yyyy}"));
                    //document.Add(new Paragraph($"Operador: {inspection.OperatorName}"));
                    //document.Add(new Paragraph($"Observaciones: {inspection.FailureReport}"));
                    //document.Add(new Paragraph("🚀 PDF generado con éxito."));
                }

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
