using JAMS.AM.Attributes;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAMS.AM
{
    public class AMPordMatlExtension : PXCacheExtension<AMProdMatl>
    {
        #region QtyDiff

        [PXQuantity]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Qty Diff", Enabled = false)]
        [PXFormula(typeof(Sub<AMProdMatl.totalQtyRequired, AMProdMatl.qtyActual>))]
        public virtual Decimal? UsrQtyDiff { get; set; }
        public abstract class usrQtyDiff : PX.Data.BQL.BqlDecimal.Field<usrQtyDiff> { }
        #endregion
    }
}
