using LumCustomizations.Descriptor;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.SO;
using System;

namespace JAMS.AM
{
    public class AMProdItemExt : PXCacheExtension<AMProdItem>
    {
        [PXGuid]
        [PXUIField(DisplayName = "SO Search")]
        [SOLineSelector(typeof(AMProdItem.inventoryID))]
        public virtual Guid? UsrSOLineNoteID { get; set; }
        public abstract class usrSOLineNoteID : BqlType<IBqlGuid, Guid>.Field<AMProdItemExt.usrSOLineNoteID> { }

        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "SO Order Nbr.", Enabled = false)]
        [PXSelector(typeof(Search<SOOrder.orderNbr>), CacheGlobal = true)]
        [PXFormula(typeof(Default<AMProdItemExt.usrSOLineNoteID>))]
        [PXDefault(typeof(Search<SOLine.orderNbr, Where<SOLine.noteID, Equal<Current<AMProdItemExt.usrSOLineNoteID>>>>), 
                   PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrSOOrderNbr { get; set; }
        public abstract class usrSOOrderNbr : BqlType<IBqlString, string>.Field<AMProdItemExt.usrSOOrderNbr> { }

        [PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "SO Order Type", Visible = false)]
        [PXSelector(typeof(Search<SOOrderType.orderType>), CacheGlobal = true)]
        [PXFormula(typeof(Default<AMProdItemExt.usrSOLineNoteID>))]
        [PXDefault(typeof(Search<SOLine.orderType, Where<SOLine.noteID, Equal<Current<AMProdItemExt.usrSOLineNoteID>>>>), 
                   PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrSOOrderType { get; set; }
        public abstract class usrSOOrderType : BqlType<IBqlString, string>.Field<AMProdItemExt.usrSOOrderType> { }

        [PXDBInt]
        [PXUIField(DisplayName = "SO Line Nbr.", Enabled = false)]
        [PXFormula(typeof(Default<AMProdItemExt.usrSOLineNoteID>))]
        [PXDefault(typeof(Search<SOLine.lineNbr, Where<SOLine.noteID, Equal<Current<AMProdItemExt.usrSOLineNoteID>>>>), 
                   PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrSOLineNbr { get; set; }
        public abstract class usrSOLineNbr : BqlType<IBqlInt, int>.Field<AMProdItemExt.usrSOLineNbr> { }

        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "SO Customer PO", Enabled = false)]
        [PXFormula(typeof(Default<AMProdItemExt.usrSOLineNoteID>))]
        [PXDefault(typeof(Search2<SOOrder.customerOrderNbr, InnerJoinSingleTable<SOLine, On<SOLine.orderType, Equal<SOLine.orderType>, 
                                                                                            And<SOLine.orderNbr, Equal<SOOrder.orderNbr>>>>, 
                                                            Where<SOLine.noteID, Equal<Current<AMProdItemExt.usrSOLineNoteID>>>>), 
                   PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrSOCustomerPO { get; set; }
        public abstract class usrSOCustomerPO : BqlType<IBqlString, string>.Field<AMProdItemExt.usrSOCustomerPO> { }
    }
}
