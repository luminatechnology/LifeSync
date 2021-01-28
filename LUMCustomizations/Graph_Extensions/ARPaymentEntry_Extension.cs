﻿using LUMCustomizations.Library;
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
        [PXUIField]
        [PXDBDecimal(2)]
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        protected virtual void _(Events.CacheAttached<ARAdjust.curyAdjdAmt> e) { }

        protected virtual void _(Events.RowSelected<ARAdjust> e, PXRowSelected BaseMethod)
        {
            BaseMethod(e.Cache, e.Args);
            var library = new LumLibrary();
            PXUIFieldAttribute.SetVisible<ARAdjustExtension.usrBaseBalance>(e.Cache, null, library.GetCrossRateOverride);
            PXUIFieldAttribute.SetVisible<ARAdjust.curyAdjdAmt>(e.Cache, null, library.GetCrossRateOverride);
        }

        protected virtual void _(Events.FieldSelecting<ARAdjustExtension.usrBaseBalance> e)
        {
            if (e.Row == null)
                return;
           
            var ARInvocieCuryInfo = from t in new PXGraph().Select<ARInvoice>()
                                    where t.RefNbr == (e.Row as ARAdjust).AdjdRefNbr
                                    select t;
            e.ReturnValue = ARInvocieCuryInfo.FirstOrDefault()?.CuryLineTotal;
        }
    }
}