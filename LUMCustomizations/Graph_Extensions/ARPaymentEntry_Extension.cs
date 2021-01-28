using LUMCustomizations.Library;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AR
{
    public class ARPaymentEntry_Extension : PXGraphExtension<ARPaymentEntry>
    {
        #region Override DAC
        [PXUIField]
        [PXDBDecimal(2)]
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        protected virtual void _(Events.CacheAttached<ARAdjust.curyAdjdAmt> e) { }
        #endregion

        #region Event handler

        protected virtual void _(Events.RowSelected<ARAdjust> e, PXRowSelected BaseMethod)
        {
            BaseMethod(e.Cache, e.Args);
            var library = new LumLibrary();
            PXUIFieldAttribute.SetVisible<ARAdjustExtension.usrBaseBalance>(e.Cache, null, library.GetCrossRateOverride);
            PXUIFieldAttribute.SetVisible<ARAdjust.curyAdjdAmt>(e.Cache, null, library.GetCrossRateOverride);
        }

        //protected virtual void _(Events.FieldUpdated<ARAdjust.curyAdjdAmt> e, PXFieldUpdated invokeMethod)
        //{
        //    invokeMethod(e.Cache, e.Args);
        //    // ReCalc usrRemCuryAdjdAmt
        //    var _RefBalance = e.Cache.GetExtension<ARAdjustExtension>(e.Row).UsrBaseBalance;
        //    // var _RefBalance = e.Cache.GetValueExt<ARAdjustExtension.usrBaseBalance>(e.Row);
        //    var _curyAdjdAmt = e.Cache.GetValue<ARAdjust.curyAdjdAmt>(e.Row);
        //    e.Cache.SetValueExt<ARAdjustExtension.usrRemCuryAdjdAmt>(e.Row, _RefBalance - (decimal?)_curyAdjdAmt);
        //}

        protected virtual void _(Events.FieldSelecting<ARAdjustExtension.usrRemCuryAdjdAmt> e)
        {
            if (e.Row == null)
                return;
            var ARInvocieCuryInfo = from t in new PXGraph().Select<ARInvoice>()
                                    where t.RefNbr == (e.Row as ARAdjust).AdjdRefNbr
                                    select t;
            var _refBalance = ARInvocieCuryInfo.FirstOrDefault()?.CuryLineTotal ?? 0;
            var _curyAdjdAmt = e.Cache.GetValue<ARAdjust.curyAdjdAmt>(e.Row) ?? 0;
            e.ReturnValue = _refBalance - (Decimal?)_curyAdjdAmt;
        }

        //protected virtual void _(Events.FieldUpdated<ARAdjust.adjdRefNbr> e, PXFieldUpdated invokeMethod)
        //{
        //    invokeMethod(e.Cache, e.Args);
        //    var _RefBalance = e.Cache.GetExtension<ARAdjustExtension>(e.Row).UsrBaseBalance;
        //    var _curyAdjdAmt = e.Cache.GetValue<ARAdjust.curyAdjdAmt>(e.Row);
        //    e.Cache.SetValueExt<ARAdjustExtension.usrRemCuryAdjdAmt>(e.Row, _RefBalance - (decimal?)_curyAdjdAmt);
        //}

        protected virtual void _(Events.FieldSelecting<ARAdjustExtension.usrBaseBalance> e)
        {
            if (e.Row == null)
                return;

            var ARInvocieCuryInfo = from t in new PXGraph().Select<ARInvoice>()
                                    where t.RefNbr == (e.Row as ARAdjust).AdjdRefNbr
                                    select t;
            e.ReturnValue = ARInvocieCuryInfo.FirstOrDefault()?.CuryLineTotal;
        }

        #endregion
    }
}
