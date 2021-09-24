using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.RQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMCustomizations.Graph_Extensions
{
    public class RQRequestEntry_Extension : PXGraphExtension<RQRequestEntry>
    {
        public virtual void _(Events.FieldUpdated<RQRequestLine.inventoryID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row as RQRequestLine;
            var itemInfo = InventoryItem.PK.Find(Base, row.InventoryID);
            if (row.EstUnitCost == 0 && !(itemInfo.StkItem ?? false))
            {
                var vendorInfo = SelectFrom<POVendorInventory>
                                 .Where<POVendorInventory.inventoryID.IsEqual<P.AsInt>
                                        .And<POVendorInventory.active.IsEqual<P.AsBool>>>
                                 .View.Select(Base, row.InventoryID, true).RowCast<POVendorInventory>().ToList().FirstOrDefault();
                if (vendorInfo != null)
                {
                    e.Cache.SetValueExt<RQRequestLine.estUnitCost>(row, vendorInfo.LastPrice);
                    e.Cache.SetValueExt<RQRequestLine.curyEstUnitCost>(row, vendorInfo.LastPrice);
                }
            }
        }
    }
}
