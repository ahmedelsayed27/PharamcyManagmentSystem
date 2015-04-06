using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PharmacyManagmentSystem.DAL;
using System.Net;
using PharmacyManagmentSystem.Models;
namespace PharmacyManagmentSystem.Controllers
{
    public class LendProductsController : Controller
    {
        PharmacyDAL pdal = new PharmacyDAL();
    
        public ActionResult LendProducts()
        {
            try
            {
                ViewData["Branches"] = pdal.GetBranches(1);
                ViewData["BorrowLend"] = pdal.GetBorrowLendAgainstAnEmpID(int.Parse(Session["EmpID"].ToString()));
                return View(ViewData["BorrowLend"]);
            }
            catch {
                return RedirectToAction("SignIn", "Account");            
            }          
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {                
                borrowlend BL = pdal.GetSingleBorrowLend(id);
               if (BL == null)
                {
                    return HttpNotFound();
                }
               ViewBag.branchId = pdal.GetBranches(BL.branchId);
               ViewBag.borrowLendStatusId = pdal.GetBorrowlendStatuses(BL.borrowLendStatusId);
                return View(BL);
            }
            catch
            {
                return RedirectToAction("SignIn", "Account");
            }  

       }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "borrowLendId, netAmount, borrowerName,branchId ,lendingNumber ,lendingDate,borrowLendStatusId ")] borrowlend BL)
        {
            try
            {
                pdal.EditLendDetails(BL);
                ViewBag.branchId = pdal.GetBranches(BL.branchId);
                ViewBag.borrowLendStatusId = pdal.GetBorrowlendStatuses(BL.borrowLendStatusId);
                return View(BL);
            }
            catch {
                ViewBag.branchId = pdal.GetBranches(BL.branchId);
                ViewBag.borrowLendStatusId = pdal.GetBorrowlendStatuses(BL.borrowLendStatusId);
                return View(BL); /// handel error response
            }
        }

              
        public ActionResult AddLendItems(int? id)
        {
            if (id == null)
            {
                HttpNotFound();
            }
            try
            {
                this.Session["LendingId"] = id;
                List<SalesTableStructure> Lending = pdal.LendingItems(id);
                ViewData["LendingItems"] = Lending;
                return View(ViewData["LendingItems"]);      
            }
            catch
            {
                return RedirectToAction("SignIn", "Account");
            }

        }

        public ActionResult DeletLendItem(int id)
        {


            return View();
        }

       
        /// ////////////////functions///////////////////////////////////////////////////////////
      
        public JsonResult SearchProducts(string SearchKey)
        {
            SelectList sList = pdal.SearchProduct(SearchKey);
            return Json(sList);
        }
        public JsonResult GetAvalibleProducts(string ProductDetailID)
        {
            int PDetailId = int.Parse(ProductDetailID);
            ReturnValuesForStock productsCount = pdal.GetNumberOfAvalibleProducts(PDetailId);
            return Json(productsCount);
        }
     
        public JsonResult SaveLendingItem(string quantity, string amount,  string proDetID)
        {
            try {
                int _Lendid = int.Parse(this.Session["LendingId"].ToString());
                int _quantity = int.Parse(quantity);
                double _amount = double.Parse(amount);
                int _proDetID = int.Parse(proDetID);
                string returnmessage = pdal.SaveALendingItem(_quantity, _amount,  _Lendid, _proDetID);

                if (returnmessage == "ok")
                {
                    ViewData["LendingItems"] = pdal.LendingItems(_Lendid);
                    return Json("ok");
                }
                else
                {
                    return Json("not ok");
                }
            }
            catch {
                return Json("not ok");
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