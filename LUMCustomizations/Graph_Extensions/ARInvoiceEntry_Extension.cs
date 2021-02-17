using LUMCustomizations.Library;
using PX.Data;
using PX.Objects.AR;
using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.IN
{
    public class ARInvoiceEntry_Extension : PXGraphExtension<ARInvoiceEntry>
    {
        public override void Initialize()
        {
            base.Initialize();
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                Base.report.AddMenuAction(CommercialInvoiceReport);
                Base.report.AddMenuAction(CreditNoteReport);
            }
        }

        #region Override DAC

        [PXUIField()]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        public virtual void _(Events.CacheAttached<ARInvoice.lineTotal> e) { } 
        
        #endregion

        public virtual void _(Events.RowSelected<ARInvoice> e)
        {
            var _library = new LumLibrary();
            var baseCompanyCuryID = _library.GetCompanyBaseCuryID();
            // Setting LineTotal
            PXUIFieldAttribute.SetDisplayName<ARInvoice.lineTotal>(e.Cache, $"Total in {baseCompanyCuryID}");
            PXUIFieldAttribute.SetVisible<ARInvoice.lineTotal>(e.Cache, null, _library.GetShowingTotalInHome);
            PXUIFieldAttribute.SetEnabled<ARInvoice.lineTotal>(e.Cache, null, false);
            // Hide CuryOrigDiscAmt
            PXUIFieldAttribute.SetVisible<ARInvoice.curyOrigDiscAmt>(e.Cache, null, !_library.GetShowingTotalInHome);

            //controll customize button based on country ID
            if (!_library.isCNorHK())
            {
                CommercialInvoiceReport.SetVisible(false);
                CreditNoteReport.SetVisible(false);
            }
        }

        #region Action
        public PXAction<ARInvoice> CommercialInvoiceReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Commercial Invoice", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable commercialInvoiceReport(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM602000", "Report LM602000");
            } 
            return adapter.Get();
        }
        #endregion

        #region Action
        public PXAction<ARInvoice> CreditNoteReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Credit Note", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable creditNoteReport(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM602005", "Report LM602005");
            }
            return adapter.Get();
        }
        #endregion
    }
}