using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagmentSystem.DAL
{
    public class ProductListWithSizeAndCategory
    {
        int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        string detailProductName;

        public string DetailProductName
        {
            get { return detailProductName; }
            set { detailProductName = value; }
        }
        int idPrevious;

        public int IdPrevious
        {
            get { return idPrevious; }
            set { idPrevious = value; }
        }

    }
}