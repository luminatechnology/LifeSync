using LUMCustomizations.Library;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.AR.Standalone;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AR
{
    public class ARPaymentEntry_Extension : PXGraphExtension<ARPaymentEntry>
    {
        public class VendorCrossRateAttr : BqlString.Constant<VendorCrossRateAttr>
        {
            public VendorCrossRateAttr() : base("CROSSRATE") { }
        }
        
        // Delegate
        public IEnumerable Adjustments()
        {
            var newAdjustments = SelectFrom<ARAdjust>.
                                    LeftJoin<ARInvoice>.On<ARInvoice.docType.IsEqual<ARAdjust.adjdDocType>.And<ARInvoice.refNbr.IsEqual<ARAdjust.adjdRefNbr>>>.
                                    InnerJoin<ARRegisterAlias>.On<ARRegisterAlias.docType.IsEqual<ARAdjust.adjdDocType>.And<ARRegisterAlias.refNbr.IsEqual<ARAdjust.adjdRefNbr>>>.
                                    LeftJoin<ARTran>.On<ARInvoice.paymentsByLinesAllowed.IsEqual<True>.And<ARTran.tranType.IsEqual<ARAdjust.adjdDocType>.And<ARTran.refNbr.IsEqual<ARAdjust.adjdRefNbr>.And<ARTran.lineNbr.IsEqual<ARAdjust.adjdLineNbr>>>>>.
                                    Where<ARAdjust.adjgDocType.IsEqual<ARPayment.docType.FromCurrent>.And<ARAdjust.adjgRefNbr.IsEqual<ARPayment.refNbr.FromCurrent>.And<ARAdjust.released.IsNotEqual<True>>>>.
                                    View.Select(Base);

            var row = Base.Document.Current;
            if (row == null) return newAdjustments;

            var aPPaymentVendorCrossRateAttr = SelectFrom<CSAnswers>.
                                                LeftJoin<BAccountR>.On<CSAnswers.refNoteID.IsEqual<BAccountR.noteID>>.
                                                LeftJoin<ARPayment>.On<BAccountR.bAccountID.IsEqual<ARPayment.customerID>>.
                                                Where<ARPayment.refNbr.IsEqual<@P.AsString>.And<ARPayment.docType.IsEqual<@P.AsString>>.And<CSAnswers.attributeID.IsEqual<VendorCrossRateAttr>>>.
                                                View.Select(Base, row.RefNbr, row.DocType).TopFirst?.Value;
            foreach (PXResult<ARAdjust, ARInvoice, ARRegisterAlias, ARTran> adjustment in newAdjustments)
            {
                ARAdjust aRAdjust = adjustment;
                ARInvoice aRInvoice = adjustment;

                if (row.CuryID != aRInvoice.CuryID && aPPaymentVendorCrossRateAttr == "1" && Convert.ToDecimal(aRAdjust.AdjdCuryRate) != 1.00m && aRInvoice.CuryInfoID != null)
                {
                    var curyInfo = SelectFrom<CurrencyInfo>.Where<CurrencyInfo.curyInfoID.IsEqual<@P.AsInt>>.View.Select(Base, aRInvoice.CuryInfoID).TopFirst;
                    var curyInfoCuryRate = curyInfo?.CuryMultDiv == "M" ? curyInfo?.CuryRate : curyInfo?.RecipRate;
                    aRAdjust.AdjdCuryRate = curyInfoCuryRate == null ? aRAdjust.AdjdCuryRate : curyInfoCuryRate;
                }
            }
            return newAdjustments;
        }
        
        #region Override DAC
        [PXUIField(Enabled = false)]
        [PXDBDecimal(2)]
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        protected virtual void _(Events.CacheAttached<ARAdjust.curyAdjdAmt> e) { }
        #endregion

        #region Event handler

        /// <summary> Row Selected Event </summary>
        protected virtual void _(Events.RowSelected<ARAdjust> e, PXRowSelected BaseMethod)
        {
            BaseMethod(e.Cache, e.Args);
            var library = new LumLibrary();
            PXUIFieldAttribute.SetVisible<ARAdjustExtension.usrBaseBalance>(e.Cache, null, library.GetCrossRateOverride);
            PXUIFieldAttribute.SetVisible<ARAdjust.curyAdjdAmt>(e.Cache, null, library.GetCrossRateOverride);
        }

        /// <summary> usrRemCuryAdjdAmt Selecting Event </summary>
        protected virtual void _(Events.FieldSelecting<ARAdjustExtension.usrRemCuryAdjdAmt> e)
        {
            if (e.Row == null)
                return;
            var _refBalance = e.Cache.GetExtension<ARAdjustExtension>(e.Row).UsrBaseBalance;
            var _curyAdjdAmt = e.Cache.GetValue<ARAdjust.curyAdjdAmt>(e.Row) ?? 0;
            e.ReturnValue = _refBalance - (Decimal?)_curyAdjdAmt;
        }
        
        protected virtual void ARAdjust_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            var row = Base.Document.Current;
            var aPPaymentVendorCrossRateAttr = SelectFrom<CSAnswers>.
                                                LeftJoin<BAccountR>.On<CSAnswers.refNoteID.IsEqual<BAccountR.noteID>>.
                                                LeftJoin<ARPayment>.On<BAccountR.bAccountID.IsEqual<ARPayment.customerID>>.
                                                Where<ARPayment.customerID.IsEqual<@P.AsInt>.And<CSAnswers.attributeID.IsEqual<VendorCrossRateAttr>>>.
                                                View.Select(Base, row.CustomerID).TopFirst?.Value;
            var arRow = e.Row as ARAdjust;
            var aRInvoiceCurData = SelectFrom<ARInvoice>.View.Select(Base).RowCast<ARInvoice>().ToList().FirstOrDefault(x => x.DocType == arRow.AdjdDocType && x.RefNbr == arRow.AdjdRefNbr);
            if (row.CuryID != aRInvoiceCurData?.CuryID && aPPaymentVendorCrossRateAttr == "1" && Convert.ToDecimal(arRow.AdjdCuryRate) != 1.00m && aRInvoiceCurData?.CuryInfoID != null)
            {
                var curyInfo = SelectFrom<CurrencyInfo>.Where<CurrencyInfo.curyInfoID.IsEqual<@P.AsInt>>.View.Select(Base, aRInvoiceCurData?.CuryInfoID).TopFirst;
                var curyInfoCuryRate = curyInfo?.CuryMultDiv == "M" ? curyInfo?.CuryRate : curyInfo?.RecipRate;
                sender.SetValueExt<ARAdjust.adjdCuryRate>(e.Row, curyInfoCuryRate == null ? arRow.AdjdCuryRate : curyInfoCuryRate);
            }

            var sumAmountPaid = Base.Adjustments.Cache.Cached.RowCast<ARAdjust>().Select(x => x.CuryAdjgAmt).Sum();
            row.CuryUnappliedBal = -sumAmountPaid;
            row.CuryApplAmt = sumAmountPaid;
        }
        #endregion
    }
}
