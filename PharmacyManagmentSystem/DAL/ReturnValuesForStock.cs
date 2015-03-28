using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagmentSystem.DAL
{
    public class ReturnValuesForStock
    {
        int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        double price;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}