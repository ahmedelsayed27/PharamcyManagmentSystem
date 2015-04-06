using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace PharmacyManagmentSystem
{
    public class justlikethat
    {
        
    }
}




///edit for borrowlend
///
/*
@{
    ViewBag.Title = "Edit";
}
<h2>Edit Details</h2>
@using (Html.BeginForm())
{

<br />
<div>
    @Html.Label("user Name =   ")
    @Html.Label(Session["userName"].ToString())
    <br />
    @Html.Label("   Date =   ")
    @Html.Label(DateTime.Today.Date.ToString())
   
    <br />
</div>
<br />
         <div id="createnewLending" style="width:50%; align-content:center;">
                 <table class="table table-condensed" style="width:inherit">
                            <tr style="background-color:#3A597A; color:white;">
                       
                        <td>@Html.Label("Borrower Name")</td>
                        <td>@Html.Label("Branch Name")</td>
                        <td>@Html.Label("Lending  Number")</td>
                        <td>@Html.Label("Date")</td>
                        <td>@Html.Label("Status")</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>@Html.DisplayFor(item => item.borrowerName)</td>
                        <td>@Html.DisplayFor(item => item.branch.branchName)</td>
                        <td>@Html.DisplayFor(item => item.lendingNumber)</td>
                        <td>@Html.DisplayFor(item => item.lendingDate)</td>
                        <td>@Html.DisplayFor(item => item.borrowlendstatus.statusName)</td>
                        
                        <td><button class="btn btn-info" id="saveLending">Save Changes</button> </td>
                    </tr>
                </table>
               <label id="ErrorLable"> hello</label>
          </div>


}*/











