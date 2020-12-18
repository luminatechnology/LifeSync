using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;

namespace AVLCustomizations
{
    [Serializable]
    [PXCacheName("AVLLine")]
    public class AVLLine : IBqlTable
    {
        #region Avlnbr
        [PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Avlnbr", Enabled = false, Visible = false)]
        [PXDBDefault(typeof(AVLTable.avlnbr))]
        [PXParent(typeof(SelectFrom<AVLTable>.Where<AVLTable.avlnbr.IsEqual<AVLLine.avlnbr.FromCurrent>>))]
        public virtual string Avlnbr { get; set; }
        public abstract class avlnbr : PX.Data.BQL.BqlString.Field<avlnbr> { }
        #endregion

        #region OrigAVLNbr
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "OrigAVLNbr", Enabled = false, Visible = false)]
        public virtual string OrigAVLNbr { get; set; }
        public abstract class origAVLNbr : PX.Data.BQL.BqlString.Field<origAVLNbr> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr", Enabled = false, Visible = false)]
        [PXLineNbr(typeof(AVLTable.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor")]
        [PXDimensionSelectorAttribute("VENDOR", typeof(Search<VendorR.bAccountID,
                                                        Where<VendorR.type, Equal<BAccountType.vendorType>,
                                                        And<VendorR.status, Equal<BAccount.status.active>>>>),
                                                typeof(VendorR.acctCD),
                                                new Type[] { typeof(VendorR.bAccountID), typeof(VendorR.acctCD), typeof(VendorR.acctName) }, 
                                      IsDirty = true)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorid : PX.Data.BQL.BqlString.Field<vendorid> { }
        #endregion

        #region VendorName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Name", Enabled = false)]
        public virtual string VendorName { get; set; }
        public abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXSelector(typeof(Search3<InventoryItem.inventoryID, OrderBy<Asc<InventoryItem.inventoryCD>>>),
                    typeof(InventoryItem.inventoryCD),
                    typeof(InventoryItem.descr),
                    typeof(InventoryItem.postClassID),
                    typeof(InventoryItem.itemStatus),
                    typeof(InventoryItem.itemType),
                    typeof(InventoryItem.baseUnit),
                    typeof(InventoryItem.salesUnit),
                    typeof(InventoryItem.purchaseUnit),
                    typeof(InventoryItem.basePrice),
                    typeof(InventoryItem.aBCCodeID),
                    SubstituteKey = typeof(InventoryItem.inventoryCD))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region InventoryDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Desc", Enabled = false)]
        public virtual string InventoryDesc { get; set; }
        public abstract class inventoryDesc : PX.Data.BQL.BqlString.Field<inventoryDesc> { }
        #endregion

        #region Remark
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
    }
}