using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace PharmacyManagmentSystem
{
    public class justlikethat
    {
        static void StartOwnTransactionWithinContext()
        {
            using (var context = new Models.pharmacyEntities())
            {
                using (var dbTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                    }
                }
            }
        } 
    }
}