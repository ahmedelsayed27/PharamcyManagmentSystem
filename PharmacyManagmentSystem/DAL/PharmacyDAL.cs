using System;
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
            SelectList list = new SelectList(db.products.Where(p => p.categoryId == ID), "productId", "productName");
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
            if (al != 0)
            {
                var getOldRow = db.orderdetails.Where(o => o.productSuppliedId == ProSuppliedID && o.orderId == orderID).FirstOrDefault(); ;
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

            if (chkAlready == null)
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
        public bool AddNewOrder(DateTime newdate, int id, string ordernumber)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    order neworder = new order();
                    neworder.orderDate = newdate;
                    neworder.orderStatusId = 1;
                    neworder.empId = id;
                    neworder.orderNumber = ordernumber;
                    db.orders.Add(neworder);
                    db.SaveChanges();
                    AddOrderHistory("Draft", "New Order creation", neworder.orderId, id, newdate);

                    dbTransaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return false;
                }
            }
        }
        public SelectList GetOrderStatus()
        {
            SelectList list = new SelectList(db.orderstatus, "orderStatusId", "statusName");

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
            List<order> list = new List<order>(db.orders.Where(o => o.empId == employeeID).OrderByDescending(o => o.orderDate));
            return list;
        }
        public List<order> getOrderByEmployeeAndOrderId(int employeeID, int? orderID)
        {
            List<order> list = new List<order>(db.orders.Where(o => o.empId == employeeID && o.orderId == orderID));
            return list;
        }

        public List<OrderTableStructure> GetOrderDetails(int? orderID)
        {
            List<int> productSuppliedIdz = new List<int>();
            OrderTableStructure ordertable = new OrderTableStructure();
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
                ordertable.Id = idz + 1;
                int O_ID = (int)orderID;
                int proSupID = productSuppliedIdz[idz];
                var qun = db.orderdetails.Where(p => p.productSuppliedId == proSupID && p.orderId == O_ID).SingleOrDefault();
                ordertable.Quantity = qun.quantityOrderd;
                ordertable.P_o_ID = qun.orderDetailId;
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
                list.Add(ordertable);
            }
            return list;
        }

        public void AddOrderHistory(string StatusChanged, string discription, int orderid, int? empID, DateTime date)
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
        public List<OrderReciveStructure> OrderReciveDisplay(int? orderID)
        {
            List<OrderReciveStructure> list = new List<OrderReciveStructure>();
            List<int> productSuppliedIdz = new List<int>();
            OrderReciveStructure ordertable = new OrderReciveStructure();
            var data = db.orderdetails.Where(p => p.orderId == orderID);
            foreach (orderdetail po in data)
            {
                productSuppliedIdz.Add(po.productSuppliedId);
            }
            data = null;
            int countforSR = 1;
            for (int idz = 0; idz < productSuppliedIdz.Count; idz++)
            {
                #region order placed part
                ordertable = new OrderReciveStructure();
                ordertable.SrNo = idz + countforSR;
                int O_ID = (int)orderID;
                int proSupID = productSuppliedIdz[idz];
                var qun = db.orderdetails.Where(p => p.productSuppliedId == proSupID && p.orderId == O_ID).SingleOrDefault();
                ordertable.QuantityOrderd = qun.quantityOrderd;
                ordertable.OrderDetailID = qun.orderDetailId;
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
                int ODID = ordertable.OrderDetailID;
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
                        int count = 0;
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
                                countforSR = countforSR + count - 1;
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
        { //need changes
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
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
                        int orderDetID = OrderRecive.OrderDetailID;
                        orderdetail ODetail = db.orderdetails.Where(o => o.orderDetailId == orderDetID).FirstOrDefault();
                        int productsupID = ODetail.productSuppliedId;
                        productsupplied proSup = db.productsupplieds.Where(p => p.productSuppliedId == productsupID).FirstOrDefault();
                        stock.productDetailId = proSup.productDetailId;
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
                        stock stock = db.stocks.Find(OrderRecive.StockId);
                        if (stock.batchNO == OrderRecive.BatchNO)
                        { //same order update values
                            stock.expireDays = OrderRecive.ExpireDays;
                            stock.expiryDate = (DateTime)OrderRecive.ExpiryDate;
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
                        else
                        { // may be partialy recived -- add  new stock row                   
                            stock newStock = new stock();
                            newStock.batchNO = OrderRecive.BatchNO;
                            newStock.expireDays = OrderRecive.ExpireDays;
                            newStock.expiryDate = (DateTime)OrderRecive.ExpiryDate;
                            newStock.itemSold = 0;
                            newStock.orderDetailId = OrderRecive.OrderDetailID;
                            newStock.quantity = (int)OrderRecive.QuantityAcepted;
                            newStock.sellingPricePrItem = OrderRecive.SellingPricePrItem;
                            int orderDetID = OrderRecive.OrderDetailID;
                            orderdetail ODetail = db.orderdetails.Where(o => o.orderDetailId == orderDetID).FirstOrDefault();
                            int productsupID = ODetail.productSuppliedId;
                            productsupplied proSup = db.productsupplieds.Where(p => p.productSuppliedId == productsupID).FirstOrDefault();
                            newStock.productDetailId = proSup.productDetailId;
                            db.stocks.Add(newStock);
                            db.SaveChanges();
                        }
                    }
                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                }
            }
        }

        public List<OrderReciveStructure> GetOrderReciveItem(int? id)
        {
            List<OrderReciveStructure> list = new List<OrderReciveStructure>();
            var getItem = db.stocks.Where(s => s.stockId == id).FirstOrDefault();
            if (getItem == null || getItem.batchNO == null)
            { //check if orderdetail item
                getItem = null;
                var getOItem = db.orderdetails.Where(s => s.orderDetailId == id).FirstOrDefault();
                OrderReciveStructure orderRecive = new OrderReciveStructure();
                List<OrderTableStructure> ordertable = new List<OrderTableStructure>();
                ordertable = GetOrderDetails(getOItem.orderId);
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
                List<OrderTableStructure> ordertable = new List<OrderTableStructure>();
                ordertable = GetOrderDetails(getItem.orderdetail.orderId);
                OrderReciveStructure orderRecive = new OrderReciveStructure();
                orderRecive.ORDERiD = getItem.orderdetail.orderId;
                orderRecive.BatchNO = getItem.batchNO;
                orderRecive.CategoryName = ordertable[0].CategoryName;
                orderRecive.DiscountPercentage = getItem.orderdetail.discountPercentage;
                orderRecive.ExpireDays = getItem.expireDays;
                orderRecive.ExpiryDate = getItem.expiryDate;
                orderRecive.ItemSold = getItem.itemSold;
                orderRecive.OrderDetailID = getItem.orderDetailId;
                orderRecive.OrderRecivingDate = getItem.orderdetail.orderRecivingDate;
                orderRecive.PackSize = getItem.orderdetail.packSize;
                orderRecive.PricePrItem = getItem.orderdetail.PricePrItem;
                orderRecive.ProductName1 = ordertable[0].ProductName1;
                orderRecive.QuantityAcepted = getItem.quantity;
                orderRecive.QuantityOrderd = getItem.orderdetail.quantityOrderd;
                orderRecive.QuantityRecived = getItem.orderdetail.quantityRecived;
                orderRecive.SellingPricePrItem = getItem.sellingPricePrItem;
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
            user User = db.users.Where(u => u.userName == username && u.password == hashedpassword).FirstOrDefault();
            if (User != null && User.enabled == true)
            {
                string user_name = User.userName;
                var user_info = User.employees.Where(us => us.userName == User.userName).FirstOrDefault();
                int Emp_ID = user_info.empId;
                string First_name = user_info.firstName;
                s = user_name + "," + First_name + "," + Emp_ID.ToString();
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
            int count = 0;
            DateTime cruntDate = DateTime.Now;
            var chkStok = db.stocks.Where(s => s.orderdetail.productsupplied.productDetailId == productDetailId && s.quantity != s.itemSold);
            foreach (var stok in chkStok)
            {
                if (stok.expireDays < (stok.expiryDate - cruntDate).TotalDays)
                {
                    count += (stok.quantity - (int)stok.itemSold);
                    if (stok.sellingPricePrItem > price)
                    {
                        price = stok.sellingPricePrItem;
                    }
                }
            }
            values.Count = count;
            values.Price = price;
            return values;
        }

        public int StartANewSale(int EmployId, DateTime Date)
        {
            sale newSale = new sale();
            newSale.empId = EmployId; ;
            newSale.date = Date;
            newSale.salesStatus = "Draft";
            var forId = db.sales.Add(newSale);
            db.SaveChanges();
            int salesid = forId.salesId;
            return salesid;
        }

        public string SaveASoldItem(int Quantity, double Amount, int DiscountPrcentage, int salesId, double NetAmount, int requiredproDetailID)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    solditem sold = new solditem();
                    sold.amount = Amount;
                    sold.discount = DiscountPrcentage;
                    sold.quantity = Quantity;
                    sold.salesId = salesId;
                    sold.productDetailId = requiredproDetailID;
                    var forsoldItem = db.solditems.Add(sold);
                    db.SaveChanges();
                    sale saleItem = db.sales.Find(salesId);
                    if (saleItem != null)
                    {
                        if (saleItem.netAmount > 0)
                        {
                            saleItem.netAmount += NetAmount;
                        }
                        else
                        {
                            saleItem.netAmount = NetAmount;
                        }
                    }
                    db.SaveChanges();
                    soldstock newsale = new soldstock();
                    List<int> idz = GetStockIdsForProduct(requiredproDetailID, Quantity);
                    foreach (int id in idz)
                    {
                        newsale = new soldstock();
                        newsale.soldItemId = forsoldItem.soldItemId;
                        newsale.stockId = id;
                        db.soldstocks.Add(newsale);
                        db.SaveChanges();
                    }
                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return "Not-ok";
                }
            }
            return "ok";
        }

        public List<int> GetStockIdsForProduct(int productDetailId, int quantityReqired)
        {
            List<int> stockIdz = new List<int>();
            int remainquantity = quantityReqired;
            DateTime cruntDate = DateTime.Now;
            var chkStok = db.stocks.Where(s => s.orderdetail.productsupplied.productDetailId == productDetailId && s.quantity != s.itemSold).OrderBy(s => s.expiryDate);
            foreach (var stok in chkStok)
            {
                if (stok.expireDays < (stok.expiryDate - cruntDate).TotalDays)
                {
                    if ((stok.quantity - stok.itemSold) >= remainquantity)
                    {
                        remainquantity = 0;
                        stockIdz.Add(stok.stockId);

                        //return stock id
                        break;
                    }
                    else
                    {
                        if (remainquantity != 0)
                        {
                            remainquantity = remainquantity - stok.quantity;
                            stockIdz.Add(stok.stockId);

                            //return stock id
                        }
                    }
                }
            }
            return stockIdz;
        }

        public List<SalesTableStructure> salesItems(int salesID)
        {
            List<SalesTableStructure> list = new List<SalesTableStructure>();
            SalesTableStructure Item = new SalesTableStructure();
            var sale = db.solditems.Where(s => s.salesId == salesID).Include(s => s.soldstocks);
            foreach (var s in sale)
            {
                Item = new SalesTableStructure();
                var sold = s.soldstocks.Where(t => t.soldItemId == s.soldItemId);
                Item.Product = GetProductNameForSoldItem(s.productDetailId);
                Item.Quantity = s.quantity;
                Item.UnitPrice = s.amount / s.quantity;
                Item.Amount = s.amount;
                Item.Discount = s.discount;
                Item.SoldItemId = s.soldItemId;
                if (s.discount > 0)
                {
                    Item.NetAmount = s.amount - ((s.amount * s.discount) / 100);

                }
                else
                {
                    Item.NetAmount = s.amount;
                }
                list.Add(Item);
            }
            return list;
        }

        public string GetProductNameForSoldItem(int productdetailid)
        {
            string ProductNametoReturn = "";
            productdetail _prodDet = db.productdetails.Where(p => p.productDetailId == productdetailid).FirstOrDefault();
            int prodId = _prodDet.productId;
            ProductNametoReturn = _prodDet.productSize.ToString();
            product _pro = db.products.Where(p => p.productId == prodId).FirstOrDefault();
            int catId = _pro.categoryId;
            ProductNametoReturn = _pro.productName.ToString() + " , " + ProductNametoReturn;
            category _cat = db.categories.Where(c => c.categoryId == catId).FirstOrDefault();
            ProductNametoReturn = ProductNametoReturn + " , " + _cat.categoryName.ToString();
            return ProductNametoReturn;
        }

        public bool DeleteSoldItem(int solditemID)
        {
            ///////delete soldstok then sold item of respective  solditem ID        
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var soldStockItems = db.soldstocks.Where(s => s.soldItemId == solditemID);
                    foreach (var s in soldStockItems)
                    {
                        soldstock SS = db.soldstocks.Find(s.soldStokId);
                        db.soldstocks.Remove(SS);
                        db.SaveChanges();
                    }
                    solditem SI = db.solditems.Find(solditemID);
                    db.solditems.Remove(SI);
                    db.SaveChanges();
                    dbTransaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return false;
                }
            }
        }

        public string BillSales(int salesID)
        {
            string MessageReturn = "";
            bool AllOK = true;
            var salesItemsTosell = db.solditems.Where(d => d.salesId == salesID);
            foreach (solditem s in salesItemsTosell)
            {
                ReturnValuesForStock avalibleItems = GetNumberOfAvalibleProducts(s.productDetailId);
                if (avalibleItems.Count < s.quantity)
                {
                    AllOK = false;
                    MessageReturn = MessageReturn + " product Name:" + GetProductNameForSoldItem(s.productDetailId) + " Require Items are " + s.quantity + "  avalible items are" + avalibleItems.Count + "." + Environment.NewLine;
                }
            }
            if (AllOK == true)
            {                ////do selling
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<TwoIntegers> SoldItemIdz = new List<TwoIntegers>();
                        var salesItemsTosel = db.solditems.Where(d => d.salesId == salesID);
                        foreach (solditem s in salesItemsTosel)
                        {/////////////get all solditem idz + quentity reqired
                            TwoIntegers t = new TwoIntegers();
                            t.Id = s.soldItemId;
                            t.Value = s.quantity;
                            SoldItemIdz.Add(t);
                        }
                        foreach (TwoIntegers values in SoldItemIdz)
                        { ///find all the respective rows of  soldstock
                            int itemRemaining = values.Value;
                            var soldstok = db.soldstocks.Where(s => s.soldItemId == values.Id);
                            #region Single stock row
                            if (soldstok.Count() == 1)
                            {
                                var sold1 = soldstok.FirstOrDefault();
                                int stockid = sold1.stockId;
                                stock s = db.stocks.Find(stockid);
                                int productDetailID = s.productDetailId;
                                if ((s.quantity - s.itemSold) >= itemRemaining)
                                {
                                    s.itemSold += itemRemaining;
                                    db.SaveChanges();
                                    itemRemaining = 0;
                                }
                                else
                                {
                                    itemRemaining = itemRemaining - (int)(s.quantity - s.itemSold);
                                    s.itemSold = s.quantity;
                                    db.SaveChanges();
                                    soldstock newsold = new soldstock();
                                    /////// find  more stock rows
                                    List<int> list = GetStockIdsForProduct(productDetailID, itemRemaining);
                                    foreach (int id in list)
                                    {
                                        stock st = db.stocks.Find(id);
                                        if ((st.quantity - st.itemSold) >= itemRemaining)
                                        {
                                            st.itemSold += itemRemaining;
                                            db.SaveChanges();
                                            newsold = new soldstock();
                                            newsold.stockId = id;
                                            newsold.soldItemId = values.Id;
                                            db.soldstocks.Add(newsold);
                                            db.SaveChanges();
                                            itemRemaining = 0;
                                        }
                                        else
                                        {
                                            itemRemaining = itemRemaining - (int)(st.quantity - st.itemSold);
                                            st.itemSold = st.quantity;
                                            db.SaveChanges();
                                            newsold = new soldstock();
                                            newsold.stockId = id;
                                            newsold.soldItemId = values.Id;
                                            db.soldstocks.Add(newsold);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            #endregion
                            else
                            {
                                int productDetailID = 0;
                                foreach (var sold in soldstok)
                                {////// update each stock row one by one- for itemsold
                                    stock s = db.stocks.Find(sold.stockId);
                                    productDetailID = s.productDetailId;
                                    if ((s.quantity - s.itemSold) >= itemRemaining)
                                    {
                                        s.itemSold += itemRemaining;
                                        db.SaveChanges();
                                        itemRemaining = 0;
                                    }
                                    else
                                    {
                                        itemRemaining = itemRemaining - (int)(s.quantity - s.itemSold);
                                        s.itemSold = s.quantity;
                                        db.SaveChanges();
                                    }
                                }

                                #region rare case

                                if (itemRemaining != 0)
                                {
                                    soldstock newsold = new soldstock();
                                    /////// find  more stock rows
                                    List<int> list = GetStockIdsForProduct(productDetailID, itemRemaining);
                                    foreach (int id in list)
                                    {
                                        stock st = db.stocks.Find(id);
                                        if ((st.quantity - st.itemSold) >= itemRemaining && itemRemaining != 0)
                                        {
                                            st.itemSold += itemRemaining;
                                            db.SaveChanges();
                                            newsold = new soldstock();
                                            newsold.stockId = id;
                                            newsold.soldItemId = values.Id;
                                            db.soldstocks.Add(newsold);
                                            db.SaveChanges();
                                            itemRemaining = 0;
                                        }
                                        else if (itemRemaining != 0)
                                        {
                                            itemRemaining = itemRemaining - (int)(st.quantity - st.itemSold);
                                            st.itemSold = st.quantity;
                                            db.SaveChanges();
                                            newsold = new soldstock();
                                            newsold.stockId = id;
                                            newsold.soldItemId = values.Id;
                                            db.soldstocks.Add(newsold);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        ////////finaly update  sales item
                        sale sales = db.sales.Find(salesID);
                        sales.salesStatus = "Done";
                        db.SaveChanges();
                        dbTransaction.Commit();
                        MessageReturn = "Done";
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        MessageReturn = MessageReturn + "can not perform this Bill , Please check Your network Connection and try again!!!!!!!!!";
                        return MessageReturn;
                    }
                }
                return MessageReturn;
            }
            else
            { ////// no selling return with erro message
                return MessageReturn;

            }
            /////////////////////////////////////////////function End////////////////////////////////////////////////////////////////////////////////////
        }

        public bool CancelSale(int salesID)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {//removing stock sales entries
                    List<int> SoldItemIdz = new List<int>();
                    var salesItemsToDel = db.solditems.Where(d => d.salesId == salesID);
                    foreach (solditem s in salesItemsToDel)
                    {
                        SoldItemIdz.Add(s.soldItemId);
                    }
                    foreach (int id in SoldItemIdz)
                    { ///find all the respective rows of  soldstock
                        var soldstok = db.soldstocks.Where(s => s.soldItemId == id);
                        foreach (var sold in soldstok)
                        {//////deleting each soldstock row one by one
                            soldstock SS = db.soldstocks.Find(sold.soldStokId);
                            db.soldstocks.Remove(SS);
                            db.SaveChanges();
                        }
                        ////delete sales Item against id
                        solditem SI = db.solditems.Find(id);
                        db.solditems.Remove(SI);
                        db.SaveChanges();
                    }
                    ////////finaly delete  sales item
                    sale sales = db.sales.Find(salesID);
                    db.sales.Remove(sales);
                    db.SaveChanges();

                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return false;
                }
            }



            return true;
        }
        public List<sale> GetDraftSalesOfAnEmployee(int employeeId)
        {
            var Draftsales = db.sales.Where(s => s.empId == employeeId);
            List<sale> list = Draftsales.ToList();
            return list;
        }

        #endregion

        #region borow Lend Part

        public SelectList GetBranches()
        {
            return new SelectList(db.branches, "branchId", "branchName");
        }
               
        public bool AddLendingRecord(int BranchId ,string borrowerName, string LendingNumber ,DateTime date, int EmpId , int StatusId)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    borrowlend BL = new borrowlend();
                    BL.borrowerName = borrowerName;
                    BL.lendingNumber = LendingNumber;
                    BL.borrowLendStatusId = StatusId;
                    BL.branchId = BranchId;
                    BL.empId = EmpId;
                    BL.lendingDate = date;
                    BL.netAmount = 0;
                    db.borrowlends.Add(BL);
                    db.SaveChanges();
                    dbTransaction.Commit();
                    return true;                    
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return false;
                }
            }
        }

        public List<borrowlend> GetBorrowLendAgainstAnEmpID(int empiD)
        { 
            var GetBorrows =db.borrowlends.Where(b=>b.empId==empiD).Include(b=>b.branch).Include(b=>b.borrowlendstatus).Include(b=>b.employee);
            List<borrowlend> list =GetBorrows.ToList();
            return list;
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
