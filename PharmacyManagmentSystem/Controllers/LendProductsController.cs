using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PharmacyManagmentSystem.DAL;
namespace PharmacyManagmentSystem.Controllers
{
    public class LendProductsController : Controller
    {
        PharmacyDAL pdal = new PharmacyDAL();
        //
        // GET: /LendProducts/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LendProducts()
        {
            try
            {
                ViewData["Branches"] = pdal.GetBranches();
                ViewData["BorrowLend"] = pdal.GetBorrowLendAgainstAnEmpID(int.Parse(Session["EmpID"].ToString()));
                return View(ViewData["BorrowLend"]);
            }
            catch {
                return RedirectToAction("SignIn", "Account");

            
            }

           
        }


        public JsonResult AddLendingData(int BranchId, string borrowerName, string LendingNumber, string date)
        {
            try
            {
                DateTime newdate = DateTime.Today;
                DateTime.TryParse(date, out newdate);
                int id = int.Parse(this.Session["EmpID"].ToString());
                bool chck = pdal.AddLendingRecord(BranchId, borrowerName, LendingNumber, newdate, id, 1);
                if (chck == true)
                {
                    return Json("ok");
                }
                else
                {
                    return Json("not ok");
                }
            }
            catch
            {
                return Json("not ok");
            }
        }
    }
}