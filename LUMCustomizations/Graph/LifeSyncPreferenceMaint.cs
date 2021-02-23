using LumCustomizations.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CM;
using System;
using System.Linq;

namespace LumCustomizations.Graph
{
    public class LifeSyncPreferenceMaint : PXGraph<LifeSyncPreferenceMaint>
    {
        public PXSave<LifeSyncPreference> Save;

        public PXCancel<LifeSyncPreference> Cancel;

        public SelectFrom<LifeSyncPreference>.View MasterView;

        public LifeSyncPreferenceMaint()
        {
            var _rateData = SelectFrom<CurrencyRateType>.View.Select(this);
            PXStringListAttribute.SetList<LifeSyncPreference.internalCostModelRateType>(
                base.Caches[typeof(LifeSyncPreference)],
                null, _rateData.FirstTableItems.Select(x => x.CuryRateTypeID).ToArray(),
                _rateData.FirstTableItems.Select(x => x.CuryRateTypeID).ToArray());
        }

    }
}