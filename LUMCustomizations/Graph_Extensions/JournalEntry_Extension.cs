using LUMCustomizations.Library;
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
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                Base.report.AddMenuAction(GLJournalAction);
            }
        }

        #region Material Issues Action
        public PXAction<Batch> GLJournalAction;
        [PXButton]
        [PXUIField(DisplayName = "GL Journal Report", MapEnableRights = PXCacheRights.Select)]
        protected void gLJournalAction()
        {
            var curBatchCache = (Batch)Base.BatchModule.Cache.Current;
            var postPeriod = curBatchCache.FinPeriodID.Substring(4, 2) + curBatchCache.FinPeriodID.Substring(0, 4); //MMyyyy
            // create the parameter for report
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["BatchNbr"] = curBatchCache.BatchNbr;
            //parameters["PeriodFrom"] = postPeriod;
            //parameters["PeriodTo"] = postPeriod;

            // using Report Required Exception to call the report
            throw new PXReportRequiredException(parameters, "LM621005", "LM621005");
        }
        #endregion

        #region controll customize button based on country ID
        protected void _(Events.RowSelected<Batch> e)
        {
            var _lumLibrary = new LumLibrary();
            if (!_lumLibrary.isCNorHK())
            {
                GLJournalAction.SetVisible(false);
            }
        }
        #endregion
    }
}