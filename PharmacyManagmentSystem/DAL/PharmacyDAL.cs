﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PharmacyManagmentSystem.Models;
using System.Web.Mvc;
using System.Web.Helpers;

namespace PharmacyManagmentSystem.DAL
{
    public class PharmacyDAL : Controller 
    {
        private pharmacyEntities db = new pharmacyEntities();
        public PharmacyDAL()
        {
          
        }

        public string justlikethat(string password)
        {                
            var hashed = Crypto.Hash(password, "MD5");
            return hashed;
        }      
      
        #region orders and place Order
        public SelectList GetCategory()
        {
                 return new SelectList(db.categories, "categoryId", "categoryName");
        }
        public SelectList GetProduct(string id)
        {
            var ID = int.Parse(id);
            SelectList list= new SelectList(db.products.Where(p => p.categoryId == ID), "productId", "productName");
            return list;
        }
        public SelectList GetProductSize(string id)
        {
            var ID = int.Parse(id);
            SelectList list = new SelectList(db.productdetails.Where(p => p.productId == ID), "productDetailId", "productSize");
            return list;
        }
        public SelectList GetSupplier(string id)
        {
            var ID = int.Parse(id);
            IQueryable<productsupplied> outer = db.productsupplieds;
            IQueryable<supplier> inner = db.suppliers;
            var results = outer.Where(product => product.productDetailId == ID)
                               .Join(
                                    inner,
                                    product => product.supplierId,
                                    supplier => supplier.supplierId,
                                    (product, supplier) => new
                                    {
                                        supplierID = supplier.supplierId,
                                        Suppliername = supplier.supplierName
                                    });
            SelectList list = new SelectList(results, "supplierID", "Suppliername");
            return list;
        }
        public SelectList GetUnit(string id)
        {
            var ID = int.Parse(id);
            SelectList list = new SelectList(db.categories.Where(c => c.categoryId == ID), "categoryId", "categoryUnit");
            return list;
        }
        public SelectList AddOrderDetails(string prodetaiID, string suplierID, string Quantity, int empId, int orderID)
        {
            int ProdDetailID = int.Parse(prodetaiID);
            int SupplierID = int.Parse(suplierID);
            int QuantityOrder = int.Parse(Quantity);
            var getProSuppliedID = db.productsupplieds.Where(p => p.productDetailId == ProdDetailID && p.supplierId == SupplierID).FirstOrDefault();
            int ProSuppliedID = getProSuppliedID.productSuppliedId;
            int al = AlreadyExsist(ProSuppliedID, orderID);
            if(al != 0)
            {
                var getOldRow = db.orderdetails.Where(o => o.productSuppliedId == ProSuppliedID&& o.orderId==orderID).FirstOrDefault(); ;
                int? oldQuantity = getOldRow.quantityOrderd;
                int? newQuantity = oldQuantity + QuantityOrder;
                getOldRow.quantityOrderd = newQuantity;
                 ///////////////save order detail
                db.SaveChanges();
            }
            else
            {
                ////create order detail ////////////////
                var orderdetailItems = new orderdetail();
                orderdetailItems.quantityOrderd = QuantityOrder;
                orderdetailItems.orderId = orderID;
                orderdetailItems.productSuppliedId = ProSuppliedID;
                ///////////////save order detail
                db.orderdetails.Add(orderdetailItems);
                db.SaveChanges();
            }
           
            SelectList list = new SelectList(db.orders.ToString());
            return list;
        }
        public int AlreadyExsist(int prosupID, int ordID)
        {
            var chkAlready = db.orderdetails.Where(p => p.orderId == ordID && p.productSuppliedId == prosupID).FirstOrDefault();

            if (chkAlready==null)
            {
                return 0;
            }
            else
            {
                return chkAlready.orderDetailId;
            }

        }
        #endregion 

