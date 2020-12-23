using JAMS.AM;
using LumCustomizations.DAC;
using LUMCustomizations.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
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
        }
        #endregion

        #region Selects & Features
        [PXFilterable()]
        public SelectFrom<LumShipmentPlan>.View ShipPlan;
        public SelectFrom<SOOrder>.Where<SOOrder.orderType.IsEqual<LumShipmentPlan.orderType>.And<SOOrder.orderNbr.IsEqual<LumShipmentPlan.orderNbr>>>.View Order;

        public PXSave<LumShipmentPlan> Save;
        public PXCancel<LumShipmentPlan> Cancel;
        #endregion

        #region Actions
        public PXAction<LumShipmentPlan> Report;
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected void report() { }

        public PXAction<LumShipmentPlan> InnerLabelGenaral;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Print Inner Label-General", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable innerLabelGenaral(PXAdapter adapter)
        {
            this.Save.Press();
            var _reportID = "lm601000";
            var parameters = GetCurrentRowToParameter();
            //if (parameters["ShipmentPlanID"] != null)
            //    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));

            // Get Printer Info
            var printer = PXGraph.CreateInstance<SMPrinterMaint>();
            var printerID = printer.Printers.Select().FirstTableItems.Where(x => x.PrinterName == "ZEBER105").FirstOrDefault().PrinterID;

            PrintSettings printSettings = new PrintSettings()
            {
                PrinterID = printerID,
                NumberOfCopies = 1,
                PrintWithDeviceHub = true,
                DefinePrinterManually = false
            };
            //PXGraph.CreateInstance<SMPrintJobMaint>().CreatePrintJob(
            //    printSettings,
            //    _reportID,
            //    parameters,
            //    $"Report {_reportID}");

            PXReportRequiredException ex = null;
            ex = PXReportRequiredException.CombineReport(ex, _reportID, parameters);

            PX.SM.SMPrintJobMaint.CreatePrintJobGroup(printSettings, ex, $"Print Report {_reportID}");

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
                ["ShipmentPlanID"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentPlanID
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
            var parameters = GetCurrentRowToParameter();

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
            var _reportID = "lm611002";
            if (string.IsNullOrEmpty(this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr))
                throw new PXException("ShipmentNbr Can Not be null");
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentNbr,
                ["ShipmentPlanID"] = this.GetCacheCurrent<LumShipmentPlan>().Current.ShipmentPlanID
            };
            if (parameters["ShipmentNbr"] != null && parameters["ShipmentPlanID"] != null)
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
                SOLine  soLine = sOResult;
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
                row.CustomerPN         = soLine.AlternateID;
                row.CartonSize         = CSAnswers.PK.Find(this, InventoryItem.PK.Find(this, row.InventoryID).NoteID, SOShipmentEntry_Extension.CartonSize)?.Value;
            }

            LumShipmentPlan aggrShipPlan = SelectFrom<LumShipmentPlan>.Where<LumShipmentPlan.prodOrdID.IsEqual<@P.AsString>>
                                                                      .AggregateTo<Max<LumShipmentPlan.nbrOfShipment,
                                                                                       Max<LumShipmentPlan.endCartonNbr>>>.View.Select(this, row.ProdOrdID);

            row.NbrOfShipment  = aggrShipPlan.NbrOfShipment == null ? 1 : aggrShipPlan.NbrOfShipment + 1;
            row.StartCartonNbr = (aggrShipPlan.EndCartonNbr ?? 0) + 1;

            aggrShipPlan = SelectFrom<LumShipmentPlan>.Where<LumShipmentPlan.orderNbr.IsEqual<@P.AsString>>
                                                             .AggregateTo<Max<LumShipmentPlan.endLabelNbr>>.View.Select(this, row.OrderNbr);

            row.StartLabelNbr = aggrShipPlan.EndLabelNbr == null ? 1 : aggrShipPlan.EndLabelNbr + 1;
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

                row.EndCartonNbr = row.StartCartonNbr + (int)Math.Round((row.PlannedShipQty / qtyCarton).Value, 0) - 1;
                row.EndLabelNbr  = row.StartLabelNbr  + (int)Math.Round((row.PlannedShipQty / qtyCarton).Value, 0) - 1;
                row.CartonQty    = qtyCarton == 0 ? 5000M : (decimal)e.NewValue / qtyCarton;
                row.NetWeight    = (decimal)e.NewValue * item.BaseItemWeight;
                row.GrossWeight  = (decimal)e.NewValue * grsWeight;
                // Round(Carton Qty / CARTONPALT in item attribute) * (PALLETWGT in item attribute) 四捨五入
                row.PalletWeight = Math.Round(row.CartonQty.Value / cartonPal * palletWgt, 0);
                row.MEAS         = row.CartonQty * item.BaseItemVolume;
                row.DimWeight    = row.CartonQty * item.BaseItemVolume * 1000000M / 5000M;
            }
        }
        #endregion

        #region Method
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
    }
}
