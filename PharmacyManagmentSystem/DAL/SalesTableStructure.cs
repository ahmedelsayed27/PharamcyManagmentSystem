using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagmentSystem.DAL
{
    public class SalesTableStructure
    {
        int salesId;
        public int SalesId
        {
            get { return salesId; }
            set { salesId = value; }
        }

        string product;
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        int? quantity;
        public int? Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }


        double unitPrice;
        public double UnitPrice
        {
            get { return unitPrice; }
            set { unitPrice = value; }
        }

        double amount;
        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        int discount;
        public int Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        double netAmount;
        public double NetAmount
        {
            get { return netAmount; }
            set { netAmount = value; }
        }

    }
}