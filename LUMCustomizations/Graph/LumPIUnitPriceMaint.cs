using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumCustomizations.Graph
{
    public class LumPIUnitPriceMaint : PXGraph<LumPIUnitPriceMaint>
    {
        public PXSave<SOLine> Save;
        public PXCancel<SOLine> Cancel;
        public SelectFrom<SOLine>
                .InnerJoin<SOOrder>.On<SOLine.orderNbr.IsEqual<SOOrder.orderNbr>
                    .And<SOLine.orderType.IsEqual<SOOrder.orderType>>>.View SOTranscation;

        public PXAction<SOLine> viewSOOrderDocument;
        [PXButton]
        [PXUIField(Visible = false)]
        public virtual IEnumerable ViewSOOrderDocument(PXAdapter adapter)
        {
            var row = this.SOTranscation.Current;
            var graph = PXGraph.CreateInstance<SOOrderEntry>();
            graph.Document.Current = SelectFrom<SOOrder>
                                    .Where<SOOrder.orderNbr.IsEqual<P.AsString>
                                        .And<SOOrder.orderType.IsEqual<P.AsString>>>.View.Select(this, row.OrderNbr, row.OrderType);
            PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
            return adapter.Get();
        }
    }
}
