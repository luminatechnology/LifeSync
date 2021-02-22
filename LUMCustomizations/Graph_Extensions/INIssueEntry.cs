using LUMCustomizations.Library;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.IN;
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

        public virtual void _(Events.RowPersisting<INRegister> e)
        {
            INRegister row = e.Row;
            if (new LumLibrary().GetJournalEnhance)
            {
                if (!string.IsNullOrEmpty(row.SOShipmentNbr) && !string.IsNullOrEmpty(row.SOShipmentType))
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
            }
        }

        #region Action
        public PXAction<INRegister> InventoryIssueReport;
        [PXButton]
        [PXUIField(DisplayName = "Inventory Issue Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable inventoryIssueReport(PXAdapter adapter)
        {
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
