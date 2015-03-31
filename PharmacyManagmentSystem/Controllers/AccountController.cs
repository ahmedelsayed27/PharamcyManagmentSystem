using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PharmacyManagmentSystem.Models;
using PharmacyManagmentSystem.DAL;
namespace PharmacyManagmentSystem.Controllers
{
    public class AccountController : Controller
    {
        PharmacyDAL pdal = new PharmacyDAL();

        public ActionResult SignIn(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(user model, string ReturnUrl)
        {
           string  returnvalue = pdal.Login(model.userName, model.password);
           if(returnvalue.Equals("invalid"))
           {
           ViewBag.ReturnUrl=returnvalue;
               return View();
           }
           char[] delimiterChars = { ',' };
           string[] list = returnvalue.Split(delimiterChars);
           this.Session["userName"] = list[0];
           this.Session["firstName"] = list[1];
           this.Session["EmpID"] = list[2];
          //////////////////////////////////////to del start///////////////////////////
           int Id = int.Parse(this.Session["EmpId"].ToString());    ///this code will be deleted after sales call from previouse page
           this.Session["SalesID"] = pdal.StartANewSale(Id, DateTime.Now);
            //////////////////to del end///////////////////////////////////////////////////
           return RedirectToAction("Sales", "Sales");          
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(user model, string ReturnUrl)
        {
            //pdal.SignOut();
            this.Session.Abandon();
            return RedirectToAction("Index", "Home");

        }
        //[Authorize (Roles ="CEO")]
        //public ActionResult something()
        //{
        //    return "";
        
        //}

      


	}
}

 //public ActionResult SignIn(string ReturnUrl)
 //       {
 //           ViewBag.ReturnUrl = ReturnUrl;
 //           return View();
 //       }

 //       [HttpPost]
 //       [ValidateAntiForgeryToken]
 //       public ActionResult SignIn(user model, string ReturnUrl)
 //       {
 //           ViewBag.ReturnUrl = ReturnUrl;
 //           return View();
 //       }
