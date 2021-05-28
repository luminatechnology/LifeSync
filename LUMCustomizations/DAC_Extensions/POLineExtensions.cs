using PX.Common;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.PO
{
    public class POLineExt : PXCacheExtension<PX.Objects.PO.POLine>
    {
        #region UsrBubbleNumber
        [PXDBString(20)]
        [PXUIField(DisplayName = "Bubble Number")]
        public virtual string UsrBubbleNumber { get; set; }
        public abstract class usrBubbleNumber : PX.Data.BQL.BqlString.Field<usrBubbleNumber> { }
        #endregion

        #region UsrCapexTrackingNbr
        [PXDBString(30)]
        [PXUIField(DisplayName = "Capex Tracking Nbr")]
        public virtual string UsrCapexTrackingNbr { get; set; }
        public abstract class usrCapexTrackingNbr : PX.Data.BQL.BqlString.Field<usrCapexTrackingNbr> { }
        #endregion

    }
}