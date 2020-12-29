using ExcelProject.Models;
using FastMember;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetPdf()
        {

            DataTable dataTable = new DataTable();
            dataTable.Load(ObjectReader.Create(new List<customer>
            {

                new customer{Id=1,Name="Erkan"},
                new customer{Id=2, Name="Aybüke"}
            }));

            string fileName = Guid.NewGuid()+ ".pdf";
            string path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/documents/"+fileName);
            var stream = new FileStream(path, FileMode.Create);

            Document document = new Document(PageSize.A4,25f,25f,25f,25f);

            PdfWriter.GetInstance(document, stream);

            document.Open();

            //Paragraph paragraph = new Paragraph("Erkan Çömez");
            PdfPTable pdfPTable = new PdfPTable(dataTable.Columns.Count);

            //pdfPTable.AddCell("Ad");
            //pdfPTable.AddCell("Soyad");
            //pdfPTable.AddCell("Erkan");
            //pdfPTable.AddCell("Çömez");

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                pdfPTable.AddCell(dataTable.Columns[i].ColumnName);
            }


            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    pdfPTable.AddCell(dataTable.Rows[i][j].ToString());
                }
            }

            document.Add(pdfPTable);

            document.Close();

            return File("/documents/"+fileName,"application/pdf",fileName);
        }

        public IActionResult GetExcel()
        {
            ExcelPackage excelPackage = new ExcelPackage();

            var excelBlank = excelPackage.Workbook.Worksheets.Add("sayfa1");
            //excelBlank.Cells[1, 1].Value = "Ad";
            //excelBlank.Cells[1, 2].Value = "Soyad";

            //excelBlank.Cells[2, 1].Value = "Erkan";
            //excelBlank.Cells[2, 2].Value = "Çömez";

            excelBlank.Cells["A1"].LoadFromCollection(new List<customer>
            {

                new customer{Id=1,Name="Erkan"},
                new customer{Id=2, Name="Aybüke"}
            },true,OfficeOpenXml.Table.TableStyles.Light15);

            var bytes = excelPackage.GetAsByteArray();

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Guid.NewGuid() + "" + ".xlsx");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
