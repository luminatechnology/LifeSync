using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAMS.AM
{
    public class AMBatchExt : PXCacheExtension<AMBatch>
    {
        protected int? _UsrPrintCount;
        [PXDBInt]
        [PXUIField(DisplayName = "Print Count", Enabled = false)]
        public virtual int? UsrPrintCount { get; set; }
        public abstract class usrPrintCount : BqlType<IBqlInt, int>.Field<AMProdItemExt.usrPrintCount> { }

        [PXDecimal(4)]
        [PXUIField(DisplayName = "Related Qty for GI", Enabled = false, Visible = false)]
        public virtual Decimal? UsrRefQty { get; set; }
    }
}
