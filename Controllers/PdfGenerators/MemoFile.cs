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
using iText.IO.Font;
using iText.Kernel.Geom;
using iText.Forms.Form.Element;

namespace WorkshopsGov.Controllers.PdfGenerators
{
    public class MemoFile
    {

        public void AddHeaderSection(Document document)
        {
            Table headerTable = new Table(3).UseAllAvailableWidth().SetMarginTop(-20);

            Image leftLogo = new Image(ImageDataFactory.Create("wwwroot/images/escudoBlack.png"))
                .SetWidth(75)
                .SetHorizontalAlignment(HorizontalAlignment.LEFT);

            Paragraph title = new Paragraph("DIRECCIÓN DE SERVICIOS GENERALES\nTALLERES MUNICIPALES")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(10)
                .SetMarginBottom(0);

            Paragraph subtitle = new Paragraph("DIAGNÓSTICO DE REVISIÓN MECÁNICA")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(-5);

            Div textContainer = new Div().Add(title).Add(subtitle);

            Table folioTable = new Table(1).UseAllAvailableWidth();
            folioTable.SetBorder(new SolidBorder(1));

            Paragraph folioTitle = new Paragraph("FOLIO")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER);

            Cell folioTitleCell = new Cell()
                .Add(folioTitle)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(1));

            Paragraph folioEmpty = new Paragraph(" ")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(10);

            Cell folioEmptyCell = new Cell()
                .Add(folioEmpty)
                .SetHeight(15)
                .SetBorder(new SolidBorder(1));

            folioTable.AddCell(folioTitleCell);
            folioTable.AddCell(folioEmptyCell);

            headerTable.AddCell(new Cell()
                .Add(leftLogo)
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                .SetPaddingLeft(-15));

            headerTable.AddCell(new Cell()
                .Add(textContainer)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(folioTable)
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetTextAlignment(TextAlignment.CENTER));

            headerTable.SetMarginBottom(10);
            document.Add(headerTable);

            int currentYear = DateTime.Now.Year;

            Paragraph dateParagraph = new Paragraph($"TIJUANA, B.C. A _____ DE _________________________ DE {currentYear}")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(-5);

            document.Add(dateParagraph);

            LineSeparator line = new LineSeparator(new SolidLine());

            Paragraph solicitudParagraph = new Paragraph("SOLICITUD DE DIAGNÓSTICO\r\n")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMargin(0)
                .SetPadding(0);

            Cell solicitudCell = new Cell()
                .Add(solicitudParagraph)
                .SetBackgroundColor(new DeviceRgb(165, 165, 165))
                .SetBorder(new SolidBorder(1))
                .SetPadding(2)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMargin(0);

            Table solicitudTable = new Table(1).UseAllAvailableWidth();
            solicitudTable.AddCell(solicitudCell);