        #region Order-Part
        public void AddNewOrder(DateTime newdate, int id,string ordernumber)
        {
            order neworder = new order();
            neworder.orderDate = newdate;
            neworder.orderStatusId = 1;
            neworder.empId = id;
            neworder.orderNumber = ordernumber;
            db.orders.Add(neworder);
            db.SaveChanges();
            AddOrderHistory("Draft", "New Order creation", neworder.orderId, id,newdate);
        }
        public SelectList GetOrderStatus()
        {
            SelectList list=new SelectList(db.orderstatus, "orderStatusId", "statusName");

            return list;
        }
         public string GetCruntOrderStatus(int? id) 
        {
            var C_Status = db.orders.Where(o => o.orderId == id).FirstOrDefault();
            string s = C_Status.orderstatu.statusName.ToString();
            return s;
        }
        public List<order> getOrderByEmployee(int employeeID)
        {
            List<order> list = new List<order>(db.orders.Where(o => o.empId == employeeID).OrderByDescending(o=> o.orderDate));
            return list;
        }
        public List<order> getOrderByEmployeeAndOrderId(int employeeID,int? orderID)
        {
            List<order> list = new List<order>(db.orders.Where(o => o.empId == employeeID && o.orderId==orderID));
            return list;
        }
       
        public List<OrderTableStructure> GetOrderDetails(int? orderID)
        { 
            List<int>  productSuppliedIdz=new List<int>();
            OrderTableStructure ordertable=new OrderTableStructure();
            var data = db.orderdetails.Where(p => p.orderId == orderID);
            foreach (orderdetail po in data)
            {
                productSuppliedIdz.Add(po.productSuppliedId);              
            }
            data = null;
            List<OrderTableStructure> list = new List<OrderTableStructure>();
            for (int idz = 0; idz < productSuppliedIdz.Count; idz++)
            {
                ordertable = new OrderTableStructure();
                ordertable.Id=idz+1;
                int O_ID=(int)orderID;
                int proSupID=productSuppliedIdz[idz];
                var qun = db.orderdetails.Where(p => p.productSuppliedId ==proSupID  && p.orderId == O_ID).SingleOrDefault();
                ordertable.Quantity = qun.quantityOrderd;
                ordertable.P_o_ID = qun.orderDetailId;
                qun = null;
                int PSid=productSuppliedIdz[idz];
                var sup = db.productsupplieds.Where(s => s.productSuppliedId == PSid).SingleOrDefault();
                int suplierId = sup.supplierId;
                int productdetailId = sup.productDetailId;
                sup = null;
                var supname = db.suppliers.Where(s => s.supplierId == suplierId).FirstOrDefault();
                ordertable.SupplierName = supname.supplierName;
                supname = null;
                var proSize = db.productdetails.Where(p => p.productDetailId == productdetailId).SingleOrDefault();
                ordertable.Size = proSize.productSize;
                int PNid = proSize.productId;
                proSize = null;
                var proName = db.products.Where(p => p.productId == PNid).SingleOrDefault();
                ordertable.ProductName1 = proName.productName;
                int catID = proName.categoryId;
                proName = null;
                var cat = db.categories.Where(c => c.categoryId == catID).SingleOrDefault();
                ordertable.CategoryName = cat.categoryName;
                cat = null;
                list.Add(ordertable);
              }
                return list;              
        }
       
        public void AddOrderHistory(string StatusChanged, string discription, int orderid, int? empID , DateTime date)
        {
        date = DateTime.Now;
        orderhistory orderhistory = new orderhistory();
        orderhistory.date = date;
        orderhistory.statusChanged = StatusChanged;
        orderhistory.discription = discription;
        orderhistory.orderId = orderid;
        orderhistory.employeeId = empID;
        db.orderhistories.Add(orderhistory);
        db.SaveChanges();

        }
        public void ChangeOrderStatus(int Oid, int newStatusId)
        {
            var ordertoUpdate = db.orders.Find(Oid);
            ordertoUpdate.orderStatusId = newStatusId;
            db.SaveChanges();           
        }
        public void DeletItemFromOrder(int id)
        {
           orderdetail OD = db.orderdetails.Find(id);
           db.orderdetails.Remove(OD);
           db.SaveChanges();           
        }

       
        #endregion

