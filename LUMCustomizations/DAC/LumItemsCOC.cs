using System;
using PX.Data;
using PX.Objects.IN;
using PX.Common.Extensions;

namespace LumCustomizations.DAC
{
    [Serializable]
    [PXCacheName("LumItemsCOC")]
    public class LumItemsCOC : IBqlTable
    {
        #region InventoryID
        [Inventory(IsKey = true, DisplayName = "Inventory ID")]
        [PXUIField(Required = true)]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region EndCustomer
        [PXStringList()]
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "End Customer", Required = true)]
        public virtual string EndCustomer { get; set; }
        public abstract class endCustomer : PX.Data.BQL.BqlString.Field<endCustomer> { }
        #endregion

        #region MaterialProductDesc
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Material Product Desc")]
        public virtual string MaterialProductDesc { get; set; }
        public abstract class materialProductDesc : PX.Data.BQL.BqlString.Field<materialProductDesc> { }
        #endregion

        #region MaterialProductDesc2
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Material Product Desc2")]
        public virtual string MaterialProductDesc2 { get; set; }
        public abstract class materialProductDesc2 : PX.Data.BQL.BqlString.Field<materialProductDesc2> { }
        #endregion

        #region COCProductDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "COCProduct Desc")]
        public virtual string COCProductDesc { get; set; }
        public abstract class cOCProductDesc : PX.Data.BQL.BqlString.Field<cOCProductDesc> { }
        #endregion
         
        #region TESTroductDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "TESTProduct Desc")]
        public virtual string TESTProductDesc { get; set; }
        public abstract class tESTProductDesc : PX.Data.BQL.BqlString.Field<tESTProductDesc> { }
        #endregion

        #region REROHSProductDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "REROHSProduct Desc")]
        public virtual string REROHSProductDesc { get; set; }
        public abstract class rEROHSProductDesc : PX.Data.BQL.BqlString.Field<rEROHSProductDesc> { }
        #endregion

        #region REACHProductDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "REACHProduct Desc")]
        public virtual string REACHProductDesc { get; set; }
        public abstract class rEACHProductDesc : PX.Data.BQL.BqlString.Field<rEACHProductDesc> { }
        #endregion

        #region REACHProductDesc2
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "REACHProduct Desc2")]
        public virtual string REACHProductDesc2 { get; set; }
        public abstract class rEACHProductDesc2 : PX.Data.BQL.BqlString.Field<rEACHProductDesc2> { }
        #endregion

        #region Compliantproductdesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Compliantproductdesc")]
        public virtual string Compliantproductdesc { get; set; }
        public abstract class compliantproductdesc : PX.Data.BQL.BqlString.Field<compliantproductdesc> { }
        #endregion

        #region QCProductDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "QCProduct Desc")]
        public virtual string QCProductDesc { get; set; }
        public abstract class qCProductDesc : PX.Data.BQL.BqlString.Field<qCProductDesc> { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}