            document.Add(line);
            document.Add(solicitudTable);
            document.Add(line);
        }

        private void AddVehicleCell(Table vehicleTable, string label, string value)
        {
            // Celda de la etiqueta (alineada a la derecha)
            vehicleTable.AddCell(new Cell()
                .Add(new Paragraph()
                    .Add(new Text(label)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(9))
                )
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.RIGHT)
            );

            // Celda del valor
            var valueCell = new Cell()
                .SetBorder(Border.NO_BORDER)
                .SetWidth(150);

            // Verificar si es "GASOLINA:"
            if (label == "GASOLINA: ")
            {
                Paragraph fuelMarkers = new Paragraph("\u00A0\u00A0\u00A0E         1/2         F")
                 .SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER))
                 .SetFontSize(9)
                 .SetTextAlignment(TextAlignment.LEFT)
                 .SetMarginBottom(-12);
                Paragraph fuelLine = new Paragraph("________|______________|_________")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.LEFT);

                // Agregar ambos párrafos a la celda
                valueCell.Add(fuelMarkers);
                valueCell.Add(fuelLine);


            }
            else
            {
                // Para otros campos, dividir el valor en partes de 14 caracteres
                for (int i = 0; i < value.Length; i += 14)
                {
                    string substring = value.Substring(i, Math.Min(14, value.Length - i));

                    // Agregar cada parte como un nuevo párrafo
                    valueCell.Add(new Paragraph(substring)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetMarginBottom(0));
                }

                // Agregar la línea solo si el valor tiene 14 caracteres o menos
                if (value.Length <= 14)
                {
                    valueCell.Add(new Paragraph("________________________")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(9)
                        .SetPaddingTop(-10));
                }
            }

            // Agregar la celda a la tabla
            vehicleTable.AddCell(valueCell);
        }

        private void AddFuelGauge(PdfDocument pdfDoc, Document document, float fuelLevel)
        {
            PdfPage page = pdfDoc.GetFirstPage();
            PdfCanvas canvas = new PdfCanvas(page);

            ImageData odometerImage = ImageDataFactory.Create("wwwroot/images/odometro.png");
            Image odometer = new Image(odometerImage)
                .SetWidth(60)
                .SetFixedPosition(253, 590);

            document.Add(odometer);

            float odometerX = 243;
            float odometerY = 590;

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


        public void AddDiagnosisAndObservationsSection(Document document)
        {
            // ==========================
            // DIAGNÓSTICO
            // ==========================
            Table diagnosticTable = new Table(1).UseAllAvailableWidth().SetHeight(100)
                .SetBorder(new SolidBorder(1))
                .SetBorderTop(Border.NO_BORDER);

            Paragraph diagnosticTitle = new Paragraph("DIAGNÓSTICO EMITIDO POR EL MECÁNICO DEL TALLER MUNICIPAL")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER);

            diagnosticTable.AddCell(new Cell()
                .Add(diagnosticTitle)
                .SetBorder(Border.NO_BORDER));

            for (int i = 0; i < 4; i++)
            {
                diagnosticTable.AddCell(new Cell()
                    .Add(new Paragraph("________________________________________________________________________________________")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(-8)
                        .SetMarginBottom(-8))
                    .SetBorder(Border.NO_BORDER));
            }

            document.Add(diagnosticTable);

            // ==========================
            // TIPO DE REVISIÓN
            // ==========================
            Table revisionTable = new Table(1).UseAllAvailableWidth().SetHeight(110)
                .SetBorder(new SolidBorder(1))
                .SetBorderTop(Border.NO_BORDER);

            Paragraph revisionTitle = new Paragraph("TIPO DE REVISIÓN SOLICITADA")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER);

            revisionTable.AddCell(new Cell()
                .Add(revisionTitle)
                .SetBorder(Border.NO_BORDER));

            for (int i = 0; i < 5; i++)
            {
                revisionTable.AddCell(new Cell()
                    .Add(new Paragraph("________________________________________________________________________________________")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(-4)
                        .SetMarginBottom(-4))
                    .SetBorder(Border.NO_BORDER));
            }

            document.Add(revisionTable);

            // ==========================
            // GARANTÍA
            // ==========================
            Table warrantyTable = new Table(1).UseAllAvailableWidth().SetHeight(50)
                .SetBorder(new SolidBorder(1))
                .SetBorderTop(Border.NO_BORDER);

            Table warrantyContentTable = new Table(new float[] { 25, 25, 50 }).UseAllAvailableWidth();

            warrantyContentTable.AddCell(new Cell()
                .Add(new Paragraph("GARANTÍA")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            Table checkBoxTable = new Table(new float[] { 8, 20, 8, 20 }).UseAllAvailableWidth();

            checkBoxTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(new SolidBorder(1)).SetHeight(8).SetWidth(8));
            checkBoxTable.AddCell(new Cell().Add(new Paragraph("SI").SetFontSize(9)).SetBorder(Border.NO_BORDER));
            checkBoxTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(new SolidBorder(1)).SetHeight(8).SetWidth(8));
            checkBoxTable.AddCell(new Cell().Add(new Paragraph("NO").SetFontSize(9)).SetBorder(Border.NO_BORDER));

            warrantyContentTable.AddCell(new Cell().Add(checkBoxTable).SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE));

            Table repairInlineTable = new Table(new float[] { 70, 30 }).UseAllAvailableWidth();
            repairInlineTable.SetBorder(Border.NO_BORDER);

            repairInlineTable.AddCell(new Cell()
                .Add(new Paragraph("Presenta reparación similar en los últimos 3 meses: ")
                    .SetFontSize(9)
                    .SetPaddingLeft(70)
                    .SetPaddingTop(5))
                .SetBorder(Border.NO_BORDER));

            Table repairCheckBoxTable = new Table(new float[] { 8, 15, 8, 15 }).UseAllAvailableWidth();
            repairCheckBoxTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(new SolidBorder(1)).SetHeight(8).SetWidth(8));
            repairCheckBoxTable.AddCell(new Cell().Add(new Paragraph("SI").SetFontSize(9)).SetBorder(Border.NO_BORDER).SetPaddingLeft(2));
            repairCheckBoxTable.AddCell(new Cell().Add(new Paragraph(" ")).SetBorder(new SolidBorder(1)).SetHeight(8).SetWidth(8));
            repairCheckBoxTable.AddCell(new Cell().Add(new Paragraph("NO").SetFontSize(9)).SetBorder(Border.NO_BORDER).SetPaddingLeft(2));

            repairInlineTable.AddCell(new Cell().Add(repairCheckBoxTable).SetBorder(Border.NO_BORDER).SetPadding(0).SetVerticalAlignment(VerticalAlignment.MIDDLE));

            warrantyContentTable.AddCell(new Cell().Add(repairInlineTable).SetBorder(Border.NO_BORDER).SetPadding(0));

            warrantyTable.AddCell(new Cell().Add(warrantyContentTable).SetBorder(Border.NO_BORDER));

            warrantyTable.AddCell(new Cell()
                .Add(new Paragraph("Especifique: ___________________________________________________________________________")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPaddingLeft(75)
                    .SetPaddingTop(-10))
                .SetBorder(Border.NO_BORDER));

            document.Add(warrantyTable);

            // ==========================
            // OBSERVACIONES
            // ==========================
            Table observationsTable = new Table(1).UseAllAvailableWidth()
                .SetBorder(new SolidBorder(1))
                .SetBorderTop(Border.NO_BORDER);

            observationsTable.AddCell(new Cell()
                .Add(new Paragraph($"OBSERVACIONES: {new string('_', 83)}")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPaddingLeft(10)
                    .SetPaddingTop(2))
                .SetBorder(Border.NO_BORDER)
                .SetPadding(0)
                .SetMargin(0)
                .SetHeight(20));

            for (int i = 0; i < 2; i++)
            {
                observationsTable.AddCell(new Cell()
                    .Add(new Paragraph(new string('_', 100))
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetPaddingLeft(10)
                        .SetPaddingTop(2))
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(0)
                    .SetMargin(0)
                    .SetHeight(15));
            }

            document.Add(observationsTable);

            // ==========================
            // ASIGNACIÓN / EXCLUSIVO
            // ==========================
            Paragraph exclusiveParagraph = new Paragraph("** EXCLUSIVO PARA JEFATURA DE TALLERES MUNICIPALES **")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER);

            Cell exclusiveCell = new Cell()
                .Add(exclusiveParagraph)
                .SetBackgroundColor(new DeviceRgb(165, 165, 165))
                .SetBorder(new SolidBorder(2))
                .SetPadding(2)
                .SetTextAlignment(TextAlignment.CENTER);

            Table exclusiveTable = new Table(1).UseAllAvailableWidth().SetMarginTop(5);
            exclusiveTable.AddCell(exclusiveCell);

            document.Add(exclusiveTable);

            Paragraph assignedParagraph = new Paragraph("LA UNIDAD HA SIDO ASIGNADA AL TALLER: ")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new LineSeparator(new SolidLine()).SetWidth(300));

            Table assignedTable = new Table(1).UseAllAvailableWidth();
            assignedTable.AddCell(new Cell()
                .Add(assignedParagraph)
                .SetBorder(new SolidBorder(1))
                .SetBorderTop(Border.NO_BORDER)
                .SetPadding(5)
                .SetTextAlignment(TextAlignment.LEFT));

            document.Add(assignedTable);
        }

        private void AddSignatureSection(Document document)
        {
            float[] signatureWidths = { 1, 1 };
            Table signatureTable = new Table(UnitValue.CreatePercentArray(signatureWidths)).UseAllAvailableWidth();

            signatureTable.AddCell(new Cell()
                .Add(new Paragraph("____________________________________"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(5)
                .SetBorder(Border.NO_BORDER));

            signatureTable.AddCell(new Cell()
                .Add(new Paragraph("____________________________________"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(5)
                .SetBorder(Border.NO_BORDER));

            signatureTable.AddCell(new Cell()
                .Add(new Paragraph("TÉCNICO VERIFICADOR / MECÁNICO"))
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(Border.NO_BORDER));

            signatureTable.AddCell(new Cell()
                .Add(new Paragraph("LIC. SAMUEL GARCIA RENTERIA\r\nJEFE DE TALLERES MUNICIPALES"))
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(Border.NO_BORDER));

            signatureTable.AddCell(new Cell()
                .Add(new Paragraph("____________________________________"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(10)
                .SetBorder(Border.NO_BORDER));

            Table infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 3 })).UseAllAvailableWidth();

            infoTable.AddCell(new Cell()
                .Add(new Paragraph("La unidad Ingresa:"))
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5)
                .SetBorder(new SolidBorder(1)));

            infoTable.AddCell(new Cell()
                .Add(new Paragraph(""))
                .SetBorder(new SolidBorder(1)));

            infoTable.AddCell(new Cell()
                .Add(new Paragraph("Fecha y hora:"))
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5)
                .SetBorder(new SolidBorder(1)));

            infoTable.AddCell(new Cell()
                .Add(new Paragraph(""))
                .SetBorder(new SolidBorder(1)));

            signatureTable.AddCell(new Cell()
                .Add(infoTable)
                .SetBorder(Border.NO_BORDER)
                .SetPaddingTop(0));

            signatureTable.AddCell(new Cell()
                .Add(new Paragraph("NOMBRE Y FIRMA DEL TALLER PARTICULAR"))
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.TOP)
                .SetPaddingTop(-20)
                .SetBorder(Border.NO_BORDER));

            signatureTable.AddCell(new Cell()
                .Add(new Paragraph(""))
                .SetBorder(Border.NO_BORDER));

            document.Add(signatureTable);
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

                    var memoFile = new MemoFile();


                    memoFile.AddHeaderSection(document);

                    // Tabla de Solicitud de diagnostico
                    Table mainTable = new Table(1).UseAllAvailableWidth().SetHeight(120);
                    mainTable.SetBorder(new SolidBorder(0));
                    mainTable.SetMarginTop(5);

                    Table leftTable = new Table(2).UseAllAvailableWidth();
                    Table rightTable = new Table(2).UseAllAvailableWidth();

                    memoFile.AddVehicleCell(leftTable, "OFICIALÍA: ", $"{inspection.Vehicle.Oficialia}");
                    memoFile.AddVehicleCell(leftTable, "PLACAS: ", $"{inspection.Vehicle.LicensePlate}");
                    memoFile.AddVehicleCell(leftTable, "DISTRITO: ", $"{inspection.Vehicle.LicensePlate}");
                    memoFile.AddVehicleCell(leftTable, "NO. DE MEMO: ", $"{inspection.Vehicle.LicensePlate}");
                    memoFile.AddVehicleCell(rightTable, "LINEA: ", $"{inspection.Vehicle.LicensePlate}");
                    memoFile.AddVehicleCell(rightTable, "MODELO: ", $"{inspection.Vehicle.Model.Name}");
                    memoFile.AddVehicleCell(rightTable, "MARCA: ", $"{inspection.Vehicle.Brand.Name}");
                    memoFile.AddVehicleCell(rightTable, "KILOMETRAJE: ", $"{inspection.Vehicle.LicensePlate}");
                    memoFile.AddVehicleCell(rightTable, "GASOLINA: ", " ");

                    Table contentTable = new Table(2).UseAllAvailableWidth();
                    contentTable.AddCell(new Cell().Add(leftTable).SetBorder(Border.NO_BORDER));
                    contentTable.AddCell(new Cell().Add(rightTable).SetBorder(Border.NO_BORDER));
                    mainTable.AddCell(new Cell().Add(contentTable).SetBorder(Border.NO_BORDER));
                    document.Add(mainTable);
                    //Gasolina odometro
                    memoFile.AddFuelGauge(pdf, document, inspection.FuelLevel);
                    // Párrafo centrado para el título
                    Paragraph inspectionParagraph = new Paragraph("INSPECCIÓN")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER);

                    Cell inspectionCell = new Cell()
                    .Add(inspectionParagraph)
                    .SetBackgroundColor(new DeviceRgb(165, 165, 165))
                    .SetBorder(new SolidBorder(2))
                    .SetPadding(2)
                    .SetTextAlignment(TextAlignment.CENTER);

                    Table inspectionTable = new Table(1).UseAllAvailableWidth()
                        .SetMarginTop(5);
                    inspectionTable.AddCell(inspectionCell);
                    document.Add(inspectionTable);


                    memoFile.AddDiagnosisAndObservationsSection(document);

                    memoFile.AddSignatureSection(document);

                    // *************** SEGUNDA HOJA  *************** 
                    PageSize landscapePage = PageSize.A4.Rotate();
                    pdf.AddNewPage(landscapePage);

            
                    Image leftLogo = new Image(ImageDataFactory.Create("wwwroot/images/logo_xxv.png"))
                        .SetWidth(200)
                        .SetHorizontalAlignment(HorizontalAlignment.LEFT);

                    Image rightLogo = new Image(ImageDataFactory.Create("wwwroot/images/logo_policia.jpeg"))
                        .SetWidth(70)
                        .SetHorizontalAlignment(HorizontalAlignment.RIGHT);

                    Paragraph centerTitle = new Paragraph("TALLER MUNICIPAL OM-SSPCM\nINVENTARIO DE UNIDAD")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(12);

                    Table headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 1 }))
                        .UseAllAvailableWidth()
                        .SetMarginBottom(10);

                    // Celdas sin bordes
                    headerTable.AddCell(new Cell().Add(leftLogo).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT));
                    headerTable.AddCell(new Cell().Add(centerTitle).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER));
                    headerTable.AddCell(new Cell().Add(rightLogo).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
                    document.Add(headerTable);


                    // Tabla contenedora con 3 columnas
                    Table inspeccionTable = new Table(UnitValue.CreatePercentArray(new float[] { 30, 40, 30 })) // Ajusté los porcentajes de ancho
                        .UseAllAvailableWidth()
                        .SetHeight(320)
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.7f));

                    // ========== COLUMNA: REVISION EN EXTERIOR ==========
                    Table exteriorTable = new Table(3).UseAllAvailableWidth();

                    // Título con solo borde derecho e inferior
                    Cell exteriorTitleCell = new Cell(1, 3)
                        .Add(new Paragraph("REVISION EN EXTERIOR")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                            .SetFontSize(9)
                            .SetTextAlignment(TextAlignment.CENTER))
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(new SolidBorder(ColorConstants.BLACK, 0.7f))
                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.7f))
                        .SetPadding(4);
                    exteriorTable.AddCell(exteriorTitleCell);

                    // Fila de encabezado vacía - SI - NO
                    exteriorTable.AddCell(new Cell().SetBorder(Border.NO_BORDER)); // Celda vacía
                    exteriorTable.AddCell(new Cell()
                        .Add(new Paragraph("SI")
                            .SetFontSize(8)
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                            .SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER));
                    exteriorTable.AddCell(new Cell()
                        .Add(new Paragraph("NO")
                            .SetFontSize(8)
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                            .SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER));

                    // Ítems
                    string[] exteriorItems = { "FOCOS", "MICAS DELANTERAS", "DEFENSA", "PARRILLA", "COFRE", "BURRERA", "TORRETA", "PARABRISAS", "GUARDAFANGOS (DER)", "ESPEJO (DER)", "PUERTA (DER)", "VENTANA (DER)",
                        "VIDRIO POSTERIOR", "TAPA TRASERA", "FOCOS POSTERIORES", "MICAS POSTERIORES", "DEFENSA POSTERIOR", "GUARDAFANGOS (IZQ)", "ESPEJO (IZQ)", "PUERTA (IZQ)", "VENTANA (IZQ)", "LLANTAS", "LLANTA EXTRA", "ANTENA", "COPAS", "PLACAS" };
                    foreach (var item in exteriorItems)
                    {
                        exteriorTable.AddCell(new Cell()
                            .Add(new Paragraph(item).SetFontSize(8).SetMargin(0).SetMultipliedLeading(0.9f))
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(0));
                        exteriorTable.AddCell(new Cell()
                            .Add(new Paragraph("O").SetFontSize(8).SetTextAlignment(TextAlignment.CENTER).SetMargin(0).SetMultipliedLeading(0.9f))
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(0));
                        exteriorTable.AddCell(new Cell()
                            .Add(new Paragraph("O").SetFontSize(8).SetTextAlignment(TextAlignment.CENTER).SetMargin(0).SetMultipliedLeading(0.9f))
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(0));
                    }

                    Cell colExterior = new Cell().Add(exteriorTable).SetBorder(Border.NO_BORDER);


                    // ========== COLUMNA: CARROCERÍA ==========
                    Table carroceriaTable = new Table(1).UseAllAvailableWidth();
                    carroceriaTable.AddCell(new Cell()
                        .Add(new Paragraph("CARROCERÍA")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                            .SetFontSize(9)
                            .SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER)
                        .SetPaddingBottom(5));

                    // Aquí irían imágenes de los laterales
                    Image lateralImg = new Image(ImageDataFactory.Create("wwwroot/images/lateral_frontal.png"))
                        .SetWidth(50)
                        .SetAutoScale(true);

                    carroceriaTable.AddCell(new Cell().Add(lateralImg).SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER));

                    Cell colCarroceria = new Cell().Add(carroceriaTable).SetBorder(Border.NO_BORDER);


                    // ========== COLUMNA: REVISION EN INTERIOR ==========
                    Table interiorTable = new Table(3).UseAllAvailableWidth();

                    Cell interiorTitleCell = new Cell(1, 3)
                         .Add(new Paragraph("REVISION EN INTERIOR")
                             .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                             .SetFontSize(9)
                             .SetTextAlignment(TextAlignment.CENTER))
                         .SetBorderTop(Border.NO_BORDER)
                         .SetBorderRight(Border.NO_BORDER)
                         .SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 0.7f))
                         .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.7f))
                         .SetPadding(4);
                    interiorTable.AddCell(interiorTitleCell);

                    // Encabezados SI/NO correctamente alineados
                    interiorTable.AddCell(new Cell().Add(new Paragraph("").SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(0).SetMarginBottom(0)).SetBorder(Border.NO_BORDER).SetPaddingTop(0).SetPaddingBottom(0));
                    interiorTable.AddCell(new Cell().Add(new Paragraph("SI").SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(0).SetMarginBottom(0)).SetBorder(Border.NO_BORDER).SetPaddingTop(0).SetPaddingBottom(0));
                    interiorTable.AddCell(new Cell().Add(new Paragraph("NO").SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(0).SetMarginBottom(0)).SetBorder(Border.NO_BORDER).SetPaddingTop(0).SetPaddingBottom(0));

                    // Lista de ítems para revisión interior
                    string[] interiorItems = {
                        "RADIO", "SIRENA", "BOCINA", "ESPEJO RETROVISOR", "LLAVE P/GASOLINA",
                        "DADO/EXTRA", "BARILLA PARA DADO", "GATO MECANICO", "CABLES P/CORRIENTE",
                        "SEÑALES MUERTAS", "EXTINTOR"
                    };

                    foreach (var item in interiorItems)
                    {
                        interiorTable.AddCell(new Cell().Add(new Paragraph(item).SetFontSize(8).SetMargin(0).SetPadding(0).SetMultipliedLeading(1f)).SetBorder(Border.NO_BORDER).SetPadding(0).SetMargin(0).SetHeight(10));
                        interiorTable.AddCell(new Cell().Add(new Paragraph("O").SetFontSize(9).SetTextAlignment(TextAlignment.CENTER).SetMargin(0).SetPadding(0).SetMultipliedLeading(1f)).SetBorder(Border.NO_BORDER).SetPadding(0).SetMargin(0).SetHeight(10));
                        interiorTable.AddCell(new Cell().Add(new Paragraph("O").SetFontSize(9).SetTextAlignment(TextAlignment.CENTER).SetMargin(0).SetPadding(0).SetMultipliedLeading(1f)).SetBorder(Border.NO_BORDER).SetPadding(0).SetMargin(0).SetHeight(10));
                    }

                    // ========== SECCIÓN CALCOMONIAS ==========
                    Table calcomaniasTable = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 })).UseAllAvailableWidth();
                    calcomaniasTable.AddCell(new Cell()
                        .Add(new Paragraph("CALCOMONIAS EN AMBOS LADOS DE")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            .SetFontSize(8)
                            .SetMultipliedLeading(1f)
                            .SetMargin(0)
                            .SetPadding(0))
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetPadding(0)); 
                    calcomaniasTable.AddCell(new Cell().SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)).SetPadding(0).SetHeight(8));

                    // ========== SECCIÓN OFICIALÍA MAYOR ==========
                    Table oficialiaTable = new Table(new float[] { 55, 8, 8 }).UseAllAvailableWidth();

                    new[] { "OFICIALIA MAYOR", "LOGOTIPOS DE POLICIA" }.ToList().ForEach(texto =>
                    {
                        oficialiaTable.AddCell(new Cell()
                            .Add(new Paragraph(texto)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                .SetFontSize(8)
                                .SetMargin(0)
                                .SetMultipliedLeading(0.9f)) // Compacta altura de línea
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(0));

                        Enumerable.Range(0, 2).ToList().ForEach(_ =>
                            oficialiaTable.AddCell(new Cell()
                                .Add(new Paragraph("O")
                                    .SetFontSize(9)
                                    .SetTextAlignment(TextAlignment.LEFT)
                                    .SetMargin(0)
                                    .SetMultipliedLeading(1))
                                .SetBorder(Border.NO_BORDER)
                                .SetPadding(0)));


                    });

                    // ========== SECCIÓN NIVELES DE ACEITE ==========
                    Table nivelesTable = new Table(4).UseAllAvailableWidth();

                    // Título que ocupa 4 columnas
                    nivelesTable.AddCell(new Cell(1, 4)
                        .Add(new Paragraph("NIVELES DE ACEITE")
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                            .SetFontSize(8)
                            .SetMargin(0)
                            .SetMultipliedLeading(1))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)));

                    // Encabezados
                    nivelesTable.AddCell(new Cell().SetBorder(Border.NO_BORDER)); // Celda vacía

                    nivelesTable.AddCell(new Cell()
                        .Add(new Paragraph("BAJO").SetFontSize(7).SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)));

                    nivelesTable.AddCell(new Cell()
                        .Add(new Paragraph("NORMAL").SetFontSize(7).SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)));

                    nivelesTable.AddCell(new Cell()
                        .Add(new Paragraph("ALTO").SetFontSize(7).SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)));

                    string[] nivelesItems = { "MOTOR", "TRANSMISION", "DIRECCION", "AGUA" };

                    foreach (var item in nivelesItems)
                    {
                        nivelesTable.AddCell(new Cell().Add(new Paragraph(item).SetFontSize(8).SetMargin(0).SetPadding(0).SetMultipliedLeading(0.9f)).SetBorder(Border.NO_BORDER).SetPadding(0));
                      
                        for (int i = 0; i < 3; i++)
                        {
                            nivelesTable.AddCell(new Cell().Add(new Paragraph("O").SetFontSize(10).SetTextAlignment(TextAlignment.CENTER).SetMargin(0).SetPadding(0).SetMultipliedLeading(0.9f)).SetBorder(Border.NO_BORDER).SetPadding(0));
                        }
                    }





                    // ========== SECCIÓN OTROS ==========
                    Table replicaTable = new Table(4).UseAllAvailableWidth();

                    // Encabezados súper compactos
                    replicaTable.AddCell(new Cell()
                        .Add(new Paragraph("OTROS")
                            .SetFontSize(7)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMargin(0)
                            .SetMultipliedLeading(0.9f))
                        .SetBorderTop(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderRight(Border.NO_BORDER)
                        .SetPadding(0));

                    replicaTable.AddCell(new Cell()
                        .Add(new Paragraph("BUENA")
                            .SetFontSize(7)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMargin(0)
                            .SetMultipliedLeading(0.9f))
                        .SetBorderTop(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetPadding(0));

                    replicaTable.AddCell(new Cell()
                        .Add(new Paragraph("REGULAR")
                            .SetFontSize(7)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMargin(0)
                            .SetMultipliedLeading(0.9f))
                        .SetBorderTop(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetPadding(0));

                    replicaTable.AddCell(new Cell()
                        .Add(new Paragraph("MALA")
                            .SetFontSize(7)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMargin(0)
                            .SetMultipliedLeading(0.9f))
                        .SetBorderTop(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderRight(new SolidBorder(ColorConstants.BLACK, 0.5f))
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetPadding(0));

                    string[] replicaItems = { "CARROCERIA", "TAPICERIA" };

                    foreach (var item in replicaItems)
                    {
                        replicaTable.AddCell(new Cell()
                            .Add(new Paragraph(item)
                                .SetFontSize(8)
                                .SetMargin(0)
                                .SetPadding(0)
                                .SetMultipliedLeading(0.9f))
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(0));

                        for (int i = 0; i < 3; i++)
                        {
                            replicaTable.AddCell(new Cell()
                                .Add(new Paragraph("O")
                                    .SetFontSize(10)
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetMargin(0)
                                    .SetPadding(0)
                                    .SetMultipliedLeading(0.9f))
                                .SetBorder(Border.NO_BORDER)
                                .SetPadding(0));
                        }
                    }



                    // ========== ENSAMBLAR TODO ==========
                    Cell colInterior = new Cell()
                        .Add(interiorTable.SetMarginBottom(5))
                        .Add(calcomaniasTable.SetMarginBottom(5))
                        .Add(oficialiaTable.SetMarginBottom(5))
                        .Add(nivelesTable.SetMarginBottom(5))
                        .Add(replicaTable)
                        .SetBorder(Border.NO_BORDER);

                    // ========== Ensamblar la fila ==========
                    inspeccionTable.AddCell(colExterior);
                    inspeccionTable.AddCell(colCarroceria);
                    inspeccionTable.AddCell(colInterior);

                    document.Add(inspeccionTable);


                    // ========== OBSERVACIONES Y ELABORADO POR (1 COLUMNA) ==========
                    Table observacionesTable = new Table(1).UseAllAvailableWidth()
                      .SetMarginBottom(10)
                      .SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 0.7f))
                      .SetBorderRight(new SolidBorder(ColorConstants.BLACK, 0.7f))
                      .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.7f))
                      .SetBorderTop(Border.NO_BORDER); 

                    Paragraph observacionesLine = new Paragraph("OBSERVACIONES: ________________________________________________________________________________________________________________________________________________________")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetMarginTop(10)       // 👈 Más espacio arriba
                        .SetMarginBottom(8)    // 👈 Más espacio abajo
                        .SetPaddingLeft(10);   // Sangría a la izquierda


                    Paragraph elaboradoLine = new Paragraph("ELABORADO POR: ________________________________________________________________________________________________________________________________________________________")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetMarginTop(10)      // 👈 Más espacio arriba
                        .SetMarginBottom(8)    // 👈 Más espacio abajo
                        .SetPaddingLeft(10);   // Sangría a la izquierda

                    observacionesTable.AddCell(new Cell()
                        .Add(observacionesLine)
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(0));

                    observacionesTable.AddCell(new Cell()
                        .Add(elaboradoLine)
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(0));

                    document.Add(observacionesTable);
                    document.Close();
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
