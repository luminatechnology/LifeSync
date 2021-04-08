using System;
using PX.Data;
using static PX.Objects.IN.INReceiptEntry_Extension;

namespace PX.Objects.IN
{
    public class INItemLotSerialExt : PXCacheExtension<PX.Objects.IN.INItemLotSerial>
    {
        #region UsrPurchReceivingDate
        /// <summary>
        /// Get transaction date from receipt.
        /// </summary>
        [PXDate]
        [PXUIField(DisplayName = "Purchase Receiving Date", Enabled = false)]
        [PXDBScalar(typeof(Search<INTran.tranDate, Where<INTran.inventoryID, Equal<INItemLotSerial.inventoryID>,  
                                                         And<INTran.lotSerialNbr, Equal<INItemLotSerial.lotSerialNbr>>>,
                                                   OrderBy<Asc<INTran.tranDate>>>))]
        public virtual DateTime? UsrPurchReceivingDate { get; set; }
        public abstract class usrPurchReceivingDate : PX.Data.BQL.BqlDateTime.Field<usrPurchReceivingDate> { }
        #endregion
    }
}
