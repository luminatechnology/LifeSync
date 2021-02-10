using LUMCustomizations.Library;
using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMCustomizations.Graph_Extensions
{
    public class INPIReview_Extension: PXGraphExtension<INPIReview>
    {
        public override void Initialize()
        {
            base.Initialize();
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                Base.actionsFolder.AddMenuAction(CountintListReport);
            }
        }

        #region Action

        public PXAction<INPIHeader> CountintListReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Counting list", Visible = false, Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable countintListReport(PXAdapter adapter)
        {
            var _reportID = "lm615000";
            if (Base.PIHeader.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["PIID"] = Base.PIHeader.Current.PIID;
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion

        #region controll customize button based on country ID
        protected void _(Events.RowSelected<INPIHeader> e)
        {
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                CountintListReport.SetVisible(true);
            }
        }
        #endregion
    }
}
