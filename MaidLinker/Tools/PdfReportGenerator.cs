using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text;
using MaidLinker.Dto;

namespace MaidLinker.Tools
{
    public class PdfReportGenerator
    {
        private Font _font;
        public MemoryStream GenerateReport(string imagePath, dynamic reportDto, ReportTypeEnum reportType, bool includeMasterDetails = true)
        {
            // Prepare Stream
            MemoryStream workStream = new MemoryStream();


            int columnCount = GetTableColumnsCount(reportType);
            // Prepare Document to write on it
            Document doc = new Document(PageSize.A4.Rotate());
            PdfPTable tableLayout = new PdfPTable(columnCount);

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            EmbedFont();
            // Create Header Logo + Master Details
            AddLogoImage(doc, imagePath);
            AddBreakLine(doc);

            if (includeMasterDetails)
            {
                BuildMasterDetails(doc, reportDto.MasterDetails);
                AddBreakLine(doc);
            }

            // Prepare For Table
            Paragraph tableParagraph = new Paragraph();
            tableParagraph.SpacingAfter = 12;
            doc.Add(tableParagraph);


            // Filling Table 
            if (reportType == ReportTypeEnum.None)
                doc.Add(BuildTable(tableLayout, reportDto.DataTable));

            if (reportType == ReportTypeEnum.ServiceProviderReportDto)
                doc.Add(BuildServiceProviderReportTable(tableLayout, reportDto.DataTable));

            if (reportType == ReportTypeEnum.BeneficiaryReportDto)
                doc.Add(BuildBeneficiaryReportTable(tableLayout, reportDto.DataTable));

            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return workStream;
        }


        private int GetTableColumnsCount(ReportTypeEnum reportType)
        {
            if (reportType == ReportTypeEnum.ServiceProviderReportDto)
            {
                Type type = typeof(ServiceProviderDataTableDto);
                return type.GetProperties().Length;

            }
            if (reportType == ReportTypeEnum.BeneficiaryReportDto)
            {
                Type type = typeof(BeneficiaryDataTableDto);
                return type.GetProperties().Length;
            }
            else
            {
                Type type = typeof(DataTableDto);
                return type.GetProperties().Length;
            }
        }
        private void EmbedFont()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //ToDo : Should Have I Place In Same Project 
            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\Tahoma.ttf";
            BaseFont bf = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            _font = new Font(bf, 12);
        }
        private void AddLogoImage(Document doc, string imagePath)
        {
            // Load your image
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imagePath);

            // Resize image depend upon your need
            jpg.ScaleToFit(140f, 120f);

            //Give space before image
            jpg.SpacingBefore = 10f;