        #region recive Order
        public List<OrderReciveStructure> OrderReciveDisplay( int? orderID)
        {   List<OrderReciveStructure> list = new List<OrderReciveStructure>();
            List<int> productSuppliedIdz = new List<int>();
            OrderReciveStructure  ordertable = new OrderReciveStructure();
            var data = db.orderdetails.Where(p => p.orderId == orderID);
            foreach (orderdetail po in data)
            {
                productSuppliedIdz.Add(po.productSuppliedId);
            }
            data = null;
           int  countforSR = 1;
            for (int idz = 0; idz < productSuppliedIdz.Count; idz++)
            {               
                #region order placed part
                ordertable = new OrderReciveStructure();
                ordertable.SrNo = idz +countforSR;
                int O_ID = (int)orderID;
                int proSupID = productSuppliedIdz[idz];
                var qun = db.orderdetails.Where(p => p.productSuppliedId == proSupID && p.orderId == O_ID).SingleOrDefault();
                ordertable.QuantityOrderd = qun.quantityOrderd;
                ordertable.OrderDetailID  = qun.orderDetailId;
                qun = null;
                int PSid = productSuppliedIdz[idz];
                var sup = db.productsupplieds.Where(s => s.productSuppliedId == PSid).SingleOrDefault();
                int suplierId = sup.supplierId;
                int productdetailId = sup.productDetailId;
                sup = null;
                var supname = db.suppliers.Where(s => s.supplierId == suplierId).FirstOrDefault();
                ordertable.SupplierName = supname.supplierName;
                supname = null;
                var proSize = db.productdetails.Where(p => p.productDetailId == productdetailId).SingleOrDefault();
                ordertable.Size = proSize.productSize;
                int PNid = proSize.productId;
                proSize = null;
                var proName = db.products.Where(p => p.productId == PNid).SingleOrDefault();
                ordertable.ProductName1 = proName.productName;
                int catID = proName.categoryId;
                proName = null;
                var cat = db.categories.Where(c => c.categoryId == catID).SingleOrDefault();
                ordertable.CategoryName = cat.categoryName;
                cat = null;
                #endregion 

                #region Order Details
                int ODID= ordertable.OrderDetailID;
               var RecivedItemlist = db.orderdetails.Where(o => o.orderDetailId == ODID).FirstOrDefault();
               if (RecivedItemlist.quantityRecived == null)
               { // order not recived yet 
                   ordertable.StockId = ODID;
                   ordertable.QuantityRecived = 0;
                   ordertable.DiscountPercentage = 0;
                   ordertable.OrderRecivingDate = null;
                   ordertable.PackSize = 1;
                   ordertable.PricePrItem = 0;
                   ordertable.BatchNO = null;
                   ordertable.ExpireDays = 0;
                   ordertable.ExpiryDate = null;
                   ordertable.ItemSold = 0;
                   ordertable.QuantityAcepted = 0;
                   ordertable.SellingPricePrItem = 0;
               }
               else
               { // order recivied get sesific order detail row
                   ordertable.QuantityRecived = RecivedItemlist.quantityRecived;
                   ordertable.DiscountPercentage = RecivedItemlist.discountPercentage;
                   ordertable.OrderRecivingDate = RecivedItemlist.orderRecivingDate;
                   ordertable.PackSize = RecivedItemlist.packSize;
                   ordertable.PricePrItem = RecivedItemlist.PricePrItem;
                   List<stock> recivingStokList = db.stocks.Where(s => s.orderDetailId == ODID).ToList();
                   if (recivingStokList.Count() > 1)
                   {// more then one stoke items against one order detail
                       int count=0;
                       foreach (stock item in recivingStokList)
                       {
                           ordertable.StockId = item.stockId;
                           ordertable.BatchNO = item.batchNO;
                           ordertable.ExpireDays = item.expireDays;
                           ordertable.ExpiryDate = item.expiryDate;
                           ordertable.ItemSold = item.itemSold;
                           ordertable.QuantityAcepted = item.quantity;
                           ordertable.SellingPricePrItem = item.sellingPricePrItem;
                           list.Add(ordertable);
                           ordertable = new OrderReciveStructure();
                           count++;
                           if (count < recivingStokList.Count())
                           {
                               ordertable.QuantityRecived = RecivedItemlist.quantityRecived;
                               ordertable.DiscountPercentage = RecivedItemlist.discountPercentage;
                               ordertable.OrderRecivingDate = RecivedItemlist.orderRecivingDate;
                               ordertable.PackSize = RecivedItemlist.packSize;
                               ordertable.PricePrItem = RecivedItemlist.PricePrItem;
                               ordertable.SrNo = idz + countforSR + count;
                               O_ID = (int)orderID;
                               proSupID = productSuppliedIdz[idz];
                               qun = db.orderdetails.Where(p => p.productSuppliedId == proSupID && p.orderId == O_ID).SingleOrDefault();
                               ordertable.QuantityOrderd = qun.quantityOrderd;
                               ordertable.OrderDetailID = qun.orderDetailId;
                               qun = null;
                               PSid = productSuppliedIdz[idz];
                               sup = db.productsupplieds.Where(s => s.productSuppliedId == PSid).SingleOrDefault();
                               suplierId = sup.supplierId;
                               productdetailId = sup.productDetailId;
                               sup = null;
                               supname = db.suppliers.Where(s => s.supplierId == suplierId).FirstOrDefault();
                               ordertable.SupplierName = supname.supplierName;
                               supname = null;
                               proSize = db.productdetails.Where(p => p.productDetailId == productdetailId).SingleOrDefault();
                               ordertable.Size = proSize.productSize;
                               PNid = proSize.productId;
                               proSize = null;
                               proName = db.products.Where(p => p.productId == PNid).SingleOrDefault();
                               ordertable.ProductName1 = proName.productName;
                               catID = proName.categoryId;
                               proName = null;
                               cat = db.categories.Where(c => c.categoryId == catID).SingleOrDefault();
                               ordertable.CategoryName = cat.categoryName;
                               cat = null;
                           }
                           else
                           {
                               countforSR = countforSR + count-1;
                           }
                       }                       
                   }
                   else
                   { //only one stock row againt specific order detail
                       recivingStokList = null;
                       var reciving = db.stocks.Where(s => s.orderDetailId == ODID).FirstOrDefault();
                       ordertable.StockId = reciving.stockId;
                       ordertable.BatchNO = reciving.batchNO;
                       ordertable.ExpireDays = reciving.expireDays;
                       ordertable.ExpiryDate = reciving.expiryDate;
                       ordertable.ItemSold = reciving.itemSold;
                       ordertable.QuantityAcepted = reciving.quantity;
                       ordertable.SellingPricePrItem = reciving.sellingPricePrItem;                       
                   }

               }
                #endregion 
               if (ordertable.CategoryName != null)
               {
                   countforSR++;
                   list.Add(ordertable);
                   ordertable = new OrderReciveStructure();
               }
            }
            return list;        
        }

