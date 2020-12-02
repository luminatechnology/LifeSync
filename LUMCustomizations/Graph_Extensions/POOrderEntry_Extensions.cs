using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.PO
{
    public class POOrderEntry_Extensions : PXGraphExtension<POOrderEntry>
    {

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(DomesticPO);
        }

        #region Action
        public PXAction<POOrder> DomesticPO;
        [PXButton]
        [PXUIField(DisplayName = "Print Domestic PO", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable domesticPO(PXAdapter adapter)
        {
            var _reportID = "lm613000";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var _poOrder = adapter.Get<POOrder>().ToList().FirstOrDefault();
            parameters["OrderNbr"] = _poOrder.OrderNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion
    }
}
