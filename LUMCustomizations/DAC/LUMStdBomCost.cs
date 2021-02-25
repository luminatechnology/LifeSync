using System;
using JAMS.AM.Attributes;
using PX.Data;
using PX.Objects.IN;

namespace LUMCustomizations
{
    [Serializable]
    [PXCacheName("LUM_STD_BOM Costs")]
    public class LUMStdBomCost : IBqlTable
    {
        #region Selected
        /// <summary>
        /// Indicates whether the record is selected for processing.
        /// </summary>
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region IdentityID
        [PXDBIdentity(IsKey = true)]
        public virtual int? IdentityID { get; set; }
        public abstract class identityID : PX.Data.BQL.BqlInt.Field<identityID> { }
        #endregion

        #region BOMID
        [BomID()]
        public virtual string BOMID { get; set; }
        public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }
        #endregion

        #region RevisionID
        [RevisionIDField()]
        public virtual string RevisionID { get; set; }
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
        #endregion

        #region LineID
        [PXDBInt()]
        public virtual int? LineID { get; set; }
        public abstract class lineID : PX.Data.BQL.BqlInt.Field<lineID> { }
        #endregion

        #region Level
        [PXDBInt()]
        public virtual int? Level { get; set; }
        public abstract class level : PX.Data.BQL.BqlInt.Field<level> { }
        #endregion

        #region InventoryID
        [Inventory()]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region SiteID
        [Site()]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
        #endregion

        #region SubItemID
        [SubItem()]
        public virtual int? SubItemID { get; set; }
        public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
        #endregion

        #region UOM
        [INUnit]
        public virtual String UOM { get; set; }
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
        #endregion

        #region ScrapFactor
        [PXDBDecimal(6)]
        public virtual Decimal? ScrapFactor { get; set; }
        public abstract class scrapFactor : PX.Data.BQL.BqlDecimal.Field<scrapFactor> { }
        #endregion

        #region LineBOMID
        [BomID()]
        public virtual String LineBOMID { get; set; }
        public abstract class lineBOMID : PX.Data.BQL.BqlString.Field<lineBOMID> { }
        #endregion

        #region LineRevisionID
        [RevisionIDField()]
        public virtual string LineRevisionID { get; set; }
        public abstract class lineRevisionID : PX.Data.BQL.BqlString.Field<lineRevisionID> { }
        #endregion

