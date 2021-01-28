using System;
using PX.Data;
using PX.Objects.IN;

namespace JAMS.AM
{
    public class MatlWizard1_Extension : PXGraphExtension<MatlWizard1>
    {
        #region Delegate Function
        public delegate void ProcessMatlWrkDelegate(AMProdItem amproditem, AMProdMatl amprodmatl, InventoryItem inventoryItem);
        [PXOverride]
        public void ProcessMatlWrk(AMProdItem amproditem, AMProdMatl amprodmatl, InventoryItem inventoryItem, ProcessMatlWrkDelegate baseMethod)
        {
            baseMethod(amproditem, amprodmatl, inventoryItem);

            var wrkMatl = Base.MatlXref.Current;

            if (wrkMatl != null)
            {
                Base.MatlXref.Current.QtyReq  = wrkMatl.QtyReq / (1 + amprodmatl.ScrapFactor);
                Base.MatlXref.Current.MatlQty = wrkMatl.QtyAvail > wrkMatl.QtyReq ? wrkMatl.QtyReq : wrkMatl.QtyAvail;
            }
        }
        #endregion
    }
}