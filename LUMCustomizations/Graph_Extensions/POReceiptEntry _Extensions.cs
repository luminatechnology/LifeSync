using LUMCustomizations.Library;
using System.Collections.Generic;
using System.Collections;
using PX.Data;

namespace PX.Objects.PO
{
    public class POReceiptEntry_Extension : PXGraphExtension<POReceiptEntry>
    {
        public static bool IsActive()
        {
            //active customize button if current country ID is CN or HK
            return new LumLibrary().isCNorHK();
        }

        public override void Initialize()
        {
            Base.report.AddMenuAction(POReceipt);
            Base.report.AddMenuAction(POReturn);
            base.Initialize();
        }

        #region Action
        public PXAction<POReceipt> POReceipt;
        [PXButton]
        [PXUIField(DisplayName = "Print PO Receipt", Visible = false, Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable pOReceipt(PXAdapter adapter)
        {
            var _reportID = "LM646000";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["ReceiptNbr"] = Base.Document.Current.ReceiptNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }

        public PXAction<POReceipt> POReturn;
        [PXUIField(DisplayName = "Print PO Return", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected virtual IEnumerable pOReturn(PXAdapter adapter)
        {
            var _reportID = "LM646005";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["ReceiptNbr"] = Base.Document.Current.ReceiptNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion
        
    }
}