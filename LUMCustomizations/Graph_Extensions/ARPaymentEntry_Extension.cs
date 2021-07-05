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

                if (row.CuryID != aRInvoice.CuryID && aPPaymentVendorCrossRateAttr == "1" && Convert.ToDecimal(aRAdjust.AdjdCuryRate) != 1.00m)
                {
                    var curyInfoID = SelectFrom<CurrencyInfo>.Where<CurrencyInfo.curyInfoID.IsEqual<@P.AsInt>>.View.Select(Base, aRInvoice.CuryInfoID).TopFirst?.CuryRate;
                    aRAdjust.AdjdCuryRate = curyInfoID == null ? aRAdjust.AdjdCuryRate : curyInfoID;
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

        protected void _(Events.FieldUpdated<ARAdjust.adjdRefNbr> e)
        {
            if (e.NewValue != null)
            {

                var row = Base.Document.Current;
                var aPPaymentVendorCrossRateAttr = SelectFrom<CSAnswers>.
                                                    LeftJoin<BAccountR>.On<CSAnswers.refNoteID.IsEqual<BAccountR.noteID>>.
                                                    LeftJoin<ARPayment>.On<BAccountR.bAccountID.IsEqual<ARPayment.customerID>>.
                                                    Where<ARPayment.customerID.IsEqual<@P.AsInt>.And<CSAnswers.attributeID.IsEqual<VendorCrossRateAttr>>>.
                                                    View.Select(Base, row.CustomerID).TopFirst?.Value;
                var tt = ((ARAdjust)e.Row).AdjdDocType;
                var tttt = ((ARAdjust)e.Row).AdjdRefNbr;
                var aRInvoiceCurTest = SelectFrom<ARInvoice>.View.Select(Base).RowCast<ARInvoice>().ToList();
                var temp = aRInvoiceCurTest.FirstOrDefault(x => x.DocType == e.Cache.GetValue<ARAdjust.adjdDocType>(e.Row) && x.RefNbr == e.Cache.GetValue<ARAdjust.adjdRefNbr>(e.Row))?.CuryInfoID;
                
                var aRInvoiceCur = SelectFrom<ARInvoice>.Where<ARInvoice.docType.IsEqual<@P.AsString>.And<ARInvoice.refNbr.IsEqual<@P.AsString>>>.
                                    View.Select(Base, ((ARAdjust)e.Row).AdjdDocType, ((ARAdjust)e.Row).AdjdRefNbr);
                var aRInvoiceCuryInfoID = aRInvoiceCur.TopFirst?.CuryInfoID;
                
                if (row.CuryID != aRInvoiceCur.TopFirst?.CuryID && aPPaymentVendorCrossRateAttr == "1" && Convert.ToDecimal(e.Cache.GetValue<ARAdjust.adjdCuryRate>(e.Row)) != 1.00m && aRInvoiceCuryInfoID != null)
                {
                    var curyInfoCuryRate = SelectFrom<CurrencyInfo>.Where<CurrencyInfo.curyInfoID.IsEqual<@P.AsInt>>.View.Select(Base, aRInvoiceCuryInfoID).TopFirst?.CuryRate;
                    e.Cache.SetValueExt<ARAdjust.adjdCuryRate>(e.Row, curyInfoCuryRate == null ? Base.Adjustments.Current.AdjdCuryRate : curyInfoCuryRate);
                }
            }
        }
        #endregion
    }
}
