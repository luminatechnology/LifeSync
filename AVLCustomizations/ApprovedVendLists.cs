using System;
using PX.Data;

namespace AVLCustomizations
{
    [Serializable]
    [PXCacheName("ApprovedVendLists")]
    public class ApprovedVendLists : IBqlTable
    {
        #region Avlnbr
        [PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AVL Nbr")]
        public virtual string Avlnbr { get; set; }
        public abstract class avlnbr : PX.Data.BQL.BqlString.Field<avlnbr> { }
        #endregion

        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        protected bool? _Selected = false;
        [PXBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr", Enabled = false, Visible = false)]
        [PXLineNbr(typeof(AVLTable.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region AVLStatus
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        public virtual string AVLStatus { get; set; }
        public abstract class aVLStatus : PX.Data.BQL.BqlString.Field<aVLStatus> { }
        #endregion

        #region Avldate
        [PXDBDate()]
        [PXUIField(DisplayName = "Date")]
        public virtual DateTime? Avldate { get; set; }
        public abstract class avldate : PX.Data.BQL.BqlDateTime.Field<avldate> { }
        #endregion

        #region Descripton
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Descripton")]
        public virtual string Descripton { get; set; }
        public abstract class descripton : PX.Data.BQL.BqlString.Field<descripton> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor")]
        public virtual int? VendorID { get; set; }
        public abstract class vendorid : PX.Data.BQL.BqlString.Field<vendorid> { }
        #endregion

        #region VendorName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Name")]
        public virtual string VendorName { get; set; }
        public abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region InventoryDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Desc")]
        public virtual string InventoryDesc { get; set; }
        public abstract class inventoryDesc : PX.Data.BQL.BqlString.Field<inventoryDesc> { }
        #endregion

        #region Remark
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region ApprovedByID
        public virtual Guid? ApprovedByID { get; set; }
        public abstract class approvedByID : PX.Data.BQL.BqlGuid.Field<approvedByID> { }
        #endregion

        #region ApprovedDateTime
        public virtual DateTime? ApprovedDateTime { get; set; }
        public abstract class approvedDateTime : PX.Data.BQL.BqlDateTime.Field<approvedDateTime> { }
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