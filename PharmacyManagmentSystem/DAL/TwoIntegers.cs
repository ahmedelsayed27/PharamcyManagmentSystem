using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagmentSystem.DAL
{
    public class TwoIntegers
    {
        int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        int value;

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}