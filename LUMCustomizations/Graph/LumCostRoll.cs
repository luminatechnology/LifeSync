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

        public PXAction<RollupSettings> aMBomCostSettings;
        [PXUIField(DisplayName = "Get BOM Cost Summary")]
        [PXButton]
        protected virtual IEnumerable AMBomCostSettings(PXAdapter adapter)
        {
            var _bomItem = new PXGraph().Select<AMBomItem>();
            if (rollsettings.Current != null && rollsettings.Current.LotSize == null)
                rollsettings.Cache.SetValueExt<RollupSettings.lotSize>(rollsettings.Current,
                    JAMS.AM.InventoryHelper.GetMfgReorderQty(this,
                        _bomItem.FirstOrDefault().InventoryID,  // Document.inventory
                        _bomItem.FirstOrDefault().SiteID));  // Document.SiteID

            if (rollsettings.Current != null && rollsettings.Current.LotSize.GetValueOrDefault() <= 0)
                rollsettings.Current.LotSize = 1;

            if (rollsettings.AskExt() == WebDialogResult.OK)
            {
                rollsettings.Current.ApplyPend = false;
                rollsettings.Current.IncFixed = true;
                rollsettings.Current.IncMatScrp = true;
                rollsettings.Current.UpdateMaterial = false;
                rollsettings.Current.IsPersistMode = false;
                // Call the action to run and display the cost roll
                PXLongOperation.StartOperation(this, () => aMBomCostSummary(rollsettings.Current, _bomItem));
                //aMBomCostSummary(rollsettings.Current, _bom.EffEndDate);
            }

            //rollsettings.Cache.Clear();
            rollsettings.ClearDialog();

            return adapter.Get();
        }

        public IEnumerable aMBomCostSummary(RollupSettings _setting, IQueryable<AMBomItem> _bomItem)
        {
            // Get All BOM Data
            int count = 0;
            List<object> result = new List<object>();
            Settings.Current = _setting.DeepClone();
            foreach (var _bom in _bomItem)
            {
                Settings.Current.BOMID = _bom.BOMID; //Documents.Current.BOMID;
                Settings.Current.RevisionID = _bom.RevisionID; // Documents.Current.RevisionID;
                if (rollsettings.Current != null)
                {
                    Settings.Current.EffectiveDate =
                        _bom.EffEndDate != null ? Settings.Current.EffectiveDate.LesserDateTime(_bom.EffEndDate)
                                                : Settings.Current.EffectiveDate;
                    base.RollCosts();
                    if (BomCostRecs.Cache != null)
                        result.Add(BomCostRecs.Cache.Current);
                    if (++count == 30)
                        break;
                }
            }
            BomCostRecs.Cache.Clear();
            result.ForEach(x => { BomCostRecs.Cache.Insert(x); });
            Actions.PressSave();
            return null;
        }

    }
}
