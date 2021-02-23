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
                var qtyReqWOScrap = GetQtyReqWOScrap(amprodmatl, amproditem.BaseQtytoProd);

                // Avoid the system from performing several processing for different product material inventory and update the wrong material inventory quantity request.
                if (wrkMatl.InventoryID == amprodmatl.InventoryID)
                { 
                    Base.MatlXref.Current.QtyReq  = amprodmatl.QtyRoundUp == false ? qtyReqWOScrap : Math.Ceiling(qtyReqWOScrap);
                    Base.MatlXref.Current.MatlQty = wrkMatl.QtyAvail > wrkMatl.QtyReq ? wrkMatl.QtyReq : wrkMatl.QtyAvail;
                }
            }
        }
        #endregion

        #region Static method
        public static decimal GetQtyReqWOScrap(AMProdMatl prodMatl, decimal? qtyToProd)
        {
            if (prodMatl == null)
            {
                throw new ArgumentNullException(nameof(prodMatl));
            }

            if (qtyToProd.GetValueOrDefault() == 0 ||
                prodMatl.IsFixedMaterial.GetValueOrDefault() && prodMatl.QtyActual.GetValueOrDefault() != 0)
            {
                return 0m;
            }

            var multiplier = prodMatl.IsByproduct.GetValueOrDefault() ? -1 : 1;

            // Since Acumatica considers rounding up logic on each line of AMProdMatl, customization will skip this logic. 
            var totalQtyReq      = AMProdMatl.GetTotalRequiredQty(prodMatl, qtyToProd.GetValueOrDefault(), false); //prodMatl.GetTotalReqQty(qtyToProd.GetValueOrDefault());
            var remainingMatlQty = prodMatl.QtyRemaining.GetValueOrDefault() * multiplier;
            var actualQty        = prodMatl.QtyActual.GetValueOrDefault();
            var qtyReqWOScrap    = (totalQtyReq / (1 + prodMatl.ScrapFactor)).Value;
            // Same as QtyRemaining PXFormula
            var calcRemainingQty = (prodMatl.IsByproduct.GetValueOrDefault() ? Math.Min(totalQtyReq - actualQty, 0) : totalQtyReq - actualQty) * multiplier;

            totalQtyReq *= multiplier;
            var remainingQty     = Math.Max(remainingMatlQty, calcRemainingQty.NotLessZero());
            var materialTotalQty = remainingQty < totalQtyReq ? remainingQty : totalQtyReq;
            // Because the customization has been deducted for scrap, the minimum value is selected.
            var resultQty        = Math.Min(qtyReqWOScrap - actualQty, materialTotalQty);

            return resultQty;
        }
        #endregion
    }
}