using LumCustomizations.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.CM;

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

        public bool GetJournalEnhance
        {
            get
            {
                return this._lifesyncPreference?.EnableJournalEnhance ?? false;
            }
        }

        // Get Comapny Base Cury ID
        public string GetCompanyBaseCuryID()
        {
           return new PXGraph().Select<Company>().FirstOrDefault()?.BaseCuryID;
        }

        public string GetInventoryItemCD(int? InventoryID)
        {
            return new PXGraph().Select<InventoryItem>().Where(x => x.InventoryID == InventoryID).FirstOrDefault()?.InventoryCD;
        }

        //Get Branch's country code
        public bool isCNorHK()
        {
            var curCoutryID = (PXSelect<Branch>.Select(new PXGraph(), PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst?.CountryID;
            return (curCoutryID == "CN" || curCoutryID == "HK") ? true : false;
        }

        /// <summary> Get Effect Currency Rate </summary>
        public IEnumerable<CurrencyRate2> GetCuryRateRecordEffData(PXGraph graph)
        {
            PXSelectBase<CurrencyRate2> sel = new PXSelect<CurrencyRate2,
                Where<CurrencyRate2.toCuryID, Equal<Required<CurrencyRate2.toCuryID>>,
                    And<CurrencyRate2.fromCuryID, Equal<Required<CurrencyRate2.fromCuryID>>,
                        And<CurrencyRate2.curyRateType, Equal<Required<CurrencyRate2.curyRateType>>,
                            And<CurrencyRate2.curyEffDate, Equal<Required<CurrencyRate2.curyEffDate>>>>>>>(graph);

            List<CurrencyRate2> ret = new List<CurrencyRate2>();

            foreach (CurrencyRate2 r in PXSelectGroupBy<CurrencyRate2,
                Where<CurrencyRate2.toCuryID, Equal<Current<CuryRateFilter.toCurrency>>,
                    And<CurrencyRate2.curyEffDate, LessEqual<Current<CuryRateFilter.effDate>>>>,
                Aggregate<Max<CurrencyRate2.curyEffDate,
                    GroupBy<CurrencyRate2.curyRateType,
                        GroupBy<CurrencyRate2.fromCuryID>>>>>.Select(graph))
            {
                ret.Add((CurrencyRate2)sel.Select("CNY", r.FromCuryID, r.CuryRateType, r.CuryEffDate));
            }
            return ret;
        }
    }
}
