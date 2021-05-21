using JAMS.AM;
using PX.Data;
using PX.Objects;
using System;

namespace JAMS.AM
{
    public class AMBomMatlExt : PXCacheExtension<JAMS.AM.AMBomMatl>
    {
        #region UsrScrapeNote
        [PXDBDecimal(6, MinValue = 0.0)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Scrape Note")]
        public virtual Decimal? UsrScrapeNote { get; set; }
        public abstract class usrScrapeNote : PX.Data.BQL.BqlDecimal.Field<usrScrapeNote> { }
        #endregion
    }
}