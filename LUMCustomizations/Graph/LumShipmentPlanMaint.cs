using JAMS.AM;
using LumCustomizations.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LumCustomizations.Graph
{
    public class LumShipmentPlanMaint : PXGraph<LumShipmentPlanMaint, LumShipmentPlan>
    {
        public const string NotDeleteConfirmed = "The Shipment Plan [{0}] Had Confirmed And Can't Be Deleted.";
        public const string QtyCannotExceeded  = "The {0} Cannot Exceed The {1}.";

        #region Ctor
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
        #endregion

        #region Selects
        [PXFilterable()]
        public SelectFrom<LumShipmentPlan>.View ShipPlan;
        public SelectFrom<SOOrder>.Where<SOOrder.orderType.IsEqual<LumShipmentPlan.orderType>.And<SOOrder.orderNbr.IsEqual<LumShipmentPlan.orderNbr>>>.View Order;
        #endregion

        #region Actions
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
        #endregion

        #region Event Handlers
        protected void _(Events.RowDeleting<LumShipmentPlan> e)
        {
            if (e.Row.Confirmed == true)
            {
                throw new PXSetPropertyException<LumShipmentPlan.confirmed>(NotDeleteConfirmed, e.Row.ShipmentPlanID);
            }
        }

        //protected void _(Events.RowPersisting<LumShipmentPlan> e)
        //{
        //    AutoNumberAttribute.SetNumberingId<LumShipmentPlan.shipmentPlanID>(e.Cache, "SHIPPLAN");
        //}

        protected void _(Events.FieldVerifying<LumShipmentPlan.plannedShipQty> e)
        {
            var row = e.Row as LumShipmentPlan;

            if (row != null && (decimal)e.NewValue > row.QtyToProd)
            {
                throw new PXSetPropertyException<LumShipmentPlan.plannedShipQty>(QtyCannotExceeded, nameof(LumShipmentPlan.plannedShipQty), nameof(LumShipmentPlan.qtyToProd));
            }
        }

        protected void _(Events.FieldUpdated<LumShipmentPlan.prodOrdID> e)
        {
            var row = e.Row as LumShipmentPlan;

            if (row == null) { return; }

            AMProdItem prodItem = SelectFrom<AMProdItem>.Where<AMProdItem.prodOrdID.IsEqual<@P.AsString>>.View.Select(this, row.ProdOrdID);
            
            row.QtyToProd   = prodItem.QtytoProd;
            row.QtyComplete = prodItem.QtyComplete;

            foreach (AMProdAttribute prodAttr in SelectFrom<AMProdAttribute>.Where<AMProdAttribute.prodOrdID.IsEqual<@P.AsString>>.View.Select(this, prodItem.ProdOrdID))
            {
                switch (prodAttr.AttributeID)
                {
                    case "PRODLINE":
                        row.ProdLine = SelectFrom<CSAttributeDetail>.Where<CSAttributeDetail.attributeID.IsEqual<@P.AsString>
                                                                           .And<CSAttributeDetail.valueID.IsEqual<@P.AsString>>>.View
                                                                    .SelectSingleBound(this, null, prodAttr.AttributeID, prodAttr.Value).TopFirst?.Description;
                        break;
                    case "LOTNO":
                        row.LotSerialNbr = prodAttr.Value;
                        break;
                    case "TOTSHIPWO":
                        row.TotalShipNbr = Convert.ToInt32(prodAttr.Value);
                        break;
                }
            }

            AMProdItemExt prodItemExt = prodItem.GetExtension<AMProdItemExt>();

            if (prodItemExt.UsrSOOrderNbr != null)
            {
                PXResult<SOOrder, SOLine> sOResult = (PXResult<SOOrder, SOLine>)SelectFrom<SOOrder>.InnerJoin<SOLine>.On<SOOrder.orderType.IsEqual<SOLine.orderType>
                                                                                                                         .And<SOOrder.orderNbr.IsEqual<SOLine.orderNbr>>>
                                                                                                   .Where<SOLine.orderType.IsEqual<@P.AsString>
                                                                                                          .And<SOLine.orderNbr.IsEqual<@P.AsString>
                                                                                                               .And<SOLine.lineNbr.IsEqual<@P.AsInt>>>>.View
                                                                                                   .Select(this, prodItemExt.UsrSOOrderType, prodItemExt.UsrSOOrderNbr, prodItemExt.UsrSOLineNbr);
                SOLine  soLine  = sOResult;
                SOOrder soOrder = sOResult;

                PXFieldState valueExt = Order.Cache.GetValueExt((object)soOrder, PX.Objects.CS.Messages.Attribute + "ENDC") as PXFieldState;

                row.Customer           = (string)valueExt.Value;
                row.OrderNbr           = soOrder.OrderNbr;
                row.OrderType          = soOrder.OrderType;
                row.CustomerLocationID = soOrder.CustomerLocationID;
                row.CustomerOrderNbr   = soOrder.CustomerOrderNbr;
                row.OrderDate          = soOrder.OrderDate;
                row.LineNbr            = soLine.LineNbr;
                row.InventoryID        = soLine.InventoryID;
                row.OpenQty            = soLine.OpenQty;
                row.OrderQty           = soLine.OrderQty;
                row.RequestDate        = soLine.RequestDate;
            }

            LumShipmentPlan aggrShipPlan = SelectFrom<LumShipmentPlan>.Where<LumShipmentPlan.prodOrdID.IsEqual<@P.AsString>>
                                                                      .AggregateTo<Max<LumShipmentPlan.nbrOfShipment,
                                                                                       Max<LumShipmentPlan.endCartonNbr>>>.View.Select(this, row.ProdOrdID);

            row.NbrOfShipment  = aggrShipPlan.NbrOfShipment == null ? 1 : aggrShipPlan.NbrOfShipment + 1;
            row.StartCartonNbr = (aggrShipPlan.EndCartonNbr ?? 0) + 1;//aggrShipPlan.EndCartonNbr  == null ? 1 : aggrShipPlan.EndCartonNbr  + 1;

            aggrShipPlan = SelectFrom<LumShipmentPlan>.Where<LumShipmentPlan.shipmentPlanID.IsEqual<@P.AsString>>
                                                             .AggregateTo<Max<LumShipmentPlan.endLabelNbr>>.View.Select(this, row.ShipmentPlanID);

            row.StartLabelNbr = aggrShipPlan.EndLabelNbr == null ? 1 : aggrShipPlan.EndLabelNbr + 1;
        }

        protected void _(Events.FieldUpdated<LumShipmentPlan.plannedShipQty> e)
        {
            var row = e.Row as LumShipmentPlan;

            if (row != null)
            {
                CSAnswers answers = CSAnswers.PK.Find(this, InventoryItem.PK.Find(this, row.InventoryID).NoteID, "QTYCARTON");

                row.EndCartonNbr = (int)(row.StartCartonNbr + (row.PlannedShipQty / Convert.ToDecimal(answers.Value)) ) + 1;
                row.EndLabelNbr  = (int)(row.StartLabelNbr  + (row.PlannedShipQty / Convert.ToDecimal(answers.Value)) ) + 1;
            }
        }
        #endregion
    }
}
