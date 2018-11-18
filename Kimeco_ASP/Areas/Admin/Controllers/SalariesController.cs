using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kimeco_ASP.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Kimeco_ASP.Areas.Admin.Controllers;
using System.Globalization;

namespace Kimeco_ASP.Areas.Admin.Controllers
{
    public class SalariesController : Controller
    {
        private string DISABLE = "disable";
        private string SPENDING = "spending";
        private string ABLE = "able";
        private KimecoEntities db = new KimecoEntities();

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salary salary = db.Salaries.Find(id);
            if (salary == null)
            {
                return HttpNotFound();
            }
            salary.Status = "Eable";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Spending()
        {
            return View(db.Salaries.Where(x => x.Status == "spending").ToList());
        }

        public FileResult TemplateUpdate()
        {
            string fileDir = Path.Combine(Server.MapPath("~/App_Data/templates"), "Salary_Template.xlsx");
            return File(fileDir, System.Net.Mime.MediaTypeNames.Application.Octet, "Salary_Template.xlsx");
        }

        [HttpGet]
        public ActionResult MultiUpload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MultiUpload(HttpPostedFileBase file)
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

                        //đọc file template
                        string _TemplatePath = Path.Combine(Server.MapPath("~/App_Data/templates"), "Salary_Template.xlsx");
                        FileStream fsTemplate = new FileStream(_TemplatePath, FileMode.Open);
                        XSSFWorkbook wbTemplate = new XSSFWorkbook(fsTemplate);
                        ISheet sheetTemplate = wbTemplate.GetSheetAt(0);
                        ISheet sheetIndex = wbIndex.GetSheetAt(0);

                        //check template
                        int flat = 2;
                        var rowIndex = sheetIndex.GetRow(flat);
                        var rowTemplate = sheetTemplate.GetRow(flat);
                        for (int i = 0; i <= 32; i++)
                        {
                            if (rowTemplate.GetCell(0).StringCellValue != rowIndex.GetCell(0).StringCellValue)
                            {
                                ViewBag.Message = "Wrong template format!";
                                return View();
                            }
                        }
                        flat += 2;

                        int TeamID;
                        //đọc team ID
                        var rowTeam = sheetIndex.GetRow(1);
                        if (HomeController.ConvertInt(rowTeam.GetCell(0)) == null)
                        {
                            ViewBag.Message = "Wrong team ID";
                            return View();
                        }
                        else
                        {
                            Team team = db.Teams.Find(HomeController.ConvertInt(rowTeam.GetCell(0)));
                            if (team == null)
                            {
                                ViewBag.Message = "Cannot find Team ID";
                                return View();
                            }
                            else
                            {
                                TeamID = team.ID;
                            }
                        }//đọc xong teamID 

                        var Mounth = HomeController.ConvertInt(rowTeam.GetCell(1));
                        var Year = HomeController.ConvertInt(rowTeam.GetCell(2));
                        if (Mounth == null || Year == null || Mounth < 1 || Mounth> 12)
                        {
                            
                            ViewBag.Message = "Mouth and Year cannot be empty.";
                            return View();
                        }

