using System;
using LumCustomizations.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using System.Linq;
using PX.Data.BQL;

namespace LumCustomizations.Graph
{
    public class LumItemsCOCMaint : PXGraph<LumItemsCOCMaint>
    {
        #region Buttons definition		

        public PXSave<LumItemsCOC> Save;

        public PXCancel<LumItemsCOC> Cancel;

        #endregion

        #region Data View

        public SelectFrom<LumItemsCOC>.View _viewItemsCOC;

        public SelectFrom<LumItemsCOC>
                .Where<LumItemsCOC.inventoryID.IsEqual<LumItemsCOC.inventoryID.FromCurrent>>
                .View _viewLine;

        #endregion

        #region Event

        public LumItemsCOCMaint()
        {
            var _dataAttribute = SelectFrom<CSAttributeDetail>
                        .Where<CSAttributeDetail.attributeID.IsEqual<@P.AsString>>
                        .View.Select(this, "ENDC");
            PXStringListAttribute.SetList<LumItemsCOC.endCustomer>(base.Caches[typeof(LumItemsCOC)], null, _dataAttribute.FirstTableItems.Select(x => x.ValueID).ToArray(), _dataAttribute.FirstTableItems.Select(x => x.Description).ToArray());

        }

        #endregion

    }
}