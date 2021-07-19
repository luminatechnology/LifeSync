using LUMCustomizations.Library;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public class INIssueEntry_Extension : PXGraphExtension<INIssueEntry>
    {
        public override void Initialize()
        {
            base.Initialize();
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                Base.report.AddMenuAction(InventoryIssueReport);
            }
        }

        public virtual void _(Events.RowPersisting<INRegister> e,PXRowPersisting baseMethod)
        {
            baseMethod?.Invoke(e.Cache,e.Args);
            INRegister row = e.Row;
            if (new LumLibrary().GetJournalEnhance && row != null)
            {
                if (!string.IsNullOrEmpty(row.SOShipmentNbr) && !string.IsNullOrEmpty(row.SOShipmentType) && string.IsNullOrEmpty(row.TranDesc))
                {
                    var CustomerID = SelectFrom<SOShipment>
                                    .Where<SOShipment.shipmentNbr.IsEqual<P.AsString>
                                        .And<SOShipment.shipmentType.IsEqual<P.AsString>>>
                                    .View.Select(Base,row.SOShipmentNbr,row.SOShipmentType)?.TopFirst?.CustomerID;
                    var CustomerName = SelectFrom<Customer>
                                       .Where<Customer.bAccountID.IsEqual<P.AsInt>>
                                       .View.Select(Base, CustomerID)?.TopFirst?.AcctName;
                    row.TranDesc = $"{row.SOShipmentNbr} {CustomerName}";
                }
                else if(row.POReceiptType == "RN" && !string.IsNullOrEmpty(row.POReceiptNbr) && string.IsNullOrEmpty(row.TranDesc))
                {
                    var poData = SelectFrom<POReceipt>
                                .Where<POReceipt.receiptNbr.IsEqual<P.AsString>.And<POReceipt.receiptType.IsEqual<P.AsString>>>
                                .View.Select(Base,row.POReceiptNbr,row.POReceiptType).RowCast<POReceipt>().FirstOrDefault();
                    if(poData != null && poData.VendorID.HasValue)
                    {
                        var vendorName = SelectFrom<BAccount2>.Where<BAccount2.bAccountID.IsEqual<P.AsInt>>
                                         .View.Select(Base,poData.VendorID).RowCast<BAccount2>().FirstOrDefault().AcctName;
                        row.TranDesc = $"{row.POReceiptNbr} {vendorName}";
                    }
                }
            }
        }

        #region Action
        public PXAction<INRegister> InventoryIssueReport;
        [PXButton]
        [PXUIField(DisplayName = "Inventory Issue Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable inventoryIssueReport(PXAdapter adapter)
        {
            var curPOReceiptCache = (INRegister)Base.issue.Cache.Current;
            var _printCount = curPOReceiptCache.GetExtension<INRegisterExt>().UsrPrintCount ?? 0;

            //Calculate Print Count
            PXUpdate<Set<INRegisterExt.usrPrintCount, Required<INRegisterExt.usrPrintCount>>,
                         INRegister,
                         Where<INRegister.refNbr, Equal<Required<INRegister.refNbr>>,
                           And<INRegister.docType, Equal<Required<INRegister.docType>>>
                     >>.Update(Base, ++_printCount, curPOReceiptCache.RefNbr, curPOReceiptCache.DocType);

            // create the parameter for report
            var _reportID = "lm612005";
            if (Base.issue.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.issue.Current.DocType;
                parameters["RefNbr"] = Base.issue.Current.RefNbr;
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion

        #region controll customize button based on country ID
        protected void _(Events.RowSelected<INRegister> e)
        {
            var _lumLibrary = new LumLibrary();
            if (!_lumLibrary.isCNorHK())
            {
                InventoryIssueReport.SetVisible(false);
            }
        }
        #endregion
    }
}
