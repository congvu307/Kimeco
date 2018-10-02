using Kimeco_ASP.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kimeco_ASP.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {

        KimecoEntities db = new KimecoEntities();
        public Decimal ConvertDecimal (ICell content)
        {
            if ((content == null) || (content.ToString() == "")||(!Decimal.TryParse(content.ToString(), out Decimal tempDecimal)))
            {
                return 0;
            }
            return Decimal.Parse(content.ToString());
        }
        [HttpGet]
        public ActionResult Cash_MultiUpload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Cash_MultiUpload(HttpPostedFileBase file)
        {
            string _fileNameToSave = Path.Combine(Server.MapPath("~/App_Data/uploads"), "test.xlsx");

            //đọc file upload
            FileStream fsIndex = new FileStream(_fileNameToSave, FileMode.Open);
            XSSFWorkbook wbIndex = new XSSFWorkbook(fsIndex);
            ISheet sheetIndex = wbIndex.GetSheetAt(0);
            var rowIndex = sheetIndex.GetRow(0);

            //đọc file template
            string _TemplatePath = Path.Combine(Server.MapPath("~/App_Data/templates"), "InputCash_Template.xlsx");
            FileStream fsTemplate = new FileStream(_TemplatePath, FileMode.Open);
            XSSFWorkbook wbTemplate = new XSSFWorkbook(fsTemplate);
            ISheet sheetTemplate = wbTemplate.GetSheetAt(0);
            var rowTemplate = sheetTemplate.GetRow(0);
            for (int i = 0; i <= 10; i++)
            {
                if (rowTemplate.GetCell(0).StringCellValue != rowIndex.GetCell(0).StringCellValue)
                {
                    ViewBag.Message = "Wrong template format!";
                    return View();
                }
            }

            var listProject = db.Projects.ToList();
            var listCash = db.Cashes.ToList();
            for (int i = 1; i < sheetIndex.LastRowNum; i++)
            {
                //dòng hiện tại
                var rowNow = sheetIndex.GetRow(i);
                if ((rowNow.Cells.All(d => d.CellType == CellType.Blank)) ||(rowNow == null)) continue;
                Cash cash = new Cash();
                cash.C_Date = rowNow.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Replace(" ", String.Empty);
                cash.Company = rowNow.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                if (listProject.Any(x=>x.Name == rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim()))
                {
                    cash.ProjectName = listProject.FirstOrDefault(x=>x.Name==rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim()).ID;
                }
                else
                {
                    ViewBag.Message = "Doesn't exist " + rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString() + " in list Project (at line "+ i.ToString() +" )! Update cancelled";
                    return View();
                }
                cash.Staff = rowNow.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                cash.C_Content = rowNow.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                cash.Input = ConvertDecimal(rowNow.GetCell(5));
                cash.Output = ConvertDecimal(rowNow.GetCell(6));
                cash.Invoice = rowNow.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                cash.Ref = rowNow.GetCell(8,MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                db.Cashes.Add(cash);
            }
            ViewBag.Message = "Cập nhật thành công! <a class=\"btn\" href=\"/admin/home\">";
            db.SaveChanges();
            return View();

        }

        [HttpGet]
        public ActionResult SiteCost_Upload()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult SiteCost_Upload(HttpPostedFileBase file)
        {
            string _fileNameToSave = Path.Combine(Server.MapPath("~/App_Data/uploads"), "cost_test.xlsx");
            var listProject = db.Projects.ToList();
            //đọc file upload
            FileStream fsIndex = new FileStream(_fileNameToSave, FileMode.Open);
            XSSFWorkbook wbIndex = new XSSFWorkbook(fsIndex);
            ISheet sheetIndex = wbIndex.GetSheetAt(0);
            var rowIndex = sheetIndex.GetRow(1);

            //đọc file template
            string _TemplatePath = Path.Combine(Server.MapPath("~/App_Data/templates"), "SiteCost_Template.xlsx");
            FileStream fsTemplate = new FileStream(_TemplatePath, FileMode.Open);
            XSSFWorkbook wbTemplate = new XSSFWorkbook(fsTemplate);
            ISheet sheetTemplate = wbTemplate.GetSheetAt(0);
            var rowTemplate = sheetTemplate.GetRow(1);
            for (int i = 0; i <= 10; i++)
            {
                if (rowTemplate.GetCell(0).StringCellValue != rowIndex.GetCell(0).StringCellValue)
                {
                    ViewBag.Message = "Wrong template format!";
                    return View();
                }
            }
            for (int i = 2;i < sheetIndex.LastRowNum; i++)
            {
                var rowNow = sheetIndex.GetRow(i);
                if ((rowNow.Cells.All(d => d.CellType == CellType.Blank)) || (rowNow == null)) continue;
                Cost cost = new Cost();
                try
                {
                    string date = rowNow.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                    cost.Date = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Lỗi định dạng ngày tại dòng " + i.ToString();
                    return View();
                }
                cost.Conpany = rowNow.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                var projectName = rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim();
                if (listProject.Any(x => x.Name == projectName))
                {
                    cost.ProjectID = listProject.FirstOrDefault(x => x.Name == projectName).ID;
                }
                else
                {
                    ViewBag.Message = "Doesn't exist " + rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString() + " in list Project (at line " + i.ToString() + " )! Update cancelled";
                    return View();
                }
                cost.Item = rowNow.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                cost.Unit = rowNow.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                cost.Quantity = ConvertDecimal(rowNow.GetCell(5));
                cost.UnitPrice = ConvertDecimal(rowNow.GetCell(6));
                cost.SubTotal = ConvertDecimal(rowNow.GetCell(7));
                cost.VAT = ConvertDecimal(rowNow.GetCell(8));
            }
            db.SaveChanges();
            ViewBag.Message = "Cập nhật thành công!";
            return View();
        }
        public ActionResult Cashes_Report()
        {
            return View();
        }


        // GET: Admin/Home
        [Authorize]
        public ActionResult Index()
        {
            foreach (var item in db.Projects.ToList())
            {
                item.Name.Replace("\r\n", String.Empty);
            }
            db.SaveChanges();
            return View();
        }
        [Authorize]
        public ActionResult Cash()
        {
            return View();
        }

        [Authorize]
        public FileResult Download(string filename)
        {
            string fileDir = Path.Combine(Server.MapPath("~/App_Data/uploads"), filename);
            return File(fileDir, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }
    }
}