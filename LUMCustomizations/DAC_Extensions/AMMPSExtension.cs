using JAMS.MRP;
using PX.Data;

namespace JAMS.AM
{
    public class AMMPSExtension : PXCacheExtension<AMMPS>
    {
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Work Center")]
        [PXSelector(typeof(Search<AMWC.wcID>),
                    typeof(AMWC.wcID),
                    typeof(AMWC.descr),
                    typeof(AMWC.siteID),
                    typeof(AMWC.stdCost))]
        public virtual string UsrWcID { get; set; }
        public abstract class usrwcID : PX.Data.BQL.BqlString.Field<AMMPSExtension.usrwcID> { }

        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string UsrRemark { get; set; }
        public abstract class usrremark : PX.Data.BQL.BqlString.Field<AMMPSExtension.usrremark> { }
    }
}