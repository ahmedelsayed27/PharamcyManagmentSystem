//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PharmacyManagmentSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class user
    {
        public user()
        {
            this.employees = new HashSet<employee>();
        }
    
        public string userName { get; set; }
        public string password { get; set; }
        public bool enabled { get; set; }
        public Nullable<short> authorityId { get; set; }
    
        public virtual ICollection<employee> employees { get; set; }
        public virtual authority authority { get; set; }
    }
}
