using PX.Data;
using PX.Data.BQL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.IN;
using System;

namespace PX.Objects.SO
{
    public class SOShipLineExt : PXCacheExtension<SOShipLine>
    {
        #region UsrCartonSize
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Carton Size")]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<InventoryItem, On<InventoryItem.noteID, Equal<CSAnswers.refNoteID>, 
                                                                               And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.CartonSizeAttr>>>>, 
                                                   Where<InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), 
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SOShipLine.inventoryID>))]
        public virtual string UsrCartonSize { get; set; }
        public abstract class usrCartonSize : PX.Data.BQL.BqlString.Field<SOShipLineExt.usrCartonSize> { }
        #endregion

        #region UsrCartonQty
        [PXDBQuantity]
        [PXUIField(DisplayName = "Carton Qty.")]
        [PXFormula(typeof(Switch<Case<Where<SOShipLineExt.usrQtyCarton, IsNotNull>, Div<SOShipLine.shippedQty, SOShipLineExt.usrQtyCarton>>, 
                                 Case<Where<SOShipLineExt.usrQtyCarton, IsNull>, Div<SOShipLine.shippedQty, SOShipmentEntry_Extension.decimal5000>>>))]
        public virtual decimal? UsrCartonQty { get; set; }
        public abstract class usrCartonQty : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrCartonQty> { }
        #endregion

        #region UsrNetWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Net Weight")]
        [PXFormula(typeof(Mult<SOShipLine.shippedQty, SOShipLineExt.usrBaseItemWeight>))]
        public virtual decimal? UsrNetWeight { get; set; }
        public abstract class usrNetWeight : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrNetWeight> { }
        #endregion

        #region UsrGrossWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Gross Weight")]
        [PXFormula(typeof(Mult<SOShipLine.shippedQty, SOShipLineExt.usrGrsWeight>))]
        public virtual decimal? UsrGrossWeight { get; set; }
        public abstract class usrGrossWeight : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrGrossWeight> { }
        #endregion

        #region UsrPalletWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Pallet Weight")]
        public virtual decimal? UsrPalletWeight { get; set; }
        public abstract class usrPalletWeight : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrPalletWeight> { }
        #endregion

        #region UsrMEAS
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "MEA S")]
        [PXFormula(typeof(Mult<SOShipLineExt.usrCartonQty, SOShipLineExt.usrBaseItemVolume>))]
        public virtual decimal? UsrMEAS { get; set; }
        public abstract class usrMEAS : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrMEAS> { }
        #endregion

        #region UsrDimWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Dimensional Weight")]
        public virtual decimal? UsrDimWeight { get; set; }
        public abstract class usrDimWeight : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrDimWeight> { }
        #endregion

        #region UsrBRNbr
        [PXDBString(255, InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "BR Nbr.")]
        public virtual string UsrBRNbr { get; set; }
        public abstract class usrBRNbr : PX.Data.BQL.BqlString.Field<usrBRNbr> { }
        #endregion

        #region Unbound Custom Fields
            #region UsrQtyCarton
            [PXString]
            [PXDBScalar(typeof(Search2<CSAnswers.value, InnerJoin<InventoryItem, On<InventoryItem.noteID, Equal<CSAnswers.refNoteID>, 
                                                                                    And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.QtyCartonAttr>>>>, 
                                                        Where<InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
            [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<InventoryItem, On<InventoryItem.noteID, Equal<CSAnswers.refNoteID>, 
                                                                                   And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.QtyCartonAttr>>>>, 
                                                       Where<InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), 
                       PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual string UsrQtyCarton { get; set; }
            public abstract class usrQtyCarton : PX.Data.BQL.BqlString.Field<SOShipLineExt.usrQtyCarton> { }
            #endregion

            #region UsrGrsWeight
            [PXString]
            [PXDBScalar(typeof(Search2<CSAnswers.value, InnerJoin<InventoryItem, On<InventoryItem.noteID, Equal<CSAnswers.refNoteID>, 
                                                                                    And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.GrsWeightAttr>>>>, 
                                                        Where<InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
            [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<InventoryItem, On<InventoryItem.noteID, Equal<CSAnswers.refNoteID>, 
                                                                                   And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.GrsWeightAttr>>>>, 
                                                       Where<InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), 
                       PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual string UsrGrsWeight { get; set; }
            public abstract class usrGrsWeight : PX.Data.BQL.BqlString.Field<SOShipLineExt.usrGrsWeight> { }
            #endregion

            #region UsrBaseItemWeight
            [PXQuantity(6)]
            [PXDBScalar(typeof(Search<InventoryItem.baseItemWeight, Where<InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
            [PXDefault(typeof(Search<InventoryItem.baseItemWeight, Where<InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), 
                       PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual Decimal? UsrBaseItemWeight { get; set; }
            public abstract class usrBaseItemWeight : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrBaseItemWeight> { }
            #endregion

            #region UsrBaseItemVolume
            [PXQuantity(6)]
            [PXDBScalar(typeof(Search<InventoryItem.baseItemVolume, Where<InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
            [PXDefault(typeof(Search<InventoryItem.baseItemVolume, Where<InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), 
                       PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual Decimal? UsrBaseItemVolume { get; set; }
            public abstract class usrBaseItemVolume : PX.Data.BQL.BqlDecimal.Field<SOShipLineExt.usrBaseItemVolume> { }
            #endregion
        #endregion
    }
}
