using JAMS.AM;
using JAMS.AM.Attributes;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using PX.Objects.CM;

namespace LumCustomizations.DAC
{
    [PXCacheName("Lum_Shipment Plan")]
    [Serializable]
    public class LumShipmentPlan : IBqlTable
    {
        //#region SOLineNoteID
        //[PXGuid]
        //[PXUIField(DisplayName = "SO Ref.", Visible = false)]
        //[PXSelector(typeof(Search2<SOLine.noteID, InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOLine.orderType>, 
        //                                                                And<SOOrder.orderNbr, Equal<SOLine.orderNbr>>>, 
        //                                          InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SOLine.inventoryID>>>>>), 
        //            typeof(SOOrder.orderType), 
        //            typeof(SOOrder.orderNbr), 
        //            typeof(SOLine.lineNbr), 
        //            typeof(SOLine.inventoryID), 
        //            typeof(InventoryItem.descr), 
        //            typeof(SOLine.orderQty), 
        //            typeof(SOLine.requestDate))]
        //public virtual Guid? SOLineNoteID { get; set; }
        //public abstract class sOLineNoteID : PX.Data.BQL.BqlGuid.Field<LumShipmentPlan.sOLineNoteID> { }
        //#endregion

        #region ShipmentPlanID
        [PXDBString(15, InputMask = "", IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Shipment Plan")]
        //[AutoNumber(typeof(SOSetup.shipmentNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string ShipmentPlanID { get; set; }
        public abstract class shipmentPlanID : PX.Data.BQL.BqlString.Field<LumShipmentPlan.shipmentPlanID> { }
        #endregion

        #region ProdOrdID
        [ProductionNbr(IsKey = true)]
        [PXSelector(typeof(Search<AMProdItem.prodOrdID, Where<AMProdItemExt.usrSOOrderNbr, IsNotNull>>))]
        public virtual string ProdOrdID { get; set; }
        public abstract class prodOrdID : PX.Data.BQL.BqlString.Field<LumShipmentPlan.prodOrdID> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, InputMask = "", IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "SO Order Nbr.", Enabled = false)]
        [PXSelector(typeof(Search<SOOrder.orderNbr>), CacheGlobal = true)]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<LumShipmentPlan.orderNbr> { }
        #endregion

        #region LineNbr      
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.", Enabled = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.lineNbr> { }
        #endregion

        #region Confirmed
        [PXDBBool]
        [PXUIField(DisplayName = "Confirmed")]
        [PXDefault(false)]
        public virtual bool? Confirmed { get; set; }
        public abstract class confirmed : PX.Data.BQL.BqlBool.Field<LumShipmentPlan.confirmed> { }
        #endregion

        #region OrderType
        [PXDBString(2, InputMask = "", IsFixed = true)]
        [PXUIField(DisplayName = "SO Order Type", Enabled = false)]
        [PXSelector(typeof(Search<SOOrderType.orderType>), CacheGlobal = true)]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<LumShipmentPlan.orderType> { }
        #endregion

        #region Customer
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer", Enabled = false)]
        public virtual string Customer { get; set; }
        public abstract class customer : PX.Data.BQL.BqlString.Field<LumShipmentPlan.customer> { }
        #endregion

        #region CustomerLocationID
        [PXUIField(DisplayName = "Customer Location", Enabled = false)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current<SOShipment.customerID>>, And<Location.isActive, Equal<True>, And<MatchWithBranch<Location.cBranchID>>>>), 
                    DescriptionField = typeof(Location.descr), 
                    Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? CustomerLocationID { get; set; }
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.customerLocationID> { }
        #endregion

        #region CustomerOrderNbr
        [PXDBString(40, InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "Customer Order Nbr.", Enabled = false)]
        public virtual string CustomerOrderNbr { get; set; }
        public abstract class customerOrderNbr : PX.Data.BQL.BqlString.Field<LumShipmentPlan.customerOrderNbr> { }
        #endregion

        #region OrderDate
        [PXDBDate]
        [PXUIField(DisplayName = "Order Date", Enabled = false)]
        public virtual DateTime? OrderDate { get; set; }
        public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<LumShipmentPlan.orderDate> { }
        #endregion

