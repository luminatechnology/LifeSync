﻿using System.Collections;
using System.Collections.Generic;
using PX.Data;

namespace PX.Objects.IN
{
    public class INReceiptEntry_Extension : PXGraphExtension<INReceiptEntry>
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(InventoryReceiptReport);
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
        #endregion

    }
}