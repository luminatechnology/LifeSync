using LUMCustomizations.Library;
using PX.Data;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AP
{
    public class APInvoiceEntry_Extension : PXGraphExtension<APInvoiceEntry>
    {
        [PXUIField()]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        public virtual void _(Events.CacheAttached<APInvoice.lineTotal> e) { }

        public virtual void _(Events.RowSelected<APInvoice> e)
        {
            var _library = new LumLibrary();
            var BaseCuryID = _library.GetCompanyBaseCuryID();
            PXUIFieldAttribute.SetDisplayName<APInvoice.lineTotal>(e.Cache, $"Total in {BaseCuryID}");
            PXUIFieldAttribute.SetVisible<APInvoice.lineTotal>(e.Cache, null, _library.GetShowingTotalInHome);
            // Hide Standard Field
            PXUIFieldAttribute.SetVisible<APInvoice.curyOrigDiscAmt>(e.Cache, null, !_library.GetShowingTotalInHome);
            PXUIFieldAttribute.SetVisible<APInvoice.curyOrigWhTaxAmt>(e.Cache, null, !_library.GetShowingTotalInHome);
        }
    }
}
