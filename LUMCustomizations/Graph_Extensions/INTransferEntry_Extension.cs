using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Data.DependencyInjection;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.GL.Descriptor;
using PX.Objects;
using PX.Objects.IN;

namespace PX.Objects.IN
{
    public class INTransferEntry_Extension : PXGraphExtension<INTransferEntry>
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(InventoryTransferReport);
        }

        #region Action
        public PXAction<INRegister> InventoryTransferReport;
        [PXButton]
        [PXUIField(DisplayName = "Inventory Transfer Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable inventoryTransferReport(PXAdapter adapter)
        {
            var _reportID = "LM612020";
            if (Base.transfer.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.transfer.Current.DocType;
                parameters["RefNbr"] = Base.transfer.Current.RefNbr;
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion
    }
}