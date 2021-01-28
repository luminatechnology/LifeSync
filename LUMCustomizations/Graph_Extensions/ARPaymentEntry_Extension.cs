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

        protected virtual void _(Events.FieldUpdated<ARAdjust.curyAdjgAmt> e, PXFieldUpdated invokeMethod)
        {
            invokeMethod(e.Cache, e.Args);
            // ReCalc usrRemCuryAdjdAmt
            e.Cache.SetValue<ARAdjustExtension.usrRemCuryAdjdAmt>(e.Row,
               (decimal?)e.Cache.GetValue<ARAdjustExtension.usrBaseBalance>(e.Row) - (decimal?)e.Cache.GetValue<ARAdjust.curyAdjdAmt>(e.Row));
        }

        #endregion
    }
}