            // Give some space after the image
            jpg.SpacingAfter = 2f;
            jpg.Alignment = Element.ALIGN_LEFT;
            doc.Add(jpg);
        }
        private void BuildMasterDetails(Document doc, MasterDetailsDto masterDetails)
        {
            // It working with arabic 
            PdfPTable table = new PdfPTable(1); // a table with 1 cell
            table.WidthPercentage = 100;
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL; // can also be set on the cell


            // Date 
            PdfPCell dateCell = new PdfPCell(new Phrase(string.Format($"Date : {masterDetails.OrderDate} "), _font));
            dateCell.HorizontalAlignment = Element.ALIGN_LEFT;
            dateCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(dateCell);

            // Invoice Number Cell
            PdfPCell invoiceNoCell = new PdfPCell(new Phrase(string.Format($"Invoice Number:{masterDetails.InvoiceNumber} "), _font));
            invoiceNoCell.HorizontalAlignment = Element.ALIGN_LEFT;
            invoiceNoCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(invoiceNoCell);

            AddLine(table);

            // Doctor Name 
            PdfPCell doctorCell = new PdfPCell(new Phrase(string.Format($"Doctor Name:{masterDetails.DoctorName}"), _font));
            doctorCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            doctorCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(doctorCell);




            // Clinic Name
            PdfPCell clinicNameCell = new PdfPCell(new Phrase(string.Format($"Clinic Name:{masterDetails.ClinicName}"), _font));
            clinicNameCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            clinicNameCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(clinicNameCell);

            // Request From 
            PdfPCell requestFromCell = new PdfPCell(new Phrase(string.Format($"Request From:{masterDetails.RequestFrom}"), _font));
            requestFromCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            requestFromCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(requestFromCell);


            // Patient's Name
            PdfPCell PatientNameCell = new PdfPCell(new Phrase(string.Format($"Patient's Name:{masterDetails.PatientName}"), _font));
            PatientNameCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            PatientNameCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(PatientNameCell);

            doc.Add(table);


        }
        protected PdfPTable BuildTable(PdfPTable tableLayout, List<DataTableDto> dataSource)
        {
            float[] headers = { 11, 35, 7, 8, 8, 8, 11, 12 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            var count = 1;

            //Add header  
            AddCellToHeader(tableLayout, "Service Code");
            AddCellToHeader(tableLayout, "Service Description");
            AddCellToHeader(tableLayout, "Qty");
            AddCellToHeader(tableLayout, "Unit Price    ");
            AddCellToHeader(tableLayout, "Total");
            AddCellToHeader(tableLayout, "Vat %");
            AddCellToHeader(tableLayout, "Vat Value");
            AddCellToHeader(tableLayout, "Net With Vat");

            foreach (var data in dataSource)
            {
                if (count >= 1)
                {
                    //Add body  
                    AddCellToBody(tableLayout, data.ServiceCode.ToString(), count);
                    AddCellToBody(tableLayout, data.ServiceDesc.ToString(), count);
                    AddCellToBody(tableLayout, data.Qty.ToString(), count);
                    AddCellToBody(tableLayout, data.UnitPrice.ToString(), count);
                    AddCellToBody(tableLayout, data.Total.ToString(), count);
                    AddCellToBody(tableLayout, data.VatPercentage.ToString(), count);
                    AddCellToBody(tableLayout, data.VatValue.ToString(), count);
                    AddCellToBody(tableLayout, data.NetWithVat.ToString(), count);
                    count++;
                }
            }
            return tableLayout;
        }

        protected PdfPTable BuildServiceProviderReportTable(PdfPTable tableLayout, List<ServiceProviderDataTableDto> dataSource)
        {
            float[] headers = { 11, 35, 7, 15, 31}; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            var count = 1;

            //Add header  
            AddCellToHeader(tableLayout, "Service Code");
            AddCellToHeader(tableLayout, "Service Description");
            AddCellToHeader(tableLayout, "Qty");
            AddCellToHeader(tableLayout, "Service Fee");
            AddCellToHeader(tableLayout, "Total amount to the Befeciary");

            foreach (var data in dataSource)
            {
                if (count >= 1)
                {
                    //Add body  
                    AddCellToBody(tableLayout, data.ServiceCode.ToString(), count);
                    AddCellToBody(tableLayout, data.ServiceDesc.ToString(), count);
                    AddCellToBody(tableLayout, data.Qty.ToString(), count);
                    AddCellToBody(tableLayout, data.ServiceFee.ToString(), count);
                    AddCellToBody(tableLayout, data.Total.ToString(), count);
                    count++;
                }
            }
            return tableLayout;
        }


        protected PdfPTable BuildBeneficiaryReportTable(PdfPTable tableLayout, List<BeneficiaryDataTableDto> dataSource)
        {
            float[] headers = { 8, 30, 6, 12, 8, 8, 10, 10,8 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            var count = 1;

            //Add header  
            AddCellToHeader(tableLayout, "Service Code");
            AddCellToHeader(tableLayout, "Service Description");
            AddCellToHeader(tableLayout, "Qty");
            AddCellToHeader(tableLayout, "Beneficiary Part");
            AddCellToHeader(tableLayout, "Total");
            AddCellToHeader(tableLayout, "Platform Fee");
            AddCellToHeader(tableLayout, "Vat %");
            AddCellToHeader(tableLayout, "T.P.F Fee");
            AddCellToHeader(tableLayout, "Net Dr.Part");

            foreach (var data in dataSource)
            {
                if (count >= 1)
                {
                    //Add body  
                    AddCellToBody(tableLayout, data.ServiceCode.ToString(), count);
                    AddCellToBody(tableLayout, data.ServiceDesc.ToString(), count);
                    AddCellToBody(tableLayout, data.Qty.ToString(), count);
                    AddCellToBody(tableLayout, data.BeneficiaryPart.ToString("F2"), count);
                    AddCellToBody(tableLayout, data.Total.ToString("F2"), count);
                    AddCellToBody(tableLayout, data.PlatformFee.ToString("F2"), count);
                    AddCellToBody(tableLayout, data.VatPercentage.ToString("F2"), count);
                    AddCellToBody(tableLayout, data.TotalPlatformFee.ToString("F2"), count);
                    AddCellToBody(tableLayout, data.NetDrPart.ToString("F2"), count);
                    count++;
                }
            }
            return tableLayout;
        }
        private void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, _font))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 8,
                BackgroundColor = new BaseColor(255, 255, 255)
            });
        }

        private void AddCellToBody(PdfPTable tableLayout, string cellText, int count)
        {
            if (count % 2 == 0)
            {
                if (cellText.Equals("Total Amount") || cellText.Equals("#"))
                {
                    tableLayout.AddCell(new PdfPCell(new Phrase(cellText, _font))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8,
                        BackgroundColor = new BaseColor(255, 255, 255),
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                        Border = iTextSharp.text.Rectangle.NO_BORDER
                    });
                }
                else
                {
                    tableLayout.AddCell(new PdfPCell(new Phrase(cellText, _font))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8,
                        BackgroundColor = new BaseColor(255, 255, 255),
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    });
                }

            }
            else
            {
                if (cellText.Equals("Total Amount") || cellText.Equals("#"))
                {
                    tableLayout.AddCell(new PdfPCell(new Phrase(cellText, _font))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8,
                        BackgroundColor = new BaseColor(255, 255, 255),
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                        Border = iTextSharp.text.Rectangle.NO_BORDER
                    });
                }
                else
                {
                    tableLayout.AddCell(new PdfPCell(new Phrase(cellText, _font))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8,
                        BackgroundColor = new BaseColor(211, 211, 211),
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL
                    });
                }
            }
        }


        private void AddBreakLine(Document document, int repateLine = 1)
        {
            for (int i = 1; i <= repateLine; i++)
            {
                document.Add(new Paragraph(" "));
            }
        }

        private void AddLine(PdfPTable table)
        {
            PdfPCell emptyCell = new PdfPCell(new Phrase(string.Format("----------------------------"), _font));
            emptyCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.AddCell(emptyCell);
        }


    }
}
