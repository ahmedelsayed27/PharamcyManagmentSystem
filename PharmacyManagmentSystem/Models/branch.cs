
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
    
public partial class branch
{

    public branch()
    {

        this.borrowlends = new HashSet<borrowlend>();

    }


    public int branchId { get; set; }

    public string branchName { get; set; }

    public string branchAdress { get; set; }

    public string branchLocation { get; set; }

    public string branchContactNumber { get; set; }



    public virtual ICollection<borrowlend> borrowlends { get; set; }

}

}
