
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
    
public partial class borroworderlend
{

    public int BorrowOrderLendId { get; set; }

    public int borrowLendId { get; set; }

    public int orderId { get; set; }



    public virtual borrowlend borrowlend { get; set; }

    public virtual order order { get; set; }

}

}
