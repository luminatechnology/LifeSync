using PX.Data;
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
        public static bool IsActive()
        {
            //active customize button if current company is ABA China and HK 
            return PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName).Contains("China") || PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName).Contains("HK");
        }

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(InventoryIssueReport);
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
    }
}