        #region OperationID
        [OperationIDField]
        public virtual int? OperationID { get; set; }
        public abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }
        #endregion

        #region OperationCD
        [OperationCDField()]
        public virtual string OperationCD { get; set; }
        public abstract class operationCD : PX.Data.BQL.BqlString.Field<operationCD> { }
        #endregion

        #region ManufBOMID
        [BomID]
        public virtual string ManufBOMID { get; set; }
        public abstract class manufBOMID : PX.Data.BQL.BqlString.Field<manufBOMID> { }
        #endregion

        #region ManufRevisionID
        [RevisionIDField]
        public virtual string ManufRevisionID { get; set; }
        public abstract class manufRevisionID : PX.Data.BQL.BqlString.Field<manufRevisionID> { }
        #endregion

        #region WcID
        [WorkCenterIDField]
        public virtual string WcID { get; set; }
        public abstract class wcID : PX.Data.BQL.BqlString.Field<wcID> { }
        #endregion

        #region SortOrder
        [PXDBInt()]
        [PXUIField(Enabled = false)]
        public virtual int? SortOrder { get; set; }
        public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }
        #endregion

        #region IsHeaderRecord
        [PXDBBool]
        [PXUIField(DisplayName = "Header Record")]
        public virtual bool? IsHeaderRecord { get; set; }
        public abstract class isHeaderRecord : PX.Data.BQL.BqlBool.Field<isHeaderRecord> { }
        #endregion

        #region EffEndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Eff End Date")]
        public virtual DateTime? EffEndDate { get; set; }
        public abstract class effEndDate : PX.Data.BQL.BqlDateTime.Field<effEndDate> { }
        #endregion

        #region EffStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Eff Start Date")]
        public virtual DateTime? EffStartDate { get; set; }
        public abstract class effStartDate : PX.Data.BQL.BqlDateTime.Field<effStartDate> { }
        #endregion

        #region CompInventoryID
        [Inventory(DisplayName = "Comp. Inventory")]
        public virtual int? CompInventoryID { get; set; }
        public abstract class compInventoryID : PX.Data.BQL.BqlInt.Field<compInventoryID> { }
        #endregion

        #region CompSubItemID
        [SubItem(DisplayName = "Comp. SubItem")]
        public virtual int? CompSubItemID { get; set; }
        public abstract class compSubItemID : PX.Data.BQL.BqlInt.Field<compSubItemID> { }
        #endregion

        #region CompUnitCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Unit Cost")]
        public virtual Decimal? CompUnitCost { get; set; }
        public abstract class compUnitCost : PX.Data.BQL.BqlDecimal.Field<compUnitCost> { }
        #endregion

        #region CompQtyReq
        [PXDBQuantity()]
        [PXUIField(DisplayName = "Comp. Qty Request")]
        public virtual Decimal? CompQtyReq { get; set; }
        public abstract class compQtyReq : PX.Data.BQL.BqlDecimal.Field<compQtyReq> { }
        #endregion

        #region CompMatlCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Material")]
        public virtual Decimal? CompMatlCost { get; set; }
        public abstract class compMatlCost : PX.Data.BQL.BqlDecimal.Field<compMatlCost> { }
        #endregion

        #region CompManufMatlCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Manuf. Material")]
        public virtual Decimal? CompManufMatlCost { get; set; }
        public abstract class compManufMatlCost : PX.Data.BQL.BqlDecimal.Field<compManufMatlCost> { }
        #endregion

        #region CompPurchMatl
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Purch Material")]
        public virtual Decimal? CompPurchMatl { get; set; }
        public abstract class compPurchMatl : PX.Data.BQL.BqlDecimal.Field<compPurchMatl> { }
        #endregion

        #region CompVarOvdCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Var Overhead")]
        public virtual Decimal? CompVarOvdCost { get; set; }
        public abstract class compVarOvdCost : PX.Data.BQL.BqlDecimal.Field<compVarOvdCost> { }
        #endregion

        #region CompFixedOvdCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Fixed Overhead")]
        public virtual Decimal? CompFixedOvdCost { get; set; }
        public abstract class compFixedOvdCost : PX.Data.BQL.BqlDecimal.Field<compFixedOvdCost> { }
        #endregion

        #region CompToolCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Tool Cost")]
        public virtual Decimal? CompToolCost { get; set; }
        public abstract class compToolCost : PX.Data.BQL.BqlDecimal.Field<compToolCost> { }
        #endregion

        #region CompMachineCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Machine Cost")]
        public virtual Decimal? CompMachineCost { get; set; }
        public abstract class compMachineCost : PX.Data.BQL.BqlDecimal.Field<compMachineCost> { }
        #endregion

        #region CompExtCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Comp. Ext. Cost")]
        public virtual Decimal? CompExtCost { get; set; }
        public abstract class compExtCost : PX.Data.BQL.BqlDecimal.Field<compExtCost> { }
        #endregion

        #region CompTotalExtCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Total Ext. Cost")]
        public virtual Decimal? CompTotalExtCost { get; set; }
        public abstract class compTotalExtCost : PX.Data.BQL.BqlDecimal.Field<compTotalExtCost> { }
        #endregion

        #region TotalQtyReq
        [PXDBQuantity]
        public virtual Decimal? TotalQtyReq { get; set; }
        public abstract class totalQtyReq : PX.Data.BQL.BqlDecimal.Field<totalQtyReq> { }
        #endregion

        #region MatlCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Material Cost")]
        public virtual Decimal? MatlCost { get; set; }
        public abstract class matlCost : PX.Data.BQL.BqlDecimal.Field<matlCost> { }
        #endregion

        #region LotSize
        [PXDBQuantity()]
        [PXUIField(DisplayName = "Lot Size")]
        public virtual Decimal? LotSize { get; set; }
        public abstract class lotSize : PX.Data.BQL.BqlDecimal.Field<lotSize> { }
        #endregion

        #region VariableLaborCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Variable Labor")]
        public virtual Decimal? VariableLaborCost { get; set; }
        public abstract class variableLaborCost : PX.Data.BQL.BqlDecimal.Field<variableLaborCost> { }
        #endregion

        #region FixedLaborCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Fixed Labor")]
        public virtual Decimal? FixedLaborCost { get; set; }
        public abstract class fixedLaborCost : PX.Data.BQL.BqlDecimal.Field<fixedLaborCost> { }
        #endregion

        #region VariableOvdCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Variable Overhead")]
        public virtual Decimal? VariableOvdCost { get; set; }
        public abstract class variableOvdCost : PX.Data.BQL.BqlDecimal.Field<variableOvdCost> { }
        #endregion

        #region FixedOvdCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Fixed Overhead")]
        public virtual Decimal? FixedOvdCost { get; set; }
        public abstract class fixedOvdCost : PX.Data.BQL.BqlDecimal.Field<fixedOvdCost> { }
        #endregion

        #region ToolCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Tool Cost")]
        public virtual Decimal? ToolCost { get; set; }
        public abstract class toolCost : PX.Data.BQL.BqlDecimal.Field<toolCost> { }
        #endregion

        #region MachineCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Machine Cost")]
        public virtual Decimal? MachineCost { get; set; }
        public abstract class machineCost : PX.Data.BQL.BqlDecimal.Field<machineCost> { }
        #endregion

        #region SubcontractCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Subcontract Cost")]
        public virtual Decimal? SubcontractCost { get; set; }
        public abstract class subcontractCost : PX.Data.BQL.BqlDecimal.Field<subcontractCost> { }
        #endregion

        #region UnitCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Unit Cost")]
        public virtual Decimal? UnitCost { get; set; }
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        #endregion

        #region TotalCost
        [PXDBPriceCost(true)]
        [PXUIField(DisplayName = "Total Cost")]
        public virtual Decimal? TotalCost { get; set; }
        public abstract class totalCost : PX.Data.BQL.BqlDecimal.Field<totalCost> { }
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
        [PXUIField(DisplayName = "Created Date")]
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
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region HasCostRoll
        public abstract class hasCostRoll : PX.Data.BQL.BqlBool.Field<hasCostRoll> { }

        protected bool? _HasCostRoll;
        [PXBool]
        [PXUIField(DisplayName = "Has Cost Roll", Visible = false)]
        public virtual bool? HasCostRoll
        {
            get { return this._HasCostRoll; }
            set { this._HasCostRoll = value; }
        }

        #endregion
    }
}