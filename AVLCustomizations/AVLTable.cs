using System;
using System.Collections;
using System.Reflection;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CS;

namespace AVLCustomizations
{
    public class AVLActionCustomSelectorAttribute : PXCustomSelectorAttribute
    {
        public AVLActionCustomSelectorAttribute() : base(typeof(AVLTable.avlnbr), 
                                                         typeof(AVLTable.aVLStatus),
                                                         typeof(AVLTable.avldate),
                                                         typeof(AVLTable.descripton))
        { }
        //=> this.DescriptionField = typeof(AVLTable.descripton);

        protected virtual IEnumerable GetRecords()
        {
            AVLTable aVLTableCache = (AVLTable)this._Graph.Caches[typeof(AVLTable)].Current;
            foreach (AVLTable aVL in PXSelect<AVLTable, Where<AVLTable.aVLAction, Equal<Required<AVLTable.aVLAction>>, 
                                                          And<AVLTable.lineCntr, NotEqual<Zero>>>>.
                                                          Select(this._Graph, aVLTableCache.AVLAction))
            {
                yield return aVL;
            }
        }
    }

    [Serializable]
    [PXCacheName("AVLTable")]
    public class AVLTable : IBqlTable
    {
        #region Avlnbr
        [PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AVL Nbr.2", Required = true)]
        [AutoNumber(typeof(APSetup.checkNumberingID), typeof(AccessInfo.businessDate))]
        //[PXSelector(typeof(Search<AVLTable.avlnbr>),
        //            typeof(AVLTable.avlnbr),
        //            typeof(AVLTable.aVLStatus),
        //            typeof(AVLTable.avldate),
        //            typeof(AVLTable.descripton))]
        [AVLActionCustomSelector()]
        public virtual string Avlnbr { get; set; }
        public abstract class avlnbr : PX.Data.BQL.BqlString.Field<avlnbr> { }
        #endregion

        #region AVLAction
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Action", Enabled = false)]
        [PXStringList(
                new string[] { "Create", "Cancel" },
                new string[] { "Create", "Cancel" })]
        public virtual string AVLAction { get; set; }
        public abstract class aVLAction : PX.Data.BQL.BqlString.Field<aVLAction> { }
        #endregion

        #region AVLStatus
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXStringList(
                new string[] { "0", "1", "2", "3" },
                new string[] { "On Hold", "Submitted", "Approved", "Cancelled" })]
        [PXDefault("0")]
        public virtual string AVLStatus { get; set; }
        public abstract class aVLStatus : PX.Data.BQL.BqlString.Field<aVLStatus> { }
        #endregion

        #region Avldate
        [PXDBDate()]
        [PXUIField(DisplayName = "Date")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? Avldate { get; set; }
        public abstract class avldate : PX.Data.BQL.BqlDateTime.Field<avldate> { }
        #endregion

        #region Descripton
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Descripton")]
        public virtual string Descripton { get; set; }
        public abstract class descripton : PX.Data.BQL.BqlString.Field<descripton> { }
        #endregion

        #region LineCntr
        [PXDBInt()]
        [PXDefault(0)]
        public virtual Int32? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
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