//selling functionality ( not used are)
//           using (var dbTransaction = db.Database.BeginTransaction())
//           {
//               try
//               {
//                   List<TwoIntegers> SoldItemIdz=new List<TwoIntegers>();
//                   var salesItemsTosel = db.solditems.Where(d => d.salesId == salesID);
//                   foreach (solditem s in salesItemsTosel)
//                   {/////////////get all solditem idz + quentity reqired
//                      TwoIntegers t=new TwoIntegers();
//                       t.Id=s.soldItemId;
//                       t.Value=s.quantity;
//                       SoldItemIdz.Add(t);
//                   }
//                   foreach (TwoIntegers values in SoldItemIdz)
//                   { ///find all the respective rows of  soldstock
//                       int itemRemaining = values.Value;                      
//                       var soldstok = db.soldstocks.Where(s => s.soldItemId == values.Id);                       
//                        if (soldstok.Count() == 1)
//                       {
//                           var sold1 = soldstok.FirstOrDefault();
//                           int stockid = sold1.soldStokId;
//                           stock s = db.stocks.Find(stockid);
//                           int productDetailID = s.productDetailId;///////////////////////////////////////////////////////////////////////
//                           ReturnValuesForStock RVFS = GetNumberOfAvalibleProducts(productDetailID);                         
//                            #region   if a single row - and reqired items are avalible                           
//                           // if (RVFS.Count > itemRemaining)
//                           //{//reqiureid items are avalible
//                               if ((s.quantity - s.itemSold) >= itemRemaining)
//                               {
//                                   s.itemSold += itemRemaining;
//                                   db.SaveChanges();
//                                   itemRemaining = 0;
//                               }//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                               else
//                               {
//                                   itemRemaining = itemRemaining - (int)(s.quantity - s.itemSold);
//                                   s.itemSold = s.quantity;
//                                   db.SaveChanges();
//                                   soldstock newsold = new soldstock();
//                                   /////// find  more stock rows
//                                   List<int> list = GetStockIdsForProduct(productDetailID , itemRemaining);
//                                   foreach(int id in list)
//                                   {
//                                       stock st = db.stocks.Find(id);
//                                       if ((st.quantity - st.itemSold) >= itemRemaining)
//                                       {
//                                           st.itemSold += itemRemaining;
//                                           db.SaveChanges();
//                                           newsold = new soldstock();
//                                           newsold.stockId = id;
//                                           newsold.soldItemId = values.Id;
//                                           db.soldstocks.Add(newsold);
//                                           db.SaveChanges();
//                                           itemRemaining = 0;                                    
//                                       }
//                                       else
//                                       {
//                                           itemRemaining = itemRemaining - (int)(st.quantity - st.itemSold);
//                                           st.itemSold = st.quantity;
//                                           db.SaveChanges();
//                                           newsold = new soldstock();
//                                           newsold.stockId = id;
//                                           newsold.soldItemId = values.Id;
//                                           db.soldstocks.Add(newsold);
//                                           db.SaveChanges();
//                                       }                                      
//                                     }
//                               }
////                           }
////                           #endregion
////                           else
////                           {  ////// number of product is less then item reqired
////                               MessageReturn = MessageReturn +"For Product  "+ GetProductNameForSoldItem(productDetailID) + ": Sorry  number of item  are less then Requried( " + itemRemaining.ToString() + " ). Selling  " + RVFS.Count.ToString() + " item  ";
////                               itemRemaining = RVFS.Count;
///////////////////////////////////////// update amount  from sales + sold items ///////////////////////////////////////////////////
////                               solditem oldSold = db.solditems.Find(values.Id);
////                               double? oldamount = oldSold.amount;
////                               double? unitprice = oldSold.amount /oldSold.quantity;
////                               double? newamount= itemRemaining * unitprice;
////                               oldSold.quantity = itemRemaining;
////                               oldSold.amount = newamount;
////                               db.SaveChanges();
////                               sale oldsales = db.sales.Find(salesID);
////                               double oldSalesNetamount = oldsales.netAmount;
////                               double newSalesNetamount = oldSalesNetamount - (double)(oldamount - newamount);
////                               oldsales.netAmount = newSalesNetamount;
////                               db.SaveChanges();
////                               //////////////////////////// delete all old soldstock record for that sold item id////////////////////////////
////                               var AllSoldStock = db.soldstocks.Where(ss => ss.soldItemId == values.Id);
////                               foreach(soldstock sStock in AllSoldStock)
////                               {
////                                   soldstock SS = db.soldstocks.Find(sStock.soldStokId);
////                                   db.soldstocks.Remove(SS);
////                                   db.SaveChanges();
////                               }                                                   
////                               ///////////////////////////////////////// get stok idz for itemremaing to sell ////////////////////////////////////
////                               List<int> list = GetStockIdsForProduct(productDetailID, itemRemaining);
////                               soldstock newsold = new soldstock();
////                                foreach (int id in list)
////                               {
////                                   stock st = db.stocks.Find(id);
////                                   if ((st.quantity - st.itemSold) >= itemRemaining)
////                                   {
////                                       st.itemSold += itemRemaining;
////                                       db.SaveChanges();
////                                       newsold = new soldstock();
////                                       newsold.stockId = id;
////                                       newsold.soldItemId = values.Id;
////                                       db.soldstocks.Add(newsold);
////                                       db.SaveChanges();
////                                       itemRemaining = 0;
////                                   }
////                                   else
////                                   {
////                                       itemRemaining = itemRemaining - (int)(st.quantity - st.itemSold);
////                                       st.itemSold = st.quantity;
////                                       db.SaveChanges();
////                                       newsold = new soldstock();
////                                       newsold.stockId = id;
////                                       newsold.soldItemId = values.Id;
////                                       db.soldstocks.Add(newsold);
////                                       db.SaveChanges();
////                                   }
////                               }
////                           }                           
//                       }
//                       else
//                       { //////more then one row associated with a single sold item
//                           foreach (var sold in soldstok)
//                           {////// update each stock row one by one- for itemsold

//                           }                      
//                       }
//                   }
//                   ////////finaly update  sales item
//                   sale sales = db.sales.Find(salesID);
//                   sales.salesStatus = "Done";
//                   db.SaveChanges();

//                   dbTransaction.Commit();
//               }
//               catch (Exception)
//               {
//                   dbTransaction.Rollback();
//                   MessageReturn = MessageReturn + "can not perform this Bill , Please check You network Connection and try again!!!!!!!!!";
//                   return MessageReturn;
//               }
//           }
//           return MessageReturn;
//       }