                        for (int i = flat; i< sheetIndex.LastRowNum; i+=3)
                        {
                            var rowM = sheetIndex.GetRow(flat);
                            var rowA = sheetIndex.GetRow(flat + 1);
                            var rowOT = sheetIndex.GetRow(flat + 2);
                            var UnitPerHour = HomeController.ConvertDecimal(rowM.GetCell(37));
                            if (UnitPerHour == null)
                            {
                                ViewBag.Message = "UnitperHour is cannot empty.";
                                return View();
                            }
                            var allow = HomeController.ConvertDecimal(rowM.GetCell(39));
                            var Position = rowM.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim();
                            var Name = rowM.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Trim();
                            var User = db.Users.Where(x => x.FullName == Name).FirstOrDefault();
                            if (User == null)
                            {
                                ViewBag.Message = "Cannot find user " + Name + " in system.";
                                return View();
                            }
                            for (int j = 4; j < 35; j++)
                            {
                                if(HomeController.ConvertDecimal(rowM.GetCell(j)) != null)
                                {
                                    Salary salaryM = new Salary();
                                    salaryM.WorkingTime = "M";
                                    salaryM.Status = "spending";
                                    salaryM.Value = HomeController.ConvertDecimal(rowM.GetCell(j))??0;
                                    salaryM.UserID = User.Username;
                                    salaryM.TeamID = TeamID;
                                    var stringDay = (j - 3).ToString() + "/" + Mounth.ToString() + "/" + Year.ToString();
                                    salaryM.Date = DateTime.ParseExact(stringDay, "d/M/yyyy", CultureInfo.InvariantCulture);
                                    salaryM.UnitPerHour = UnitPerHour??0;
                                    salaryM.Allowance = allow;
                                    db.Salaries.Add(salaryM);
                                }

                                if (HomeController.ConvertDecimal(rowA.GetCell(j)) != null)
                                {
                                    Salary salaryM = new Salary();
                                    salaryM.WorkingTime = "A";
                                    salaryM.Status = "spending";
                                    salaryM.Value = HomeController.ConvertDecimal(rowA.GetCell(j))??0;
                                    salaryM.UserID = User.Username;
                                    salaryM.TeamID = TeamID;
                                    var stringDay = (j - 3).ToString() + "/" + Mounth.ToString() + "/" + Year.ToString();
                                    salaryM.Date = DateTime.ParseExact(stringDay, "d/M/yyyy", CultureInfo.InvariantCulture);
                                    salaryM.UnitPerHour = UnitPerHour??0;
                                    salaryM.Allowance = allow;
                                    db.Salaries.Add(salaryM);
                                }

                                if (HomeController.ConvertDecimal(rowOT.GetCell(j)) != null)
                                {
                                    Salary salaryM = new Salary();
                                    salaryM.WorkingTime = "OT";
                                    salaryM.Status = "spending";
                                    salaryM.Value = HomeController.ConvertDecimal(rowOT.GetCell(j))??0;
                                    salaryM.UserID = User.Username;
                                    salaryM.TeamID = TeamID;
                                    var stringDay = (j - 3).ToString() + "/" + Mounth.ToString() + "/" + Year.ToString();
                                    salaryM.Date = DateTime.ParseExact(stringDay, "d/M/yyyy", CultureInfo.InvariantCulture);
                                    salaryM.UnitPerHour = UnitPerHour??0;
                                    salaryM.Allowance = allow;
                                    db.Salaries.Add(salaryM);
                                }

                            }
                            flat += 3;
                        }

                        ViewBag.Message = "Cập nhật thành công! <a class=\"btn\" href=\"/admin/salaries\">Về trang chủ</a>";
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

        // GET: Admin/Salaries
        public ActionResult Index()
        {
            ViewData["ListTeam"] = db.Teams.ToList();
            return View(db.Salaries.ToList());
        }

        // GET: Admin/Salaries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salary salary = db.Salaries.Find(id);
            if (salary == null)
            {
                return HttpNotFound();
            }
            return View(salary);
        }

