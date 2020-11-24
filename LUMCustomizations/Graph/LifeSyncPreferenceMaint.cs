using LUMCustomizations.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using System;

namespace LUMCustomizations.Graph
{
    public class LifeSyncPreferenceMaint : PXGraph<LifeSyncPreferenceMaint>
    {
        public PXSave<LifeSyncPreference> Save;

        public PXCancel<LifeSyncPreference> Cancel;

        public SelectFrom<LifeSyncPreference>.View MasterView;


        [Serializable]
        public class DetailsTable : IBqlTable
        {
            public DetailsTable()
            {
            }
        }

        [Serializable]
        public class MasterTable : IBqlTable
        {
            public MasterTable()
            {
            }
        }
    }
}