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
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Carton Size")]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<PX.Objects.IN.InventoryItem, On<PX.Objects.IN.InventoryItem.noteID, Equal<CSAnswers.refNoteID>, And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.CartonSizeAttr>>>>, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SOShipLine.inventoryID>))]
        public virtual string UsrCartonSize { get; set; }

        [PXDBQuantity]
        [PXUIField(DisplayName = "Carton Qty.")]
        [PXFormula(typeof(Switch<Case<Where<SOShipLineExt.usrQtyCarton, PX.Data.IsNotNull>, Div<SOShipLine.shippedQty, SOShipLineExt.usrQtyCarton>>, Case<Where<SOShipLineExt.usrQtyCarton, PX.Data.IsNull>, Div<SOShipLine.shippedQty, SOShipmentEntry_Extension.decimal5000>>>))]
        public virtual Decimal? UsrCartonQty { get; set; }

        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Net Weight")]
        [PXFormula(typeof(Mult<SOShipLine.shippedQty, SOShipLineExt.usrBaseItemWeight>))]
        public virtual Decimal? UsrNetWeight { get; set; }

        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Gross Weight")]
        [PXFormula(typeof(Mult<SOShipLine.shippedQty, SOShipLineExt.usrGrsWeight>))]
        public virtual Decimal? UsrGrossWeight { get; set; }

        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Pallet Weight")]
        public virtual Decimal? UsrPalletWeight { get; set; }

        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "MEA S")]
        [PXFormula(typeof(Mult<SOShipLineExt.usrCartonQty, SOShipLineExt.usrBaseItemVolume>))]
        public virtual Decimal? UsrMEAS { get; set; }

        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Dimensional Weight")]
        public virtual Decimal? UsrDimWeight { get; set; }

        [PXString]
        [PXDBScalar(typeof(Search2<CSAnswers.value, InnerJoin<PX.Objects.IN.InventoryItem, On<PX.Objects.IN.InventoryItem.noteID, Equal<CSAnswers.refNoteID>, And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.QtyCartonAttr>>>>, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<PX.Objects.IN.InventoryItem, On<PX.Objects.IN.InventoryItem.noteID, Equal<CSAnswers.refNoteID>, And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.QtyCartonAttr>>>>, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrQtyCarton { get; set; }

        [PXString]
        [PXDBScalar(typeof(Search2<CSAnswers.value, InnerJoin<PX.Objects.IN.InventoryItem, On<PX.Objects.IN.InventoryItem.noteID, Equal<CSAnswers.refNoteID>, And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.GrsWeightAttr>>>>, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<PX.Objects.IN.InventoryItem, On<PX.Objects.IN.InventoryItem.noteID, Equal<CSAnswers.refNoteID>, And<CSAnswers.attributeID, Equal<SOShipmentEntry_Extension.GrsWeightAttr>>>>, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrGrsWeight { get; set; }

        [PXQuantity(6)]
        [PXDBScalar(typeof(Search<PX.Objects.IN.InventoryItem.baseItemWeight, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
        [PXDefault(typeof(Search<PX.Objects.IN.InventoryItem.baseItemWeight, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrBaseItemWeight { get; set; }

        [PXQuantity(6)]
        [PXDBScalar(typeof(Search<PX.Objects.IN.InventoryItem.baseItemVolume, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<SOShipLine.inventoryID>>>))]
        [PXDefault(typeof(Search<PX.Objects.IN.InventoryItem.baseItemVolume, Where<PX.Objects.IN.InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrBaseItemVolume { get; set; }

        public abstract class usrCartonSize : BqlType<IBqlString, string>.Field<SOShipLineExt.usrCartonSize>
        {
        }

        public abstract class usrCartonQty : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrCartonQty>
        {
        }

        public abstract class usrNetWeight : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrNetWeight>
        {
        }

        public abstract class usrGrossWeight : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrGrossWeight>
        {
        }

        public abstract class usrPalletWeight : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrPalletWeight>
        {
        }

        public abstract class usrMEAS : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrMEAS>
        {
        }

        public abstract class usrDimWeight : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrDimWeight>
        {
        }

        public abstract class usrQtyCarton : BqlType<IBqlString, string>.Field<SOShipLineExt.usrQtyCarton>
        {
        }

        public abstract class usrGrsWeight : BqlType<IBqlString, string>.Field<SOShipLineExt.usrGrsWeight>
        {
        }

        public abstract class usrBaseItemWeight : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrBaseItemWeight>
        {
        }

        public abstract class usrBaseItemVolume : BqlType<IBqlDecimal, Decimal>.Field<SOShipLineExt.usrBaseItemVolume>
        {
        }
    }
}
