using ClosedXML.Excel;
using ExcelDataReader;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using OfficeOpenXml;
using System.ComponentModel;
using System.Data;

namespace FAP.Common.Infrastructure.Files
{
    
    public class ExcelService : IExcelService
    {
        public async Task<List<T>> ImportAsync<T>(Stream fileStream, Func<DataRow, T> map)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(fileStream);
            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            var table = result.Tables[0];
            var list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(map(row));
            }

            return await Task.FromResult(list);
        }

        public async Task<List<UserImportDto>> ReadUsersAsync(Stream stream)
        {
            // Bắt buộc: ExcelDataReader yêu cầu đăng ký code page provider
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var users = new List<UserImportDto>();

            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true // hàng đầu tiên là tiêu đề cột
                }
            });

            var table = dataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                users.Add(new UserImportDto
                {
                    Login = row["UserName"]?.ToString() ?? "",
                    Fullname = row["FullName"]?.ToString() ?? "",
                    Email = row["Email"]?.ToString() ?? ""
                });
            }

            return await Task.FromResult(users);
        }

        public async Task<byte[]> ExportAsync<T>(IEnumerable<T> data, string sheetName)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);
            var props = typeof(T).GetProperties();

            // Header
            for (int i = 0; i < props.Length; i++)
                worksheet.Cell(1, i + 1).Value = props[i].Name;

            // Data
            int row = 2;
            foreach (var item in data)
            {
                for (int i = 0; i < props.Length; i++)
                    worksheet.Cell(row, i + 1).Value = (XLCellValue)props[i].GetValue(item);
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }
    }
}