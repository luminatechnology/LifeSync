using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.IN
{
    public class INRegisterExt : PXCacheExtension<INRegister>
    {
        protected int? _UsrPrintCount;
        [PXDBInt]
        [PXUIField(DisplayName = "Print Count", Enabled = false)]
        public virtual int? UsrPrintCount { get; set; }
        public abstract class usrPrintCount : BqlType<IBqlInt, int>.Field<INRegisterExt.usrPrintCount> { }
    }
}
