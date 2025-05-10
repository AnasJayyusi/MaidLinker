using MaidLinker.Data.Entites;
using MaidLinker.Models;
using NPOI.SS.Formula.Functions;

namespace MaidLinker.Tools
{
    public class ExcelReportGenerator
    {
        //ToDo:Should Be Dynamic
        public string GenerateReport(List<ServicesReport> servicesReports, double vatValue, double sitePercentage)
        {
            // Prepare Stream
            MemoryStream workStream = new MemoryStream();

            // Create a new Excel workbook and worksheet
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            var sheet = workbook.CreateSheet("Invoices");




            // Create header row
            var headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("#");
            headerRow.CreateCell(1).SetCellValue("Created By");
            headerRow.CreateCell(2).SetCellValue("Assigned To");
            headerRow.CreateCell(3).SetCellValue("Title (Arabic)");
            headerRow.CreateCell(4).SetCellValue("Title (English)");
            headerRow.CreateCell(5).SetCellValue("Price");
            headerRow.CreateCell(6).SetCellValue("Fee");
            headerRow.CreateCell(7).SetCellValue($"Fee + ({vatValue})% Vat");
            headerRow.CreateCell(8).SetCellValue("\tMaidLinker Percentage (12%)");


            // Fill data rows
            var data = servicesReports;
            int rowIndex = 1;
            var sitePercentageValue = sitePercentage / 100;
            var vatValueInPrentage = vatValue / 100;
            foreach (var item in data)
            {
                var dataRow = sheet.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue(rowIndex - 1);
                dataRow.CreateCell(1).SetCellValue(item.CreatedByUserName);
                dataRow.CreateCell(2).SetCellValue(item.AssignedToUserName);
                dataRow.CreateCell(3).SetCellValue(item.ServiceNameAr);
                dataRow.CreateCell(4).SetCellValue(item.ServiceNameEn);
                dataRow.CreateCell(5).SetCellValue(item.Price);
                dataRow.CreateCell(6).SetCellValue(item.Fee);
                dataRow.CreateCell(7).SetCellValue(item.Fee + (item.Fee * vatValueInPrentage));
                dataRow.CreateCell(8).SetCellValue((item.Fee + (item.Fee * vatValueInPrentage)) * sitePercentageValue);
            }

            sheet.Autobreaks = true;
            int columnLength = 8;
            for (int i = 0; i <= columnLength; i++)
            {
                sheet.AutoSizeColumn(i);
                sheet.AutoSizeRow(i);
            }

            // Create a temporary file path
            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".xlsx");

            // Save the Excel workbook to the temporary file
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                workbook.Write(fileStream);
            }
            return tempFilePath;
        }
    }
}
