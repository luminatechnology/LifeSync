using LumCustomizations.DAC;
using PX.Data;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMCustomizations.Library
{
    // public Function
    public class LumLibrary
    {
        public LumLibrary()
        {
            var lifeSyncPreference = from t in new PXGraph().Select<LifeSyncPreference>()
                                     select t;
            this._lifesyncPreference = lifeSyncPreference.FirstOrDefault();
        }

        protected LifeSyncPreference _lifesyncPreference;

        public bool GetShowingTotalInHome
        {
            get
            {
                return this._lifesyncPreference?.ShowingTotalInHomeCurrency ?? false;
            }
        }

        public bool GetCrossRateOverride
        {
            get 
            {
                return this._lifesyncPreference?.CrossRateOverride ?? false;

            }
        }

        public bool GetProformaInvoicePrinting
        {
            get
            {
                return this._lifesyncPreference?.ProformaInvoicePrinting ?? false;
            }
        }

        // Get Comapny Base Cury ID
        public string GetCompanyBaseCuryID()
        {
           return new PXGraph().Select<Company>().FirstOrDefault()?.BaseCuryID;
        }
    }
}
