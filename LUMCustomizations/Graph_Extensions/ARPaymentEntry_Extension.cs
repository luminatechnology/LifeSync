using PX.Data;
using PX.Objects.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMCustomizations.Graph_Extensions
{
    public class ARPaymentEntry_Extension : PXGraphExtension<ARPaymentEntry>
    {
        protected virtual void _(Events.FieldUpdated<ARAdjust.curyAdjdAmt> e, PXFieldUpdated baseMethod)
        {
            baseMethod(e.Cache, e.Args);
            e.Cache.SetValue<ARAdjust.adjdCuryRate>(e.Cache, (decimal?)e.NewValue / Base.Document.Current.CuryDocBal);
        }
    }
}
