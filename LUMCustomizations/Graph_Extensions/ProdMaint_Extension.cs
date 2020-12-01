using JAMS.AM;
using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public class ProdMaint_Extension : PXGraphExtension<ProdMaint>
    {

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(ProductionInstruction);
        }

        #region Action
        public PXAction<AMProdItem> ProductionInstruction;
        [PXButton]
        [PXUIField(DisplayName = "Production Instruction", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable productionInstruction(PXAdapter adapter)
        {
            var _reportID = "lm625000";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Production_Nbr"] = Base.ProdMaintRecords.Current.ProdOrdID;
            parameters["OrderType"] = Base.ProdMaintRecords.Current.OrderType;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion
    }
}
