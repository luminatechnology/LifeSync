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
                decimal qtyReqWOScrap = (wrkMatl.QtyReq / (1 + amprodmatl.ScrapFactor)).Value;

                Base.MatlXref.Current.QtyReq  = amprodmatl.QtyRoundUp == false ? qtyReqWOScrap : Math.Ceiling(qtyReqWOScrap);
                Base.MatlXref.Current.MatlQty = wrkMatl.QtyAvail > wrkMatl.QtyReq ? wrkMatl.QtyReq : wrkMatl.QtyAvail;
            }
        }
        #endregion
    }
}