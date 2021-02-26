using JAMS.AM;
using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAMS.AM.Attributes;

namespace LUMCustomizations.Graph
{
    public class LUMUpdateBOMQty : PXGraph<LUMUpdateBOMQty>
    {
        public SelectFrom<AMBomMatl>.InnerJoin<AMBomItem>.On<AMBomItem.bOMID.IsEqual<AMBomMatl.bOMID>
                .And<AMBomItem.revisionID.IsEqual<AMBomMatl.revisionID>>
                .And<AMBomItem.status.IsEqual<AMBomStatus.active>>>
               .Where<AMBomMatl.inventoryID.IsEqual<_inventoryID>
                   .Or<AMBomMatl.inventoryID.IsEqual<_inventoryID2>>>.View Bom;

        public PXAction<AMBomMatl> UpdateMaterial;
        [PXButton]
        [PXUIField(DisplayName = "Update Material", MapEnableRights = PXCacheRights.Select)]
        protected IEnumerable updateMaterial(PXAdapter adapter) 
        {
            var data = Bom.Select();
            data.Where(x => ((AMBomMatl)x).BOMID == "210293").ToList()
                .ForEach(x => 
                {
                    var row = x.GetItem<AMBomMatl>();
                    Bom.SetValueExt<AMBomMatl.qtyReq>(row, (decimal?)0.006666);
                    Bom.Update(row);
                });
            this.Actions.PressSave();
            return adapter.Get();
        }
    }

    public class _inventoryID : PX.Data.BQL.BqlInt.Constant<_inventoryID>
    {
        public _inventoryID() : base(19356) { }
    }

    public class _inventoryID2 : PX.Data.BQL.BqlInt.Constant<_inventoryID>
    {
        public _inventoryID2() : base(16207) { }
    }
}
