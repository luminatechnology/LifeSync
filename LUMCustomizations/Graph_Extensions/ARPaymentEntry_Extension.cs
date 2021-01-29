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

        #endregion
    }
}
