using PX.Data;
using System.Collections.Generic;

namespace JAMS.AM
{
  public class MoveEntry_Extension : PXGraphExtension<MoveEntry>
  {
        public override void Initialize()
        {
            ReportAction.AddMenuAction(ProductionMoveAction);
            ReportAction.MenuAutoOpen = true;
        }

        #region  Actions

        #region Report Action
        public PXAction<AMBatch> ReportAction;
        [PXButton]
        [PXUIField(DisplayName = "Report")]
        protected void reportAction() { }
        #endregion

        #region Material Issues Action
        public PXAction<AMBatch> ProductionMoveAction;
        [PXButton]
        [PXUIField(DisplayName = "Production Move", MapEnableRights = PXCacheRights.Select)]
        protected void productionMoveAction()
        {
            var curAMBatchCache = (AMBatch)Base.batch.Cache.Current;
            // create the parameter for report
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["BatNbr"] = curAMBatchCache.BatNbr;
            parameters["AttributeID"] = "PRODLINE";

            // using Report Required Exception to call the report
            throw new PXReportRequiredException(parameters, "LM603020", "LM603020");
        }
        #endregion

        #endregion

        #region Event Handlers

        #endregion
    }
}