        [HttpPost]
        public FileResult Team_Report(int? TeamID)
        {
            var team = db.Teams.Find(TeamID);
          if (TeamID != null)
            {
                var listSalary = db.Salaries.Where(x => x.TeamID == TeamID && x.Status == ABLE).OrderBy(x => x.Date).ToList();

                //read teamplate
                string _TemplatePath = Path.Combine(Server.MapPath("~/App_Data/templates"), "Salary__Team_Report.xlsx");
                FileStream fsTemplate = new FileStream(_TemplatePath, FileMode.Open);
                XSSFWorkbook wbTemplate = new XSSFWorkbook(fsTemplate);
                var sheetTemplate = wbTemplate.GetSheetAt(0);
                var rowTemplate1 = sheetTemplate.GetRow(2);
                //lấy ra danh sách các tháng
                var listMouth = listSalary.Select(x => x.Date).GroupBy(y => new { y.Month, y.Year }).ToList();
                
                string ReportFileName = team.Name + "_Report.xlsx";
                string REportPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), ReportFileName);
                FileStream fsReport = new FileStream(REportPath, FileMode.Create);
                int flat = 2;
                foreach (var mounth in listMouth)
                {
                    if (flat != 2)
                    {
                        sheetTemplate.CopyRow(2, flat);
                        sheetTemplate.CopyRow(3, flat + 1);
                    }
                    sheetTemplate.GetRow(flat).CreateCell(4).SetCellValue(mounth.Key.Month.ToString() + "/" + mounth.Key.Year.ToString());
                    flat += 2;
                    //dah sách các nhân viên distinct 
                    var listEmployee = listSalary.Where(x => x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year).Select(y => y.User).GroupBy(z=>new { z.Username });
                    foreach(var employee in listEmployee)
                    {
                        var row1 = sheetTemplate.CreateRow(flat);
                        var row2 = sheetTemplate.CreateRow(flat + 1);
                        var row3 = sheetTemplate.CreateRow(flat + 2);
                        decimal unitperhour = 0;
                        var firstUnitperHour = listSalary.Where(x =>x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year && x.UserID == employee.Key.Username).FirstOrDefault();
                        if (firstUnitperHour != null)
                        {
                            unitperhour = firstUnitperHour.UnitPerHour;
                        }
                        for (int day = 1;day <= 31; day++)
                        {
                            var dayM = listSalary.Where(x => x.Date.Day == day && x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year && x.WorkingTime == "M" && x.UserID == employee.Key.Username).SingleOrDefault();
                            if (dayM != null)
                            {
                                row1.CreateCell(day + 3).SetCellValue(dayM.Value.ToString());
                            }
                            var dayA = listSalary.Where(x => x.Date.Day == day && x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year && x.WorkingTime == "A" && x.UserID == employee.Key.Username).SingleOrDefault();
                            if (dayA != null)
                            {
                                row2.CreateCell(day + 3).SetCellValue(dayA.Value.ToString());
                            }
                            var dayOT = listSalary.Where(x => x.Date.Day == day && x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year && x.WorkingTime == "OT" && x.UserID == employee.Key.Username).SingleOrDefault();
                            if (dayOT != null)
                            {
                                row3.CreateCell(day + 3).SetCellValue(dayOT.Value.ToString());
                            }
                        }
                        //set user full name
                        var user = db.Users.Find(employee.Key.Username);
                        if (user != null)
                        {
                            row1.CreateCell(1).SetCellValue(user.Position);
                            row1.CreateCell(2).SetCellValue(user.FullName);
                        }
                        row1.CreateCell(3).SetCellValue("M");
                        row2.CreateCell(3).SetCellValue("A");
                        row3.CreateCell(3).SetCellValue("OT");
                        var totalM = listSalary.Where(x => x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year && x.WorkingTime == "M").Sum(x => x.Value);
                        var totalA = listSalary.Where(x => x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year && x.WorkingTime == "A").Sum(x => x.Value);
                        var totalOT = listSalary.Where(x => x.Date.Month == mounth.Key.Month && x.Date.Year == mounth.Key.Year && x.WorkingTime == "OT").Sum(x => x.Value);
                        row1.CreateCell(35).SetCellValue(totalM.ToString());
                        row1.CreateCell(37).SetCellValue(unitperhour.ToString());
                        row1.CreateCell(38).SetCellValue((totalM * unitperhour).ToString());

                        row2.CreateCell(35).SetCellValue(totalA.ToString());
                        row2.CreateCell(37).SetCellValue(unitperhour.ToString());
                        row2.CreateCell(38).SetCellValue((totalA * unitperhour).ToString());

                        row3.CreateCell(35).SetCellValue(totalOT.ToString());
                        row3.CreateCell(37).SetCellValue(unitperhour.ToString());
                        row3.CreateCell(38).SetCellValue((totalOT * unitperhour).ToString());
                        flat += 3;
                    }
                }
                
                wbTemplate.Write(fsReport);
                fsTemplate.Close();
                fsReport.Close();
                return File(REportPath, System.Net.Mime.MediaTypeNames.Application.Octet, ReportFileName);
            }
            return null;
           
        }
        [Authorize]
        // GET: Admin/Salaries/Create
        public ActionResult Create()
        {
            ViewData["ListTeam"] = db.Teams.ToList();
            ViewData["ListUser"] = db.Users.ToList();
            return View();
        }

        // POST: Admin/Salaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,WorkingTime,Date,UnitPerHour,Advanced,Allowance,TeamID,CreateDate,CreateBy,Status,Note,Value,Sign")] Salary salary,string stringDay)
        {
            if (ModelState.IsValid)
            {
                salary.Date = DateTime.ParseExact(stringDay, "d/M/yyyy", CultureInfo.InvariantCulture);
                salary.Status = "spending";
                db.Salaries.Add(salary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(salary);
        }

        // GET: Admin/Salaries/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salary salary = db.Salaries.Find(id);
            if (salary == null)
            {
                return HttpNotFound();
            }
            ViewData["ListTeam"] = db.Teams.ToList();
            ViewData["ListUser"] = db.Users.ToList();
            return View(salary);
        }

        // POST: Admin/Salaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,WorkingTime,Date,UnitPerHour,Advanced,Allowance,TeamID,CreateDate,CreateBy,Status,Note,Value,Sign")] Salary salary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salary);
        }

        // GET: Admin/Salaries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salary salary = db.Salaries.Find(id);
            if (salary == null)
            {
                return HttpNotFound();
            }
            return View(salary);
        }

        // POST: Admin/Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Salary salary = db.Salaries.Find(id);
            db.Salaries.Remove(salary);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
