
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
    
public partial class soldstock
{

    public int soldStokId { get; set; }

    public int stockId { get; set; }

    public int soldItemId { get; set; }



    public virtual solditem solditem { get; set; }

    public virtual stock stock { get; set; }

}

}