        public void EditOrAddForOrderReciveItem(OrderReciveStructure OrderRecive)
        {
            //try{
                if (OrderRecive.StockId == null)
                {//new order
                    stock stock = new stock();
                    stock.orderDetailId = OrderRecive.OrderDetailID;
                    stock.itemSold = OrderRecive.ItemSold;
                    stock.batchNO = OrderRecive.BatchNO;
                    stock.expireDays = OrderRecive.ExpireDays;
                    stock.expiryDate = (DateTime)OrderRecive.ExpiryDate;
                    stock.quantity = (int)OrderRecive.QuantityAcepted;
                    stock.sellingPricePrItem = OrderRecive.SellingPricePrItem;
                    db.stocks.Add(stock);
                    db.SaveChanges();
                    orderdetail detail = db.orderdetails.Find(OrderRecive.OrderDetailID);
                    detail.discountPercentage = OrderRecive.DiscountPercentage;
                    detail.orderRecivingDate = OrderRecive.OrderRecivingDate;
                    detail.packSize = OrderRecive.PackSize;
                    detail.PricePrItem = OrderRecive.PricePrItem;
                    detail.quantityRecived = OrderRecive.QuantityRecived;
                    db.SaveChanges();                   
                }
                else
                {  //old order
                    stock stock =db.stocks.Find(OrderRecive.StockId);
                    if (stock.batchNO == OrderRecive.BatchNO)
                    { //same order update values
                        stock.expireDays=OrderRecive.ExpireDays;
                        stock.expiryDate =(DateTime) OrderRecive.ExpiryDate;
                        stock.quantity = (int)OrderRecive.QuantityAcepted;
                        stock.sellingPricePrItem = OrderRecive.SellingPricePrItem;
                        db.SaveChanges();
                        orderdetail detail = db.orderdetails.Find(OrderRecive.OrderDetailID);
                        detail.discountPercentage = OrderRecive.DiscountPercentage;
                        detail.orderRecivingDate = OrderRecive.OrderRecivingDate;
                        detail.packSize = OrderRecive.PackSize;
                        detail.PricePrItem = OrderRecive.PricePrItem;
                        detail.quantityRecived = OrderRecive.QuantityRecived;
                        db.SaveChanges();
                    }
                    else { // may be partialy recived -- add  new stock row                   
                        stock newStock = new stock();
                        newStock.batchNO = OrderRecive.BatchNO;
                        newStock.expireDays = OrderRecive.ExpireDays;
                        newStock.expiryDate = (DateTime)OrderRecive.ExpiryDate;
                        newStock.itemSold = 0;
                        newStock.orderDetailId = OrderRecive.OrderDetailID;
                        newStock.quantity = (int)OrderRecive.QuantityAcepted;
                        newStock.sellingPricePrItem = OrderRecive.SellingPricePrItem;
                        db.stocks.Add(newStock);
                        db.SaveChanges();
                    }               
                }            
            //}
            //catch(Exception e)
            //{
            //    throw new UpdateException("Can Not Update Record, Please Provid All Valid Values. Try Again");
            //}         
        }

