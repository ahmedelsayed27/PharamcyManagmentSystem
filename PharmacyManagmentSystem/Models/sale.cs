
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
    
public partial class sale
{

    public sale()
    {

        this.solditems = new HashSet<solditem>();

    }


    public int salesId { get; set; }

    public System.DateTime date { get; set; }

    public int empId { get; set; }

    public double netAmount { get; set; }

    public string salesStatus { get; set; }

    public Nullable<int> overallDiscounPercentage { get; set; }

    public Nullable<double> netAmountAfterOD { get; set; }

    public string customerName { get; set; }



    public virtual employee employee { get; set; }

    public virtual ICollection<solditem> solditems { get; set; }

}

}
