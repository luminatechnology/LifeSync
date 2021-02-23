using JAMS.AM;
using LUMCustomizations.Library;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public class ProdMaint_Extension : PXGraphExtension<ProdMaint>
    {
        public SelectFrom<SOOrder>
            .Where<SOOrder.orderNbr.IsEqual<AMProdItemExt.usrSOOrderNbr.AsOptional>
                .And<SOOrder.orderType.IsEqual<AMProdItemExt.usrSOOrderType.AsOptional>>>
            .View soOrderData;

        public SelectFrom<AMBomCost>
                .Where<AMBomCost.bOMID.IsEqual<AMProdItem.bOMID.FromCurrent>
                    .And<AMBomCost.revisionID.IsEqual<AMProdItem.bOMRevisionID.FromCurrent>>
                    .And<AMBomCost.userID.IsEqual<AccessInfo.userID.FromCurrent>>>.View amBomCost;

        public override void Initialize()
        {
            base.Initialize();
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                Base.report.AddMenuAction(ProductionInstruction);
                Base.report.AddMenuAction(InnerLabel);
            }
        }

        [PXUIField(DisplayName = "CalculatePlanCost", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update, Visible = false)]
        [PXProcessButton]
        public IEnumerable CalculatePlanCost(PXAdapter adapter)
        {
            Base.CalculatePlanCost(adapter);
            var BomCostData = amBomCost.Select(this);
            AMBomCost model = new AMBomCost();
            if (amBomCost.Select(this).Count == 0)
            {
                model.BOMID = Base.ProdItemSelected.Current.BOMID;
                model.InventoryID = Base.ProdMaintRecords.Current.InventoryID;
                model.RevisionID = Base.ProdItemSelected.Current.BOMRevisionID;
                model.UserID = Base.Accessinfo.UserID;
                model.SiteID = Base.ProdItemSelected.Current.SiteID;
                model.MachCost = Base.ProdTotalRecs.Current.PlanMachine;
                model.ToolCost = Base.ProdTotalRecs.Current.PlanTool;
                model.FOvdCost = Base.ProdTotalRecs.Current.PlanFixedOverhead;
                model.VOvdCost = Base.ProdTotalRecs.Current.PlanVariableOverhead;
                model.SubcontractMaterialCost = Base.ProdTotalRecs.Current.PlanSubcontract;
                model.TotalCost = Base.ProdTotalRecs.Current.PlanTotal;
                model.UnitCost = Base.ProdTotalRecs.Current.PlanUnitCost;
                this.amBomCost.Cache.Insert(model);
            }
            else
            {
                model = BomCostData.TopFirst;
                model.SiteID = Base.ProdItemSelected.Current.SiteID;
                model.MachCost = Base.ProdTotalRecs.Current.PlanMachine;
                model.ToolCost = Base.ProdTotalRecs.Current.PlanTool;
                model.FOvdCost = Base.ProdTotalRecs.Current.PlanFixedOverhead;
                model.VOvdCost = Base.ProdTotalRecs.Current.PlanVariableOverhead;
                model.SubcontractMaterialCost = Base.ProdTotalRecs.Current.PlanSubcontract;
                model.TotalCost = Base.ProdTotalRecs.Current.PlanTotal;
                model.UnitCost = Base.ProdTotalRecs.Current.PlanUnitCost;
                this.amBomCost.Cache.Update(model);
            }
            Base.Persist();
            return adapter.Get();
        }

        #region Action
        public PXAction<AMProdItem> ProductionInstruction;
        [PXButton]
        [PXUIField(DisplayName = "生产指令单", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable productionInstruction(PXAdapter adapter)
        {
            var _reportID = "lm625000";

            //Get Print Count and Update
            var row = (AMProdItem)Base.GetCacheCurrent<AMProdItem>().Current;
            var _printCount = row.GetExtension<AMProdItemExt>().UsrPrintCount;
            PXUpdate<Set<AMProdItemExt.usrPrintCount, Required<AMProdItemExt.usrPrintCount>>,
                         AMProdItem,
                         Where<AMProdItem.prodOrdID, Equal<Required<AMProdItem.prodOrdID>>,
                           And<AMProdItem.orderType,Equal<Required<AMProdItem.orderType>>>
                     >>.Update(Base, _printCount == null ? 1 : _printCount.Value + 1, row.ProdOrdID,row.OrderType);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Production_Nbr"] = Base.ProdMaintRecords.Current.ProdOrdID;
            parameters["OrderType"] = Base.ProdMaintRecords.Current.OrderType;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }

        public PXAction<AMProdItem> InnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Inner label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable innerLabel(PXAdapter adapter)
        {
            var _reportID = "lm601002";
            var _CurrentRow = Base.GetCacheCurrent<AMProdItem>().Current;

            PXResultset<InventoryItem> data =
                SelectFrom<InventoryItem>
                .LeftJoin<INItemXRef>.On<INItemXRef.inventoryID.IsEqual<InventoryItem.inventoryID>>
                .LeftJoin<CSAnswers>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>>
                .Where<InventoryItem.inventoryID.IsEqual<P.AsInt>>.View.Select(Base, _CurrentRow.InventoryID);

            SOOrder soData = soOrderData.Select(
                _CurrentRow.GetExtension<AMProdItemExt>().UsrSOOrderNbr,
                _CurrentRow.GetExtension<AMProdItemExt>().UsrSOOrderType);

            SOLine soLineData =
                SelectFrom<SOLine>
                .Where<SOLine.orderNbr.IsEqual<P.AsString>
                    .And<SOLine.orderType.IsEqual<P.AsString>>
                    .And<SOLine.lineNbr.IsEqual<P.AsInt>>>
                .View.Select(Base,
                            _CurrentRow.GetExtension<AMProdItemExt>().UsrSOOrderNbr,
                            _CurrentRow.GetExtension<AMProdItemExt>().UsrSOOrderType,
                            _CurrentRow.GetExtension<AMProdItemExt>().UsrSOLineNbr);

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["OrderType"] = _CurrentRow.OrderType,
                ["ProdOrdID"] = _CurrentRow.ProdOrdID,
                ["Customer"] = (soOrderData.Cache.GetValueExt(soData, PX.Objects.CS.Messages.Attribute + "ENDC") as PXFieldState).Value.ToString(),
                ["CustomerPartNo"] = soLineData?.AlternateID,
                ["Description"] = data.FirstOrDefault().GetItem<InventoryItem>().Descr,
                ["Resistor"] = data.RowCast<CSAnswers>().Where(x => x.AttributeID == "RESISTOR").FirstOrDefault()?.Value,
                ["DATE"] = null
            };
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion

        #region controll customize button based on country ID
        protected void _(Events.RowSelected<AMProdItem> e)
        {
            var _lumLibrary = new LumLibrary();
            if (!_lumLibrary.isCNorHK())
            {
                ProductionInstruction.SetVisible(false);
                InnerLabel.SetVisible(false);
            }
        }
        #endregion
    }
}