        public List<OrderReciveStructure> GetOrderReciveItem(int? id)
        {
            List<OrderReciveStructure> list = new List<OrderReciveStructure>();
            var getItem = db.stocks.Where(s => s.stockId == id).FirstOrDefault();
            if (getItem == null||getItem.batchNO==null)
            { //check if orderdetail item
                getItem = null;
               var getOItem = db.orderdetails.Where(s => s.orderDetailId == id).FirstOrDefault();
               OrderReciveStructure orderRecive = new OrderReciveStructure();
               List< OrderTableStructure> ordertable = new List<OrderTableStructure>();
                ordertable= GetOrderDetails(getOItem.orderId);
                foreach (OrderTableStructure o in ordertable)
                {
                    if (o.P_o_ID == id)
                    {
                        orderRecive.ORDERiD = getOItem.orderId;
                        orderRecive.CategoryName = o.CategoryName;
                        orderRecive.ProductName1 = o.ProductName1;
                        orderRecive.Size = o.Size;
                        orderRecive.SupplierName = o.SupplierName;                        
                    }                
                }                           
                     orderRecive.BatchNO = null;                    
                     orderRecive.DiscountPercentage = null;
                     orderRecive.ExpireDays = null;
                     orderRecive.ExpiryDate = null;
                     orderRecive.ItemSold = 0;
                     orderRecive.OrderDetailID = (int)id;
                     orderRecive.OrderRecivingDate = null;
                     orderRecive.PackSize = 1;
                     orderRecive.PricePrItem = null;                   
                     orderRecive.QuantityAcepted = null;
                     orderRecive.QuantityOrderd = getOItem.quantityOrderd;
                     orderRecive.QuantityRecived = null;
                     orderRecive.SellingPricePrItem = 0;                    
                     orderRecive.StockId = null;
                     list.Add(orderRecive);                
            }
            else 
            {
               List< OrderTableStructure> ordertable = new List<OrderTableStructure>();
                ordertable= GetOrderDetails(getItem.orderdetail.orderId);
                OrderReciveStructure orderRecive = new OrderReciveStructure();
                orderRecive.ORDERiD = getItem.orderdetail.orderId;
                orderRecive.BatchNO = getItem.batchNO;
                orderRecive.CategoryName =   ordertable[0].CategoryName;
                orderRecive.DiscountPercentage = getItem.orderdetail.discountPercentage;
                orderRecive.ExpireDays = getItem.expireDays ;
                orderRecive.ExpiryDate = getItem.expiryDate;
                orderRecive.ItemSold = getItem.itemSold;
                orderRecive.OrderDetailID = getItem.orderDetailId;
                orderRecive.OrderRecivingDate = getItem.orderdetail.orderRecivingDate;
                orderRecive.PackSize = getItem.orderdetail.packSize ;
                orderRecive.PricePrItem = getItem.orderdetail.PricePrItem;
                orderRecive.ProductName1 =ordertable[0].ProductName1 ;
                orderRecive.QuantityAcepted = getItem.quantity;
                orderRecive.QuantityOrderd = getItem.orderdetail.quantityOrderd;
                orderRecive.QuantityRecived = getItem.orderdetail.quantityRecived;
                orderRecive.SellingPricePrItem = getItem.sellingPricePrItem ;
                orderRecive.Size = ordertable[0].Size;
                orderRecive.StockId = getItem.stockId;
                orderRecive.SupplierName = ordertable[0].SupplierName;
                list.Add(orderRecive);                          
            }
            return list;
        
        }
        #endregion 

