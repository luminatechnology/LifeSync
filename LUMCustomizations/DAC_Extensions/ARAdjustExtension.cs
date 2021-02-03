using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AR
{
    public class ARAdjustExtension :  PXCacheExtension<ARAdjust>
    {
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Ref. Balance",Enabled = false)]
        [PXDBScalar(typeof(
            SelectFrom<ARInvoice>.Where<ARInvoice.refNbr.IsEqual<ARAdjust.adjdRefNbr>>
                                 .SearchFor<ARInvoice.curyLineTotal>
            ))]
        [PXFormula(typeof(Default<ARAdjust.adjdRefNbr>))]
        [PXDefault(typeof(
            SelectFrom<ARInvoice>.Where<ARInvoice.refNbr.IsEqual<ARAdjust.adjdRefNbr.FromCurrent>>
                                 .SearchFor<ARInvoice.curyLineTotal>
            ))]
        public virtual Decimal? UsrBaseBalance { get; set; }
        public abstract class usrBaseBalance : BqlType<IBqlDecimal, decimal>.Field<ARAdjustExtension.usrBaseBalance> { }

        [PXDecimal(2)]
        [PXUIField(DisplayName = "Remaining CuryAdjdAmt",Enabled = false)]
        public virtual Decimal? UsrRemCuryAdjdAmt { get; set; }
        public abstract class usrRemCuryAdjdAmt : BqlType<IBqlDecimal, decimal>.Field<ARAdjustExtension.usrRemCuryAdjdAmt> { }
    }
}
