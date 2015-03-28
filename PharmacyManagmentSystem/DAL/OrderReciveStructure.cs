using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PharmacyManagmentSystem.Models;
using System.ComponentModel.DataAnnotations;


namespace PharmacyManagmentSystem.DAL
{
    public class OrderReciveStructure
    {
        #region Order Part
        int ORDERid;

        public int ORDERiD
        {
            get { return ORDERid; }
            set { ORDERid = value; }
        }
        int orderDetailID;

        public int OrderDetailID
        {
            get { return orderDetailID; }
            set { orderDetailID = value; }
        }

        int lineNumber;
        public int SrNo
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }

        [Display(Name = "Category")]
        string categoryName;
        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value; }
        }

         [Display(Name = "Product")]
        string ProductName;
        public string ProductName1
        {
            get { return ProductName; }
            set { ProductName = value; }
        }

        double size;
        public double Size
        {
            get { return size; }
            set { size = value; }
        }

        string supplierName;
        public string SupplierName
        {
            get { return supplierName; }
            set { supplierName = value; }
        }

        int? quantityOderd;
        public int? QuantityOrderd
        {
            get { return quantityOderd; }
            set { quantityOderd = value; }
        }

        #endregion

        #region Order Reciving Part

        int? quantityRecived;
        public int? QuantityRecived
        {
            get { return quantityRecived; }
            set { quantityRecived = value; }
        }
       
        [Required]
        DateTime? orderRecivingDate;
        public DateTime? OrderRecivingDate
        {
            get { return orderRecivingDate; }
            set { orderRecivingDate = value; }
        }

        double? pricePrItem;
        public double? PricePrItem
        {
            get { return pricePrItem; }
            set { pricePrItem = value; }
        }

        double? discountPercentage;
        public double? DiscountPercentage
        {
            get { return discountPercentage; }
            set { discountPercentage = value; }
        }

        int? packSize;
        public int? PackSize
        {
            get { return packSize; }
            set { packSize = value; }
        }

        #endregion;

        #region Stoke Part
        int? stockId;
        public int? StockId
        {
            get { return stockId; }
            set { stockId = value; }
        }

        int? quantity;
        public int? QuantityAcepted
        {
            get { return quantity; }
            set { quantity = value; }
        }

        string batchNO;
        public string BatchNO
        {
            get { return batchNO; }
            set { batchNO = value; }
        }

        DateTime? expiryDate;
        public DateTime? ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }

        double sellingPricePrItem;
        public double SellingPricePrItem
        {
            get { return sellingPricePrItem; }
            set { sellingPricePrItem = value; }
        }

        int? itemSold;
        public int? ItemSold
        {
            get { return itemSold; }
            set { itemSold = value; }
        }

        int? expireDays;

        public int? ExpireDays
        {
            get { return expireDays; }
            set { expireDays = value; }
        }
        #endregion

    }
}