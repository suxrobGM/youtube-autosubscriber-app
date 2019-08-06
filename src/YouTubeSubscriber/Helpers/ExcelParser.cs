using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using YouTubeSubscriber.Models;

namespace YouTubeSubscriber.Helpers
{
    public static class ExcelParser
    {
        public static List<Account> ParseAccounts(string excelFilePath)
        {
            var usernamesPair = new List<Account>();
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets.First();
                int rowCount = worksheet.Dimension.End.Row;
                int columnCount = 2; 
                var dataCells = worksheet.Cells[1, 1, rowCount, columnCount];

                for (int i = 1; i <= rowCount; i++)
                {
                    var rowCells = dataCells[i, 1, i, columnCount];
                    var username = rowCells[i, 1].Value.ToString().Trim();
                    var password = rowCells[i, 2].Value.ToString().Trim();
                    usernamesPair.Add(new Account()
                    {
                        Email = username,
                        Password = password
                    });
                }
            }
            return usernamesPair;
        }
    }
}
