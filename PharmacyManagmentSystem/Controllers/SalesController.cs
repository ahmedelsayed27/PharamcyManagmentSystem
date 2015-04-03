using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PharmacyManagmentSystem.DAL;

namespace PharmacyManagmentSystem.Controllers
{
    public class SalesController : Controller
    {
        PharmacyDAL pdal = new PharmacyDAL();
       public ActionResult Index()
        {
            return View();
        }

      public ActionResult Sales( )//int id)
       {
           //if (id == null)
           //{ }
          
           int id = int.Parse(this.Session["SalesID"].ToString());
           List<SalesTableStructure> sales = pdal.salesItems(id);
           ViewData["salesItems"] = sales;
           return View(ViewData["salesItems"]);         
       }


       public JsonResult SearchProducts(string SearchKey)
        {
            SelectList sList = pdal.SearchProduct(SearchKey);
            return Json(sList);        
        }

       public JsonResult GetAvalibleProducts(string ProductDetailID)
       {
           int PDetailId=int.Parse(ProductDetailID);
           ReturnValuesForStock productsCount = pdal.GetNumberOfAvalibleProducts(PDetailId);
           return Json(productsCount);
       }

       public ActionResult StartNewSale()
       {
           int id = int.Parse(this.Session["EmpId"].ToString());
           this.Session["SalesID"] = pdal.StartANewSale(id, DateTime.Now);  //create a new sale
           return (RedirectToAction("Sales","Sales"));
       }

       public ActionResult Bill()
       {
          int salesID=int.Parse(this.Session["SalesID"].ToString());
          var check =pdal.BillSales(salesID);
          if (check == "Done")
           {
               int id = int.Parse(this.Session["EmpId"].ToString());
               this.Session["SalesID"] = pdal.StartANewSale(id, DateTime.Now);  //create a new sale
               return (RedirectToAction("Sales", "Sales"));
           }
          ViewData["BillSaleError"] = check;
           return (RedirectToAction("Sales", "Sales"));
       }

       public ActionResult CancelSales()
       {
           int salesID = int.Parse(this.Session["SalesID"].ToString());
           var check = pdal.CancelSale(salesID);
           if (check == true)
           {
               int id = int.Parse(this.Session["EmpId"].ToString());
               this.Session["SalesID"] = pdal.StartANewSale(id, DateTime.Now);  //create a new sale
               return (RedirectToAction("Sales", "Sales"));
           }
           ViewData["CancelSaleError"] = "Can Not Cancel , Try Again!!!!!!!!";
           return (RedirectToAction("Sales", "Sales"));
       }
        public ActionResult DeletSalesItem(int id)
       {
           if (id ==null)
       {
           HttpNotFound();
       }
           var check=  pdal.DeleteSoldItem(id);
           if (check == true)
           {
               return RedirectToAction("Sales", "Sales"); 
           }
           ViewData["SoldItemDeleteError"] = "can not Delete Item, Try Again!!!!!!!!!!!!";
           return RedirectToAction("Sales", "Sales");     
       }

       public ActionResult GetDraftSales()
       {
           try
           {
               int EmpId = int.Parse(this.Session["EmpId"].ToString());
               var DraftSales= pdal.GetDraftSalesOfAnEmployee(EmpId);
               ViewData["DraftSales"] = DraftSales;
               return View(ViewData["DraftSales"]);
           }
           catch
           {
               return View();           
           }       
       }

       public JsonResult SaveSoldItem(string quantity,string  amount,string discount,string  netAmount, string proDetID)
       {
           int _salesid = int.Parse(this.Session["SalesID"].ToString());
           int _quantity = int.Parse(quantity);
           double _amount = double.Parse(amount);
           int _discount = int.Parse(discount);
           double _netAmount = double.Parse(netAmount);
           int _proDetID = int.Parse(proDetID);
          string returnmessage= pdal.SaveASoldItem(_quantity,_amount,_discount,_salesid,_netAmount,_proDetID);
        //  ViewData["salesItems"] = pdal.salesItems(_salesid);
          if (returnmessage == "ok")
          {
              ViewData["salesItems"] = pdal.salesItems(_salesid);
              return Json("ok");
          }
          else
          {
              return Json("not ok");
          }          
       }
	}
}