using LumCustomizations.DAC;
using LUMCustomizations.Library;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.PO
{
    public class POOrderEntry_Extensions : PXGraphExtension<POOrderEntry>
    {
        private const string bubbleNumber = "BUBBLENO";
        public class constBubbleNumber : PX.Data.BQL.BqlString.Constant<constBubbleNumber>
        {
            public constBubbleNumber() : base(bubbleNumber) { }
        }
        public static bool IsActive()
        {
            //active customize button if current company is ABA China and HK 
            return PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName).Contains("China") || PX.Data.PXLogin.ExtractCompany(PX.Common.PXContext.PXIdentity.IdentityName).Contains("HK");
        }

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(DomesticPO);
            Base.report.AddMenuAction(OverseasPO);
        }

        public virtual void _(Events.RowSelected<POOrder> e)
        {
            var _library = new LumLibrary();
            var BaseCuryID = _library.GetCompanyBaseCuryID();
            PXUIFieldAttribute.SetDisplayName<POOrder.orderTotal>(e.Cache, $"Total in {BaseCuryID}");
            PXUIFieldAttribute.SetVisible<POOrder.orderTotal>(e.Cache, null, _library.GetShowingTotalInHome);
        }


        #region Action
        public PXAction<POOrder> DomesticPO;
        [PXButton]
        [PXUIField(DisplayName = "Print Domestic PO", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable domesticPO(PXAdapter adapter)
        {
            var _reportID = "lm613000";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var _poOrder = adapter.Get<POOrder>().ToList().FirstOrDefault();
            parameters["OrderNbr"] = _poOrder.OrderNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion

        #region Action
        public PXAction<POOrder> OverseasPO;
        [PXButton]
        [PXUIField(DisplayName = "Print Overseas PO", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable overseasPO(PXAdapter adapter)
        {
            var _reportID = "lm603010";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["OrderNbr"] = Base.Document.Current.OrderNbr;
            throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
        }
        #endregion

        #region Bubble Number Setting Event
        protected virtual void _(Events.RowSelected<POLine> e)
        {
            // Control Header PI Column Visible
            var _graph = PXGraph.CreateInstance<POOrderEntry>();
            var _PIPreference = from t in _graph.Select<LifeSyncPreference>()
                                select t;
            var _visible = _PIPreference.FirstOrDefault() == null ? false : 
                           _PIPreference.FirstOrDefault().BubbleNumberPrinting.HasValue ? _PIPreference.FirstOrDefault().BubbleNumberPrinting.Value : false;

            PXUIFieldAttribute.SetVisible<POLineExt.usrBubbleNumber>(e.Cache, null, _visible);
        }
        #endregion

        #region Update Bubble Number
        protected virtual void _(Events.FieldUpdated<POLine.inventoryID> e)
        {
            PXResult _bubbleNumber = SelectFrom<InventoryItem>.
                                LeftJoin<CSAnswers>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>.
                                                    And<CSAnswers.attributeID.IsEqual<constBubbleNumber>>>.
                                Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.
                                Select(Base, ((POLine)e.Row).InventoryID);
            e.Cache.SetValue<POLineExt.usrBubbleNumber>(e.Row, _bubbleNumber.GetItem<CSAnswers>().Value);
        }
        #endregion
    }
}
