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

       public JsonResult StartNewSale(string ProductDetailID)
       {
           int id = int.Parse(this.Session["EmpId"].ToString());
           this.Session["SalesID"] = pdal.StartANewSale(id, DateTime.Now);  //create a new sale
           return Json("ok");
       }
       public ActionResult DeletSalesItem(int solditemID)
       {
           pdal.DeleteSoldItem(solditemID);
           return RedirectToAction("Sales", "Sales");     
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