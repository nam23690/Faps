using FAP.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Interfaces
{
    public interface IExcelService
    {
        Task<List<T>> ImportAsync<T>(Stream fileStream, Func<DataRow, T> map);
        Task<List<UserImportDto>> ReadUsersAsync(Stream stream);
        Task<byte[]> ExportAsync<T>(IEnumerable<T> data, string sheetName);
    }
}
