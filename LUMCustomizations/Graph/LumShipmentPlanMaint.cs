using JAMS.AM;
using LumCustomizations.DAC;
using LUMCustomizations.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Reports;
using PX.Objects.CR;
using PX.Objects.CR.DAC;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LumCustomizations.Graph
{
    public class LumShipmentPlanMaint : PXGraph<LumShipmentPlanMaint>
    {
        public const string NotDeleteConfirmed = "The Shipment Plan [{0}] Had Confirmed And Can't Be Deleted.";
        public const string QtyCannotExceeded = "The {0} Cannot Exceed The {1}.";
        public const string ENDC = "ENDC";

        #region Constant Class
        public class ENDCAttr : PX.Data.BQL.BqlString.Constant<ENDCAttr>
        {
            public ENDCAttr() : base("ENDC") { }
        }
        #endregion

        #region Ctor
        public LumShipmentPlanMaint()
        {
            Report.AddMenuAction(OuterLabel);

            // Get Visible
            var _graph = PXGraph.CreateInstance<SOOrderEntry>();
            var _PIPreference = from t in _graph.Select<LifeSyncPreference>()
                                select t;
            var _visible = _PIPreference.FirstOrDefault() == null ? false : _PIPreference.FirstOrDefault().ProformaInvoicePrinting.Value
                                                                  ? true : false;
            // Set Button Visible
            ProformaInvoice.SetVisible(_visible);
            // Add Button
            if (_visible)
                Report.AddMenuAction(ProformaInvoice);

            Report.MenuAutoOpen = true;

            Report.AddMenuAction(printPackingList);
            Report.AddMenuAction(COCReport);
            Report.AddMenuAction(CommericalInvoice);
            Report.AddMenuAction(DGCommericalInvoice);
        }
        #endregion

        #region Selects & Features
        [PXFilterable()]
        public SelectFrom<LumShipmentPlan>.OrderBy<Asc<LumShipmentPlan.shipmentPlanID,
                                                   Asc<LumShipmentPlan.sortOrder>>>.View ShipPlan;
        public SelectFrom<SOOrder>.Where<SOOrder.orderType.IsEqual<LumShipmentPlan.orderType>.And<SOOrder.orderNbr.IsEqual<LumShipmentPlan.orderNbr>>>.View Order;

        public PXSave<LumShipmentPlan> Save;
        public PXCancel<LumShipmentPlan> Cancel;
        #endregion

        #region Actions
        public PXAction<LumShipmentPlan> Report;
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected void report() { }

        public PXAction<LumShipmentPlan> OuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Outer Label", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable outerLabel(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601001";
            //ActiveStandardReport(_reportID);
            var parameters = GetCurrentRowToParameter();
            if (parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<LumShipmentPlan>().ToList();
        }

        public PXAction<SOShipment> ProformaInvoice;
        [PXButton]
        [PXUIField(DisplayName = "Print Proforma Invoice Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable proformaInvoice(PXAdapter adapter)
        {
            var _reportID = "lm611001";
            if (string.IsNullOrEmpty(this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr))
                throw new PXException("ShipmentNbr Can Not be null");
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr,
                ["ShipmentPlanID"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentPlanID,
                ["ProdOrdID"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ProdOrdID
            };
            if (parameters["ShipmentNbr"] != null && parameters["ShipmentPlanID"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }

        public PXAction<LumShipmentPlan> printPackingList;
        [PXButton()]
        [PXUIField(DisplayName = "Print Packing List", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrintPackingList(PXAdapter adapter)
        {
            var _CurrentRow = this.GetCacheCurrent<LumShipmentPlan>().Current;

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["ShipmentPlanID"] = _CurrentRow.ShipmentPlanID,
                ["ShipmentNbr"] = _CurrentRow.ShipmentNbr
            };

            if (parameters.Values.Count > 0)
            {
                throw new PXReportRequiredException(parameters, "SO642011");
            }

            return adapter.Get();
        }

        public PXAction<LumShipmentPlan> COCReport;
        [PXButton]
        [PXUIField(DisplayName = "Print COC Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable cOCReport(PXAdapter adapter)
        {
            var _reportID = "lm601100";
            var parameters = new Dictionary<string, string>()
            {
                ["ProductionID"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ProdOrdID,
                ["ShipmentPlanID"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentPlanID,
                ["OrderNbr"] = this.GetCacheCurrent<LumShipmentPlan>().Current.OrderNbr,
                ["LineNbr"] = this.GetCacheCurrent<LumShipmentPlan>().Current.LineNbr.ToString()
            };
            if (parameters["ProductionID"] != null && parameters["ShipmentPlanID"] != null && parameters["LineNbr"] != null && parameters["OrderNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }

        public PXAction<SOShipment> CommericalInvoice;
        [PXButton]
        [PXUIField(DisplayName = "Print Commerical Invoice Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable commericalInvoice(PXAdapter adapter)
        {
            var _reportID = "LM602025";
            if (string.IsNullOrEmpty(this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr))
                throw new PXException("ShipmentNbr Can Not be null");
            var parameters = new Dictionary<string, string>()
            {

                ["ShipmentNbr"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }

        
        public PXAction<SOShipment> DGCommericalInvoice;
        [PXButton]
        [PXUIField(DisplayName = "DG to HK Invoice Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable dGcommericalInvoice(PXAdapter adapter)
        {
            var _reportID = "LM602030";
            if (string.IsNullOrEmpty(this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr))
                throw new PXException("ShipmentNbr Can Not be null");
            var parameters = new Dictionary<string, string>()
            {
                ["RefNbr"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr
            };
            if (parameters["RefNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
            
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

            row.QtyToProd = prodItem.QtytoProd;
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
                SOLine soLine = sOResult;
                SOOrder soOrder = sOResult;

                PXFieldState valueExt = Order.Cache.GetValueExt((object)soOrder, PX.Objects.CS.Messages.Attribute + ENDC) as PXFieldState;

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
                row.CustomerPN = soLine.AlternateID;
                row.CartonSize = CSAnswers.PK.Find(this, InventoryItem.PK.Find(this, row.InventoryID).NoteID, SOShipmentEntry_Extension.CartonSize)?.Value;
            }

            LumShipmentPlan aggrShipPlan = SelectFrom<LumShipmentPlan>.Where<LumShipmentPlan.prodOrdID.IsEqual<@P.AsString>>
                                                                      .AggregateTo<Max<LumShipmentPlan.nbrOfShipment,
                                                                                       Max<LumShipmentPlan.endCartonNbr>>>.View.Select(this, row.ProdOrdID);

            row.NbrOfShipment = aggrShipPlan.NbrOfShipment == null ? 1 : aggrShipPlan.NbrOfShipment + 1;
            row.StartCartonNbr = (aggrShipPlan.EndCartonNbr ?? 0) + 1;

        }

        protected void _(Events.FieldUpdated<LumShipmentPlan.plannedShipQty> e)
        {
            var row = e.Row as LumShipmentPlan;

            if (row != null)
            {
                decimal qtyCarton = 1;
                decimal grsWeight = 0;
                decimal cartonPal = 1;
                decimal palletWgt = 0;

                InventoryItem item = InventoryItem.PK.Find(this, row.InventoryID);

                foreach (CSAnswers answers in SelectFrom<CSAnswers>.Where<CSAnswers.refNoteID.IsEqual<@P.AsGuid>>.View.Select(this, item.NoteID))
                {
                    switch (answers.AttributeID)
                    {
                        case SOShipmentEntry_Extension.QtyCarton:
                            qtyCarton = Convert.ToDecimal(answers.Value);
                            break;
                        case SOShipmentEntry_Extension.GrsWeight:
                            grsWeight = Convert.ToDecimal(answers.Value);
                            break;
                        case SOShipmentEntry_Extension.CartonPalt:
                            cartonPal = Convert.ToDecimal(answers.Value);
                            break;
                        case SOShipmentEntry_Extension.PalletWgt:
                            palletWgt = Convert.ToDecimal(answers.Value);
                            break;
                    }
                }
                row.EndCartonNbr = (row.StartCartonNbr ?? 1) + (int)Math.Ceiling((row.PlannedShipQty / qtyCarton).Value) - 1;
                row.EndLabelNbr = (row.StartLabelNbr ?? 1) + (int)Math.Ceiling((row.PlannedShipQty / qtyCarton).Value) - 1;
                row.CartonQty = qtyCarton == 0 ? 5000M : (decimal)e.NewValue / qtyCarton;
                row.NetWeight = (decimal)e.NewValue * item.BaseItemWeight;
                row.GrossWeight = (decimal)e.NewValue * grsWeight;
                // Round(Carton Qty / CARTONPALT in item attribute) * (PALLETWGT in item attribute) 四捨五入
                row.PalletWeight = Math.Round(row.CartonQty.Value / cartonPal * palletWgt, 0);
                row.MEAS = row.CartonQty * item.BaseItemVolume;
                row.DimWeight = row.CartonQty * item.BaseItemVolume * 1000000M / 5000M;
            }
        }

        protected void _(Events.FieldUpdated<LumShipmentPlan.customer> e)
        {
            var row = e.Row as LumShipmentPlan;

            if (e.NewValue != null && row != null)
            {
                SOOrder soOrder = SOOrder.PK.Find(this, row.OrderType, row.OrderNbr);

                Order.Cache.SetValueExt(soOrder, PX.Objects.CS.Messages.Attribute + ENDC, e.NewValue);
                Order.Update(soOrder);
            }
        }
        
        protected void _(Events.FieldUpdated<LumShipmentPlan.plannedShipDate> e)
        {
            var row = e.Row as LumShipmentPlan;
            var data = (LumShipmentPlan)
                SelectFrom<LumShipmentPlan>
                .Where<LumShipmentPlan.plannedShipDate.IsEqual<@P.AsDateTime>
                    .And<LumShipmentPlan.customerOrderNbr.IsEqual<@P.AsString>>
                    .And<LumShipmentPlan.inventoryID.IsEqual<@P.AsInt>>
                    .And<LumShipmentPlan.shipmentPlanID.IsEqual<@P.AsString>>
                    .And<LumShipmentPlan.prodOrdID.IsNotEqual<@P.AsString>>>
                .AggregateTo<Max<LumShipmentPlan.endLabelNbr>>
                .View.Select(this, row.PlannedShipDate,row.CustomerOrderNbr,row.InventoryID,row.ShipmentPlanID,row.ProdOrdID);
            row.StartLabelNbr = data.EndLabelNbr == null ? 1 : data.EndLabelNbr + 1;
        }
        
        #endregion

        #region Method
        /// <summary> Get Current Value to Report Parameter </summary>
        public Dictionary<string, string> GetCurrentRowToParameter(bool isOutter = false)
        {
            var _CurrentRow = this.GetCacheCurrent<LumShipmentPlan>().Current;
            PXResultset<InventoryItem> data =
                SelectFrom<InventoryItem>
                .LeftJoin<INItemXRef>.On<INItemXRef.inventoryID.IsEqual<InventoryItem.inventoryID>>
                .LeftJoin<CSAnswers>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>>
                .Where<InventoryItem.inventoryID.IsEqual<P.AsInt>>.View.Select(this, _CurrentRow.InventoryID);

            PXResult<AMProdItem, SOLine> soData =
                (PXResult<AMProdItem, SOLine>)SelectFrom<AMProdItem>
                .LeftJoin<SOLine>.On<AMProdItemExt.usrSOLineNbr.IsEqual<SOLine.lineNbr>
                    .And<AMProdItemExt.usrSOOrderNbr.IsEqual<SOLine.orderNbr>>
                    .And<AMProdItemExt.usrSOOrderType.IsEqual<SOLine.orderType>>>
                .Where<AMProdItem.prodOrdID.IsEqual<P.AsString>>.View.Select(this, _CurrentRow.ProdOrdID);

            var ENDCDescr = new PXGraph().Select<CSAttributeDetail>().Where(x => x.ValueID == _CurrentRow.Customer).FirstOrDefault()?.Description;
            ENDCDescr = ENDCDescr ?? _CurrentRow.Customer;

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["ShipmentPlanID"] = _CurrentRow.ShipmentPlanID,
                ["ProdOrdID"] = _CurrentRow.ProdOrdID,
                ["Customer"] = ENDCDescr,
                ["CustomerPartNo"] = soData.GetItem<SOLine>()?.AlternateID,
                ["Description"] = data.FirstOrDefault().GetItem<InventoryItem>().Descr,
                ["Resistor"] = data.RowCast<CSAnswers>().Where(x => x.AttributeID == "RESISTOR").FirstOrDefault()?.Value,
                ["DATE"] = _CurrentRow.PlannedShipDate?.ToString("yyyy/MM/dd")
            };
            return parameters;
        }

        /// <summary> Active Standard Report </summary>
        public void ActiveStandardReport(string _reportID)
        {
            PXUpdate<Set<
               UserReport.isActive, Required<UserReport.isActive>>,
               UserReport,
               Where<UserReport.reportFileName, Equal<Required<UserReport.reportFileName>>>>.Update(this, false, $"{_reportID}.rpx");

            PXUpdate<Set<
               UserReport.isActive, Required<UserReport.isActive>>,
               UserReport,
               Where<UserReport.reportFileName, Equal<Required<UserReport.reportFileName>>,
                 And<UserReport.description, Equal<Required<UserReport.description>>>>>.Update(this, true, $"{_reportID}.rpx", "Standard");
        }

        #endregion
    }
}