        #region Department
        public List<department> GetDepartments()
        {
            return db.departments.ToList();
        }

        public department FindDepartment(int? id)
        {
            return db.departments.Find(id);
        }

        public void AddNewDeprtment(department dep)
        {
            db.departments.Add(dep);
            db.SaveChanges();
        }

        public void EditDepartment(department dep)
        {
            db.Entry(dep).State = EntityState.Modified;
            db.SaveChanges();
        }

        #endregion 

        #region Designation
        
        #endregion

        #region Login
        public string Login(string username, string Password)
        {
            string s = "invalid";
            var hashedpassword = Crypto.Hash(Password, "MD5");
            user User = db.users.Where(u=>u.userName ==username && u.password ==hashedpassword ).FirstOrDefault();
            if (User != null && User.enabled == true)
            {            
               string user_name = User.userName;
               var user_info=User.employees.Where(us=> us.userName==User.userName).FirstOrDefault();
               int Emp_ID = user_info.empId;
               string First_name = user_info.firstName;
               s = user_name + ","+First_name + ","+Emp_ID.ToString();
            }
            
             return s;
        }
       public void SignOut()
        {
        
        }

        #endregion

       #region Sales Part

       public SelectList SearchProduct(string SearchKey)
       {
           List<ProductListWithSizeAndCategory> Productlist = new List<ProductListWithSizeAndCategory>();
           var results = db.products.Where(p => p.productName.Contains("" + SearchKey));
           if (results.Count() > 0)
           {
               foreach (var result in results)
               {
                   ProductListWithSizeAndCategory product = new ProductListWithSizeAndCategory();
                   product.DetailProductName = result.productName;
                   product.Id = result.productId;
                   product.IdPrevious = result.categoryId;
                   Productlist.Add(product);
                   product = new ProductListWithSizeAndCategory();
               }
           }
           List<ProductListWithSizeAndCategory> SizeProduct = new List<ProductListWithSizeAndCategory>();
           foreach (var pro in Productlist)
           {
               ProductListWithSizeAndCategory product = new ProductListWithSizeAndCategory();
               var size = db.productdetails.Where(d => d.productId == pro.Id);
               if (size.Count() > 0)
               {
                   foreach (var s in size)
                   {
                       product.DetailProductName = pro.DetailProductName + " , " + s.productSize;
                       product.Id = s.productDetailId;
                       product.IdPrevious = pro.IdPrevious;
                       SizeProduct.Add(product);
                       product = new ProductListWithSizeAndCategory();
                   }
               }
           }
           List<ProductListWithSizeAndCategory> CategoryProduct = new List<ProductListWithSizeAndCategory>();
           if (SizeProduct.Count() > 0)
           {
               foreach (var pro in SizeProduct)
               {
                   ProductListWithSizeAndCategory product = new ProductListWithSizeAndCategory();
                   var cat = db.categories.Where(c => c.categoryId == pro.IdPrevious).FirstOrDefault();
                   product.DetailProductName = pro.DetailProductName + " , " + cat.categoryName;
                   product.Id = pro.Id;
                   CategoryProduct.Add(product);
                   product = new ProductListWithSizeAndCategory();
               }
           }
           SelectList list = new SelectList(CategoryProduct, "Id", "DetailProductName");
           return list;
       }

       public ReturnValuesForStock GetNumberOfAvalibleProducts(int productDetailId)
       {
           ReturnValuesForStock values = new ReturnValuesForStock();
           double price = 0.0;
           int count=0;
           DateTime cruntDate = DateTime.Now;
           var chkStok = db.stocks.Where(s => s.orderdetail.productsupplied.productDetailId == productDetailId && s.quantity != s.itemSold );
                foreach(var stok in chkStok)
                {
                    if(stok.expireDays< (stok.expiryDate-cruntDate).TotalDays)
                    {                       
                    count += (stok.quantity - (int)stok.itemSold);
                    if (stok.sellingPricePrItem > price)
                    {
                        price = stok.sellingPricePrItem;
                    }
                    }
                }
           values.Count=count;
           values.Price=price;
            return values ;
       }
       
       
       
       
       #endregion 
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