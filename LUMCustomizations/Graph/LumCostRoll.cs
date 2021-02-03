using JAMS;
using JAMS.AM;
using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (rollsettings.Current != null && rollsettings.Current.LotSize == null)
            {
                rollsettings.Cache.SetValueExt<RollupSettings.lotSize>(rollsettings.Current,
                    InventoryHelper.GetMfgReorderQty(this,
                        22325,  // Document.inventory
                        337));  // Document.SiteID
            }

            if (rollsettings.Current != null && rollsettings.Current.LotSize.GetValueOrDefault() <= 0)
                rollsettings.Current.LotSize = 1;

            if (rollsettings.AskExt() == WebDialogResult.OK)
            {
                rollsettings.Current.ApplyPend = false;
                rollsettings.Current.BOMID = "S210634"; //Documents.Current.BOMID;
                rollsettings.Current.RevisionID = "A"; // Documents.Current.RevisionID;
                rollsettings.Current.IncFixed = true;
                rollsettings.Current.IncMatScrp = true;
                rollsettings.Current.UpdateMaterial = false;
                // Call the action to run and display the cost roll
                //aMBomCostSummary(adapter);
                //PXLongOperation.StartOperation(this, () => aMBomCostSummary(this.rollsettings.Current));
                aMBomCostSummary(this.rollsettings.Current);
            }

            rollsettings.Cache.Clear();
            rollsettings.ClearDialog();

            return adapter.Get();
        }

        public virtual void aMBomCostSummary(RollupSettings _rollSetting)
        {
            if (rollsettings.Current != null)
            {
                //if (Documents.Current?.EffEndDate != null)
                //{
                //rollsettings.Current.EffectiveDate = rollsettings.Current.EffectiveDate.LesserDateTime(Documents.Current?.EffEndDate);
                //}
                rollsettings.Current.EffectiveDate = DateTime.Now;
                BOMCostRoll.RollCostsRetry(_rollSetting);
            }
        }

    }
}
