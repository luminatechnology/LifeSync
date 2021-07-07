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
        [PXUIField(DisplayName = "Print PO Receipt", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable pOReceipt(PXAdapter adapter)
        {
            var curPOReceiptCache = (POReceipt)Base.Document.Cache.Current;
            var _printCount = curPOReceiptCache.GetExtension<POReceiptExt>().UsrPrintCount ?? 0;

            //Calculate Print Count
            PXUpdate<Set<POReceiptExt.usrPrintCount, Required<POReceiptExt.usrPrintCount>>,
                         POReceipt,
                         Where<POReceipt.receiptNbr, Equal<Required<POReceipt.receiptNbr>>,
                           And<POReceipt.receiptType, Equal<Required<POReceipt.receiptType>>>
                     >>.Update(Base, ++_printCount, curPOReceiptCache.ReceiptNbr, curPOReceiptCache.ReceiptType);

            // create the parameter for report
            var _reportID = "LM646000";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["ReceiptNbr"] = Base.Document.Current.ReceiptNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }

        public PXAction<POReceipt> POReturn;
        [PXButton]
        [PXUIField(DisplayName = "Print PO Return", Enabled = true, MapEnableRights = PXCacheRights.Select)]
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
            if (!_lumLibrary.isCNorHK())
            {
                POReceipt.SetVisible(false);
                POReturn.SetVisible(false);
            }
        }
        #endregion
    }
}