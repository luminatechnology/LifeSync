using System;
using System.Linq;
using LumCustomizations.DAC;
using LUMCustomizations.Library;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using static JAMS.AEF.SOOrderEntryAMExtension.AMEstimateReferenceSO;
using Location = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extension : PXGraphExtension<SOOrderEntry>
    {
        [PXUIField()]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        public virtual void _(Events.CacheAttached<SOOrder.orderTotal> e) { }

        /// <summary> SOOrder RowSelected Event </summary>
        protected virtual void _(Events.RowSelected<SOOrder> e)
        {
            var _library = new LumLibrary();

            PXUIFieldAttribute.SetVisible<SOOrderExt.usrPICustomerID>(e.Cache, null, _library.GetProformaInvoicePrinting);
            PXUIFieldAttribute.SetVisible<SOOrderExt.usrPICuryID>(e.Cache, null, _library.GetProformaInvoicePrinting);

            // Control Line PI Column Visible
            var _lineCache = Base.Transactions.Cache;
            PXUIFieldAttribute.SetVisible<SOLineExt.usrPIUnitPrice>(_lineCache, null, _library.GetProformaInvoicePrinting);
            PXUIFieldAttribute.SetEnabled<SOLineExt.usrPIUnitPrice>(_lineCache, null, _library.GetProformaInvoicePrinting);

            // Reset OrderToal Display Name
            var baseCompanyCuryID = _library.GetCompanyBaseCuryID();
            PXUIFieldAttribute.SetDisplayName<SOOrder.orderTotal>(e.Cache, $"Total in {baseCompanyCuryID}");
            PXUIFieldAttribute.SetEnabled<SOOrder.orderTotal>(e.Cache, null, false);
            PXUIFieldAttribute.SetVisible<SOOrder.orderTotal>(e.Cache, null, _library.GetShowingTotalInHome);
        }

        /// <summary> SOOrderExt.usrPICustomerID FieldUppdated Event </summary>
        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrPICustomerID> e)
        {
            int? oldPICustomerID = (int?)e.OldValue;
            int? newPICustomerID = ((SOOrder)e.Row).GetExtension<SOOrderExt>().UsrPICustomerID;
            var customerChanged = oldPICustomerID != null && newPICustomerID != oldPICustomerID;
            if (oldPICustomerID == null || customerChanged && !Base.HasDetailRecords())
            {
                var _graph = PXGraph.CreateInstance<SOOrderEntry>();
                var _cryIDs = from c in _graph.Select<PX.Objects.AR.Customer>()
                              where c.StatementCustomerID == newPICustomerID
                              select c.CuryID;
                if (_cryIDs != null)
                    e.Cache.SetValue<SOOrderExt.usrPICuryID>(e.Row, _cryIDs.FirstOrDefault());
                else
                    e.Cache.SetDefaultExt<SOOrderExt.usrPICuryID>(e.Row);
            }
            Base.Transactions.Cache.SetDefaultExt<SOLineExt.usrPIUnitPrice>(Base.Transactions.Current);
        }

        /// <summary> SOLine.curyUnitPrice FieldDefaulting Event </summary>
        protected virtual void _(Events.FieldDefaulting<SOLine.curyUnitPrice> e, PXFieldDefaulting baseMethod)
        {
            // Invoke BaseMethod 
            baseMethod(e.Cache, e.Args);
            e.Cache.SetDefaultExt<SOLineExt.usrPIUnitPrice>(e.Row);
        }

        /// <summary> SOLineExt.usrPIUnitPrice FieldDefaulting Event </summary>
        protected virtual void _(Events.FieldDefaulting<SOLineExt.usrPIUnitPrice> e)
        {
            var _lineRow = (SOLine)e.Row;
            if (_lineRow == null)
                return;
            var _headerRow = (Base.Caches[(typeof(SOOrder))].Current as SOOrder).GetExtension<SOOrderExt>();

            string customerPriceClass = PX.Objects.AR.ARPriceClass.EmptyPriceClass;
            PX.Objects.AR.ARSalesPriceMaint salesPriceMaint = PX.Objects.AR.ARSalesPriceMaint.SingleARSalesPriceMaint;
            PX.Objects.AR.ARSalesPriceMaint.SalesPriceItem priceItem = null;

            bool isPriceUpdateNeeded;
            using (var priceScope = GetPriceCalculationScope())
                isPriceUpdateNeeded = priceScope.IsUpdateNeeded<SOLine.inventoryID>();
            if (_lineRow.TranType == INTranType.Transfer)
                e.NewValue = 0m;
            else if (_lineRow.InventoryID != null && _lineRow.IsFree != true && !e.Cache.Graph.IsCopyPasteContext && isPriceUpdateNeeded)
            {
                try
                {
                    priceItem = salesPriceMaint.FindSalesPrice(
                        e.Cache,
                        customerPriceClass,
                        _headerRow.UsrPICustomerID,
                        _lineRow.InventoryID,
                        _lineRow.SiteID,
                        _headerRow.UsrPICuryID,
                        _headerRow.UsrPICuryID,
                        Math.Abs(_lineRow.Qty ?? 0m),
                        _lineRow.UOM,
                        Base.Document.Current.OrderDate.Value);
                    e.NewValue = priceItem?.Price ?? 0m;
                }
                catch (PXUnitConversionException) { }
            }
            else
                e.NewValue = _lineRow.GetExtension<SOLineExt>().UsrPIUnitPrice ?? 0m;
        }

        #region Method

        public virtual UpdateIfFieldsChangedScope GetPriceCalculationScope()
                    => new SOOrderPriceCalculationScope();

        public virtual bool ExtHasDetailRecords()
        {
            if (Base.Transactions.Current != null)
                return true;

            if (Base.Document.Cache.GetStatus(Base.Document.Current) == PXEntryStatus.Inserted)
                return Base.Transactions.Cache.IsDirty;
            else
                return Base.Transactions.Select().Count > 0;
        }

        #endregion

    }
}
