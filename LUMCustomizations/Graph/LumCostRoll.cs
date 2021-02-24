using JAMS;
using JAMS.AM;
using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;

namespace LUMCustomizations.Graph
{
    public class LumCostRoll : BOMCostRoll
    {
        public PXFilter<RollupSettings> rollsettings;

        public PXProcessingJoin<AMBomCost,
                InnerJoin<AMBomItem, On<AMBomCost.bOMID, Equal<AMBomItem.bOMID>,
                    And<AMBomCost.revisionID, Equal<AMBomItem.revisionID>>>>,
                Where<AMBomCost.userID, Equal<Current<AccessInfo.userID>>>> ProcBomCostRecs;

        public LumCostRoll()
        {
            ProcBomCostRecs.SetProcessVisible(false);
            ProcBomCostRecs.SetProcessAllCaption("Cost Roll");

            var _bomItem = new PXGraph().Select<AMBomItem>();
            if ((rollsettings.Current.LotSize == 0 || rollsettings.Current.LotSize == null) && rollsettings.Current.SnglMlti == "S")
            {
                rollsettings.Current.LotSize = 1000;
                rollsettings.Current.SnglMlti = "M";
            }


            if (ProcBomCostRecs.Select().Count == 0)
                BomCostRecs.Cache.Insert(DoingCostRoll(_bomItem.FirstOrDefault()));

            ProcBomCostRecs.SetProcessDelegate(delegate (List<AMBomCost> list)
            {
                rollsettings.Current.ApplyPend = false;
                rollsettings.Current.IncFixed = true;
                rollsettings.Current.IncMatScrp = true;
                rollsettings.Current.UpdateMaterial = false;
                rollsettings.Current.IsPersistMode = false;
                // Call the action to run and display the cost roll
                PXLongOperation.StartOperation(this, () => aMBomCostSummary(rollsettings.Current, _bomItem));
                rollsettings.ClearDialog();
            });
        }

        public IEnumerable aMBomCostSummary(RollupSettings _setting, IQueryable<AMBomItem> _bomItem)
        {
            // Delete All Data By User
            PXDatabase.Delete<AMBomCost>(new PXDataFieldRestrict<AMBomCost.userID>(Accessinfo.UserID));

            // Get All BOM Data
            int count = 0;
            string errorMsg = string.Empty;
            Settings.Current = _setting.DeepClone();
            List<object> result = new List<object>();
            foreach (var _bom in _bomItem)
            {
                try
                {
                    result.Add(DoingCostRoll(_bom));
                }
                catch (Exception ex)
                {
                    errorMsg += $"Error:{ex.Message} BOMID:{_bom.BOMID} Revision:{_bom.RevisionID}\n";
                }
            }
            // write Error Msg
            if (string.IsNullOrEmpty(errorMsg))
                PXProcessing.SetWarning(errorMsg);

            BomCostRecs.Cache.Clear();
            result.ForEach(x => { BomCostRecs.Cache.Insert(x); });
            Actions.PressSave();
            return null;
        }

        public Object DoingCostRoll(AMBomItem _bom)
        {
            Settings.Current.BOMID = _bom.BOMID; //Documents.Current.BOMID;
            Settings.Current.RevisionID = _bom.RevisionID; // Documents.Current.RevisionID;
            if (Settings.Current != null)
            {
                Settings.Current.EffectiveDate =
                    _bom.EffEndDate != null ? Settings.Current.EffectiveDate.LesserDateTime(_bom.EffEndDate)
                                            : Settings.Current.EffectiveDate;
                base.RollCosts();
            }
            return BomCostRecs.Cache.Current;
        }

    }
}
