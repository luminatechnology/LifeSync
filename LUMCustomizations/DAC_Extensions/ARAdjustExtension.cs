using PX.Data;
using PX.Data.BQL;
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
        [PXUIField(DisplayName = "Ref. Balance")]
        public virtual string UsrBaseBalance { get; set; }
        public abstract class usrBaseBalance : BqlType<IBqlString, string>.Field<ARAdjustExtension.usrBaseBalance> { }
    }
}
