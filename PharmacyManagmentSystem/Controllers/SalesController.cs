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

       public ActionResult Sales()
       {
           List<SalesTableStructure> sales=new List<SalesTableStructure>();
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

	}
}