        #region InventoryID
        [Inventory(Enabled = false)]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.inventoryID> { }
        #endregion

        #region OrderQty
        [PXDBQuantity]
        [PXUIField(DisplayName = "Order Qty.", Enabled = false)]
        public virtual Decimal? OrderQty { get; set; }
        public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<LumShipmentPlan.orderQty> { }
        #endregion

        #region OpenQty
        [PXDBQuantity]
        [PXUIField(DisplayName = "Open Qty.", Enabled = false)]
        public virtual Decimal? OpenQty { get; set; }
        public abstract class openQty : PX.Data.BQL.BqlDecimal.Field<LumShipmentPlan.openQty> { }
        #endregion

        #region RequestDate        
        [PXDBDate]
        [PXUIField(DisplayName = "Request Date", Enabled = false)]
        public virtual DateTime? RequestDate { get; set; }
        public abstract class requestDate : PX.Data.BQL.BqlDateTime.Field<LumShipmentPlan.requestDate> { }
        #endregion

        #region PlannedShipDate        
        [PXDBDate]
        [PXUIField(DisplayName = "Planned Shipment Date")]
        public virtual DateTime? PlannedShipDate { get; set; }
        public abstract class plannedShipDate : PX.Data.BQL.BqlDateTime.Field<LumShipmentPlan.plannedShipDate> { }
        #endregion

        #region PlannedShipQty
        [PXDBQuantity(0)]
        [PXUIField(DisplayName = "Planned Shipment Qty.")]
        public virtual Decimal? PlannedShipQty { get; set; }
        public abstract class plannedShipQty : PX.Data.BQL.BqlDecimal.Field<LumShipmentPlan.plannedShipQty> { }
        #endregion

        #region ShipVia
        [PXDBString(60, InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "Ship Via")]
        [PXSelector(typeof(Search<Carrier.carrierID>), CacheGlobal = true, DescriptionField = typeof(Carrier.description))]
        public virtual string ShipVia { get; set; }
        public abstract class shipVia : PX.Data.BQL.BqlString.Field<LumShipmentPlan.shipVia> { }
        #endregion

        #region ShipmentNbr
        [PXDBString(15, InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "Actual Shipment Nbr.")]
        [PXSelector(typeof(Search2<SOShipment.shipmentNbr, InnerJoin<SOShipLine, On<SOShipLine.shipmentType, Equal<SOShipment.shipmentType>, 
                                                                                    And<SOShipLine.shipmentNbr, Equal<SOShipment.shipmentNbr>>>>, 
                                                           Where<SOShipLine.inventoryID, Equal<Current<LumShipmentPlan.inventoryID>>>>))]
        public virtual string ShipmentNbr { get; set; }
        public abstract class shipmentNbr : PX.Data.BQL.BqlString.Field<LumShipmentPlan.shipmentNbr> { }
        #endregion

        #region ProdLine        
        [PXDBString(255, InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "Production Line", Enabled = false)]
        public virtual string ProdLine { get; set; }
        public abstract class prodLine : PX.Data.BQL.BqlString.Field<LumShipmentPlan.prodLine> { }
        #endregion

        #region LotSerialNbr
        [PXDBString(255, InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "Lot #", Enabled = false)]
        public virtual string LotSerialNbr { get; set; }
        public abstract class lotSerialNbr : PX.Data.BQL.BqlString.Field<LumShipmentPlan.lotSerialNbr> { }
        #endregion

        #region BRNbr
        [PXDBString(255, InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "BR Nbr.", Enabled = true)]
        //[PXFormula(typeof(Default<shipmentNbr>))]
        //[PXDefault(typeof(Search<SOShipLineExt.usrBRNbr, Where<SOShipLine.shipmentNbr, Equal<Current<LumShipmentPlan.shipmentNbr>>>>),
        //           PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string BRNbr { get; set; }
        public abstract class bRNbr : PX.Data.BQL.BqlString.Field<LumShipmentPlan.bRNbr> { }
        #endregion

        #region QtyToProd
        [PXDBQuantity(0)]
        [PXUIField(DisplayName = "Qty. to Produce", Enabled = false)]
        public virtual Decimal? QtyToProd { get; set; }
        public abstract class qtyToProd : PX.Data.BQL.BqlDecimal.Field<LumShipmentPlan.qtyToProd> { }
        #endregion

