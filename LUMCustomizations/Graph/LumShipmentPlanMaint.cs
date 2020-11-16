// Decompiled with JetBrains decompiler
// Type: LumCustomizations.Graph.LumShipmentPlanMaint
// Assembly: LumCustomizations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CAF5AA38-83F0-40FF-909D-BF37C3B18F4B
// Assembly location: C:\Program Files\Acumatica ERP\LifeSync\Bin\LumCustomizations.dll

using JAMS.AM;
using LumCustomizations.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LumCustomizations.Graph
{
    public class LumShipmentPlanMaint : PXGraph<LumShipmentPlanMaint, LumShipmentPlan>
    {

        public LumShipmentPlanMaint()
        {
            Report.AddMenuAction(InnerLabelGenaral);
            Report.AddMenuAction(OuterLabelGenaral);
            Report.AddMenuAction(InnerLabelOsi);
            Report.AddMenuAction(OuterLabelOsi);
            Report.AddMenuAction(OuterLabelAmc);
            Report.AddMenuAction(OuterLabelSjm);
            Report.AddMenuAction(InnerLabelMasimo);
            Report.AddMenuAction(OuterLabelMasimo);
            Report.MenuAutoOpen = true;
        }

        public const string NotDeleteConfirmed = "The Shipment Plan [{0}] Had Confirmed And Can't Be Deleted.";
        [PXFilterable(new System.Type[] { })]
        public FbqlSelect<SelectFromBase<LumShipmentPlan, TypeArrayOf<IFbqlJoin>.Empty>, LumShipmentPlan>.View ShipPlan;
        public FbqlSelect<SelectFromBase<SOOrder, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrder.orderType, Equal<LumShipmentPlan.orderType>>>>>.And<BqlOperand<SOOrder.orderNbr, IBqlString>.IsEqual<LumShipmentPlan.orderNbr>>>, SOOrder>.View Order;

        protected void _(PX.Data.Events.RowDeleting<LumShipmentPlan> e)
        {
            int num;
            if (e.Row != null)
            {
                bool? confirmed = e.Row.Confirmed;
                bool flag = true;
                num = confirmed.GetValueOrDefault() == flag & confirmed.HasValue ? 1 : 0;
            }
            else
                num = 0;
            if (num != 0)
                throw new PXSetPropertyException<LumShipmentPlan.confirmed>("The Shipment Plan [{0}] Had Confirmed And Can't Be Deleted.", new object[1]
                {
          (object) e.Row.ShipmentPlanID
                });
        }

        protected void _(PX.Data.Events.RowPersisting<LumShipmentPlan> e) => AutoNumberAttribute.SetNumberingId<LumShipmentPlan.shipmentPlanID>(e.Cache, "SHIPPLAN");

        protected void _(
          PX.Data.Events.FieldUpdated<LumShipmentPlan.sOLineNoteID> e)
        {
            if (!(e.Row is LumShipmentPlan row))
                return;
            SOLine soLine = (SOLine)PXSelectBase<SOLine, PXViewOf<SOLine>.BasedOn<SelectFromBase<SOLine, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<SOLine.noteID, IBqlGuid>.IsEqual<P.AsGuid>>>.Config>.Select((PXGraph)this, (object)row.SOLineNoteID);
            SOOrder soOrder = (SOOrder)PXSelectBase<SOOrder, PXViewOf<SOOrder>.BasedOn<SelectFromBase<SOOrder, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrder.orderType, Equal<P.AsString>>>>>.And<BqlOperand<SOOrder.orderNbr, IBqlString>.IsEqual<P.AsString>>>>.Config>.SelectSingleBound((PXGraph)this, (object[])null, (object)soLine.OrderType, (object)soLine.OrderNbr);
            PXFieldState valueExt = this.Order.Cache.GetValueExt((object)soOrder, "AttributeENDC") as PXFieldState;
            row.Customer = (string)valueExt.Value;
            row.OrderNbr = soOrder.OrderNbr;
            row.OrderType = soOrder.OrderType;
            row.CustomerLocationID = soOrder.CustomerLocationID;
            row.CustomerOrderNbr = soOrder.CustomerOrderNbr;
            row.OrderDate = soOrder.OrderDate;
            row.LineNbr = soLine.LineNbr;
            row.InventoryID = soLine.InventoryID;
            row.OpenQty = soLine.OpenQty;
            row.OrderQty = soLine.OrderQty;
            row.RequestDate = soLine.RequestDate;
        }

        protected void _(PX.Data.Events.FieldUpdated<LumShipmentPlan.prodOrdID> e)
        {
            if (!(e.Row is LumShipmentPlan row))
                return;
            AMProdItem amProdItem = (AMProdItem)PXSelectBase<AMProdItem, PXViewOf<AMProdItem>.BasedOn<SelectFromBase<AMProdItem, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<AMProdItem.prodOrdID, IBqlString>.IsEqual<P.AsString>>>.Config>.Select((PXGraph)this, (object)row.ProdOrdID);
            row.QtyToProd = amProdItem.QtytoProd;
            row.QtyComplete = amProdItem.QtyComplete;
            foreach (PXResult<AMProdAttribute> pxResult in PXSelectBase<AMProdAttribute, PXViewOf<AMProdAttribute>.BasedOn<SelectFromBase<AMProdAttribute, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<AMProdAttribute.prodOrdID, IBqlString>.IsEqual<P.AsString>>>.Config>.Select((PXGraph)this, (object)amProdItem.ProdOrdID))
            {
                AMProdAttribute amProdAttribute = (AMProdAttribute)pxResult;
                switch (amProdAttribute.AttributeID)
                {
                    case "PRODLINE":
                        row.ProdLine = amProdAttribute.Value;
                        break;
                    case "LOTNO":
                        row.LotSerialNbr = amProdAttribute.Value;
                        break;
                    case "BR":
                        row.BRNbr = amProdAttribute.Value;
                        break;
                }
            }
        }

        public PXAction<LumShipmentPlan> Report;
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected void report() { }

        public PXAction<LumShipmentPlan> InnerLabelGenaral;
        [PXButton]
        [PXUIField(DisplayName = "Print Inner Label-General", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable innerLabelGenaral(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601000";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        public PXAction<LumShipmentPlan> OuterLabelGenaral;
        [PXButton]
        [PXUIField(DisplayName = "Print Outer Label-General", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable outerLabelGenaral(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601001";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList(); 
        }

        public PXAction<LumShipmentPlan> InnerLabelOsi;
        [PXButton]
        [PXUIField(DisplayName = "Print Inner Label-OSI", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable innerLabelOsi(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601002";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        public PXAction<LumShipmentPlan> OuterLabelOsi;
        [PXButton]
        [PXUIField(DisplayName = "Print Outer Label-OSI", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable outerLabelOsi(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601003";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        public PXAction<LumShipmentPlan> OuterLabelAmc;
        [PXButton]
        [PXUIField(DisplayName = "Print Outer Label-AMC", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable outerLabelAmc(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601004";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        public PXAction<LumShipmentPlan> OuterLabelSjm;
        [PXButton]
        [PXUIField(DisplayName = "Print Outer Label-SJM", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable outerLabelSjm(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601005";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        public PXAction<LumShipmentPlan> InnerLabelMasimo;
        [PXButton]
        [PXUIField(DisplayName = "Print Inner Label-MASIMO", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable innerLabelMasimo(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601006";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        public PXAction<LumShipmentPlan> OuterLabelMasimo;
        [PXButton]
        [PXUIField(DisplayName = "Print Outer Label-MASIMO", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable outerLabelMasimo(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601007";
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        /// <summary> Get Current Value to Report Parameter </summary>
        public Dictionary<string, string> GetCurrentRowToParameter()
        {
            var _CurrentRow = this.GetCacheCurrent<LumShipmentPlan>().Current;
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["ShipmentPlanID"] = _CurrentRow.ShipmentPlanID
            };
            return parameters;
        }

    }
}
