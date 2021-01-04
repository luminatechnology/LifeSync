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
            Base.report.AddMenuAction(POReceipt);
        }

        
        #region Action
        public PXAction<POOrder> POReceipt;
        [PXButton]
        [PXUIField(DisplayName = "Print PO Receipt", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable pOReceipt(PXAdapter adapter)
        {
            var _reportID = "LM646000";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["ReceiptNbr"] = Base.Document.Current.ReceiptNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion
    }
}