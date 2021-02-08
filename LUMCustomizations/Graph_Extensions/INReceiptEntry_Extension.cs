using System.Collections;
using System.Collections.Generic;
using LUMCustomizations.Library;
using PX.Data;

namespace PX.Objects.IN
{
    public class INReceiptEntry_Extension : PXGraphExtension<INReceiptEntry>
    {
        public static bool IsActive()
        {
            //active customize button if current country ID is CN or HK
            return new LumLibrary().isCNorHK();
        }

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(InventoryReceiptReport);
            Base.report.AddMenuAction(InventoryReceiptReportruku);
        }

        #region Action
        public PXAction<INRegister> InventoryReceiptReport;
        [PXButton]
        [PXUIField(DisplayName = "Inventory Receipt Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable inventoryReceiptReport(PXAdapter adapter)
        {
            if (Base.receipt.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.receipt.Current.DocType;
                parameters["RefNbr"] = Base.receipt.Current.RefNbr;
                parameters["PeriodTo"] = null;
                parameters["PeriodFrom"] = null;
                throw new PXReportRequiredException(parameters, "LM612000", "Report LM612000");
            }
            return adapter.Get();
        }

        public PXAction<INRegister> InventoryReceiptReportruku;
        [PXButton]
        [PXUIField(DisplayName = "Inventory Receipt Report - rukudan", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable inventoryReceiptReportruku(PXAdapter adapter)
        {
            if (Base.receipt.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.receipt.Current.DocType;
                parameters["RefNbr"] = Base.receipt.Current.RefNbr;
                parameters["PeriodTo"] = null;
                parameters["PeriodFrom"] = null;
                throw new PXReportRequiredException(parameters, "LM612010", "Report LM612010");
            }
            return adapter.Get();
        }
        #endregion

    }
}