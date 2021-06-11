using LumCustomizations.DAC;
using LUMCustomizations.Library;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JAMS.AM
{
    public class MaterialEntry_Extension : PXGraphExtension<MaterialEntry>
    {
        public override void Initialize()
        {
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                ReportAction.AddMenuAction(MaterialIssuesAction);
                ReportAction.AddMenuAction(MaterialReturnAction);
                ReportAction.MenuAutoOpen = true;
            }
        }

        #region Override transactions view
        [PXImport(typeof(AMBatch))]
        public PXSelect<AMMTran, Where<AMMTran.docType, Equal<Current<AMBatch.docType>>, And<AMMTran.batNbr, Equal<Current<AMBatch.batNbr>>>>, OrderBy<Asc<AMMTran.inventoryID>>> transactions;
        #endregion

        #region Override Attribute
        [PXDBQuantity(2, typeof(AMMTran.uOM), typeof(AMMTran.baseQty), HandleEmptyKey = true)]
        [PXDefault(TypeCode.Decimal, "0")]
        [PXUIField(DisplayName = "Quantity")]
        [PXFormula(null, typeof(SumCalc<AMBatch.totalQty>))]
        public virtual void _(Events.CacheAttached<AMMTran.qty> e) { }

        #endregion

        #region  Actions

        #region Report Action
        public PXAction<AMBatch> ReportAction;
        [PXButton]
        [PXUIField(DisplayName = "Report")]
        protected void reportAction() { }
        #endregion

        #region Material Issues Action
        public PXAction<AMBatch> MaterialIssuesAction;
        [PXButton]
        [PXUIField(DisplayName = "Material Issues", MapEnableRights = PXCacheRights.Select)]
        protected void materialIssuesAction()
        {
            var curAMBatchCache = (AMBatch)Base.batch.Cache.Current;
            var _printCount = curAMBatchCache.GetExtension<AMBatchExt>().UsrPrintCount ?? 0;

            //Calculate Print Count
            PXUpdate<Set<AMBatchExt.usrPrintCount, Required<AMBatchExt.usrPrintCount>>,
                         AMBatch,
                         Where<AMBatch.batNbr, Equal<Required<AMBatch.batNbr>>,
                           And<AMBatch.docType, Equal<Required<AMBatch.docType>>>
                     >>.Update(Base, ++_printCount, curAMBatchCache.BatNbr, curAMBatchCache.DocType);

            // create the parameter for report
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["BatNbr"] = curAMBatchCache.BatNbr;
            parameters["AttributeID"] = "PRODLINE";

            // using Report Required Exception to call the report
            throw new PXReportRequiredException(parameters, "LM206100", "LM206100");
        }
        #endregion

        #region Material Return Action
        public PXAction<AMBatch> MaterialReturnAction;
        [PXButton]
        [PXUIField(DisplayName = "Material Return", MapEnableRights = PXCacheRights.Select)]
        protected void materialReturnAction()
        {
            var curAMBatchCache = (AMBatch)Base.batch.Cache.Current;
            // create the parameter for report
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["BatNbr"] = curAMBatchCache.BatNbr;
            parameters["AttributeID"] = "PRODLINE";

            // using Report Required Exception to call the report
            throw new PXReportRequiredException(parameters, "LM206105", "LM206105");
        }
        #endregion

        #endregion

        #region controll customize button based on country ID
        protected void _(Events.RowSelected<AMBatch> e)
        {
            var _lumLibrary = new LumLibrary();
            if (!_lumLibrary.isCNorHK())
            {
                ReportAction.SetVisible(false);
                MaterialIssuesAction.SetVisible(false);
                MaterialReturnAction.SetVisible(false);
            }
        }
        #endregion

        public virtual void _(Events.FieldVerifying<AMMTran.qty> e, PXFieldVerifying baseMethod)
        {
            var row = e.Row as AMMTran;
            var inputValue = e.NewValue;
            if (!(row.GetExtension<AMMTranExt>().UsrOverIssue ?? false))
                baseMethod?.Invoke(e.Cache, e.Args);
            // valid over Issue
            else
            {
                var amProdMatl = SelectFrom<AMProdMatl>
                                 .Where<AMProdMatl.prodOrdID.IsEqual<P.AsString>
                                    .And<AMProdMatl.inventoryID.IsEqual<P.AsInt>>>.View.Select(Base, row.ProdOrdID, row.InventoryID)
                                 .RowCast<AMProdMatl>().FirstOrDefault();
                var maxOverIssue = SelectFrom<LifeSyncPreference>.View.Select(Base).RowCast<LifeSyncPreference>().FirstOrDefault().MaxOverIssue;
                var overIssueQty = Math.Round((amProdMatl?.TotalQtyRequired ?? 0) * (1 + (maxOverIssue ?? 0) / 100), 4);
                if ((decimal?)e.NewValue > overIssueQty)
                {
                    e.NewValue = overIssueQty;
                    e.Cache.SetValue<AMMTran.qty>(row, overIssueQty);
                    throw new PXSetPropertyException<AMMTran.qty>($"Material Quantity {inputValue} to be issued is greater then maximum allowed over issue {overIssueQty}");
                }
            }
        }

        public virtual void _(Events.RowPersisting<AMMTran> e, PXRowPersisting baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            var amTran = Base.transactions.Select().RowCast<AMMTran>().ToList();
            var duplicateData = amTran.GroupBy(x => new { x.ProdOrdID, x.InventoryID })
                                      .Where(g => g.Count() > 1)
                                      .Select(x => new { x.Key.ProdOrdID, x.Key.InventoryID });
            if (duplicateData.Count() > 0)
                throw new PXException($"You cannot add the material twice for one production order , and not allow users to save");

            if (!(row.GetExtension<AMMTranExt>().UsrOverIssue ?? false))
            {
                object qty = row.Qty;
                var valid = e.Cache.RaiseFieldVerifying<AMMTran.qty>(e.Row, ref qty);
                if (!valid)
                    throw new PXException("You cannot save, please check error message");
            }
        }

        public virtual void _(Events.FieldUpdated<AMMTranExt.usrOverIssue> e)
        {
            var row = e.Row as AMMTran;
            object qty = ((AMMTran)e.Row).Qty;
            var matl = SelectFrom<AMProdMatl>
                       .Where<AMProdMatl.prodOrdID.IsEqual<P.AsString>
                            .And<AMProdMatl.inventoryID.IsEqual<P.AsInt>>>
                       .View.Select(Base, row.ProdOrdID, row.InventoryID).RowCast<AMProdMatl>().FirstOrDefault();
            if (!(bool)e.NewValue)
                e.Cache.RaiseFieldVerifying<AMMTran.qty>(e.Row, ref qty);
        }
    }
}