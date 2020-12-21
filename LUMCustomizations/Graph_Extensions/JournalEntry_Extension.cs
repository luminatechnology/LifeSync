using PX.Data;
using System;
using System.Collections.Generic;

namespace PX.Objects.GL
{
  public class JournalEntry_Extension : PXGraphExtension<JournalEntry>
  {
        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(GLJournalAction);
            //Base.report.MenuAutoOpen = true;
        }

        #region  Actions

        #region Material Issues Action
        public PXAction<Batch> GLJournalAction;
        [PXButton]
        [PXUIField(DisplayName = "GL Journal Report", MapEnableRights = PXCacheRights.Select)]
        protected void gLJournalAction()
        {
            var period = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToString("MMyyyy");
            var curBatchCache = (Batch)Base.BatchModule.Cache.Current;
            // create the parameter for report
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["PeriodFrom"] = period;
            parameters["PeriodTo"] = period;

            // using Report Required Exception to call the report
            throw new PXReportRequiredException(parameters, "LM621005", "LM621005");
        }
        #endregion

        #endregion
    }
}