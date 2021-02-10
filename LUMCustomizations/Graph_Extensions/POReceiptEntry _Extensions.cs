using LUMCustomizations.Library;
using System.Collections.Generic;
using System.Collections;
using PX.Data;

namespace PX.Objects.PO
{
    public class POReceiptEntry_Extension : PXGraphExtension<POReceiptEntry>
    {
        public override void Initialize()
        {
            base.Initialize();
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                Base.report.AddMenuAction(POReceipt);
                Base.report.AddMenuAction(POReturn);
            }
        }

        #region Action
        public PXAction<POReceipt> POReceipt;
        [PXButton]
        [PXUIField(DisplayName = "Print PO Receipt", Visible = false, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable pOReceipt(PXAdapter adapter)
        {
            var _reportID = "LM646000";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["ReceiptNbr"] = Base.Document.Current.ReceiptNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }

        public PXAction<POReceipt> POReturn;
        [PXButton]
        [PXUIField(DisplayName = "Print PO Return", Visible = false, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable pOReturn(PXAdapter adapter)
        {
            var _reportID = "LM646005";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["ReceiptNbr"] = Base.Document.Current.ReceiptNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion

        #region controll customize button based on country ID
        protected void _(Events.RowSelected<POReceipt> e)
        {
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                POReceipt.SetVisible(true);
                POReturn.SetVisible(true);
            }
        }
        #endregion
    }
}