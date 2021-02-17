using LUMCustomizations.Library;
using PX.Data;
using PX.Objects.AR;
using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.SO
{
    public class SOInvoiceEntry_Extension : PXGraphExtension<SOInvoiceEntry>
    {
        [PXUIField()]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        public virtual void _(Events.CacheAttached<ARInvoice.lineTotal> e) { }

        public override void Initialize()
        {
            base.Initialize();
            var _lumLibrary = new LumLibrary();
            if (_lumLibrary.isCNorHK())
            {
                Base.report.AddMenuAction(CommercialInvoiceReport);
                Base.report.AddMenuAction(CreditNoteReport);
                Base.report.AddMenuAction(CommercialInvoiceFromDGReport);
            }
        }

        public virtual void _(Events.RowSelected<ARInvoice> e)
        {
            var library = new LumLibrary();
            var BaseComapnyCuryID = library.GetCompanyBaseCuryID();
            PXUIFieldAttribute.SetDisplayName<ARInvoice.lineTotal>(e.Cache, $"Total in {BaseComapnyCuryID}");
            PXUIFieldAttribute.SetEnabled<ARInvoice.lineTotal>(e.Cache, null, false);
            // Defaul Visiable is false
            PXUIFieldAttribute.SetVisible<ARInvoice.lineTotal>(e.Cache, null, library.GetShowingTotalInHome);

            //controll customize button based on country ID
            var _lumLibrary = new LumLibrary();
            if (!_lumLibrary.isCNorHK())
            {
                CommercialInvoiceReport.SetVisible(false);
                CreditNoteReport.SetVisible(false);
                CommercialInvoiceFromDGReport.SetVisible(false);
            }
        }


        #region Action
        public PXAction<ARInvoice> CommercialInvoiceReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Commercial Invoice (HK for tooling)", Enabled = true, MapEnableRights = PXCacheRights.Select)]
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
        [PXUIField(DisplayName = "Print Credit Note", Enabled = true, MapEnableRights = PXCacheRights.Select)]
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

        #region Action
        public PXAction<ARInvoice> CommercialInvoiceFromDGReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Commercial Invoice From DG", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable commercialInvoiceFromDGReport(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM602015", "Report LM602015");
            }
            return adapter.Get();
        }
        #endregion
    }
}