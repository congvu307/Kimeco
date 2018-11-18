using Kimeco_ASP.Models;
using Microsoft.Ajax.Utilities;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kimeco_ASP.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public const string DISABLE = "Disable";
        public ICellStyle hederStyle(XSSFWorkbook workbook)
        {
            IFont font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.BorderBottom = BorderStyle.Medium;
            cellStyle.BorderTop = BorderStyle.Medium;
            cellStyle.BorderRight = BorderStyle.Medium;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);
            cellStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            return cellStyle;

        }
        public ICellStyle total(XSSFWorkbook workbook)
        {
            IFont font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.BorderBottom = BorderStyle.Medium;
            cellStyle.BorderTop = BorderStyle.Medium;
            cellStyle.BorderRight = BorderStyle.Medium;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);
            cellStyle.DataFormat = workbook.CreateDataFormat().GetFormat(" #,##");
            cellStyle.FillForegroundColor = IndexedColors.Gold.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            return cellStyle;

        }
        public ICellStyle Currency(XSSFWorkbook workbook)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = workbook.CreateDataFormat().GetFormat(" #,##");
            return cellStyle;
        }
        public ICellStyle text_bold( XSSFWorkbook workbook)
        {
            IFont font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.SetFont(font);
            return cellStyle;
        }
        public ICellStyle Npoi_header(ICellStyle icellstyle)
        {
            return icellstyle;
        }
        KimecoEntities db = new KimecoEntities();
        public static Int32? ConvertInt(ICell content)
        {
            if ((content == null) || (content.ToString() == "") || (!Int32.TryParse(content.ToString(), out Int32 tempDecimal)))
            {
                return null;
            }
            return Int32.Parse(content.ToString());
        }
        public static Decimal? ConvertDecimal(ICell content)
        {
            if ((content == null) || (content.ToString() == "") || (!Decimal.TryParse(content.ToString(), out Decimal tempDecimal)))
            {
                return null;
            }
            return Decimal.Parse(content.ToString());
        }
        [HttpGet]
        [Authorize]
        public ActionResult Cash_MultiUpload()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Cash_MultiUpload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _FileExtension = Path.GetExtension(_FileName);

                    if (_FileExtension == ".xlsx")
                    {
                        string _fileNameToSave = Path.Combine(Server.MapPath("~/App_Data/uploads"), "xxx" + file.FileName);
                        file.SaveAs(_fileNameToSave);
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
                            if ((rowNow.Cells.All(d => d.CellType == CellType.Blank)) || (rowNow == null)) continue;
                            Cash cash = new Cash();
                            string date = rowNow.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                            cash.C_Date = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
                            cash.Company = rowNow.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                            if (listProject.Any(x => x.Name == rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim()))
                            {
                                cash.ProjectName = listProject.FirstOrDefault(x => x.Name == rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim()).ID;
                            }
                            else
                            {
                                ViewBag.Message = "Doesn't exist " + rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString() + " in list Project (at line " + i.ToString() + " )! Update cancelled";
                                return View();
                            }
                            cash.Staff = rowNow.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                            cash.C_Content = rowNow.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                            cash.Input = ConvertDecimal(rowNow.GetCell(5));
                            cash.Output = ConvertDecimal(rowNow.GetCell(6));
                            cash.Invoice = rowNow.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                            cash.Ref = rowNow.GetCell(8, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                            cash.CreateDate = DateTime.Now;
                            cash.Status = false;
                            db.Cashes.Add(cash);
                        }
                        ViewBag.Message = "Cập nhật thành công! <a class=\"btn\" href=\"/admin/home\">Về trang chủ</a>";
                        db.SaveChanges();
                        fsIndex.Close();
                        fsTemplate.Close();
                    }
                    else
                    {
                        ViewBag.Message = "Sai định dang template (.xlsx)";
                    }
                    return View();
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex;
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult SiteCost_Upload()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult SiteCost_Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _FileExtension = Path.GetExtension(_FileName);
                    if (_FileExtension == ".xlsx")
                    {
                        string _fileNameToSave = Path.Combine(Server.MapPath("~/App_Data/uploads"), _FileName);
                        file.SaveAs(_fileNameToSave);
                        var listCompany = db.Companies.ToList();
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
                        for (int i = 2; i < sheetIndex.LastRowNum; i++)
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
                                ViewBag.Message = "Lỗi ở dòng " + i.ToString();
                                return View();
                            }
                            var CompanyName = rowNow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim();
                            if (listCompany.Any(x => x.Name == CompanyName))
                            {
                                cost.ConpanyID = listCompany.FirstOrDefault(x => x.Name == CompanyName).ID;
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
                            cost.CreateDate = DateTime.Now;
                            db.Costs.Add(cost);
                        }
                        db.SaveChanges();
                        ViewBag.Message = "Cập nhật thành công!";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
                return View();
            }
            return View();
        }

        [HttpPost]
        public FileResult Cashes_Report(int ProjectID, int? type)
        {
            string ReportFileName = "";
            string ReportPath = "";
            if (ProjectID != 0)
            {
                //lấy danh sách cash với projectID và sắp xếp tăng dần theo ngày
                var listCash = db.Cashes.Where(x => x.ProjectName == ProjectID && x.Status == true).OrderBy(x => x.C_Date).ToList();
                var projectName = db.Projects.FirstOrDefault(x => x.ID == ProjectID).Name.Replace(" ", "_");
                //đọc file
                ReportFileName = projectName + "_Report.xlsx";
                ReportPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), ReportFileName);
                FileStream fsReport = new FileStream(ReportPath, FileMode.Create);
                XSSFWorkbook wbReport = new XSSFWorkbook();
                ISheet sheetReport = wbReport.CreateSheet();

                ICellStyle tittle = wbReport.CreateCellStyle();
                tittle.Alignment = HorizontalAlignment.Center;
                tittle.VerticalAlignment = VerticalAlignment.Center;
                tittle.WrapText = true;

                tittle.BorderBottom = BorderStyle.Thick;
                IRow row0 = sheetReport.CreateRow(0);
                CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 8);
                sheetReport.AddMergedRegion(cellMerge);
                row0.CreateCell(0).SetCellValue(projectName + "- Daily Report");
                row0.GetCell(0).CellStyle = tittle;
                //tạo danh sách header
                List<string> hder = new List<string>(new string[] { "Date", "Company", "Project Name", "Staff", "Content", "Invoice", "Input", "Output", "Ref" });

                //khởi tạo header cho file excel
                IRow row1 = sheetReport.CreateRow(1);
                for (int i = 0; i < hder.Count; i++)
                {
                    row1.CreateCell(i).SetCellValue(hder[i]);
                }
                if (listCash.Count == 0)
                {
                    wbReport.Write(fsReport);
                    fsReport.Close();
                    return File(ReportPath, System.Net.Mime.MediaTypeNames.Application.Octet, ReportFileName);
                }

                int flat = 2;
                DateTime minDate = listCash[0].C_Date;
                for (int i = 0; i < listCash.Count; i++)
                {
                    if (listCash[i].C_Date.Month != minDate.Month)
                    {
                        IRow reBalandRow = sheetReport.CreateRow(i + flat);
                        for (int j = 0; j <= 8; j++)
                        {
                            reBalandRow.CreateCell(j).CellStyle = hederStyle(wbReport);
                        }
                        reBalandRow.GetCell(0).SetCellValue(minDate.ToString("MMMM"));

                        //tỉnh tổng theo tháng
                        var paymentContent = "Payment " + minDate.ToString("m/yyyy", CultureInfo.InvariantCulture);
                        var sumInput = listCash.Where(x => x.C_Date.Month == minDate.Month).Sum(x => x.Input);
                        var sumOutput = listCash.Where(x => x.C_Date.Month == minDate.Month).Sum(x => x.Output);
                        reBalandRow.GetCell(4).SetCellValue(paymentContent);
                        reBalandRow.GetCell(6).SetCellValue(sumInput.ToString());
                        reBalandRow.GetCell(7).SetCellValue(sumOutput.ToString());
                        flat++;
                        minDate = listCash[i].C_Date;
                    }
                    IRow rpRowNow = sheetReport.CreateRow(i + flat);
                    rpRowNow.CreateCell(0).SetCellValue(listCash[i].C_Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                    rpRowNow.CreateCell(1).SetCellValue(listCash[i].Company);
                    rpRowNow.CreateCell(2).SetCellValue(listCash[i].ProjectName.ToString());
                    rpRowNow.CreateCell(3).SetCellValue(listCash[i].Staff);
                    rpRowNow.CreateCell(4).SetCellValue(listCash[i].C_Content);
                    rpRowNow.CreateCell(5).SetCellValue(listCash[i].Invoice);
                    rpRowNow.CreateCell(6).SetCellValue(listCash[i].Input.ToString());
                    rpRowNow.CreateCell(7).SetCellValue(listCash[i].Output.ToString());
                    rpRowNow.CreateCell(8).SetCellValue(listCash[i].Ref);
                }
                for (int i = 0; i <= 8; i++)
                {
                    sheetReport.AutoSizeColumn(i);
                }
                wbReport.Write(fsReport);
                fsReport.Close();
            }
            else
            {

            }
            return File(ReportPath, System.Net.Mime.MediaTypeNames.Application.Octet, ReportFileName);
        }

        //get general Reports
        [HttpGet]
        [Authorize]
        public FileResult Cash_General_Report()
        {
            var listReportCashes = db.CashReports.Where(x => x.Status == true && x.Note != DISABLE).OrderBy(x=>x.Date).ToList();
            var ReportFileName = "Cash_General_Report.xlsx";
            var ReportPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), ReportFileName);
            FileStream fsReport = new FileStream(ReportPath, FileMode.Create);
            XSSFWorkbook wbReport = new XSSFWorkbook();
            ISheet sheetReport = wbReport.CreateSheet();
            List<string> hder = new List<string>(new string[] { "DATE", "PROJECT NAME", "LAST MONTH REMAIN", "SUB TOTAL INPUT", "SUB TOTAL OUTPUT", "BANK REMAIN", "CASH IN HAND", "PROJECT TOTAL" });

            //Create template report
            CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, hder.Count - 1);
            sheetReport.AddMergedRegion(cellMerge);
            var row0 = sheetReport.CreateRow(0);
            row0.CreateCell(0).SetCellValue("CASH GENERAL REPORT");
            var row1 = sheetReport.CreateRow(1);
            row1.CreateCell(0).SetCellValue("Company");
            row1.GetCell(0).CellStyle = text_bold(wbReport);
            row1.CreateCell(1).SetCellValue("Kimeco");
            row1.GetCell(1).CellStyle = text_bold(wbReport);
            var row2 = sheetReport.CreateRow(2);
            row2.Height = 500;
            for (int i = 0; i < hder.Count;i++)//loop set header
            {
                row2.CreateCell(i).CellStyle = hederStyle(wbReport);
                row2.GetCell(i).SetCellValue(hder[i]);
            }
            int flat = 3;
            for (int i = 0;i < listReportCashes.Count; i++)
            {
                if (i!= 0 && listReportCashes[i].Date.Day != listReportCashes[i - 1].Date.Day)
                {
                    IRow rpTotalRow = sheetReport.CreateRow(i + flat);
                    rpTotalRow.CreateCell(0).SetCellValue(listReportCashes[i-1].Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                    rpTotalRow.CreateCell(1).SetCellValue("TOTAL");
                    var sum_LastMonthRemain = listReportCashes.Where(x => x.Date == listReportCashes[i - 1].Date).Sum(x => x.LastMonthRemain);
                    var sum_SubTotalInput = listReportCashes.Where(x => x.Date == listReportCashes[i - 1].Date).Sum(x => x.SubTotalInput);
                    var sum_SubTotalOutput = listReportCashes.Where(x => x.Date == listReportCashes[i - 1].Date).Sum(x => x.SubTotalOutput);
                    var sum_BankRemain = listReportCashes.Where(x => x.Date == listReportCashes[i - 1].Date).Sum(x => x.BankRemain);
                    var sum_CashInHand = listReportCashes.Where(x => x.Date == listReportCashes[i - 1].Date).Sum(x => x.CashInHand);
                    var sum_ProjectTotal = listReportCashes.Where(x => x.Date == listReportCashes[i - 1].Date).Sum(x => x.ProjectTotal);
                    
                    rpTotalRow.CreateCell(2).SetCellValue(Convert.ToDouble(sum_LastMonthRemain));
                    rpTotalRow.CreateCell(3).SetCellValue(Convert.ToDouble(sum_SubTotalInput));
                    rpTotalRow.CreateCell(4).SetCellValue(Convert.ToDouble(sum_SubTotalOutput));
                    rpTotalRow.CreateCell(5).SetCellValue(Convert.ToDouble(sum_BankRemain));
                    rpTotalRow.CreateCell(6).SetCellValue(Convert.ToDouble(sum_CashInHand));
                    rpTotalRow.CreateCell(7).SetCellValue(Convert.ToDouble(sum_ProjectTotal));
                    for (int j = 0; j <= 7; j++)
                    {
                        rpTotalRow.GetCell(j).CellStyle = total(wbReport);
                    }
                    flat++;
                }
                IRow rpRowNow = sheetReport.CreateRow(i + flat);
                rpRowNow.CreateCell(0).SetCellValue(listReportCashes[i].Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                rpRowNow.CreateCell(1).SetCellValue(listReportCashes[i].Project.Name);
                rpRowNow.CreateCell(2).SetCellValue(Convert.ToDouble(listReportCashes[i].LastMonthRemain));
                rpRowNow.CreateCell(3).SetCellValue(Convert.ToDouble(listReportCashes[i].SubTotalInput));
                rpRowNow.CreateCell(4).SetCellValue(Convert.ToDouble(listReportCashes[i].SubTotalOutput));
                rpRowNow.CreateCell(5).SetCellValue(Convert.ToDouble(listReportCashes[i].BankRemain));
                rpRowNow.CreateCell(6).SetCellValue(Convert.ToDouble(listReportCashes[i].CashInHand));
                rpRowNow.CreateCell(7).SetCellValue(Convert.ToDouble(listReportCashes[i].ProjectTotal));
                //set format cell is Currency
                for (int j = 0;j<= 7; j++)
                {
                    rpRowNow.GetCell(j).CellStyle = Currency(wbReport);
                }
            }

            for (int i = 0; i <= hder.Count + 1; i++)
            {
                sheetReport.AutoSizeColumn(i);
            }
            wbReport.Write(fsReport);
            fsReport.Close();
            return File(ReportPath, System.Net.Mime.MediaTypeNames.Application.Octet, ReportFileName);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public FileResult Cash_TotalReport()
        {
            var listCashReport = db.CashReports.DistinctBy(x => x.Date).ToList();
            var listProject = db.Projects.ToList();
            List<DateTime> listDate = new List<DateTime>();
            var ReportFileName = "Cash_Total_Report.xlsx";
            var ReportPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), ReportFileName);
            FileStream fsReport = new FileStream(ReportPath, FileMode.Create);
            XSSFWorkbook wbReport = new XSSFWorkbook();
            ISheet sheetReport = wbReport.CreateSheet();

            //Create template report
            CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, listProject.Count + 1);
            sheetReport.AddMergedRegion(cellMerge);
            var row0 = sheetReport.CreateRow(0);
            row0.CreateCell(0).SetCellValue("Cash Total Report");
           
            var row1 = sheetReport.CreateRow(1);
            row1.CreateCell(0).SetCellValue("Date");
            int flatProject = 1;
            for(int i = 0;i<listProject.Count;i++)
            {
                row1.CreateCell(flatProject).SetCellValue(listProject[i].Name);
                flatProject++;
            }
            // <-- end of create teamplate report

            int flat = 2;
            for (int i = 0;i < listCashReport.Count; i++)
            {
                var dateNow = listCashReport[i].Date;
                var rowIndex = sheetReport.CreateRow(flat);
                rowIndex.CreateCell(0).SetCellValue(listCashReport[i].Date.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture));
                for (int j =0;j< listProject.Count; j++)
                {
                   
                    var projectID = listProject[j].ID;
                    var cashReportNow = db.CashReports.Where(x => x.Date == dateNow && x.ProjectName == projectID).SingleOrDefault();
                    if (cashReportNow != null)
                    {
                        rowIndex.CreateCell(j + 1).SetCellValue(cashReportNow.ProjectTotal.ToString());
                    }
                }
                flat++;
            }

            
            //auto size columm
            for (int i = 0; i <= listProject.Count() + 1; i++)
            {
                sheetReport.AutoSizeColumn(i);
            }
            wbReport.Write(fsReport);
            fsReport.Close();
            return File(ReportPath, System.Net.Mime.MediaTypeNames.Application.Octet, ReportFileName);
        }

        // GET: Admin/Home
        [Authorize]
        public ActionResult Index()
        {
            //foreach(var item in db.CashReports.ToList())
            //{
            //    item.Date = item.Date.Replace('.', '/');
            //    item.Date = DateTime.ParseExact(item.Date, "d/M/yyyy", CultureInfo.InvariantCulture).ToString();
            //}
            //db.SaveChanges();
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