        #region QtyComplete
        [PXDBQuantity(0)]
        [PXUIField(DisplayName = "Qty. Completed", Enabled = false)]
        public virtual Decimal? QtyComplete { get; set; }
        public abstract class qtyComplete : PX.Data.BQL.BqlDecimal.Field<LumShipmentPlan.qtyComplete> { }
        #endregion

        #region NbrOfShipment
        [PXDBInt]
        [PXUIField(DisplayName = "Nbr. Of Shipment")]
        public virtual int? NbrOfShipment { get; set; }
        public abstract class nbrOfShipment : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.nbrOfShipment> { }
        #endregion

        #region TotalShipNbr
        [PXDBInt]
        [PXUIField(DisplayName = "Total Shipment Nbr.")]
        public virtual int? TotalShipNbr { get; set; }
        public abstract class totalShipNbr : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.totalShipNbr> { }
        #endregion

        #region StartLabelNbr
        [PXDBInt]
        [PXUIField(DisplayName = "Start Label Nbr.")]
        public virtual int? StartLabelNbr { get; set; }
        public abstract class startLabelNbr : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.startLabelNbr> { }
        #endregion

        #region EndLabelNbr
        [PXDBInt]
        [PXUIField(DisplayName = "End Label Nbr.")]
        public virtual int? EndLabelNbr { get; set; }
        public abstract class endLabelNbr : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.endLabelNbr> { }
        #endregion

        #region StartCartonNbr
        [PXDBInt]
        [PXUIField(DisplayName = "Start Carton Nbr.")]
        public virtual int? StartCartonNbr { get; set; }
        public abstract class startCartonNbr : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.startCartonNbr> { }
        #endregion

        #region EndCartonNbr
        [PXDBInt]
        [PXUIField(DisplayName = "End Carton Nbr.")]
        public virtual int? EndCartonNbr { get; set; }
        public abstract class endCartonNbr : PX.Data.BQL.BqlInt.Field<LumShipmentPlan.endCartonNbr> { }
        #endregion

        #region Remarks
        [PXDBString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Remarks")]
        public virtual string Remarks { get; set; }
        public abstract class remarks : PX.Data.BQL.BqlString.Field<LumShipmentPlan.remarks> { }
        #endregion

        #region CartonSize
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Carton Size")]
        public virtual string CartonSize { get; set; }
        public abstract class cartonSize : PX.Data.BQL.BqlString.Field<cartonSize> { }
        #endregion

        #region CartonQty
        [PXDBQuantity]
        [PXUIField(DisplayName = "Carton Qty.")]
        public virtual decimal? CartonQty { get; set; }
        public abstract class cartonQty : PX.Data.BQL.BqlDecimal.Field<cartonQty> { }
        #endregion

        #region NetWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Net Weight")]
        public virtual decimal? NetWeight { get; set; }
        public abstract class netWeight : PX.Data.BQL.BqlDecimal.Field<netWeight> { }
        #endregion

        #region GrossWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Gross Weight")]
        public virtual decimal? GrossWeight { get; set; }
        public abstract class  grossWeight : PX.Data.BQL.BqlDecimal.Field<grossWeight> { }
        #endregion

        #region PalletWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Pallet Weight")]
        public virtual decimal? PalletWeight { get; set; }
        public abstract class palletWeight : PX.Data.BQL.BqlDecimal.Field<palletWeight> { }
        #endregion

        #region MEAS
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "MEA S")]
        public virtual decimal? MEAS { get; set; }
        public abstract class mEAS : PX.Data.BQL.BqlDecimal.Field<mEAS> { }
        #endregion

        #region DimWeight
        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Dimensional Weight")]
        public virtual decimal? DimWeight { get; set; }
        public abstract class dimWeight : PX.Data.BQL.BqlDecimal.Field<dimWeight> { }
        #endregion

        #region CustomerPN
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer P/N", Enabled = false)]
        public virtual string CustomerPN { get; set; }
        public abstract class customerPN : PX.Data.BQL.BqlString.Field<customerPN> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region NoteID
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    }
}
